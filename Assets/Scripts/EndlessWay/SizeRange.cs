using System;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	/// <summary>
	/// Правило, на основании которого может выдаваться случайный размер из диапазона в пределах loSize...hiSize 
	/// или диапазона предопределенных размеров fixedSizes
	/// </summary>
	[Serializable]
	public class SizeRange
	{
		/// <summary>
		/// Используются ли фиксированные варианты размеров или диапазон loSize ... hiSize
		/// </summary>
		public bool isFixedSizes;

		public float[] fixedSizes;

		public float loSize;
		public float hiSize;

		private Func<float> _randomFactor;

		//Кешированные параметры для быстрого расчета
		private float _min, _delta;
		private bool _isConst;
		private int _fixedSizesLength;

		private static Type _selfType = typeof(SizeRange);


		//=== Props ===========================================================

		public bool IsWrong { get; private set; }


		//=== Public ==========================================================

		public float GetSizeByRandomFactor()
		{
			if (IsWrong)
				return 0;

			if (isFixedSizes)
			{
				if (_fixedSizesLength == 1)
					return fixedSizes[0];

				int index = (int)(_randomFactor() * _fixedSizesLength);
				if (index == _fixedSizesLength)
					index = _fixedSizesLength - 1;

				return fixedSizes[index];
			}
			return _isConst ? _min : _delta * _randomFactor() + _min;
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

			if (isFixedSizes)
			{
				if (fixedSizes == null || fixedSizes.Length == 0)
				{
					IsWrong = true;
					return;
				}

				_fixedSizesLength = fixedSizes.Length;
			}
			else
			{
				_min = Mathf.Min(loSize, hiSize);
				_delta = Mathf.Abs(loSize - hiSize);
				_isConst = Mathf.Approximately(loSize, hiSize);
			}
		}
	}
}
