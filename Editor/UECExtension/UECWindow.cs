using System;
using UEC.Event;
using UnityEditor;
using UnityEngine;

namespace UEC
{
    public class UECWindow : EditorWindow
    {
        [MenuItem("Tools/UECWindow")]
        private static void ShowWindow()
        {
            var window = CreateWindow<UECWindow>();
            window.minSize = new Vector2(400, 500);
            window.titleContent = new GUIContent("UEC");
            window.Show();
        }

        private void OnEnable()
        {
            var root = UECUI.CreateUI();
            rootVisualElement.Add(root.Self);
        }
    }
}