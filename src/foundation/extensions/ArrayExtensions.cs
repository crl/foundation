namespace foundation
{
    public static class ArrayExtensions
    {

        public static bool GetBool(this object[] self, int index)
        {
            return self.GetValue<bool>(index);
        }

        public static int GetInt(this object[] self,int index)
        {
            return self.GetValue<int>(index);
        }

        public static float GetFloat(this object[] self, int index)
        {
            return self.GetValue<float>(index);
        }

        public static double GetDouble(this object[] self, int index)
        {
            return self.GetValue<float>(index);
        }

        public static long GetLong(this object[] self, int index)
        {
            return self.GetValue<long>(index);
        }

        public static string GetString(this object[] self, int index)
        {
            return self.GetValue<string>(index);
        }

        public static object[] GetArr(this object[] self,int index)
        {
            return self.GetValue<object[]>(index);
        }

        public static T GetValue<T>(this object[] self,int index)
        {
            object t = self[index];
            if (t == null)
            {
                return default(T);
            }
            return (T)t;
        }
    }
}