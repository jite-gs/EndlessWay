using System;
using DebugStuff;
using SomeRandom;
using UnityEngine;

namespace EndlessWay
{
	public class Stranger : MonoBehaviour
	{
		public EnvObjectSpecification spec;
		public EnvObject obj;

		private Type _selfType = typeof(Stranger);

		//=== Unity ==========================================================

		private void Start()
		{
			if (spec.IsNull("spec", _selfType) || obj.IsNull("obj", _selfType))
				return;

			if (!spec.Init(new UnityRandom()))
				return;

			obj.SetSpecification(spec);
			obj.Setup();
		}


		private float _period = .33f;
		private float _lastTime;
//		private float _pseudoRandomDelta = .01f;
//		private float _pseudoRandom = 0;

		private void Update()
		{
			var newTime = Time.time;
			if (newTime - _lastTime < _period)
				return;

			_lastTime = newTime;


			//			for (int i = 0; i < 1000; i++)
			obj.Setup();
		}


		//=== Private =========================================================





	}
}
