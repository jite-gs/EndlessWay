using UnityEngine;

public static class Logs
{
	public static bool DoLogging
	{
		get { return Debug.logger.logEnabled && Application.isEditor; }
		set { Debug.logger.logEnabled = value; }
	}

	public static void Log(string str, params object[] parameters)
	{
		LogWork(LogType.Log, str, parameters);
	}

	public static void LogWarning(string str, params object[] parameters)
	{
		LogWork(LogType.Warning, str, parameters);
	}

	public static void LogError(string str, params object[] parameters)
	{
		LogWork(LogType.Error, str, parameters);
	}

	private static void LogWork(LogType logType, string str, params object[] parameters)
	{
		if (!DoLogging)
			return;

		switch (logType)
		{
			case LogType.Error:
			case LogType.Assert:
				Debug.LogErrorFormat(str, parameters);
				break;

			case LogType.Warning:
				Debug.LogWarningFormat(str, parameters);
				break;

			default:
				Debug.LogFormat(str, parameters);
				break;
		}
	}
}
