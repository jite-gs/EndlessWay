using System;
using DebugStuff;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessWay
{
	public class ControlsManager : MonoBehaviour
	{
		public Slider sliderMovementSpeed;
		public Slider sliderDensity;
		public Slider sliderFirstFillMaxObjects;

		public Canvas canvasControls;
		public Canvas canvasActivator;

		public event Action<IntParamChangedEventArgs> IntParamChangedEvent;
		public event Action<FloatParamChangedEventArgs> FloatParamChangedEvent;


		//=== Enums ===========================================================

		public enum ParamName
		{
			MovementSpeed,
			FirstFillMaxObjects,
			FillDensity,
		}


		//=== Props ==========================================================

		public bool IsWrong { get; private set; }


		//=== Unity ===========================================================

		void Awake()
		{
			var selfType = GetType();

			if (canvasControls.IsNull("canvasControls", selfType) ||
				canvasActivator.IsNull("canvasActivator", selfType) ||
				sliderMovementSpeed.IsNull("sliderMovementSpeed", selfType) ||
				sliderDensity.IsNull("sliderDensity", selfType) ||
				sliderFirstFillMaxObjects.IsNull("sliderFirstFillMaxObjects", selfType))
			{
				Logs.LogError("<{0}> '{1}' IsWrong", selfType, name);
				IsWrong = true;
				return;
			}

			canvasControls.enabled = false;
		}


		//=== OnEvents ==========================================================

		public void OnClickExit()
		{
			Application.Quit();
		}

		public void OnClickShowControls()
		{
			canvasActivator.enabled = false;
			canvasControls.enabled = true;
		}

		public void OnClickHideControls()
		{
			canvasControls.enabled = false;
			canvasActivator.enabled = true;
		}

		public void OnSliderMovementSpeed()
		{
			FloatParamChangedEvent(new FloatParamChangedEventArgs(ParamName.MovementSpeed, sliderMovementSpeed.value));
		}

		public void OnSliderDensity()
		{
			FloatParamChangedEvent(new FloatParamChangedEventArgs(ParamName.FillDensity, sliderDensity.value));
		}

		public void OnSliderFirstFillMaxObjects()
		{
			IntParamChangedEvent(new IntParamChangedEventArgs(ParamName.FirstFillMaxObjects, (int)sliderFirstFillMaxObjects.value));
		}


		//=== Public ==========================================================

		public void SetSliderValue(FloatParamChangedEventArgs args)
		{
			switch (args.ParamName)
			{
				case ParamName.FillDensity:
					sliderDensity.value = args.ParamValue;
					break;

				case ParamName.FirstFillMaxObjects:
					sliderFirstFillMaxObjects.value = args.ParamValue;
					break;

				case ParamName.MovementSpeed:
					sliderMovementSpeed.value = args.ParamValue;
					break;

				default:
					Logs.LogError("<{0}> SetSliderValue() Unhandled ParamName.{1}", GetType(), args.ParamName);
					break;
			}
		}

	}
}
