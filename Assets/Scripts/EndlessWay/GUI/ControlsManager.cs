using System;
using DebugStuff;
using UnityEngine;
using UnityEngine.UI;

namespace EndlessWay
{
	public class ControlsManager : MonoBehaviour
	{
		public Canvas canvasControls;
		public Canvas canvasActivator;

		public Slider sliderMovementSpeed;
		public Slider sliderDensity;
		public Slider sliderMaxObjects;

		public Text textSceneObjects;
		public Text textFreeObjects;
		private int _lastSceneObjectsCount, _lastFreeObjectsCount;

		public event Action<IntParamChangedEventArgs> IntParamChangedEvent;
		public event Action<FloatParamChangedEventArgs> FloatParamChangedEvent;


		//=== Enums ===========================================================

		public enum ParamName
		{
			MovementSpeed,
			MaxObjects,
			FillDensity,
			SceneObjectsCount,
			FreeObjectsCount,
		}


		//=== Props ==========================================================

		public bool IsWrong { get; private set; }


		//=== Unity ===========================================================

		void Awake()
		{
			var selfType = GetType();

			if (canvasControls.IsNull("canvasControls", selfType) ||
				canvasActivator.IsNull("canvasActivator", selfType) ||
				textSceneObjects.IsNull("textSceneObjects", selfType) ||
				textFreeObjects.IsNull("textFreeObjects", selfType) ||
				sliderMovementSpeed.IsNull("sliderMovementSpeed", selfType) ||
				sliderDensity.IsNull("sliderDensity", selfType) ||
				sliderMaxObjects.IsNull("sliderMaxObjects", selfType))
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

		public void OnSliderMaxObjects()
		{
			IntParamChangedEvent(new IntParamChangedEventArgs(ParamName.MaxObjects, (int)sliderMaxObjects.value));
		}


		//=== Public ==========================================================

		public void SetSliderValue(FloatParamChangedEventArgs args)
		{
			switch (args.ParamName)
			{
				case ParamName.FillDensity:
					sliderDensity.value = args.ParamValue;
					break;

				case ParamName.MaxObjects:
					sliderMaxObjects.value = args.ParamValue;
					break;

				case ParamName.MovementSpeed:
					sliderMovementSpeed.value = args.ParamValue;
					break;

				default:
					Logs.LogError("<{0}> SetSliderValue() Unhandled ParamName.{1}", GetType(), args.ParamName);
					break;
			}
		}

		public void SetObjectsCount(int newSceneObjectsCount, int newFreeObjectsCount)
		{
			if (_lastSceneObjectsCount != newSceneObjectsCount)
			{
				_lastSceneObjectsCount = newSceneObjectsCount;
				textSceneObjects.text = newSceneObjectsCount.ToString();
			}

			if (_lastFreeObjectsCount != newFreeObjectsCount)
			{
				_lastFreeObjectsCount = newFreeObjectsCount;
				textFreeObjects.text = newFreeObjectsCount.ToString();
			}
		}

	}
}
