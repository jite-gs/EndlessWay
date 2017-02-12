using System;
using System.Collections.Generic;
using DebugStuff;
using SomeRandom;
using UnityEngine;

namespace EndlessWay
{
	/// <summary>
	/// Стартовый скрипт EndlessWay
	/// </summary>
	public class Stranger : MonoBehaviour
	{
		public EnvObject[] envObjects;
		public EnvObjectSpecification[] specifications;

		private Dictionary<string, KeyValuePair<EnvObject, EnvObjectSpecification>> _prefabs
			= new Dictionary<string, KeyValuePair<EnvObject, EnvObjectSpecification>>();

		private Type _selfType = typeof(Stranger);


		//=== Unity ==========================================================

		private void Start()
		{
			if (envObjects.IsNull("envObjects", _selfType) || specifications.IsNull("specifications", _selfType))
				return;

			for (int i = 0, len = envObjects.Length; i < len; i++)
			{
				var envObject = envObjects[i];
				if (envObject.IsNull("envObjects[" + i + "]", _selfType))
					continue;

				if (_prefabs.ContainsKey(envObject.name))
				{
					Logs.LogError("_prefabs already contains envObject with name '{0}'", envObject.name);
					continue;
				}

				_prefabs.Add(envObject.name, new KeyValuePair<EnvObject, EnvObjectSpecification>(envObject, null));
			}

			var unityRandom = new UnityRandom();
			for (int i = 0, len = specifications.Length; i < len; i++)
			{
				var specification = specifications[i];
				if (specification.IsNull("specification[" + i + "]", _selfType) ||
					!specification.Init(unityRandom))
					continue;

				KeyValuePair<EnvObject, EnvObjectSpecification> kvp;
				if (!_prefabs.TryGetValue(specification.associatedEnvObjectName, out kvp))
				{
					Logs.LogError("_prefabs don't contains envObject '{0}' for specification[{0}]",
						specification.associatedEnvObjectName, i);
					continue;
				}

				_prefabs[specification.associatedEnvObjectName] =
					new KeyValuePair<EnvObject, EnvObjectSpecification>(kvp.Key, specification);
			}

			foreach (var kvp in _prefabs)
			{
				if (kvp.Value.Value == null)
					Logs.Log("_prefabs['{0}'] hasn't specification", kvp.Key);
			}

			//			Logs.Log(_prefabs.VarDump("_prefabs")); //2del

			//			obj.SetSpecification(spec);
			//			obj.Setup();

		}



		private float _period = .33f;
		private float _lastTime;
		//		private float _pseudoRandomDelta = .01f;
		//		private float _pseudoRandom = 0;

		private void Update()
		{
			var newTime = Time.time;
			if (newTime - _lastTime < _period)
				return;

			_lastTime = newTime;


			//			for (int i = 0; i < 1000; i++)
			//			obj.Setup();
		}


		//=== Private =========================================================





	}
}
