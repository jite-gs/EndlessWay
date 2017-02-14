using System;
using System.Collections.Generic;
using System.Linq;
using DebugStuff;
using SomeRandom;
using UnityEngine;

namespace EndlessWay
{
	public class Composer
	{
		private Dictionary<string, IAreaObjectSpecification> _areaObjectSpecifications;
		private List<string> _areaObjectNames;
		private IAreaObjectSource _areaObjectSource;
		private IRandom _random;

		private Type _selfType;


		//=== Enums ===========================================================

		private enum PickObjectStrategy
		{
			Random,
			Big,
			Small,
		}

		private enum ChooseObjectPointStrategy
		{
			BlindRandom,
			CheckedByAreaRandom,
		}


		//=== Props ===========================================================

		/// <summary>
		/// Плотность засадки площади объектами
		/// </summary>
		public float Density { get; set; }


		//=== Ctor ============================================================

		public Composer(Dictionary<string, IAreaObjectSpecification> areaObjectSpecifications, IAreaObjectSource areaObjectSource,
			IRandom random)
		{
			_selfType = GetType();
			_random = random;
			_areaObjectSpecifications = areaObjectSpecifications;
			_areaObjectSource = areaObjectSource;
			if (_areaObjectSource == null || _areaObjectSpecifications == null || random == null ||
				_areaObjectSpecifications.Count == 0)
				throw new NullReferenceException("Composer: Some ctor params are null or empty!");

			_areaObjectNames = _areaObjectSpecifications.Keys.ToList();
		}


		//=== Public ==========================================================

		/// <summary>
		/// Заполняет прямоугольную площадь между точками corner1 и corner2 объектами IAreaObject, получая объекты от _areaObjectSource.
		/// Перед выставлением объекта применяет к нему стиль согласно имеющейся спецификации из словаря _areaObjectSpecifications.
		/// Возвращает список добавленных объектов
		/// </summary>
		/// <param name="density">Плотность заполнения площади: отношение заполненной объектами площади к общей площади</param>
		/// <param name="parentTransform">Transform, внутри которого выставляются объекты (может быть null)</param>
		/// <param name="maxObjects">Максимальное число выставляемых объектов</param>
		/// <param name="height">Высота выставления объектов</param>
		/// <returns>Список добавленных объектов</returns>
		public List<IAreaObject> FillArea(Vector2 corner1, Vector2 corner2, Transform parentTransform,
			float density, int maxObjects, float height = 0)
		{
			if (density <= 0)
				throw new Exception("FillArea() Wrong density value: " + density);

			Vector2 leftBottomCorner, rightTopCorner;
			NormalizeCorners(corner1, corner2, out leftBottomCorner, out rightTopCorner);

			var area = (rightTopCorner.x - leftBottomCorner.x) * (rightTopCorner.y - leftBottomCorner.y);
			float filledArea = 0;

			var areaObjects = new List<IAreaObject>();
			var maxFilledArea = area * density;
			int objectsCount = 0;
			while (objectsCount < maxObjects && filledArea < maxFilledArea)
			{
				IAreaObjectSpecification specification;
				var areaObjectName = PickAreaObject(PickObjectStrategy.Random, out specification);
				if (specification.IsNull("specification", _selfType))
					break;

				var areaObject = _areaObjectSource.GetObject(areaObjectName, parentTransform);
				if (areaObject == null)
					break;

				areaObject.ApplySpecification(specification);
				if (areaObject.IsWrong)
				{
					_areaObjectSource.ReleaseObject(areaObject);
					Logs.LogError("areaObject '{0}' is wrong", areaObjectName);
					break;
				}

				Vector3 point;
				if (!ChooseObjectPoint(ChooseObjectPointStrategy.BlindRandom, areaObject,
					leftBottomCorner, rightTopCorner, height, out point))
				{
					_areaObjectSource.ReleaseObject(areaObject);
					Logs.LogError("Unable to choose point for areaObject '{0}'", areaObjectName);
					break;
				}
				areaObject.Point = point;
				var occupiedAreaAsVector = areaObject.GetOccupiedArea();
				filledArea += occupiedAreaAsVector.x * occupiedAreaAsVector.y;
				areaObjects.Add(areaObject);
				objectsCount++;
				//				Logs.Log("[{0}] point={1}, AreaVector={2} ({3})",
				//					areaObjectName, 
				//					point,
				//					occupiedAreaAsVector,
				//					occupiedAreaAsVector.x * occupiedAreaAsVector.y);
			}

			return areaObjects;
		}

