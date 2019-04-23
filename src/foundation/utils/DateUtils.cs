using System;
using System.Collections.Generic;
using UnityEngine;

namespace foundation
{
    public class DateUtils
    {
        private static readonly uint[] _daysInMonth = new uint[] {31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};

        private static long GELIN_OFFSET_MILLISECOND=0 ;
        static DateUtils()
        {
            GELIN_OFFSET_MILLISECOND = DateTime.Parse("1970-1-1").Ticks/ ONE_MILLISECONDS_TICK;
        }
        /// <summary>
        /// 一分种
        /// </summary>
        public const int ONE_MINUTE_SECONDS = 60;
        public const int ONE_MINUTE_MILLISECOND =60000;
        /// <summary>
        ///  一小时;
        /// </summary>
        public const int ONE_HOURS_SECOND = 3600;
        public const int ONE_HOURS_MILLISECOND =3600000;

        /// <summary>
        /// 半小时
        /// </summary>
        public const int HALF_HOURS_SECOND = 1800;
        public const int HALF_HOURS_MILLISECOND =1800000;

        /// <summary>
        ///  一天
        /// </summary>
        public const int ONE_DAY_SECOND = 86400;
        public const int ONE_DAY_MILLISECOND =86400000;


        public const int ONE_MILLISECONDS_TICK = 10000;
        public const int ONE_SECOND_MILLISECOND = 1000;
        public const int ONE_SECONDS_TICK = 10000000;

        public static bool IsLeapYear(DateTime d)
        {
            return (d.Year%4 == 0) ? true : false;
        }

        public static uint DaysInMonth(DateTime d)
        {
            uint i = 0;
            if (IsLeapYear(d) && d.Month == 1)
            {
                i = 1;
            }

            i += _daysInMonth[d.Month];
            return i;
        }

        /// <summary>
        /// 加入格林尼治起始数
        /// </summary>
        /// <param name="value">已去除格林尼治起始数的毫秒</param>
        /// <returns>加入格林尼治起始数</returns>
        public static long FormatGelinTime(long milliseconds)
        {
            return milliseconds + GELIN_OFFSET_MILLISECOND;
        }

        public static void TraceBySeconds(float seconds, bool hasGelinOffset = true)
        {
            TraceByMilliseconds((long)(seconds * 1000), hasGelinOffset);
        }

        public static void TraceByMilliseconds(long milliseconds, bool hasGelinOffset = true)
        {
            if (hasGelinOffset)
            {
                milliseconds += GELIN_OFFSET_MILLISECOND;
            }
            DateTime time = GetDateTimeByMilliseconds(milliseconds);
            Debug.Log(time.ToString());
        }
        public static DateTime GetDateTimeBySeconds(float seconds)
        {
            DateTime time = new DateTime((long)(seconds*ONE_SECONDS_TICK));
            return time;
        }
        public static DateTime GetDateTimeByMilliseconds(long milliseconds)
        {
            DateTime time = new DateTime(milliseconds * ONE_MILLISECONDS_TICK);
            return time;
        }

        protected static string addZero(int value, int count)
        {
            string result = value.ToString();
            while (result.Length < count)
            {
                result = "0" + result;
            }

            return result;
        }

