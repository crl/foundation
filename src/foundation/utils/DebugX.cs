using UnityEngine;
using System;
#if !UNITY_WEBPLAYER
using System.IO;
#endif

namespace foundation
{
    public interface ILog
    {
        void log(string msg);
    }

    /// <summary>
    /// 调试辅助类.
    /// </summary>
    public class DebugX
    {
        public static bool enabled = true;

        /// <summary>
        /// 是否写入到文件
        /// </summary>
        public static bool isWriteFile = true;
        /// <summary>
        /// 日志的外部UI提示
        /// </summary>
        public static Action<string, LogType> UITipHandle;

        private static string outLogPath = "";
        private static StreamWriter writer;
        private static FileStream fs;

        /// <summary>
        /// 初始化日志
        /// </summary>
        /// <param name="fileName">日志文件名字</param>
        public static void UnhandledExceptionInit(string fileName = "")
        {
            //Application.logMessageReceived += handleException;
            Application.logMessageReceived += handleException;

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "lingyuLog.txt";
            }

            outLogPath = Application.persistentDataPath + "/" + fileName;
            bool autoCreate = false;
            if (File.Exists(outLogPath) == false)
            {
                autoCreate = true;
            }
            else
            {
                FileInfo fileInfo = new FileInfo(outLogPath);

                if (fileInfo.Length > 1 * 1024 * 1024)
                {
                    try
                    {
                        File.Delete(outLogPath);
                        autoCreate = true;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("删除log失败:" + e.Message);
                    }
                }
            }

            if (autoCreate)
            {
                writer = File.CreateText(outLogPath);
                writer.WriteLine(".....................");
            }
            else
            {
                fs = new FileStream(outLogPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                writer = new StreamWriter(fs);
            }

        }

        public static void Release()
        {
            if (writer != null)
            {
                writer.Close();
                writer.Dispose();
                writer = null;
            }
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
                fs = null;
            }
        }

        private static void handleException(string condition, string stackTrace, LogType type)
        {
            string msg = StringUtil.substitute("{0}:{1}", condition, stackTrace);

            if (UITipHandle != null)
            {
                UITipHandle(msg, type);
            }
            ///只是错误才写入文件
            if ((type == LogType.Error || type == LogType.Exception) && isWriteFile)
            {
                formatLogFile(msg, type.ToString());
            }
        }

        public static void Log(string message, params object[] args)
        {
            if (!DebugX.enabled || message == null)
            {
                return;
            }
            string msg = StringUtil.substitute(message, args);
            Debug.Log(msg);
        }

        public static void LogWarning(string message, params object[] args)
        {
            if (!DebugX.enabled || message == null)
            {
                return;
            }
            string msg = StringUtil.substitute(message, args);
            Debug.LogWarning(msg);
        }

        public static void LogError(string message, params object[] args)
        {
            if (!DebugX.enabled || message == null)
            {
                return;
            }

            string msg = StringUtil.substitute(message, args);
            Debug.LogError(msg);
        }

        private static object writerLock = new object();
        private static void formatLogFile(string msg, string type)
        {
            string now = DateUtils.GetSimple(DateTime.Now);
            string message = StringUtil.substitute("[{0}:{1}]\t{2}", now, type, msg);

            lock (writerLock)
            {
                try
                {
                    writer.Write(message + "\n");
                    writer.Flush();
                }
                catch (Exception) { }
            }
        }
    }


}