		/// <summary>
		/// Очищает объекты из списка areaObjectsForCheck 
		/// </summary>
		public void ClearAllObjects(List<IAreaObject> areaObjectsForCheck)
		{
			ClearObjects(areaObjectsForCheck, Vector2.zero, Vector2.one, false);
		}

		/// <summary>
		/// Очищает прямоугольную площадь между точками corner1 и corner2 от объектов из списка areaObjectsForCheck.
		/// Возвращает список оставшихся из areaObjectsForCheck
		/// </summary>
		public List<IAreaObject> ClearArea(List<IAreaObject> areaObjectsForCheck, Vector2 corner1, Vector2 corner2)
		{
			return ClearObjects(areaObjectsForCheck, corner1, corner2, true);
		}


		//=== Private =========================================================


		//Приводит координаты 2 произвольных углов, описывающих прямоугольник к левому нижнему и правому верхнему углу
		private void NormalizeCorners(Vector2 corner1, Vector2 corner2, out Vector2 leftBottomCorner, out Vector2 rightTopCorner)
		{
			if (Mathf.Approximately(corner1.x, corner2.x) || Mathf.Approximately(corner1.y, corner2.y))
				throw new Exception(string.Format(
					"NormalizeCorners() Area between corners is zero! (corner1={0}, corner2={1})", corner1, corner2));

			leftBottomCorner = new Vector2(Mathf.Min(corner1.x, corner2.x), Mathf.Min(corner1.y, corner2.y));
			rightTopCorner = new Vector2(Mathf.Max(corner1.x, corner2.x), Mathf.Max(corner1.y, corner2.y));
		}

		/// <summary>
		/// Очищает объекты с территории ограниченной corner1 и corner1 (расчеты ведутся в local-координатах объектов) 
		/// или вообще объекты из areaObjectsForCheck, если byArea false. Возвращает список оставшихся из areaObjectsForCheck
		/// </summary>
		private List<IAreaObject> ClearObjects(List<IAreaObject> areaObjectsForCheck, Vector2 corner1, Vector2 corner2, bool byArea)
		{
			var restOfObjects = new List<IAreaObject>(areaObjectsForCheck.Capacity);
			var objectsToRelease = areaObjectsForCheck;
			if (byArea)
			{
				objectsToRelease = new List<IAreaObject>();
				Vector2 leftBottomCorner, rightTopCorner;
				NormalizeCorners(corner1, corner2, out leftBottomCorner, out rightTopCorner);

				Bounds areaBounds = new Bounds();
				areaBounds.SetMinMax(
					new Vector3(leftBottomCorner.x,-1, leftBottomCorner.y),
					new Vector3(rightTopCorner.x, 1, rightTopCorner.y));

				for (int i = 0, len = areaObjectsForCheck.Count; i < len; i++)
				{
					var areaObject = areaObjectsForCheck[i];
					if (areaBounds.Contains(areaObject.Point))
					{
						objectsToRelease.Add(areaObject);
					}
					else
					{
						restOfObjects.Add(areaObject);
					}
				}
			}

			for (int i = 0, len = objectsToRelease.Count; i < len; i++)
			{
				_areaObjectSource.ReleaseObject(objectsToRelease[i]);
			}

			return restOfObjects;
		}

		private string PickAreaObject(PickObjectStrategy pickObjectStrategy, out IAreaObjectSpecification specification)
		{
			switch (pickObjectStrategy)
			{
				case PickObjectStrategy.Random:
					var areaObjectName = _areaObjectNames[_random.Range(0, _areaObjectNames.Count)];
					specification = _areaObjectSpecifications[areaObjectName];
					return areaObjectName;

				//TODO

				default:
					throw new Exception("PickAreaObject: Unhandled pickObjectStrategy=" + pickObjectStrategy);
			}

		}

		private bool ChooseObjectPoint(ChooseObjectPointStrategy strategy, IAreaObject areaObject, Vector2
			leftBottomCorner, Vector2 rightTopCorner, float height, out Vector3 chosenPoint)
		{
			chosenPoint = Vector3.zero;
			switch (strategy)
			{
				case ChooseObjectPointStrategy.BlindRandom:
					chosenPoint = new Vector3(
						_random.Range(leftBottomCorner.x, rightTopCorner.x),
						height,
						_random.Range(leftBottomCorner.y, rightTopCorner.y));
					return true;

				//TODO

				default:
					throw new Exception("ChooseObjectPoint: Unhandled pickObjectStrategy=" + strategy);
			}
		}
	}
}
