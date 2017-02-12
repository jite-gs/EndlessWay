using UnityEngine;

namespace EndlessWay
{
	public class SimpleInstantiator : IAreaObjectSource
	{
		public IAreaObject GetObject(string objectPrototypeName)
		{
			throw new System.NotImplementedException();
		}

		public void Release(IAreaObject areaObject)
		{
			throw new System.NotImplementedException();
		}
	}
}
