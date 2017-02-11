using System;
using DebugStuff;
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

		private Func<float> _randomFactor;

		//Кешированные параметры для быстрого расчета
		private float _minR, _deltaR, _minG, _deltaG, _minB, _deltaB, _minA, _deltaA;
		private bool _isConstR, _isConstG, _isConstB, _isConstA;
		private int _fixedColorsLength;

		private static Type _selfType = typeof(ColorRange);


		//=== Props ===========================================================

		public bool IsWrong { get; private set; }


		//=== Public ==========================================================

		public Color GetColorByRandomFactor()
		{
			if (IsWrong)
				return Color.black;

			if (isFixedColors)
			{
				if (_fixedColorsLength == 1)
					return fixedColors[0];

				int index = (int)(_randomFactor() * _fixedColorsLength);
				if (index == _fixedColorsLength)
					index = _fixedColorsLength - 1;

				return fixedColors[index];
			}
			return new Color(
				_isConstR ? _minR : _deltaR * _randomFactor() + _minR,
				_isConstG ? _minG : _deltaG * _randomFactor() + _minG,
				_isConstB ? _minB : _deltaB * _randomFactor() + _minB,
				_isConstA ? _minA : _deltaA * _randomFactor() + _minA);
		}

		public void Init(Func<float> randomFactor)
		{
			IsWrong = false;
			_randomFactor = randomFactor;
			if (_randomFactor.IsNull("_randomFactor", _selfType))
			{
				IsWrong = true;
				return;
			}

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
				_deltaR = Mathf.Abs(loColor.r - hiColor.r);
				_deltaG = Mathf.Abs(loColor.g - hiColor.g);
				_deltaB = Mathf.Abs(loColor.b - hiColor.b);
				_deltaA = Mathf.Abs(loColor.a - hiColor.a);
				_isConstR = Mathf.Approximately(loColor.r, hiColor.r);
				_isConstG = Mathf.Approximately(loColor.g, hiColor.g);
				_isConstB = Mathf.Approximately(loColor.b, hiColor.b);
				_isConstA = Mathf.Approximately(loColor.a, hiColor.a);
			}
		}
	}
}
