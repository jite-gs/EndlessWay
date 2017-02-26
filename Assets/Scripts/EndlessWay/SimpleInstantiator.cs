using System;
using System.Collections.Generic;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class SimpleInstantiator : IAreaObjectSource
	{
		private Dictionary<string, EnvObject> _prefabsByPrototypeName;

		private bool _isVerbose = false; 
		
		private Type _selfType = typeof(SimpleInstantiator);


		//=== Props ===========================================================

		public int FreeObjectsCount { get { return 0; } }

		public int Capacity { get; set; }

		public int ObjectsCount { get; set; }


		//=== Public ==========================================================

		public void Init(Dictionary<string, EnvObject> prefabsByPrototypeName, int capacity)
		{
			if (prefabsByPrototypeName == null)
				throw new NullReferenceException("prefabsByPrototypeName is null");

			_prefabsByPrototypeName = prefabsByPrototypeName;
			Capacity = capacity;
		}

		public IAreaObject GetObject(string objectPrototypeName, Transform parentTransform)
		{
			if (ObjectsCount >= Capacity)
			{
				if (_isVerbose)
					Logs.Log("GetObject() Return null cause Capacity={0}", Capacity); //DEBUG
				return null;
			}

			EnvObject prefabByPrototypeName;
			if (!_prefabsByPrototypeName.TryGetValue(objectPrototypeName, out prefabByPrototypeName))
			{
				Logs.LogError("Not found prefabByPrototypeName '{0}'", objectPrototypeName);
				return null;
			}

			ObjectsCount++;
			return UnityEngine.Object.Instantiate<EnvObject>(prefabByPrototypeName, parentTransform);
		}

		public void ReleaseObject(IAreaObject areaObject)
		{
			var envObject = (EnvObject)areaObject;
			if (envObject.IsNull("envObject", _selfType))
				return;

			var go = envObject.gameObject;
			UnityEngine.Object.Destroy(go);
			ObjectsCount--;
		}
	}
}
