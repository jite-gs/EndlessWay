namespace EndlessWay
{
	public class FloatParamChangedEventArgs : ParamChangedEventArgs
	{
		public float ParamValue;

		public FloatParamChangedEventArgs(ControlsManager.ParamName paramName, float paramValue)
			: base(paramName)
		{
			ParamValue = paramValue;
		}
	}
}
