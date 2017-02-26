using System;
using DebugStuff;
using SomeRandom;
using UnityEngine;

namespace EndlessWay
{
	/// <summary>
	/// Базовые правила случайных параметров объекта
	/// </summary>
	[Serializable]
	public class EnvObjectRule : IObjectRule
	{

		public EnvObject.Tag[] tags;
		public string associatedEnvObjectName;
		public int colorsLength;
		public int sizesLength;

		public ColorRange[] colorRanges;
		public SizeRange[] sizeRanges;

		private IRandom _random;

		private static Type _selfType = typeof(EnvObjectRule);


		//=== Props ===========================================================

		public bool IsSizeable { get { return sizesLength > 0; } }

		public bool IsColorable { get { return colorsLength > 0; } }

		public bool IsWrong { get; private set; }


		//=== Public ==========================================================

		public bool Init(IRandom random)
		{
			IsWrong = false;
			_random = random;
			if (_random.IsNull("_random", _selfType))
			{
				IsWrong = true;
				return false;
			}

			if (IsSizeable)
			{
				if (sizeRanges == null || sizeRanges.Length != sizesLength)
				{
					Logs.LogError("EnvObjectRule: sizeRanges ({0}) isn't corresponds sizesLength={1}",
						sizeRanges == null ? "null" : "length=" + sizeRanges.Length,
						sizesLength);
					IsWrong = true;
					return false;
				}

				for (int i = 0, len = sizeRanges.Length; i < len; i++)
				{
					var sizeRange = sizeRanges[i];
					sizeRange.Init();
					if (sizeRange.IsWrong)
					{
						Logs.LogError("EnvObjectRule: sizeRange[{0}] is wrong!", i);
						IsWrong = true;
						return false;
					}
				}
			}

			if (IsColorable)
			{
				if (colorRanges == null || colorRanges.Length != colorsLength)
				{
					Logs.LogError("EnvObjectRule: colorRanges ({0}) isn't corresponds colorsLength={1}",
						colorRanges == null ? "null" : "length=" + colorRanges.Length,
						colorsLength);
					IsWrong = true;
					return false;
				}

				for (int i = 0, len = colorRanges.Length; i < len; i++)
				{
					var colorRange = colorRanges[i];
					colorRange.Init();
					if (colorRange.IsWrong)
					{
						Logs.LogError("EnvObjectRule: colorRange[{0}] is wrong!", i);
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
				randomColors[i] = colorRanges[i].GetColorByRandomFactor(_random);
			}
			return randomColors;
		}

		public float[] GetSizes()
		{
			var randomSizes = new float[sizesLength];
			for (int i = 0; i < sizesLength; i++)
			{
				randomSizes[i] = sizeRanges[i].GetSizeByRandomFactor(_random);
			}
			return randomSizes;
		}
	}
}
