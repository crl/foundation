using System.Text;
using UnityEngine;

namespace foundation
{
    public class PathDefine
    {
        public const string U3D = ".unity3d";
        public static string configPath;
        public static string uiPath;

        public static string avatarPath;
        public static string scenePath;
        public static string storyPath;

        public static string texturePath;
        public static string effectPath;
        public static string soundPath;

        public static string commonPath;

        public static string globalPath;

        private static StringBuilder sbBuilder=new StringBuilder();


        private static string persistentDataPath;
        private static string streamingAssetsPath;
        static PathDefine()
        {
            persistentDataPath = Application.persistentDataPath;
            streamingAssetsPath = Application.streamingAssetsPath;
        }

        public static string getPlatformVersionFileName()
        {
            return platformFolderName + "V.txt";
        }

        public static string getPlatformUpdateXMLFileName(string platform="")
        {
            string prefix = platformFolderName;
            if (string.IsNullOrEmpty(platform)==false)
            {
                prefix += "_" + platform;
            }
            return prefix + "U.xml";
        }

        /// <summary>
        /// 持久数据路径(可读写)
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="isWWW"></param>
        /// <returns></returns>
        public static string getPersistentLocal(string uri = "", bool isWWW = false)
        {
            string prefix = "";
            if (isWWW == true)
            {
                prefix = getLocalHttpPrefix();
            }
            sbBuilder.Clear();
            sbBuilder.Append(prefix);
            sbBuilder.Append(persistentDataPath);
            sbBuilder.Append("/");
            sbBuilder.Append(uri);

            return sbBuilder.ToString();
        }

        public static string getLocalHttpPrefix()
        {
            string prefix = "file://";
            if (Application.isEditor)
            {
                prefix = "file:///";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                prefix = "file://";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                prefix = "file://";
            }
            return prefix;
        }

        /// <summary>
        /// 包内 资源根目录 的StreamingAssets
        /// 在apk中会加入jar:
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="isWWW"></param>
        /// <returns></returns>
        public static string getStreamingAssetsLocal(string uri = "",bool isWWW=false)
        {
            string prefix = "";
            if (isWWW == true)
            {
                prefix = "file://";
                if (Application.isEditor)
                {
                    prefix = "file://";
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    prefix = "";
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    prefix = "file://";
                }
            }

            sbBuilder.Clear();
            sbBuilder.Append(prefix);
            sbBuilder.Append(streamingAssetsPath);
            sbBuilder.Append("/");
            sbBuilder.Append(uri);

            return sbBuilder.ToString();
        }

        private static string _platformFolderName;
        public static string platformFolderName
        {
            get
            {
                if (_platformFolderName != null)
                {
                    return _platformFolderName;
                }
                _platformFolderName = "Android";
#if UNITY_EDITOR
        switch (UnityEditor.EditorUserBuildSettings.activeBuildTarget)
        {
            case UnityEditor.BuildTarget.iOS:
                PathDefine.forceSetPlatformFolderName = "iOS";
                break;
            case UnityEditor.BuildTarget.Android:
                PathDefine.forceSetPlatformFolderName = "Android";
                break;
        }
#endif
                switch (Application.platform)
                {
                    case RuntimePlatform.Android:
                        _platformFolderName = "Android";
                        break;
                    case RuntimePlatform.IPhonePlayer:
                        _platformFolderName = "iOS";
                        break;
                }
                return _platformFolderName;
            }
        }


        public static string forceSetPlatformFolderName
        {
            set { _platformFolderName = value; }
        }

        
    }
}
