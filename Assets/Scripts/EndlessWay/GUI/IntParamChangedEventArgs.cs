namespace EndlessWay
{
	public class IntParamChangedEventArgs : ParamChangedEventArgs
	{
		public int ParamValue;

		public IntParamChangedEventArgs(ControlsManager.ParamName paramName, int paramValue)
			: base(paramName)
		{
			ParamValue = paramValue;
		}
	}
}
