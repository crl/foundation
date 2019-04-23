using System.Collections.Generic;
using System.IO;
using foundation;
using UnityEngine;

namespace gameSDK
{
    public class PrefabVODB
    {
        private static Dictionary<string, List<PrefabVO>> dataProvider = new Dictionary<string, List<PrefabVO>>();
        private static Dictionary<string,PrefabVO> mapping=new Dictionary<string, PrefabVO>();

        public static PrefabVO Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (dataProvider.Count == 0)
            {
                if(RequestLimit.check(dataProvider, 1f)){
                    Reload();
                }
            }
            PrefabVO prefabVo = null;
            mapping.TryGetValue(key, out prefabVo);
            return prefabVo;
        }

        public static Dictionary<string, List<PrefabVO>> Reload()
        {
            dataProvider.Clear();
            mapping.Clear();
            string[] parentDirectoryList = Directory.GetDirectories("Assets/Prefabs");

            foreach (string parentDirectory in parentDirectoryList)
            {
                DirectoryInfo parentDirectoryInfo = new DirectoryInfo(parentDirectory);
                string parentDirectoryName = parentDirectoryInfo.Name;
                FileInfo[] tempFileInfos = parentDirectoryInfo.GetFiles("*.prefab", SearchOption.AllDirectories);

                List<PrefabVO> list = new List<PrefabVO>();
                PrefabVO prefabVo = null;
                foreach (FileInfo fileInfo in tempFileInfos)
                {
                    if (fileInfo.Extension.ToLower() != ".prefab")
                    {
                        continue;
                    }
                    string fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);
                    prefabVo = new PrefabVO();
                    prefabVo.keys = fileInfo.Directory.Name;
                    prefabVo.fileName = fileName;
                    prefabVo.rootFolder = parentDirectory.Replace("\\", "/") + "/";
                    prefabVo.rootKey = parentDirectoryName;

                    string path = fileInfo.FullName.Replace("\\", "/");
                    prefabVo.prefabPath = path.Replace(Application.dataPath, "Assets");

                    list.Add(prefabVo);

                    if (mapping.ContainsKey(fileName) == false)
                    {
                        mapping.Add(fileName, prefabVo);
                    }
                    else
                    {
                        mapping[fileName] = prefabVo;
                    }
                }
                dataProvider.Add(parentDirectoryName, list);
            }

            return dataProvider;
        }

        public static string GetPath(string uri)
        {
            string path = "";
            PrefabVO prefabVo = Get(uri);
            if (prefabVo != null)
            {
                path = prefabVo.prefabPath;
            }
            return path;
        }
    }
}