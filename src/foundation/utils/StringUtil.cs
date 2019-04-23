
using System.Text;

/// <summary>
/// 字符串处理类
/// </summary>
public class StringUtil
{
    public static string trim(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        else
        {
            return value.Trim();
        }
    }


    public static string ltrim(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        else
        {
            return value.TrimStart();
        }
    }

    public static string rtrim(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        else
        {
            return value.TrimEnd();
        }
    }

    public static bool isWhitespace(string character)
    {
        switch (character)
        {
            case " ":
            case "\t":
            case "\r":
            case "\n":
            case "\f":
                return true;
        }

        return false;
    }

    public static string substitute(string value, params object[] parms)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }
        string v;
        int len = parms.Length;
        for (int i = 0; i < len; i++)
        {
            object vo=parms[i];
            if (vo == null)
            {
                continue;
            }
            v=vo.ToString();
            value = value.Replace("{" + i + "}", v);
        }
        return value;
    }

    public static string substitute(string value, string[] parms)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "";
        }

        if (parms == null)
        {
            return value;
        }

        string v;
        int len = parms.Length;
        for (int i = 0; i < len; i++)
        {
            object vo = parms[i];
            if (vo == null)
            {
                continue;
            }
            v = vo.ToString();
            value = value.Replace("{" + i + "}", v);
        }
        return value;
    }

    /// <summary>
    /// 去除数字
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static string trimDig(string s)
    {
        int l = s.Length;
        StringBuilder sb=new StringBuilder();
        int i = 0;
        while (i<l)
        {
            char c=s[i];
            if (c >= '0' && c <= '9')
            {
                //是数字
            }
            else
            {
                sb.Append(c);
            }
            i++;
        }

        return sb.ToString();
    }

    /// <summary>
    /// 计算一个字符串所占用的字节数，英文占用1个，中文占用2个
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static int GerStringLen(string str)
    {
        int num = 0;
        foreach (char c in str)
        {
            if ((int) c < 128)
            {
                num++;
            }
            else
            {
                num += 2;
            }
        }
        return num;
    }
}
