using System;
using UnityEngine;

namespace DebugStuff
{
	public static class DebugExtensions
	{

		//=== IsNull ==========================================================

		public static bool IsNull<T>(this T targetVar, string varName, Type reporterType, string reporterName = null)
		{
			bool res = targetVar == null;
			if (res)
				Debug.Log(string.Format("In <{0}>{1}: <{2}> '{3}' is null!",
					reporterType.Name,
					string.IsNullOrEmpty(reporterName) ? "" : " '" + reporterName + "'",
					typeof(T),
					varName));
			return res;
		}

	}
}