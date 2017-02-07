using DebugStuff;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{

	void Start()
	{
		Logs.Log("Test {0} {1}", 1.VarDump("vd test"), 2.VarDump("vd test", 0, true));
		//		Logs.DoLogging = false;
		//		Logs.Log("Test {0} {1}", 5, 6);
		//		Logs.DoLogging = true;
		//		Logs.Log("Test {0} {1}", 3, 4);
		var pf = new PuzzleField();
		pf.InitSize(12);

	}

}
