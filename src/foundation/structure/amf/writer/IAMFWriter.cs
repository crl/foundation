namespace foundation
{
    interface IAMFWriter
	{

		bool IsPrimitive{ get; }

		void WriteData(AMFWriter writer, object data);
	}
}
