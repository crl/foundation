using System;
using System.Collections;
using System.Text;

namespace foundation
{
    public static class StringExtension
    {
        public static string[] As3Split(this string self,string sp)
        {
            return self.Split(new string[] {sp}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void Clear(this StringBuilder self)
        {
            self.Length = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="sp"></param>
        /// <param name="start"> </param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string As3Join(this IList self, string sp,int start=0,int end=-1)
        {
            StringBuilder sb = new StringBuilder();
            int len = self.Count;
            if (len == 0)
            {
                return "";
            }

            if (end == -1)
            {
                end = len;
            }

            if (start > end)
            {
                return "";
            }

            sb.Append(self[start]);
            for (int i = start+1; i < end; i++)
            {
                sb.Append(sp);
                sb.Append(self[i]);
            }

            return sb.ToString();
        }
    }
}