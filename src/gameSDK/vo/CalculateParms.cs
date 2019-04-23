namespace gameSDK
{
    public class CalculateParms<T>
    {
        public string key;
        public T value;
    }

    public class CalculateFloatParms:CalculateParms<float>
    {
    }
    public class CalculateIntParms : CalculateParms<int>
    {
    }
}