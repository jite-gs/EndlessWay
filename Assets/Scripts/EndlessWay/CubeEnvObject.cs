using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class CubeEnvObject : EnvObject
	{
		public MeshRenderer meshRenderer;

		private Transform cubeTransform { get { return meshRenderer == null ? null : meshRenderer.transform; } }


		//=== Props ===========================================================

		public float CubeSize { get { return sizes == null ? 1 : sizes[0]; } }

		public Color CubeColor { get { return colors == null ? Color.black : colors[0]; } }


		//=== Public ==========================================================

		public override void Setup()
		{
			if (meshRenderer == null)
				meshRenderer = transform.GetComponentInChildren<MeshRenderer>();

			meshRenderer.IsNull("meshRenderer", GetType());
			base.Setup();
		}

		public override void ApplyColors()
		{
			if (meshRenderer == null)
				return;

			meshRenderer.material.SetColor("_Color", CubeColor);
		}

		public override void ApplySizes()
		{
			cubeTransform.localScale = new Vector3(CubeSize, CubeSize, CubeSize);
			cubeTransform.localPosition = new Vector3(0, CubeSize / 2, 0);
			cubeTransform.localRotation = Quaternion.identity;
		}
	}
}
