using System;
using System.Collections.Generic;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class SimpleInstantiator : IAreaObjectSource
	{
		private Dictionary<string, EnvObject> _prefabsByPrototypeName;

		private Type _selfType = typeof(SimpleInstantiator);


		//=== Public ==========================================================

		public void Init(Dictionary<string, EnvObject> prefabsByPrototypeName, int maxObjectsByPrototype)
		{
			if (prefabsByPrototypeName == null)
				throw new NullReferenceException("prefabsByPrototypeName is null");

			_prefabsByPrototypeName = prefabsByPrototypeName;
		}

		public IAreaObject GetObject(string objectPrototypeName, Transform parentTransform)
		{
			EnvObject prefabByPrototypeName;
			if (!_prefabsByPrototypeName.TryGetValue(objectPrototypeName, out prefabByPrototypeName))
			{
				Logs.LogError("Not found prefabByPrototypeName '{0}'", objectPrototypeName);
				return null;
			}

			return UnityEngine.Object.Instantiate<EnvObject>(prefabByPrototypeName, parentTransform);
		}

		public void ReleaseObject(IAreaObject areaObject)
		{
			var envObject = (EnvObject)areaObject;
			if (envObject.IsNull("envObject", _selfType))
				return;

			var go = envObject.gameObject;
			UnityEngine.Object.Destroy(go);
		}
	}
}
