using UnityEngine;

namespace EndlessWay
{
	public interface IAreaObjectSpecification
	{
		bool IsSizeable { get; }
		bool IsColorable { get; }
		bool IsWrong { get; }

		Color[] GetColors();
		float[] GetSizes();
	}
}
