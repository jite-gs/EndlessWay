using DebugStuff;
using UnityEngine;
using UnityEngine.UI;

public class ControlsStorage : MonoBehaviour
{
	public Slider sliderMovementSpeed;

	public float MovementSpeed
	{
		get { return sliderMovementSpeed == null ? 10 : sliderMovementSpeed.value; }
	}

	void Awake()
	{
		var selfType = GetType();
		sliderMovementSpeed.IsNull("sliderMovementSpeed", selfType);

	}
}
