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
	public class Main : MonoBehaviour
	{
		public float period = 1f;
		public Transform objectsParent;
		public EnvObject[] envObjects;
		public EnvObjectSpecification[] specifications;

		/// <summary>
		/// Префабы со своими спецификациями - по подтипам 
		/// </summary>
		private Dictionary<string, KeyValuePair<EnvObject, EnvObjectSpecification>> _objectInfosByPrototypeName
			= new Dictionary<string, KeyValuePair<EnvObject, EnvObjectSpecification>>(5000);

		private Composer _composer;
		private ObjectPoolsManager _objectPoolsManager;
		private List<IAreaObject> _allAreaObjects = new List<IAreaObject>(5000);

		private Type _selfType = typeof(Main);


		//=== Unity ==========================================================

		private void Start()
		{
			if (envObjects.IsNull("envObjects", _selfType) ||
				specifications.IsNull("specifications", _selfType) ||
				objectsParent.IsNull("objectsParent", _selfType))
				return;

			for (int i = 0, len = envObjects.Length; i < len; i++)
			{
				var envObject = envObjects[i];
				if (envObject.IsNull("envObjects[" + i + "]", _selfType))
					continue;

				if (_objectInfosByPrototypeName.ContainsKey(envObject.name))
				{
					Logs.LogError("_objectInfosByPrototypeName already contains envObject with name '{0}'", envObject.name);
					continue;
				}

				_objectInfosByPrototypeName.Add(envObject.name, new KeyValuePair<EnvObject, EnvObjectSpecification>(envObject, null));
			}

			var unityRandom = new UnityRandom();
			for (int i = 0, len = specifications.Length; i < len; i++)
			{
				var specification = specifications[i];
				if (specification.IsNull("specification[" + i + "]", _selfType) ||
					!specification.Init(unityRandom))
					continue;

				KeyValuePair<EnvObject, EnvObjectSpecification> kvp;
				if (!_objectInfosByPrototypeName.TryGetValue(specification.associatedEnvObjectName, out kvp))
				{
					Logs.LogError("_objectInfosByPrototypeName don't contains envObject '{0}' for specification[{0}]",
						specification.associatedEnvObjectName, i);
					continue;
				}

				_objectInfosByPrototypeName[specification.associatedEnvObjectName] =
					new KeyValuePair<EnvObject, EnvObjectSpecification>(kvp.Key, specification);
			}

			var areaObjectSpecifications = new Dictionary<string, IAreaObjectSpecification>();
			var prefabsByPrototypeName = new Dictionary<string, EnvObject>();
			foreach (var kvp in _objectInfosByPrototypeName)
			{
				prefabsByPrototypeName.Add(kvp.Key, kvp.Value.Key);
				if (kvp.Value.Value == null)
				{
					Logs.Log("_objectInfosByPrototypeName['{0}'] hasn't specification", kvp.Key);
				}
				else
				{
					areaObjectSpecifications.Add(kvp.Key, kvp.Value.Value);
				}
			}

//			var simpleInstantiator = new SimpleInstantiator();
//			simpleInstantiator.Init(prefabsByPrototypeName, 10000);
//			_composer = new Composer(areaObjectSpecifications, simpleInstantiator, unityRandom);

			_objectPoolsManager = new ObjectPoolsManager();
			_objectPoolsManager.Init(prefabsByPrototypeName, 10000);
			_composer = new Composer(areaObjectSpecifications, _objectPoolsManager, unityRandom);

			TestAtStart();
		}

		private void TestAtStart()
		{
			//			_allAreaObjects.AddRange(_composer.FillArea(new Vector2(-300, -100), new Vector2(300, 100), objectsParent, .9f, 5000));
			//			_allAreaObjects = _composer.ClearArea(_allAreaObjects, new Vector2(-150, -50), new Vector2(-50, 50));
			//			_allAreaObjects = _composer.ClearArea(_allAreaObjects, new Vector2(50, -50), new Vector2(150, 50));
			//			_allAreaObjects = _composer.ClearArea(_allAreaObjects, new Vector2(-50, -20), new Vector2(50, 20));
		}

		private void TestUp()
		{
			_allAreaObjects.AddRange(_composer.FillArea(new Vector2(-400, -400), new Vector2(400, 400), objectsParent, .9f, 5000));
			_allAreaObjects.AddRange(_composer.FillArea(new Vector2(-10, -10), new Vector2(-100, -100), objectsParent, .9f, 5000));
//			Logs.Log("+ setted={0} free={1}", _allAreaObjects.Count, _objectPoolsManager.FreeObjectsCount);
		}

		private void TestDown()
		{
			_composer.ClearAllObjects(_allAreaObjects);
			_allAreaObjects.Clear();
//			Logs.Log("- setted={0} free={1}", _allAreaObjects.Count, _objectPoolsManager.FreeObjectsCount);
			//			_allAreaObjects = _composer.ClearArea(_allAreaObjects, new Vector2(-400, -400), new Vector2(400, 400));
		}


		private float _lastTime;
		private bool _isTimeToCreate = false;

		private void Update()
		{
			var newTime = Time.time;
			if (newTime - _lastTime < period)
				return;

			_lastTime = newTime;

			_isTimeToCreate = !_isTimeToCreate;

			if (_isTimeToCreate)
			{
				TestUp();
			}
			else
			{
				TestDown();
			}
		}


		//=== Private =========================================================





	}
}
