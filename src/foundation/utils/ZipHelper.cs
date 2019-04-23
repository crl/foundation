using System;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

namespace foundation
{
    public class ZipHelper
    {
        public static IEnumerator UnZip(byte[] bytes, string destDirectory, bool overWrite, Action completeAction, Action<float> progressAction = null)
        {
            float preTime = Time.realtimeSinceStartup;
            int len = bytes.Length;
            MemoryStream memoryStream = new MemoryStream(bytes);
            using (ZipInputStream s = new ZipInputStream(memoryStream))
            {
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                    {
                        directoryName = Path.GetDirectoryName(pathToZip) + @"/";
                    }
                    string fileName = Path.GetFileName(pathToZip);

                    Directory.CreateDirectory(destDirectory + directoryName);

                    if (fileName != "")
                    {
                        string fullPath = destDirectory + directoryName + fileName;
                        bool isExists = File.Exists(fullPath);
                        if (isExists)
                        {
                            if (overWrite)
                            {
                                File.Delete(fullPath);
                            }
                            else
                            {
                                continue;
                            }
                        }

                        using (FileStream streamWriter = File.Create(fullPath))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);

                                if (size > 0)
                                    streamWriter.Write(data, 0, size);
                                else
                                    break;
                            }
                            streamWriter.Close();
                        }
                    }


                    if (Time.realtimeSinceStartup - preTime > 0.1f)
                    {
                        if (progressAction != null)
                        {
                            progressAction(memoryStream.Position / (float)len);
                        }
                        preTime = Time.realtimeSinceStartup;
                        yield return null;
                    }
                }
                s.Close();
            }

            if (completeAction != null)
            {
                completeAction();
            }
        }
    }
}