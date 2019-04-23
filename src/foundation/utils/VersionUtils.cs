namespace foundation
{
    public class VersionUtils
    {
        public static int Get(string v)
        {
            string[] vs=v.Split('.');
            v = vs.As3Join("");
            int len = v.Length;
            int result = 0;
            int avg = 10000000;
            for (int i = 0; i < len; i++)
            {
                int j =int.Parse(v[i].ToString());
                result += j*avg;
                avg = avg / 10;
            }

            return result;
        } 
    }
}