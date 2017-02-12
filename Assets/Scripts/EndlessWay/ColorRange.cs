using System;
using DebugStuff;
using SomeRandom;
using UnityEngine;

namespace EndlessWay
{
	/// <summary>
	/// Правило, на основании которого может выдаваться случайный цвет из RGBA-диапазона в пределах loColor...hiColor 
	/// или диапазона предопределенных цветов fixedColors
	/// </summary>
	[Serializable]
	public class ColorRange
	{
		/// <summary>
		/// Используются ли фиксированные варианты цветов или диапазон loColor ... hiColor
		/// </summary>
		public bool isFixedColors;

		public Color[] fixedColors;

		public Color loColor;
		public Color hiColor;

		//Кешированные параметры для быстрого расчета
		private float _minR, _maxR, _minG, _maxG, _minB, _maxB, _minA, _maxA;
		private bool _isConstR, _isConstG, _isConstB, _isConstA;
		private int _fixedColorsLength;


		//=== Props ===========================================================

		public bool IsWrong { get; private set; }


		//=== Public ==========================================================

		public Color GetColorByRandomFactor(IRandom random)
		{
			if (IsWrong)
				return Color.black;

			if (isFixedColors)
			{
				return _fixedColorsLength == 1
					? fixedColors[0]
					: fixedColors[random.Range(0, _fixedColorsLength)];
			}
			return new Color(
				_isConstR ? _minR : random.Range(_minR, _maxR),
				_isConstG ? _minG : random.Range(_minG, _maxG),
				_isConstB ? _minB : random.Range(_minB, _maxB),
				_isConstA ? _minA : random.Range(_minA, _maxA));
		}

		public void Init()
		{
			IsWrong = false;
			if (isFixedColors)
			{
				if (fixedColors == null || fixedColors.Length == 0)
				{
					IsWrong = true;
					return;
				}

				_fixedColorsLength = fixedColors.Length;
			}
			else
			{
				_minR = Mathf.Min(loColor.r, hiColor.r);
				_minG = Mathf.Min(loColor.g, hiColor.g);
				_minB = Mathf.Min(loColor.b, hiColor.b);
				_minA = Mathf.Min(loColor.a, hiColor.a);
				_maxR = Mathf.Max(loColor.r, hiColor.r);
				_maxG = Mathf.Max(loColor.g, hiColor.g);
				_maxB = Mathf.Max(loColor.b, hiColor.b);
				_maxA = Mathf.Max(loColor.a, hiColor.a);
				_isConstR = Mathf.Approximately(loColor.r, hiColor.r);
				_isConstG = Mathf.Approximately(loColor.g, hiColor.g);
				_isConstB = Mathf.Approximately(loColor.b, hiColor.b);
				_isConstA = Mathf.Approximately(loColor.a, hiColor.a);
			}
		}
	}
}
