namespace EndlessWay
{
	public interface IAreaObjectSource
	{
		IAreaObject GetObject(string objectPrototypeName);
		void Release(IAreaObject areaObject);
	}
}