using System;
using System.Collections.Generic;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class ObjectPoolsManager : IAreaObjectSource
	{
		private Dictionary<string, ObjectPool<EnvObject>> _pools = new Dictionary<string, ObjectPool<EnvObject>>();
		private Dictionary<EnvObject, ObjectPool<EnvObject>> _allInstances;
		private Vector3 _freeObjectsPoint;
		private int _capacity;

		private Type _selfType;


		//=== Prop ============================================================

		public int Capacity
		{
			get { return _capacity; }
			set
			{
				if (_pools == null)
					throw new NullReferenceException("Attempt to set Capacity whereas pools is null");

				if (value <= 0)
					throw new Exception("Attempt to set wrong value for Capacity: " + value);

				_capacity = value;
				foreach (var objectPool in _pools.Values)
					objectPool.Capacity = _capacity;
			}
		}

		public int FreeObjectsCount
		{
			get
			{
				if (_allInstances == null)
					return -1;

				int count = 0;
				foreach (var pool in _pools.Values)
				{
					count += pool.FreeObjectsCount;
				}
				return count;
			}
		}


		//=== Ctor ============================================================

		public ObjectPoolsManager(Vector3 freeObjectsPoint)
		{
			_freeObjectsPoint = freeObjectsPoint;
		}


		//=== Public ==========================================================

		public void Init(Dictionary<string, EnvObject> prefabsByPrototypeName, int capacity)
		{
			_selfType = GetType();
			if (prefabsByPrototypeName == null)
				throw new NullReferenceException("ObjectPoolsManager.Init() prefabsByPrototypeName is null");

			_allInstances = new Dictionary<EnvObject, ObjectPool<EnvObject>>(Capacity);
			foreach (var kvp in prefabsByPrototypeName)
			{
				_pools.Add(kvp.Key, new ObjectPool<EnvObject>(kvp.Value));
			}
			Capacity = capacity;
		}

		public IAreaObject GetObject(string objectPrototypeName, Transform parentTransform)
		{
			ObjectPool<EnvObject> pool;
			if (!_pools.TryGetValue(objectPrototypeName, out pool))
			{
				Logs.LogError("GetObject('{0}') ObjectPoolsManager don't contains pool for this name", objectPrototypeName);
				return null;
			}

			var envObject = pool.GetObject(parentTransform);
			if (envObject != null && !_allInstances.ContainsKey(envObject))
				_allInstances.Add(envObject, pool);

			return envObject;
		}

		public void ReleaseObject(IAreaObject areaObject)
		{
			var envObject = areaObject as EnvObject;
			if (envObject.IsNull("envObject", _selfType))
				return;

			ObjectPool<EnvObject> pool;
			if (!_allInstances.TryGetValue(envObject, out pool))
			{
				Logs.LogError("_allInstances don't contains object '{0}'", envObject.name);
				return;
			}

			pool.Release(envObject);
			envObject.transform.localPosition = _freeObjectsPoint;
		}
	}
}
