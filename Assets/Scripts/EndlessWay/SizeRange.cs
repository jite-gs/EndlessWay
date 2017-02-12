using System;
using SomeRandom;
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

		//Кешированные параметры для быстрого расчета
		private float _min, _max;
		private bool _isConst;
		private int _fixedSizesLength;


		//=== Props ===========================================================

		public bool IsWrong { get; private set; }


		//=== Public ==========================================================

		public float GetSizeByRandomFactor(IRandom random)
		{
			if (IsWrong)
				return 0;

			if (isFixedSizes)
			{
				return _fixedSizesLength == 1
					? fixedSizes[0]
					: fixedSizes[random.Range(0, _fixedSizesLength)];
			}
			return _isConst ? _min: random.Range(_min, _max);
		}

		public void Init()
		{
			IsWrong = false;
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
				_max = Mathf.Max(loSize, hiSize);
				_isConst = Mathf.Approximately(loSize, hiSize);
			}
		}
	}
}
