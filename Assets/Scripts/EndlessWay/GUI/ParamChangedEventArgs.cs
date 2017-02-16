namespace EndlessWay
{
	public abstract class ParamChangedEventArgs
	{
		public ControlsManager.ParamName ParamName;

		protected ParamChangedEventArgs(ControlsManager.ParamName paramName)
		{
			ParamName = paramName;
		}
	}
}
