using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace UEC
{
    public class ManifestModel
    {
        [Serializable]
        public class ScopedRegistry
        {
            public string tag;
            public string url;
            public string name;
            public List<string> scopes;

            public override string ToString()
            {
                return $"{tag},{url},{name},{scopes.Count}";
            }
        }

        [Serializable]
        public class Manifest
        {
            public Dictionary<string, string> dependencies = new Dictionary<string, string>();
            public List<ScopedRegistry> scopedRegistries = new List<ScopedRegistry>();
        }

        private const string ManifestFile = "manifest.json";

        private const string TagContent = "uecconfig";

        private const string GithubUrl = "https://npm.pkg.github.com/@";

        public static string _manifestPath;
        public static string ManifestPath => GetManifestPath();

        public UECContext Context;

        public ManifestModel(UECContext context)
        {
            Context = context;
        }

        public void Update()
        {
            var manifest = GetManifest();
            var items = Context.UECConfigModel.GetItems();

            var ret = manifest.scopedRegistries.Select(registry => registry)
                .Where(registry => TagContent.Equals(registry.tag)).ToArray();

            for (var i = 0; i < ret.Length; i++)
            {
                manifest.scopedRegistries.Remove(ret[i]);
            }

            foreach (var configItem in items)
            {
                var registry = new ScopedRegistry()
                {
                    tag = TagContent,
                    name = configItem.Username,
                    url = GithubUrl + configItem.Username,
                    scopes = new List<string>(configItem.Scopes),
                };
                manifest.scopedRegistries.Add(registry);
            }

            foreach (var registry in manifest.scopedRegistries)
            {
                Debug.Log(registry.ToString());
            }

            SaveManifest(manifest);

            Client.Resolve();
        }

        private Manifest GetManifest()
        {
            var manifestJsonPath = GetManifestPath();
            var manifestJson = File.ReadAllText(manifestJsonPath);
            var manifest = JsonConvert.DeserializeObject<Manifest>(manifestJson);
            return manifest;
        }

        private void SaveManifest(Manifest manifest)
        {
            var manifestJsonPath = GetManifestPath();
            var setting = new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};
            var json = JsonConvert.SerializeObject(manifest, Formatting.Indented, setting);
            using (var sw = new StreamWriter(manifestJsonPath, false, System.Text.Encoding.Default))
            {
                sw.Write(json);
                sw.Close();
            }
        }

        private static string GetManifestPath()
        {
            if (_manifestPath == null)
            {
                var assetPath = new DirectoryInfo(Application.dataPath);
                var projectPath = assetPath.Parent.FullName;
                var packagePath = Path.Combine(projectPath, "Packages");
                var manifestJsonPath = Path.Combine(packagePath, ManifestFile);

                _manifestPath = manifestJsonPath;
            }

            return _manifestPath;
        }
    }
}