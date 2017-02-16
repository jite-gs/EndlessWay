using DebugStuff;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Показывает value слайдера
/// Навесить на трансформ с компонентом Text, находящимся внутри Slider (или вручную указать поля slider и text)
/// На компоненте Slider в разделе OnValueChanged сделать ссылку на метод OnSliderValueChanged данного компонента
/// </summary>
public class SliderValueText : MonoBehaviour
{
	public Slider slider;
	public Text text;
	public int signsAfterPoint = 1;

	public bool IsWrong { get; private set; }


	//=== Unity ===============================================================

	private void Awake()
	{
		IsWrong = false;
		if (text == null)
		{
			text = GetComponent<Text>();
			if (text.IsNull("text", GetType()))
			{
				Logs.Log("<{0}> Not found Text component ({1})", GetType(), transform.FullName());
				IsWrong = true;
				return;
			}
		}

		if (slider == null)
		{
			slider = transform.GetParentComponent<Slider>();
			if (slider.IsNull("slider", GetType()))
			{
				Logs.Log("<{0}> Not found Slider component ({1})", GetType(), transform.FullName());
				IsWrong = true;
				return;
			}
		}
	}


	//=== Unity ===============================================================

	public void OnSliderValueChanged()
	{
		if (IsWrong)
			return;

		text.text = slider.wholeNumbers ? Mathf.RoundToInt(slider.value).ToString() : slider.value.ToString("f" + signsAfterPoint);
	}
}
