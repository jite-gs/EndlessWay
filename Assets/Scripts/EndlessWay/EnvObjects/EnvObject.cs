using System;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class EnvObject : MonoBehaviour
	{
		/// <summary>
		/// Цвета объекта
		/// </summary>
		public Color[] colors;

		/// <summary>
		/// Размеры экземпляра объекта
		/// </summary>
		public float[] sizes;

		private EnvObjectSpecification _envObjectSpecification;

		private static Type _selfType = typeof(EnvObject);


		//=== Public ==========================================================

		public void SetSpecification(EnvObjectSpecification envObjectSpecification)
		{
			_envObjectSpecification = envObjectSpecification;
		}

		public virtual void Setup()
		{
			if (_envObjectSpecification.IsNull("_envObjectSpecification", _selfType) || _envObjectSpecification.IsWrong)
				return;

			if (_envObjectSpecification.IsColorable)
			{
				colors = _envObjectSpecification.GetColors();
				ApplyColors();
			}

			if(_envObjectSpecification.IsSizeable)
			{
				sizes = _envObjectSpecification.GetSizes();
				ApplySizes();
			}
		}

		public virtual void ApplySizes()
		{

		}

		public virtual void ApplyColors()
		{

		}
	}
}
