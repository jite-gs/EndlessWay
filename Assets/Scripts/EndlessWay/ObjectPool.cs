using System;
using System.Collections.Generic;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class ObjectPool<T> where T : MonoBehaviour
	{
		private List<T> _pool;
		private T _prefab;

		private Type _selfType;


		//=== Props ===========================================================

		public int MaxObjects { get; set; }

		public int ObjectsCount { get; private set; }

		public int FreeObjectsCount { get { return _pool.Count; } }


		//=== Ctor ============================================================

		public ObjectPool(T prefab, int maxObjects = 10000)
		{
			_selfType = GetType();
			_prefab = prefab;
			if (_prefab == null)
				throw new NullReferenceException(_selfType.NiceName() + " ObjectPool() prefab is null");

			MaxObjects = maxObjects;
			_pool = new List<T>(MaxObjects);
			ObjectsCount = 0;
		}


		//=== Public ==========================================================

		public T GetObject(Transform parentTransform)
		{
			T freeObject = GetFreeObject(parentTransform);
			if (freeObject != null)
				return freeObject;

			if (ObjectsCount < MaxObjects)
				return CreateNewObject(parentTransform);

			return null;
		}


		//=== Private =========================================================

		private T GetFreeObject(Transform parentTransform)
		{
			if (_pool.Count == 0)
				return null;

			var objectIndex = _pool.Count - 1;
			var freeObject = _pool[objectIndex];
			_pool.RemoveAt(objectIndex);
			if (freeObject == null)
			{
				Logs.LogError("<{0}> GetFreeObject() freeObject[{1}] is null", _selfType.NiceName(), objectIndex);
			}
			else
			{
				if (parentTransform != freeObject.transform.parent)
					freeObject.transform.parent = parentTransform;
			}

			return freeObject;
		}

		private T CreateNewObject(Transform parentTransform)
		{
			ObjectsCount++;
			return UnityEngine.Object.Instantiate<T>(_prefab, parentTransform);
		}

		public void Release(T objectToRelease)
		{
			objectToRelease.IsNull("objectToRelease", _selfType);
			_pool.Add(objectToRelease);
		}
	}
}
