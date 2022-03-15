using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace UEC
{
    public class ManifestModel
    {
        private const string ManifestFile = "manifest.json";

        public class Scope
        {
            public string tag;
            public string url;
            public string name;
            public List<string> scopes;
        }

        [Serializable]
        public class Manifest
        {
            public Dictionary<string, string> dependencies = new Dictionary<string, string>();
            public List<Scope> scopedRegistries = new List<Scope>();
        }

        public void Test()
        {
            DirectoryInfo assetPath = new DirectoryInfo(Application.dataPath);
            //E:\AR
            string projectPath = assetPath.Parent.FullName;
            //E:\AR\Packages
            string packagePath = Path.Combine(projectPath, "Packages");
            var manifestJsonPath = Path.Combine(packagePath, "manifest.json");
            string manifestJson = File.ReadAllText(manifestJsonPath);
            var manifest = JsonConvert.DeserializeObject<Manifest>(manifestJson);
            // foreach (var package in manifest.dependencies)
            // {
            //     Debug.Log(package.Key + " - " + package.Value);
            // }
            foreach (var package in manifest.scopedRegistries)
            {
                Debug.Log(package.name);
                Debug.Log(package.tag);
                Debug.Log(package.url);
                Debug.Log(package.scopes[0]);
            }
        }
    }
}