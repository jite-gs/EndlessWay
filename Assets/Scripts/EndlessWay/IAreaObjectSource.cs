using System.Collections.Generic;
using UnityEngine;

namespace EndlessWay
{
	public interface IAreaObjectSource
	{
		void Init(Dictionary<string, EnvObject> prefabsByPrototypeName, int maxObjectsByPrototype);
		IAreaObject GetObject(string objectPrototypeName, Transform parentTransform);
		void ReleaseObject(IAreaObject areaObject);
	}
}
