using UnityEngine;

namespace EndlessWay
{
	public interface IObjectRule
	{
		bool IsSizeable { get; }
		bool IsColorable { get; }
		bool IsWrong { get; }

		Color[] GetColors();
		float[] GetSizes();
	}
}
