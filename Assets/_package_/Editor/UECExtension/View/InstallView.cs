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

        private Dictionary<string, VisualElement> addButtons;

        protected override void OnInitialize(VisualElement parent)
        {
            var temp = parent.Q("Install_view_root");
            temp.parent.Add(Self);
            Add(temp);
            _cache = new VisualElementCache(temp);

            addButtons = new Dictionary<string, VisualElement>();

            var ibtn = _cache.Get<Button>("install_btn");
            // ibtn.clicked += () => { list(); };
            ibtn.SetDisplay(false);

            addButton(ibtn.parent, "com.chino.upm.kits");
            addButton(ibtn.parent, "com.chino.upm.list");
            
            list();
        }

        private void addButton(VisualElement parent, string packageId)
        {
            var btn = new Button {text = packageId};
            btn.clicked += () =>
            {
                add(packageId);
            };
            parent.Add(btn);
            addButtons.Add(packageId, btn);
            btn.SetEnabled(true);
        }


        public void Refresh()
        {
        }

        private async void list()
        {
            Self.SetEnabled(false);
            var request = Client.List(true, true);
            var condition = new TaskCondition();
            await condition.WaitUntilProgress(() => request.IsCompleted);
            var success = request.Status == StatusCode.Success;
            if (request.Status >= StatusCode.Failure)
            {
                Debug.LogError($"ListPackageAsync Failure: {request.Error.message}");
                success = false;
            }

            if (success)
            {
                foreach (var package in request.Result)
                {
                    addButtons.TryGetValue(package.name, out var ve);
                    ve?.SetEnabled(false);
                }
            }
            Self.SetEnabled(true);
        }

        private async void add(string packageId)
        {
            Self.SetEnabled(false);
            var request = Client.Add(packageId);
            var condition = new TaskCondition();
            await condition.WaitUntilProgress(() => request.IsCompleted);
            var success = request.Status == StatusCode.Success;
            if (request.Status >= StatusCode.Failure)
            {
                Debug.LogError($"AddPackageAsync Failure: {request.Error.message}");
                success = false;
            }
            Self.SetEnabled(true);
            list();
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