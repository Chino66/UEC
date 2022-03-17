using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UEC.Event;
using UEC.UIFramework;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace UEC
{
    public class InstallView : View<UECUI>
    {
        private VisualElementCache _cache;
        private UECContext context => UI.Context;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("Install_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            var ibtn = _cache.Get<Button>("install_btn");
            ibtn.clicked += () =>
            {
                // Client.Add("com.chino.object.pool:0.0.2");
                list();
                Debug.Log("sss");
            };
            Refresh();

            var btn = new Button();
            btn.text = "list";
            btn.clicked += () => { list(); };
            ibtn.parent.Add(btn);

            btn = new Button();
            btn.text = "add";
            btn.clicked += () =>
            {
                // add("com.chino.object.pool@0.0.2");
                Client.Add("com.chino.object.pool@0.0.2");
            };
            ibtn.parent.Add(btn);

            Self.SetDisplay(false);
        }

        public void Refresh()
        {
        }

        private async void list()
        {
            var request = Client.List(true, true);
            var condition = new TaskCondition();
            await condition.WaitUntilProgress(() => request.IsCompleted);
            var success = request.Status == StatusCode.Success;
            if (request.Status >= StatusCode.Failure)
            {
                Debug.LogError($"ListPackageAsync Failure: {request.Error.message}");
                success = false;
            }

            var list = new Dictionary<string, PackageInfo>();
            if (success)
            {
                foreach (var package in request.Result)
                {
                    list.Add(package.name, package);
                    // Debug.Log(package.name);

                    Debug.Log(package.packageId);
                }
            }
        }

        private async void add(string packageId)
        {
            var request = Client.Add(packageId);
            var condition = new TaskCondition();
            await condition.WaitUntilProgress(() => request.IsCompleted);
            var success = request.Status == StatusCode.Success;
            if (request.Status >= StatusCode.Failure)
            {
                Debug.LogError($"AddPackageAsync Failure: {request.Error.message}");
                success = false;
            }
        }
    }

    public class TaskCondition
    {
        public bool IsRunning;

        public int Timeout = 10000;

        public CancellationTokenSource TokenSource;

        public TaskCondition()
        {
        }

        public void Start(int timeout = 10000)
        {
            Timeout = timeout;
            IsRunning = true;
            TokenSource = new CancellationTokenSource();
        }

        public void Complete()
        {
            IsRunning = false;
            TokenSource?.Cancel();
        }

        public async Task<bool> WaitUntilComplete(int timeout = 10000)
        {
            Start(timeout);
            return await WaitUntil(this);
        }

        public async Task<bool> WaitUntilProgress([NotNull] Func<bool> check,
            int millisecondsDelay = 100,
            int timeout = 10000)
        {
            if (check == null)
            {
                Debug.LogError("check is null");
                return false;
            }

            var timecount = 0;
            while (check?.Invoke() == false)
            {
                await Task.Delay(millisecondsDelay);
                timecount += millisecondsDelay;
                if (timecount >= timeout)
                {
                    break;
                }
            }

            return check.Invoke();
        }

        #region Static

//        public static async Task<bool> WaitUntilCondition(TaskCondition condition, int millisecondsDelay = 100)
//        {
//            while (condition.Value == false)
//            {
//                await Task.Delay(millisecondsDelay);
//            }
//
//            return condition.Value;
//        }
//
//        public static async Task<bool> WaitUntilCondition(TaskCondition condition)
//        {
//            await WaitUntilCondition(condition, 100);
//
//            return condition.Value;
//        }

        public static async Task<bool> WaitUntil(TaskCondition condition)
        {
            var task = Task.Delay(condition.Timeout, condition.TokenSource.Token)
                .ContinueWith(tsk => tsk.Exception == default);

            try
            {
                await task;
            }
            catch (OperationCanceledException e)
            {
                Debug.LogError(e);
            }
            finally
            {
                condition.TokenSource?.Dispose();
            }

            // 如果condition.IsRunning是true,则返回false表示超时
            return !condition.IsRunning;
        }

        #endregion
    }
}