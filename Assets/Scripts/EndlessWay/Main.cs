using System;
using System.Collections.Generic;
using DebugStuff;
using SomeRandom;
using UnityEngine;
using UnityEngine.UI;

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
		/// <summary>
		/// Плотность заполнения объектами
		/// </summary>
		public float density = .1f;
		/// <summary>
		/// Максимальное число объектов при первом заполнении
		/// </summary>
		public int firstFillMaxObjects = 1000;
		/// <summary>
		/// Скорость движения камеры
		/// </summary>
		public float movementSpeed = .5f;
		/// <summary>
		/// Начальная глубина заполнения объектами
		/// </summary>
		public float objectsAreaDepth = 1000;
		/// <summary>
		/// Общая ширина заполнения обьектами
		/// </summary>
		public float objectsAreaWidth = 1000;

		/// <summary>
		/// Ширина пустого пространства непоср. перед камерой (дороги)
		/// </summary>
		public float centerEmptySpaceWidth = 10;

		/// <summary>
		/// Через какое расстояние делается заполнение объектов вдалеке
		/// </summary>
		public float areaFillsInterval = 50;
		/// <summary>
		/// Через какое расстояние делается очистка объектов сзади камеры
		/// </summary>
		public float areaClearInterval = 10;

		public Transform objectsParent;
		public EnvObject[] envObjectPrefabs;
		public EnvObjectSpecification[] specifications;

		/// <summary>
		/// Префабы со своими спецификациями - по подтипам 
		/// </summary>
		private Dictionary<string, KeyValuePair<EnvObject, EnvObjectSpecification>> _objectInfosByPrototypeName;

		private Composer _composer;
		private ObjectPoolsManager _objectPoolsManager;
		//private List<IAreaObject> _allAreaObjects = new List<IAreaObject>(MaxSettedObjectsTotal);
		private Queue<IAreaObject> _areaObjectsQueue = new Queue<IAreaObject>(MaxSettedObjectsTotal);

		private Transform _cameraTransform;
		private float _lastFillZ, _lastClearZ;

		//Tests
		private float _lastTime;
		private bool _isTimeToCreate = false;

		private Type _selfType = typeof(Main);


		//=== Unity ===========================================================

		private void Start()
		{
			if (envObjectPrefabs.IsNull("envObjectPrefabs", _selfType) ||
				specifications.IsNull("specifications", _selfType) ||
				objectsParent.IsNull("objectsParent", _selfType))
				return;

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

			FirstFill(_cameraTransform.localPosition.z);
			//TestAtStart();
		}

		private void Update()
		{
			_cameraTransform.localPosition = new Vector3(
				_cameraTransform.localPosition.x,
				_cameraTransform.localPosition.y,
				_cameraTransform.localPosition.z + movementSpeed * Time.deltaTime);

			var newCamZ = _cameraTransform.localPosition.z;

			UpdateClear(newCamZ);
			UpdateFill(newCamZ);

			UpdateTest();
		}


		//=== Public ==========================================================

		public void OnClick() //TODO
		{
			Logs.Log("{0} Click", Time.time);
		}

		public void OnSlider(float val) //TODO
		{

		}


		//=== Private =========================================================

		private void FirstFill(float currentCamZ)
		{
			_lastFillZ = _lastClearZ = currentCamZ;
			var nearZ = currentCamZ;
			var farZ = nearZ + objectsAreaDepth;

			FillArea(nearZ, farZ, objectsAreaWidth, centerEmptySpaceWidth, density, firstFillMaxObjects, objectsParent);
		}

		private void UpdateFill(float currentCamZ)
		{
			if (currentCamZ - _lastFillZ < areaFillsInterval)
				return;

			var nearZ = _lastFillZ + objectsAreaDepth;
			var farZ = nearZ + areaFillsInterval;
			var updateFillMaxObjects = (int)(firstFillMaxObjects * areaFillsInterval / objectsAreaDepth);
			_lastFillZ += areaFillsInterval;
			FillArea(nearZ, farZ, objectsAreaWidth, centerEmptySpaceWidth, density, updateFillMaxObjects, objectsParent);
		}

		private void FillArea(float nearZ, float farZ, float fillWdt, float centerEmptySpaceWdt,
			float objectsDensity, int maxObjects, Transform parentTransformForObjects)
		{
			int newObjectsCount = 0;
			var newObjects = _composer.FillArea(
				new Vector2(-fillWdt / 2, nearZ),
				new Vector2(-centerEmptySpaceWdt / 2, farZ),
				parentTransformForObjects,
				objectsDensity,
				maxObjects / 2);

			if (newObjects != null)
			{
				for (int i = 0, len = newObjects.Count; i < len; i++)
				{
					_areaObjectsQueue.Enqueue(newObjects[i]);
				}
				newObjectsCount += newObjects.Count;
			}

			newObjects = _composer.FillArea(
							new Vector2(centerEmptySpaceWdt / 2, nearZ),
							new Vector2(fillWdt / 2, farZ),
							parentTransformForObjects,
							objectsDensity,
							maxObjects / 2);

			if (newObjects != null)
			{
				for (int i = 0, len = newObjects.Count; i < len; i++)
				{
					_areaObjectsQueue.Enqueue(newObjects[i]);
				}
				newObjectsCount += newObjects.Count;
			}
			Logs.Log("FillArea(nearZ={0:f0}, farZ={1:f0}, fillWidth={2:f0}, emptyWidth={3:f0}) Objects: onScene={4} new={5}  inPools={6}",
				nearZ, farZ, fillWdt, centerEmptySpaceWdt, _areaObjectsQueue.Count, newObjectsCount, _objectPoolsManager.FreeObjectsCount);
		}

		private void UpdateClear(float currentCamZ)
		{
			if (currentCamZ - _lastClearZ < areaClearInterval)
				return;

			_lastClearZ += areaClearInterval;
			var areaObject = _areaObjectsQueue.Peek();
			var areaObjectsToClear = new List<IAreaObject>();
			while (_areaObjectsQueue.Count > 0 && areaObject.Point.z < currentCamZ - AreaOvermeasure)
			{
				_areaObjectsQueue.Dequeue();
				areaObjectsToClear.Add(areaObject);
				areaObject = _areaObjectsQueue.Peek();
			}
			_composer.ClearObjects(areaObjectsToClear);

			if (areaObjectsToClear.Count == 0)
			{
				Logs.Log("UpdateClear(currentCamZ={0:f0}) none", currentCamZ);
			}
			else
			{
				Logs.Log("UpdateClear(currentCamZ={0:f0}) cleared={1} onScene={2}  inPools={3}",
					currentCamZ, areaObjectsToClear.Count, _areaObjectsQueue.Count, _objectPoolsManager.FreeObjectsCount);
			}
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
			//			_allAreaObjects.AddRange(_composer.FillArea(new Vector2(-400, -400), new Vector2(400, 400), objectsParent, .9f, 5000));
			//			_allAreaObjects.AddRange(_composer.FillArea(new Vector2(-10, -10), new Vector2(-100, -100), objectsParent, .9f, 5000));
			//			Logs.Log("+ setted={0} free={1}", _allAreaObjects.Count, _objectPoolsManager.FreeObjectsCount);
		}

		private void TestDown()
		{
			//			_composer.ClearObjects(_allAreaObjects);
			//			_allAreaObjects.Clear();
			//			Logs.Log("- setted={0} free={1}", _allAreaObjects.Count, _objectPoolsManager.FreeObjectsCount);
			//			_allAreaObjects = _composer.ClearArea(_allAreaObjects, new Vector2(-400, -400), new Vector2(400, 400));
		}

	}
}
