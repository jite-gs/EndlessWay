using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DebugStuff
{
	public static class VarDumpExtensions
	{
		/// <summary>
		/// Число элементов коллекции/массива, после которого ее дамп прекращается (и переходим к следующему элементу)
		/// </summary>
		public const int CollectionElementsLimit = 200;

		private static Type _selfType = typeof(VarDumpExtensions);


		//=== Public ==========================================================

		/// <summary>
		/// Возвращает строку описания объекта obj. Сам разбирает коллекции (IEnumerable, ICollection, IList, IDictionary), generic и нет, 
		/// а также массивы, включая многомерные
		/// </summary>
		/// <param name="title">Заголовок перед объектом</param>
		/// <param name="depth">Уровень отступа</param>
		public static string VarDump(this object obj, string title, int depth = 0)
		{
			return obj.VarDumpWork(title, depth, false);
		}

		public static string VarDump(this object obj, int depth = 0)
		{
			return obj.VarDumpWork(null, depth, false);
		}

		/// <summary>
		/// Возвращает строку описания объекта obj. Сам разбирает коллекции (IEnumerable, ICollection, IList, IDictionary), generic и нет, 
		/// а также массивы, включая многомерные. Также показывает тип объекта и типы всех составляющих коллекции/массива
		/// </summary>
		/// <param name="title">Заголовок перед объектом</param>
		/// <param name="depth">Уровень отступа</param>
		public static string VarDumpVerbose(this object obj, string title, int depth = 0)
		{
			return obj.VarDumpWork(title, depth, true);
		}

		public static string VarDumpVerbose(this object obj, int depth = 0)
		{
			return obj.VarDumpWork(null, depth, true);
		}

		public static string ColonByCount(int count)
		{
			return count > 0 ? ":" : "";
		}


		//=== Private =========================================================

		private static string VarDumpWork(this object obj, string title, int depth, bool isVerbose)
		{
			var objectDumpLines = new List<DumpLine>();
			objectDumpLines.AddDumpLines(title, obj, depth, isVerbose);
			if (objectDumpLines.IsNull("objectDumpLines", _selfType))
				return "";

			var sb = new StringBuilder();
			for (int i = 0; i < objectDumpLines.Count; i++)
			{
				var debugLine = objectDumpLines[i];
				if (debugLine.IsNull("debugLine", _selfType))
					continue;

				sb.AppendLine(debugLine.ToDump(isVerbose));
			}

			return sb.ToString();
		}

		private static void AddDumpLines(this List<DumpLine> dumpLines, string key, object obj, int depth, bool isVerbose)
		{
			if (dumpLines == null)
				throw new NullReferenceException("AddDumpLines(): dumpLines is null!");

			var sbValue = new StringBuilder();
			Array asArray = null;
			int[] arrayDimensions = null;
			IEnumerable asEnumerable = null;
			ICollection asCollection = null;
			IDictionary asDictionary = null;
			IList asList = null;
			string asString = null;
			if (obj == null)
			{
				sbValue.Append("(null)");
			}
			else
			{
				if (isVerbose)
					sbValue.AppendFormat("<{0}> ", obj.GetType().NiceName());

				asArray = obj as Array;
				if (asArray != null)
				{
					sbValue.AppendFormat("Array[{0}] ({1}){2}",
						GetLengthsByRank(asArray, out arrayDimensions),
						asArray.Length,
						ColonByCount(asArray.Length));
				}
				else
				{
					asEnumerable = obj as IEnumerable;
					if (asEnumerable != null)
					{
						asString = obj as string;
						if (asString != null)
						{
							sbValue.AppendFormat("\"{0}\"", asString);
						}
						else
						{
							asCollection = obj as ICollection;
							if (asCollection != null)
							{
								asList = obj as IList;
								if (asList != null)
								{
									sbValue.AppendFormat("IList ({0}){1}", asList.Count, ColonByCount(asList.Count));
								}
								else
								{
									asDictionary = obj as IDictionary;
									if (asDictionary != null)
									{
										sbValue.AppendFormat("IDictionary ({0}){1}", asDictionary.Count, ColonByCount(asDictionary.Count));
									}
									else
									{
										sbValue.AppendFormat("ICollection ({0}){1}", asCollection.Count, ColonByCount(asCollection.Count));
									}
								}
							}
							else
							{
								sbValue.Append("IEnumerable:");
							}
						}
					}
					else
					{
						sbValue.Append(obj); //Любые другие объекты
					}
				}
			}

			dumpLines.Add(new DumpLine(depth, key, sbValue.ToString()));

			int i = 0;
			if ((asString == null && asEnumerable != null) || asArray != null)
			{
				//Что-то перечисляемое
				if (asArray != null)
				{
					if (arrayDimensions.Length == 1)
					{
						for (int len = asArray.Length; i < len; )
						{
							if (!CollectionElementToDumpLines(dumpLines, null, ref i, asArray.GetValue(i), depth, isVerbose))
								break;
						}
					}
					else
					{
						MultiDimentionArrayElementsToDumpLines(dumpLines, ref i, asArray, 0, null, arrayDimensions, depth, isVerbose);
					}
				}
				else
				{
					if (asDictionary != null)
					{
						var dctKeys = asDictionary.Keys;
						foreach (var elemKey in dctKeys)
						{
							if (!CollectionElementToDumpLines(dumpLines, elemKey, ref i, asDictionary[elemKey], depth, isVerbose))
								break;
						}
					}
					else
					{
						if (asList != null && asList.Count > 0)
						{
							for (int listCount = asList.Count; i < listCount; )
							{
								if (!CollectionElementToDumpLines(dumpLines, null, ref i, asList[i], depth, isVerbose))
									break;
							}
						}
						else
						{
							foreach (var elem in asEnumerable)
							{
								if (!CollectionElementToDumpLines(dumpLines, null, ref i, elem, depth, isVerbose))
									break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Добавляет в dumpLines строки описания элемента коллекции
		/// </summary>
		/// <returns>False - прерываем обход из-за превышения отображаемых элементов</returns>
		private static bool CollectionElementToDumpLines(List<DumpLine> dumpLines, object key, ref int elemIndex, object elem,
			int depth, bool isVerbose)
		{
			if (elemIndex > CollectionElementsLimit)
			{
				dumpLines.Add(new DumpLine(depth + 1, null, "..."));
				return false;
			}

			string keyString = key == null
				? "[" + elemIndex + "]"
				: (key is string
					? "\"" + key + "\""
					: key.ToString());

			dumpLines.AddDumpLines(keyString, elem, depth + 1, isVerbose);

			elemIndex++;
			return true;
		}

		/// <summary>
		/// Добавляет в dumpLines строки описания элементов многомерного (Rank > 1) массива
		/// </summary>
		private static bool MultiDimentionArrayElementsToDumpLines(List<DumpLine> dumpLines, ref int elemIndex, Array asArray,
			int currentDimension, int[] currentIndices, int[] dimensionLengts, int depth, bool isVerbose)
		{
			if (currentIndices == null)
			{
				currentIndices = (int[])dimensionLengts.Clone();
				for (int i = 0, len = dimensionLengts.Length; i < len; i++)
					currentIndices[i] = 0;
			}

			string keyString;
			for (int i = 0, len = dimensionLengts[currentDimension]; i < len; i++)
			{
				currentIndices[currentDimension] = i;
				keyString = GetMultiDimentionArrayKey(currentDimension, currentIndices);
				if (currentDimension + 1 == dimensionLengts.Length)
				{
					//последний уровень - пишем ключи со значениями
					if (elemIndex > CollectionElementsLimit)
					{
						dumpLines.Add(new DumpLine(depth + 1, null, "..."));
						return false;
					}

					dumpLines.AddDumpLines(keyString, asArray.GetValue(currentIndices), depth + 1, isVerbose);
					elemIndex++;
				}
				else
				{
					//верхние уровни - пишем ключ из индексов по текущий
					currentIndices[currentDimension] = i;
					keyString = GetMultiDimentionArrayKey(currentDimension, currentIndices);
					dumpLines.Add(new DumpLine(depth, null, keyString));

					if (!MultiDimentionArrayElementsToDumpLines(dumpLines, ref elemIndex, asArray, currentDimension + 1,
						currentIndices, dimensionLengts, depth + 1, isVerbose))
						return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Возвращает строчку вида '[x,y,z]' с длинами измерений массива array, а также массив этих длин в dimensions
		/// </summary>
		private static string GetLengthsByRank(Array array, out int[] dimensions)
		{
			dimensions = null;
			if (array == null)
				return "";

			var arrayRank = array.Rank;
			if (arrayRank == 1)
			{
				dimensions = new[] { array.Length };
				return "";
			}

			var lstDimensions = new List<int>();
			var sb = new StringBuilder();
			for (int i = 0; i < arrayRank; i++)
			{
				var len = array.GetLength(i);
				lstDimensions.Add(len);
				sb.Append(len);
				if (i + 1 < arrayRank)
					sb.Append(",");
			}
			dimensions = lstDimensions.ToArray();
			return sb.ToString();
		}

		/// <summary>
		/// Возвращает строчку вида '[x,y,]' с указанием индексов currentIndices по currentDimension (далее только запятые)
		/// </summary>
		private static string GetMultiDimentionArrayKey(int currentDimension, int[] currentIndices)
		{
			var sb = new StringBuilder("[");
			for (int i = 0, len = currentIndices.Length; i < len; i++)
			{
				if (i <= currentDimension)
					sb.Append(currentIndices[i]);
				if (i + 1 < len)
					sb.Append(",");
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
}
