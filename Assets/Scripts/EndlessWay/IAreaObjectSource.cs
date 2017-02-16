using System.Collections.Generic;
using UnityEngine;

namespace EndlessWay
{
	public interface IAreaObjectSource
	{
		int FreeObjectsCount { get; }
		void Init(Dictionary<string, EnvObject> prefabsByPrototypeName, int maxObjectsByPrototype);
		IAreaObject GetObject(string objectPrototypeName, Transform parentTransform);
		void ReleaseObject(IAreaObject areaObject);
	}
}
