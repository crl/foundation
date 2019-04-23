using System.IO;
using System.Text;
using CompressionMode = Ionic.Zlib.CompressionMode;
using GZipStream = Ionic.Zlib.GZipStream;

/// <summary>
/// 对字符串进行压缩,字符编码为UTF-8
/// </summary>
public class GZipStrUtil
{
    /// <summary>
    /// 对字符串进行压缩
    /// </summary>
    /// <param name="str">待压缩的字符串</param>
    /// <returns>压缩后的字符串</returns>
    public static string CompressString(string str)
    {
        string compressString = "";
        byte[] compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
        byte[] compressAfterByte = Compress(compressBeforeByte);
        compressString = Encoding.GetEncoding("ISO-8859-1").GetString(compressAfterByte);
        //compressString = Convert.ToBase64String(compressAfterByte);
        return compressString;
    }
    /// <summary>
    /// 对字符串进行解压缩
    /// </summary>
    /// <param name="str">待解压缩的字符串</param>
    /// <returns>解压缩后的字符串</returns>
    public static string DecompressString(string str)
    {
        string compressString = "";
        byte[] compressBeforeByte = Encoding.GetEncoding("ISO-8859-1").GetBytes(str);
        //byte[] compressBeforeByte = Convert.FromBase64String(str);
        byte[] compressAfterByte = Decompress(compressBeforeByte);
        compressString = Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
        return compressString;
    }

    /// <summary>
    /// 对byte数组进行压缩
    /// </summary>
    /// <param name="data">待压缩的byte数组</param>
    /// <returns>压缩后的byte数组</returns>
    public static byte[] Compress(byte[] data)
    {
        try
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(data, 0, data.Length);
            zip.Close();
            byte[] buffer = new byte[ms.Length];
            ms.Position = 0;
            ms.Read(buffer, 0, buffer.Length);
            ms.Close();
            return buffer;

        }
        catch (IOException e)
        {
            throw new IOException(e.Message);
        }
    }

    public static byte[] Decompress(byte[] data)
    {
        try
        {
            MemoryStream ms = new MemoryStream(data);
            GZipStream zip = new GZipStream(ms, CompressionMode.Decompress, true);
            MemoryStream msreader = new MemoryStream();
            byte[] buffer = new byte[0x1000];
            while (true)
            {
                int reader = zip.Read(buffer, 0, buffer.Length);
                if (reader <= 0)
                {
                    break;
                }
                msreader.Write(buffer, 0, reader);
            }
            zip.Close();
            ms.Close();
            msreader.Position = 0;
            buffer = msreader.ToArray();
            msreader.Close();
            return buffer;
        }
        catch (IOException e)
        {
            throw new IOException(e.Message);
        }
    }
}
