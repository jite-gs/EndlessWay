using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DebugStuff
{
	public static class DebugExtensions
	{

		//=== IsNull ==========================================================

		public static bool IsNull<T>(this T targetVar, string varName, Type reporterType, string reporterName = null) where T : class 
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


		//=== Type ============================================================

		public static Dictionary<string, string> SystemTypeNameToNiceName = new Dictionary<string, string>()
		{
			{"Boolean", "bool"},
			{"Int32", "int"},
			{"Single", "float"},
			{"Int64", "long"},
			{"Int16", "short"},
			{"UInt32", "uint"},
			{"UInt64", "ulong"},
			{"UInt16", "ushort"},
		};

		public static string NiceName(this Type type)
		{
			if (type == null)
				return "";

			if (!type.IsGenericType)
			{
				var simpleTypeName = type.Name;
				var isArray = type.IsArray;
				if (type.IsPrimitive || isArray) // �� object, �� decimal
				{
					foreach (var kvp in SystemTypeNameToNiceName)
					{
						if (isArray)
						{
							var stringBegin = kvp.Key + "[";
							if (simpleTypeName.IndexOf(stringBegin) == 0)
							{
								simpleTypeName = simpleTypeName.Replace(stringBegin, kvp.Value + "[");
							}
						}
						else
						{
							if (simpleTypeName == kvp.Key)
								simpleTypeName = kvp.Value;
						}
					}
				}
				else
				{
					if (type == typeof(string) || type == typeof(decimal) || type == typeof(object))
						simpleTypeName = simpleTypeName.ToLower();
				}
				return simpleTypeName;
			}

			var typeName = type.Name;
			var apostropheFirstIndex = typeName.IndexOf('`');
			var sb = new StringBuilder(typeName.Substring(0, apostropheFirstIndex));

			sb.Append("<");
			var genericArgs = type.GetGenericArguments();
			for (int i = 0; i < genericArgs.Length; i++)
			{
				if (i > 0)
					sb.Append(", ");
				sb.Append(genericArgs[i].NiceName());
			}
			sb.Append(">");
			return sb.ToString();
		}

	}
}
