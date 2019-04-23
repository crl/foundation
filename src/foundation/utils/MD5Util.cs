using System;
using System.Security.Cryptography;
using UnityEngine;

namespace foundation
{
    public class MD5Util
    {
        public static string MD5Byte9(byte[] buffer)
        {
            string strResult = Hash(buffer);
            strResult=strResult.Substring(0, 9);
            return strResult;
        }


        public static string Hash(byte[] buffer,bool isUpper=false)
        {
            string strResult = "";
            string strHashData = "";
            byte[] arrbytHashValue;
            MD5CryptoServiceProvider oMD5Hasher =
                       new MD5CryptoServiceProvider();
            try
            {
                arrbytHashValue = oMD5Hasher.ComputeHash(buffer);//计算指定Stream 对象的哈希值  
                //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”  
                strHashData = BitConverter.ToString(arrbytHashValue);
                //替换-  
                strHashData = strHashData.Replace("-", "");
                if (isUpper == false)
                {
                    strResult = strHashData.ToLower();
                }
            }
            catch (Exception ex)
            {
                DebugX.Log("md5Error:"+ex.Message);
            }
            return strResult;
        }
    }

}

