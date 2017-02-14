using System;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class EnvObject : MonoBehaviour, IAreaObject
	{
		/// <summary>
		/// Цвета объекта
		/// </summary>
		public Color[] colors;

		/// <summary>
		/// Размеры экземпляра объекта
		/// </summary>
		public float[] sizes;

		private static Type _selfType = typeof(EnvObject);


		//=== Props ===========================================================

		public bool IsWrong { get; protected set; }

		public Vector3 Point
		{
			get { return transform.localPosition; }
			set { transform.localPosition = value; }
		}


		//=== Public ==========================================================

		public virtual void ApplySpecification(IAreaObjectSpecification areaObjectSpecification)
		{
			if (areaObjectSpecification.IsNull("areaObjectSpecification", _selfType) || areaObjectSpecification.IsWrong)
			{
				IsWrong = true;
				return;
			}

			if (areaObjectSpecification.IsColorable)
			{
				colors = areaObjectSpecification.GetColors();
				ApplyColors();
			}

			if (areaObjectSpecification.IsSizeable)
			{
				sizes = areaObjectSpecification.GetSizes();
				ApplySizes();
			}
		}

		public virtual Vector2 GetOccupiedArea()
		{
			return Vector3.zero;
		}

		public virtual void ApplySizes()
		{

		}

		public virtual void ApplyColors()
		{

		}
	}
}
