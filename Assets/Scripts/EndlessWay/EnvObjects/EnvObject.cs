using System;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public abstract class EnvObject : MonoBehaviour, IAreaObject
	{
		public enum Tag
		{
			MajorObject,
			MidObject,
			MinorObject,
			SmallPart,
			Regular,
			Oriented,
		}

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

		public virtual void ApplySpecification(IObjectRule objectRule)
		{
			if (objectRule.IsNull("objectRule", _selfType) || objectRule.IsWrong)
			{
				IsWrong = true;
				return;
			}

			if (objectRule.IsColorable)
			{
				colors = objectRule.GetColors();
				ApplyColors();
			}

			if (objectRule.IsSizeable)
			{
				sizes = objectRule.GetSizes();
				ApplySizes();
			}
		}

		public abstract Vector2 GetOccupiedArea();

		public abstract void ApplySizes();

		public abstract void ApplyColors();
	}
}
