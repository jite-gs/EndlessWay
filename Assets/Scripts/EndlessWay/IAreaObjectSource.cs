using System.Collections.Generic;
using UnityEngine;

namespace EndlessWay
{
	public interface IAreaObjectSource
	{
		void Init(Dictionary<string, EnvObject> prefabsByPrototypeName);
		IAreaObject GetObject(string objectPrototypeName, Transform parenTransform);
		void ReleaseObject(IAreaObject areaObject);
	}
}
