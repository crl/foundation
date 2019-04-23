using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace foundation
{
    /// <summary>
    /// 版本比较管理
    /// </summary>
    public class VersionLoaderFactory 
    {
        public const string KEY = "vdata";
        protected static string PRE_HASH = "";
        private static int PRE_HASH_LEN = 0;

        public Dictionary<string, HashSizeFile> localMapping = new Dictionary<string, HashSizeFile>();
        public Dictionary<string, HashSizeFile> remoteMapping = new Dictionary<string, HashSizeFile>();
        private static VersionLoaderFactory instance;
        public static VersionLoaderFactory GetInstance()
        {
            if (instance == null)
            {
                instance=new VersionLoaderFactory();
            }
            return instance;
        }

        /// <summary>
        /// 解析版本列表文件到对像
        /// </summary>
        /// <param name="v"></param>
        /// <param name="mapping"></param>
        /// <param name="sizeMapping"></param>
        public static void ParserVersion(string v, Dictionary<string,HashSizeFile> mapping)
        {
            if (string.IsNullOrEmpty(v))
            {
                return;
            }
            StringReader reader=new StringReader(v);
            string key=reader.ReadLine();
            string[] keyHash;
            while (key!=null)
            {
                keyHash = key.Split(':');
                HashSizeFile hashSizeFile=new HashSizeFile(keyHash[0]);
                hashSizeFile.hash = keyHash[1];

                int size = 0;
                int.TryParse(keyHash[2], out size);
                hashSizeFile.size = size;

                mapping[hashSizeFile.uri] = hashSizeFile;

                key = reader.ReadLine();
            }
        }

        /// <summary>
        /// 保存包内的v.txt
        /// </summary>
        /// <param name="localVersionText"></param>
        public static void SavePackageVersion(string localVersionText)
        {
            Dictionary<string, HashSizeFile> localMapping = new Dictionary<string, HashSizeFile>();
            VersionLoaderFactory.ParserVersion(localVersionText, localMapping);

            Dictionary<string, string> stringMapping = new Dictionary<string, string>();
            foreach (KeyValuePair<string, HashSizeFile> item in localMapping)
            {
                stringMapping[item.Key] = item.Value.hash;
            }

            FileHelper.SaveAMF(stringMapping, VdatPath);
        }

        /// <summary>
        /// 本地Hash文件列表
        /// </summary>
        /// <param name="v"></param>
        public void initLocal()
        {
            IDictionary dic = null;
            string fullPath = VdatPath;
            if (File.Exists(fullPath))
            {
                dic = FileHelper.GetAMF(fullPath) as IDictionary;
            }

            localMapping.Clear();
            if (dic == null)
            {
                return;
            }

            foreach (DictionaryEntry entry in dic)
            {
                HashSizeFile hashSizeFile = new HashSizeFile((string)entry.Key);
                hashSizeFile.hash = (string)entry.Value;
                localMapping[hashSizeFile.uri] = hashSizeFile;
            }
        }

        /// <summary>
        /// 远程Hash文件列表
        /// </summary>
        /// <param name="v"></param>
        public void initRemote(string remoteVersionText)
        {
            remoteMapping.Clear();
            ParserVersion(remoteVersionText, remoteMapping);
        }

        /// <summary>
        /// 由文件key,查看是本地与完程的文件hash是否相等
        /// </summary>
        /// <param name="hashKey">文件key值</param>
        /// <returns></returns>
        public bool checkHashEquals(string hashKey)
        {
            HashSizeFile remoteHash;
            if (remoteMapping.TryGetValue(hashKey, out remoteHash)==false)
            {
                return true;
            }

            HashSizeFile localHash;
            if (localMapping.TryGetValue(hashKey, out localHash) == false)
            {
                return false;
            }

            if (localHash.hash == remoteHash.hash)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 取得远程文件在本地的映射路径
        /// </summary>
        /// <param name="url">远程文件路径</param>
        /// <param name="isSubfix">是否只含后缀(不是完整路径)</param>
        /// <returns></returns>
        public string getLocalPathByURL(string url,bool isSubfix=false)
        {
            string localPath = "";
            int index = url.IndexOf(PRE_HASH);
            if (index != -1)
            {
                localPath = url.Substring(index + PRE_HASH_LEN);
                localPath = formatedLocalURL(localPath);

                if (isSubfix==false)
                {
                    localPath = PathDefine.getPersistentLocal(localPath);
                }
            }
            return localPath;
        }

        /// <summary>
        ///  本地是否存在此文件
        /// </summary>
        /// <param name="httpFullPath"></param>
        /// <returns></returns>
        public static bool IsHasLocalFile(string httpFullPath)
        {
            return GetInstance().isHasLocalFile(httpFullPath);
        }

        /// <summary>
        /// 是否远程有这个文件
        /// </summary>
        /// <param name="httpFullPath"></param>
        /// <returns></returns>
        public static bool IsHasRemoteFile(string httpFullPath)
        {
            return GetInstance().isHasRemoteFile(httpFullPath);
        }

        protected bool isHasLocalFile(string httpFullPath)
        {
            if (string.IsNullOrEmpty(httpFullPath))
            {
                return false;
            }
            string locaPath = getLocalPathByURL(httpFullPath, true);
            string fullLocalPath = PathDefine.getPersistentLocal(locaPath);
            if (File.Exists(fullLocalPath) == true)
            {
                return true;
            }
            return false;
        }

        protected bool isHasRemoteFile(string httpFullPath)
        {
            if (string.IsNullOrEmpty(httpFullPath))
            {
                return false;
            }
            string locaPath = getLocalPathByURL(httpFullPath, true);
            return remoteMapping.ContainsKey("/"+locaPath);
        }

        public bool isInvalidate = false;
        /// <summary>
        /// 保存本地最新hash 使用脏数据形式保存
        /// </summary>
        public void saveLocalMapping()
        {
            if (isInvalidate==false)
            {
                return;
            }
            isInvalidate = false;

            Dictionary<string, string> stringMapping = new Dictionary<string, string>();
            foreach (KeyValuePair<string, HashSizeFile> item in localMapping)
            {
                stringMapping[item.Key] = item.Value.hash;
            }
            FileHelper.SaveAMF(stringMapping, VdatPath, false);
        }

        public static string VdatPath
        {
            get
            {
                return PathDefine.getPersistentLocal("v.dat");
            }
        }

       

        /// <summary>
        /// 本地路径 格式化(主要去掉服务器?后参数)
        /// </summary>
        /// <param name="localPath"></param>
        /// <returns></returns>
        protected virtual string formatedLocalURL(string localPath)
        {
            int index = localPath.IndexOf('?');
            if (index == -1)
            {
                return localPath;
            }
            return localPath.Substring(0,index);
        }

        /// <summary>
        /// 设置hash比对的前缀
        /// </summary>
        /// <param name="value"></param>
        public static void SetPreHashValue(string value)
        {
            PRE_HASH = value;
            PRE_HASH_LEN = value.Length;
        }
    }
}
