using UnityEngine;

namespace EndlessWay
{
	public interface IAreaObject
	{
		bool IsWrong { get; }
		Vector3 Point { get; set; }
		Vector2 GetOccupiedArea();
		void ApplySpecification(IObjectRule objectRule);
	}
}
