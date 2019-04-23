using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace foundation
{
    public class FileHelper
    {
        public static byte[] GetBytes(string fullPath)
        {
            if (File.Exists(fullPath) == false)
            {
                return null;
            }
            FileStream fs = File.OpenRead(fullPath);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            fs.Close();
            return bytes;
        }

        public static string GetUTF8Text(string fullPath)
        {
            if (File.Exists(fullPath) == false)
            {
                return "";
            }
            FileStream fs = File.OpenRead(fullPath);
            byte[] bytes = new byte[fs.Length];
            fs.Read(bytes, 0, bytes.Length);
            string v = Encoding.UTF8.GetString(bytes);
            fs.Close();

            return v;
        }

        public static void AutoCreateDirectory(string fullPath)
        {
            FileInfo fi = new FileInfo(fullPath);
            DirectoryInfo di = fi.Directory;
            if (di.Exists == false)
            {
                Directory.CreateDirectory(di.FullName);
            }
        }

        public static object GetAMF(string fullPath, bool deflate = true)
        {
            if (File.Exists(fullPath) == false)
            {
                return null;
            }
            byte[] bytes;
            using (FileStream fileStream = File.OpenRead(fullPath))
            {
                bytes = new byte[fileStream.Length];
                fileStream.Read(bytes, 0, bytes.Length);
                fileStream.Close();
            }
            if (bytes.Length<1)
            {
                return null;
            }

            object result = null;
            if (bytes != null)
            {
                ByteArray bytesArray = new ByteArray(bytes);
                if (deflate)
                {
                    try
                    {
                        bytesArray.Inflate();
                    }
                    catch (Exception)
                    {
                        bytesArray.Position = 0;
                    }
                    if (bytesArray.BytesAvailable>0)
                    {
                        result = bytesArray.ReadObject();
                    }
                }
                else
                {
                    try
                    {
                        result = bytesArray.ReadObject();
                    }
                    catch (Exception)
                    {
                        bytesArray.Position = 0;
                        bytesArray.Inflate();
                        result = bytesArray.ReadObject();
                    }
                }
            }

            return result;
        }

        public static string GetFullPathParent(string path)
        {
            string[] list=path.As3Split(@"/");
            return list.As3Join(@"/");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vo"></param>
        /// <param name="fullPath"></param>
        /// <param name="deflate">是否压缩</param>
        public static void SaveAMF(object vo, string fullPath, bool deflate = true)
        {
            AutoCreateDirectory(fullPath);
            ByteArray bytesArray = new ByteArray();
            bytesArray.WriteObject(vo);
            if (deflate)
            {
                bytesArray.Deflate();
            }
            byte[] bytes = bytesArray.ToArray();

            File.WriteAllBytes(fullPath, bytes);
        }

        /// <summary>
        /// 递归查找指定目录下指定后缀名的所有文件（含所有子级目录）
        /// </summary>
        /// <param name="dirPath">根目录</param>
        /// <param name="exNameArr">要查找的后缀名,写法如 new string[] { "*.prefab"} 或 new string[] { "*.*"}</param>
        /// <param name="list">查找到的所有文件的完整路径</param>
        public static List<string> FindFile(string dirPath, List<string> exNameArr)
        {
            return FindFile(dirPath, exNameArr.ToArray());
        }

        public static List<string> FindFile(string dirPath, string[] exNameArr, SearchOption option= SearchOption.AllDirectories)
        {
            List<string> list = new List<string>();
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

            if (dirInfo.Exists == false)
            {
                return list;
            }

            //FileInfo[] fileInfoArr = new FileInfo[] { };

            for (int i = 0; i < exNameArr.Length; i++)
            {
                FileInfo[] fileInfoArr2 = dirInfo.GetFiles(exNameArr[i], option);

                foreach (FileInfo filePath in fileInfoArr2)
                {
                    string fileFullPath = filePath.FullName.Replace("\\", "/");
                    list.Add(fileFullPath);
                }
            }

            return list;
        }

        public static void SaveUTF8(string content, string path)
        {
            byte[] bytes=Encoding.UTF8.GetBytes(content);
           SaveBytes(bytes,path);
        }

        public static DirectoryInfo[] FinDirectory(string dirPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            if (dirInfo.Exists == false)
            {
                DebugX.Log(dirPath+":不存在");
                return new DirectoryInfo[0];
            }
            return dirInfo.GetDirectories();
        }

        public static void Copy(string sourceFileName, string destFileName,bool overrideIt=true)
        {
            if (overrideIt == false)
            {
                if (File.Exists(destFileName) == true)
                {
                    return;
                }
            }
            AutoCreateDirectory(destFileName);
            File.Copy(sourceFileName,destFileName, overrideIt);
        }

        public static void CopyDirectory(string sourcePath, string destPath)
        {
            if (!Directory.Exists(destPath))
            {
                Directory.CreateDirectory(destPath);
            }
            DirectoryInfo dir = new DirectoryInfo(sourcePath);
            //先复制文件
            CopyFile(dir, destPath);
            //再复制子文件夹
            DirectoryInfo[] dirs = dir.GetDirectories();
            if (dirs.Length > 0)
            {
                foreach (DirectoryInfo temDirectoryInfo in dirs)
                {
                    string sourceDirectoryFullName = temDirectoryInfo.FullName;
                    string destDirectoryFullName = sourceDirectoryFullName.Replace(sourcePath, destPath);
                    if (!Directory.Exists(destDirectoryFullName))
                    {
                        Directory.CreateDirectory(destDirectoryFullName);
                    }
                    CopyFile(temDirectoryInfo, destDirectoryFullName);
                    CopyDirectory(sourceDirectoryFullName, destDirectoryFullName);
                }
            }
        }

        /// <summary>
        /// 拷贝目录下的所有文件到目的目录。
        /// </summary>
        /// <param >源路径</param>
        /// <param >目的路径</param>
        private static void CopyFile(DirectoryInfo path, string desPath)
        {
            string sourcePath = path.FullName;
            FileInfo[] files = path.GetFiles();
            foreach (FileInfo file in files)
            {
                string sourceFileFullName = file.FullName;
                string destFileFullName = sourceFileFullName.Replace(sourcePath, desPath);
                file.CopyTo(destFileFullName, true);
            }
        }

        public static string GetDirectory(string path,bool isFull=true)
        {
            
            FileInfo fileInfo=new FileInfo(path);

            if (isFull)
            {
                return fileInfo.Directory.FullName;
            }

            return fileInfo.DirectoryName;
        }

        public static void SaveBytes(byte[] bytes, string itemPath)
        { 
            AutoCreateDirectory(itemPath);
            try
            {
                File.WriteAllBytes(itemPath, bytes);
            }
            catch (Exception ex)
            {
                DebugX.Log("saveBytesError:"+ex.Message);
            }
        }
    }
}
