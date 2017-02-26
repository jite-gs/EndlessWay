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

		private bool _isVerbose = false;

		private Type _selfType;


		//=== Props ===========================================================

		public int Capacity { get; set; }

		/// <summary>
		/// ����� ��������������� ��������
		/// </summary>
		public int ObjectsCount { get; private set; }

		/// <summary>
		/// ����� ��������� �������� (�.�. ������� � ����)
		/// </summary>
		public int FreeObjectsCount { get { return _pool.Count; } }


		//=== Ctor ============================================================

		public ObjectPool(T prefab, int capacity = 10000)
		{
			_selfType = GetType();
			_prefab = prefab;
			if (_prefab == null)
				throw new NullReferenceException(_selfType.NiceName() + " ObjectPool() prefab is null");

			Capacity = capacity;
			_pool = new List<T>(Capacity);
			ObjectsCount = 0;
		}


		//=== Public ==========================================================

		public T GetObject(Transform parentTransform)
		{
			if(ObjectsCount >= Capacity)
			{
				if (_isVerbose)
					Logs.Log("GetObject() Return null cause Capacity={0}", Capacity); //DEBUG
				return null;
			}

			ObjectsCount++;
			T freeObject = GetFreeObject(parentTransform);
			if (freeObject != null)
				return freeObject;

			return CreateNewObject(parentTransform);
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
				Logs.LogError("<{0}>({1}) GetFreeObject() freeObject[{2}] is null", _selfType.NiceName(), _prefab.name, objectIndex);
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
			return UnityEngine.Object.Instantiate<T>(_prefab, parentTransform);
		}

		public void Release(T objectToRelease)
		{
			if (objectToRelease == null)
			{
				Logs.LogError("<{0}>({1}) Release() object is null", _selfType.NiceName(), _prefab.name);
				return;
			}

			ObjectsCount--;
			_pool.Add(objectToRelease);
		}
	}
}
