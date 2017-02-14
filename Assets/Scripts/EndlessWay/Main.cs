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
		public const int MaxSettedObjectsTotal = 10000;
		public const int AreaOvermeasure = 10;

		private float updateTestPeriod = 1f;
		public float density = .5f;
		public int maxObjects = 1000;
		public float movementSpeed = .5f;
		public float objectsAreaDepth = 1000;
		public float objectsAreaWidth = 1000;
		public float objectsAreaOffset = 10;
		public float areaFillsInterval = 10;

		public Transform objectsParent;
		public EnvObject[] envObjectPrefabs;
		public EnvObjectSpecification[] specifications;

		/// <summary>
		/// Префабы со своими спецификациями - по подтипам 
		/// </summary>
		private Dictionary<string, KeyValuePair<EnvObject, EnvObjectSpecification>> _objectInfosByPrototypeName;

		private Composer _composer;
		private ObjectPoolsManager _objectPoolsManager;
		private List<IAreaObject> _allAreaObjects = new List<IAreaObject>(MaxSettedObjectsTotal);

		private Transform _cameraTransform;
		private Vector3 _lastAreaFillPosition;
		private float _areaFillsIntervalSqr;

		//Tests
		private float _lastTime;
		private bool _isTimeToCreate = false;

		private Type _selfType = typeof(Main);


		//=== Props ===========================================================

		private Vector2 NearAreaLBCorner
		{
			get
			{
				return new Vector2(
					_cameraTransform.localPosition.x - objectsAreaWidth / 2,
					_cameraTransform.localPosition.z + objectsAreaOffset);
			}
		}

		private Vector2 NearAreaRTCorner
		{
			get
			{
				return new Vector2(
					_cameraTransform.localPosition.x + objectsAreaWidth / 2,
					_cameraTransform.localPosition.z + objectsAreaOffset + objectsAreaDepth);
			}
		}

		private Vector2 FarAreaLBCorner
		{
			get
			{
				return NearAreaLBCorner + new Vector2(0, objectsAreaDepth);
			}
		}

		private Vector2 FarAreaRTCorner
		{
			get
			{
				return NearAreaRTCorner + new Vector2(0, areaFillsInterval);
			}
		}

		private Vector2 BehindAreaLBCorner
		{
			get
			{
				return new Vector2(
					_cameraTransform.localPosition.x - objectsAreaWidth / 2 - AreaOvermeasure,
					_cameraTransform.localPosition.z - objectsAreaDepth);
			}
		}

		private Vector2 BehindAreaRTCorner
		{
			get
			{
				return new Vector2(
					_cameraTransform.localPosition.x + objectsAreaWidth / 2 + AreaOvermeasure,
					_cameraTransform.localPosition.z - AreaOvermeasure);
			}
		}


		//=== Unity ===========================================================

		private void Start()
		{
			if (envObjectPrefabs.IsNull("envObjectPrefabs", _selfType) ||
				specifications.IsNull("specifications", _selfType) ||
				objectsParent.IsNull("objectsParent", _selfType))
				return;

			_areaFillsIntervalSqr = areaFillsInterval * areaFillsInterval;
			int maxObjectsCountByPrototype = MaxSettedObjectsTotal / envObjectPrefabs.Length;
			_objectInfosByPrototypeName =
				new Dictionary<string, KeyValuePair<EnvObject, EnvObjectSpecification>>(maxObjectsCountByPrototype);
			for (int i = 0, len = envObjectPrefabs.Length; i < len; i++)
			{
				var envObject = envObjectPrefabs[i];
				if (envObject.IsNull("envObjectPrefabs[" + i + "]", _selfType))
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

			_objectPoolsManager = new ObjectPoolsManager(Vector3.up * 100 + Vector3.back * 100);
			_objectPoolsManager.Init(prefabsByPrototypeName, maxObjectsCountByPrototype);
			_composer = new Composer(areaObjectSpecifications, _objectPoolsManager, unityRandom);

			_cameraTransform = Camera.main.transform;
			_lastAreaFillPosition = _cameraTransform.localPosition;

			FirstFill();
			//TestAtStart();
		}

		private void Update()
		{
			var cameraNewPos = new Vector3(
				_cameraTransform.localPosition.x,
				_cameraTransform.localPosition.y,
				_cameraTransform.localPosition.z + movementSpeed * Time.deltaTime);
			_cameraTransform.localPosition = cameraNewPos;
			if (Vector3.SqrMagnitude(_lastAreaFillPosition - cameraNewPos) > _areaFillsIntervalSqr)
			{
				_lastAreaFillPosition = cameraNewPos;
				_allAreaObjects = _composer.ClearArea(_allAreaObjects, BehindAreaLBCorner, BehindAreaRTCorner);
				Logs.Log("- setted={0} free={1}", _allAreaObjects.Count, _objectPoolsManager.FreeObjectsCount);
				_allAreaObjects.AddRange(_composer.FillArea(FarAreaLBCorner, FarAreaRTCorner, objectsParent, density, maxObjects));
				Logs.Log("+ setted={0} free={1}", _allAreaObjects.Count, _objectPoolsManager.FreeObjectsCount);
			}


			//			UpdateTest();
		}


		//=== Private =========================================================

		private void FirstFill()
		{
			_allAreaObjects.AddRange(_composer.FillArea(NearAreaLBCorner, NearAreaRTCorner, objectsParent, density, maxObjects));
			Logs.Log("1 setted={0} free={1}", _allAreaObjects.Count, _objectPoolsManager.FreeObjectsCount);

		}

		private void UpdateTest()
		{
			var newTime = Time.time;
			if (newTime - _lastTime < updateTestPeriod)
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

		private void TestAtStart()
		{
			//			
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

	}
}