        /// <summary>
        /// 比较日期
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static int CompareDates(DateTime first, DateTime second)
        {
            long d1ms = first.Ticks;
            long d2ms = second.Ticks;

            if (d1ms > d2ms)
            {
                return -1;
            }
            else if (d1ms < d2ms)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public static List<List<char>> GetOutputType(string value)
		{
			List<List<char> > resultList=new List<List<char>>();
            char tempChar =' ';
			List<char> tempList=null;
			int len=value.Length;
			for (int i=0; i<len; i++)
			{
				char newChar=value[i];
				if (newChar != tempChar)
				{
                    tempList = new List<char>();
                    resultList.Add(tempList);
				}
                tempList.Add(newChar);
				tempChar= newChar;
			}
			return resultList;
		}

        /// <summary>
        /// 返回格式{dd:hh:mm:ss}
        /// </summary>
        /// <param name="seconds"></param>
        /// <param name="type"></param>
        /// <param name="auto"></param>
        /// <returns></returns>
        public static string GetCountdownStringBySecond(float seconds, string type = "HH:mm:ss", bool auto = false)
        {
            return GetCountdownStringByMilliseconds((long)(seconds * 1000), type, auto);
        }

        /// <summary>
        /// 返回格式{dd:hh:mm:ss}
        /// 只用于显示倒计时
        /// </summary>
        public static string GetCountdownStringByMilliseconds(long milliseconds, string type = "HH:mm:ss", bool auto = false)
        {
            string result = "";
            int day = (int) (milliseconds / ONE_DAY_MILLISECOND);
            milliseconds %= ONE_DAY_MILLISECOND;
            int hour = (int) (milliseconds / ONE_HOURS_MILLISECOND);
            milliseconds %= ONE_HOURS_MILLISECOND;
            int minutes = (int) (milliseconds / ONE_MINUTE_MILLISECOND);
            milliseconds %= ONE_MINUTE_MILLISECOND;
            int second = (int) (milliseconds/ ONE_SECOND_MILLISECOND);

            List<List<char>> tempList = GetOutputType(type);
            List<char> arr;
            int len = tempList.Count;
            for (int i = 0; i < len; i++)
            {
                arr = tempList[i];
                switch (arr[0])
                {
                    case 'd':
                        if (auto && day == 0)
                        {
                            break;
                        }
                        result += addZero(day, arr.Count);
                        break;

                    case 'h':
                    case 'H':
                        if (auto && day == 0)
                        {
                            result = "";
                            if (hour == 0)
                            {
                                break;
                            }
                        }
                        result += addZero(hour, arr.Count);
                        break;

                    case 'm':
                        if (auto && hour == 0)
                        {
                            result = "";
                            if (minutes == 0)
                            {
                                break;
                            }
                        }
                        result += addZero(minutes, arr.Count);
                        break;
                    case 's':
                        if (auto && minutes == 0)
                        {
                            result = "";
                        }
                        result += addZero(second, arr.Count);
                        break;
                    default:
                        int tlen = arr.Count;
                        for (int j = 0; j < tlen; j++)
                        {
                            result += arr[j];
                        }
                        break;
                }
            }
            return result;
        }



        public static string GetSimple(DateTime date)
        {
            return date.Month + "/" + date.Day + " " + date.Hour + ":" + date.Minute + ":" + date.Second;
        }


        public static long GetDayStartByMilliseconds(long milliseconds)
        {
            DateTime sharedDate = new DateTime((milliseconds* ONE_MILLISECONDS_TICK));
            DateTime temp = new DateTime(sharedDate.Year, sharedDate.Month, sharedDate.Day, 0, 0, 0);
            return (temp.Ticks / ONE_MILLISECONDS_TICK);
        }

        static public long GetDayEndByMilliseconds(long milliseconds)
		{
            DateTime sharedDate = new DateTime((milliseconds * ONE_MILLISECONDS_TICK));

            DateTime temp = new DateTime(sharedDate.Year, sharedDate.Month, sharedDate.Day, 23,59, 59);

            return (temp.Ticks/ ONE_MILLISECONDS_TICK);
        }

        /// <summary>
        /// 取得当前时分秒
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns></returns>
        public static int GetDayByMilliseconds(long milliseconds)
        {
            DateTime sharedDate = new DateTime(milliseconds * ONE_MILLISECONDS_TICK);
            return sharedDate.Hour*ONE_HOURS_MILLISECOND + sharedDate.Minute*ONE_MINUTE_MILLISECOND + sharedDate.Second*ONE_SECOND_MILLISECOND;
        }
        public static int GetDayBySeconds(float seconds)
        {
            return GetDayByMilliseconds((long)(seconds * ONE_SECOND_MILLISECOND));
        }

        public static int GetHourByMilliseconds(long milliseconds)
        {
            DateTime sharedDate = new DateTime((milliseconds * ONE_MILLISECONDS_TICK));
            return sharedDate.Hour;
        }

        public static int GetHourBySeconds(float seconds)
        {
            DateTime sharedDate = new DateTime((long)(seconds * ONE_SECONDS_TICK));
            return sharedDate.Hour;
        }
    }
}