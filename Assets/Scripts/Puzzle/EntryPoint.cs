using System.Collections.Generic;
using DebugStuff;
using UnityEngine;

namespace Puzzle
{
	public class EntryPoint : MonoBehaviour
	{
		//		private int[, ,] arrInt3 = new int[,,]
		//		{
		//			{
		//				{ 1, 2, 0 }, 
		//				{ 2, 4, 4 }
		//			},
		//			{
		//				{ 21, 22, 20 }, 
		//				{ 22, 24, 24 }
		//			}
		//		};

		void Start()
		{
			//Logs.Log(arrInt3.VarDumpVerbose("arrInt3"));

			var puzzleField = new PuzzleField();
			puzzleField.InitSize(10);

			List<SliceMove> moves;
			var res = puzzleField.TryToSlide(new SlicePosition(0, puzzleField.Size - 1), out moves);
			Logs.Log("try={0}, {1}", res, moves.VarDump("moves"));
			Logs.Log(puzzleField.SlicesToString());
			
			res = puzzleField.TryToSlide(new SlicePosition(0, 0), out moves);
			Logs.Log("try={0}, {1}", res, moves.VarDump("moves"));
			Logs.Log(puzzleField.SlicesToString());
		}

	}
}
