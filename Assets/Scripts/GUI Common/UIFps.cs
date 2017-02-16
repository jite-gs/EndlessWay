using UnityEngine;
using System;
using UnityEngine.UI;

public class UIFps : MonoBehaviour
{
	// Attach this to a Text to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval, so the display does not keep changing wildly.

	public float updateInterval = 0.5F;
	public Text _text;
	public Color32 normalFpsColor = Color.white;
	public Color32 lowFpsColor = Color.yellow;
	public Color32 veryLowFpsColor = Color.red;

	private Color32 _color;
	private float _accum = 0; // FPS accumulated over the interval
	private int _frames = 0; // Frames drawn over the interval
	private float _timeleft; // Left time for current interval
	const int LowFps = 30, VeryLowFps = 10;


	//=== Unity ===============================================================

	void Update()
	{
		if (_text == null)
		{
			enabled = false;
			return;
		}

		_timeleft -= Time.deltaTime;
		_accum += Time.timeScale / Time.deltaTime;
		++_frames;

		// Interval ended - update GUI text and start new interval
		if (_timeleft <= 0.0)
		{
			var fps = _accum / _frames;
			_text.color = (fps > LowFps)
				? normalFpsColor
				: ((fps > VeryLowFps) ? lowFpsColor : veryLowFpsColor);

			_text.text = string.Format("fps {0:f1}", fps);
			_timeleft = updateInterval;
			_accum = 0.0F;
			_frames = 0;
		}
	}

	void OnEnable()
	{
		if (_text == null)
		{
			_text = GetComponent<Text>();
			if (_text == null)
			{
				Logs.Log("UIFps needs a UILabel component!");
				enabled = false;
				return;
			}
		}

		_timeleft = updateInterval;
	}

}
