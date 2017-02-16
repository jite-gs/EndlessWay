using System;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public abstract class EnvObject : MonoBehaviour, IAreaObject
	{
		/// <summary>
		/// Цвета объекта
		/// </summary>
		public Color[] colors;

		/// <summary>
		/// Размеры экземпляра объекта
		/// </summary>
		public float[] sizes;

		protected Vector3 orgPosition;
		protected Vector3 orgScale;
		protected Quaternion orgRotation;

		private static Type _selfType;


		//=== Props ===========================================================

		public bool IsWrong { get; protected set; }

		public Vector3 Point
		{
			get { return transform.localPosition; }
			set { transform.localPosition = value; }
		}

		protected float MainSize { get { return sizes == null ? 1 : sizes[0]; } }

		protected Color MainColor { get { return colors == null ? Color.black : colors[0]; } }


		//=== Unty ============================================================

		private void Awake()
		{
			if (_selfType == null)
				_selfType = GetType();
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

		public abstract Vector2 GetOccupiedArea();

		public abstract void ApplySizes();

		public abstract void ApplyColors();
	}
}
