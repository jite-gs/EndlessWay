using System;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	[Serializable]
	public class EnvObjectSpecification
	{
		public int colorsLength;
		public int sizesLength;

		public ColorRange[] colorRanges;
		public SizeRange[] sizeRanges;


		private Func<float> _randomFactor;
		private static Type _selfType = typeof(EnvObjectSpecification);


		//=== Props ===========================================================

		public bool IsSizeable { get { return sizesLength > 0; } }

		public bool IsColorable { get { return colorsLength > 0; } }

		public bool IsWrong { get; private set; }


		//=== Public ==========================================================

		public bool Init(Func<float> randomFactor)
		{
			IsWrong = false;
			_randomFactor = randomFactor;
			if (_randomFactor.IsNull("_randomFactor", _selfType))
			{
				IsWrong = true;
				return false;
			}

			if (IsSizeable)
			{
				if (sizeRanges == null || sizeRanges.Length != sizesLength)
				{
					Logs.LogError("EnvObjectSpecification: sizeRanges ({0}) isn't corresponds sizesLength={1}",
						sizeRanges == null ? "null" : "length=" + sizeRanges.Length,
						sizesLength);
					IsWrong = true;
					return false;
				}

				for (int i = 0, len = sizeRanges.Length; i < len; i++)
				{
					var sizeRange = sizeRanges[i];
					sizeRange.Init(_randomFactor);
					if (sizeRange.IsWrong)
					{
						Logs.LogError("EnvObjectSpecification: sizeRange[{0}] is wrong!", i);
						IsWrong = true;
						return false;
					}
				}
			}

			if (IsColorable)
			{
				if (colorRanges == null || colorRanges.Length != colorsLength)
				{
					Logs.LogError("EnvObjectSpecification: colorRanges ({0}) isn't corresponds colorsLength={1}",
						colorRanges == null ? "null" : "length=" + colorRanges.Length,
						colorsLength);
					IsWrong = true;
					return false;
				}

				for (int i = 0, len = colorRanges.Length; i < len; i++)
				{
					var colorRange = colorRanges[i];
					colorRange.Init(_randomFactor);
					if (colorRange.IsWrong)
					{
						Logs.LogError("EnvObjectSpecification: colorRange[{0}] is wrong!", i);
						IsWrong = true;
						return false;
					}
				}
			}

			return true;
		}

		public Color[] GetColors()
		{
			var randomColors = new Color[colorsLength];
			for (int i = 0; i < colorsLength; i++)
			{
				randomColors[i] = colorRanges[i].GetColorByRandomFactor();
			}
			return randomColors;
		}

		public float[] GetSizes()
		{
			var randomSizes = new float[sizesLength];
			for (int i = 0; i < sizesLength; i++)
			{
				randomSizes[i] = sizeRanges[i].GetSizeByRandomFactor();
			}
			return randomSizes;
		}
	}
}
