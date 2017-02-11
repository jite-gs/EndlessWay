using System.Collections.Generic;
using DebugStuff;
using UnityEngine;

namespace Puzzle
{
	public class EntryPoint : MonoBehaviour
	{

		void Start()
		{

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
