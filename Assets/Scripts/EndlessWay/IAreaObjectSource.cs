using System.Collections.Generic;
using UnityEngine;

namespace EndlessWay
{
	public interface IAreaObjectSource
	{
		int FreeObjectsCount { get; }
		int Capacity { get; set; }
		void Init(Dictionary<string, EnvObject> prefabsByPrototypeName, int capacity);
		IAreaObject GetObject(string objectPrototypeName, Transform parentTransform);
		void ReleaseObject(IAreaObject areaObject);
	}
}
