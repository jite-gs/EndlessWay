using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class SimpleEnvObject : EnvObject
	{
		public MeshRenderer meshRenderer;

		protected Vector3 meshOrgPosition;
		protected Vector3 meshOrgScale;
		protected Quaternion meshOrgRotation;


		//=== Props ===========================================================

		public float MeshSize { get { return sizes == null ? 1 : sizes[0]; } }

		public Color MeshColor { get { return colors == null ? Color.black : colors[0]; } }

		protected Transform meshTransform { get { return meshRenderer == null ? null : meshRenderer.transform; } }


		//=== Unity ===========================================================

		private void Awake()
		{
			if (meshRenderer == null)
				meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
			if (meshRenderer.IsNull("meshRenderer", GetType()))
				return;

			meshOrgPosition = meshTransform.localPosition;
			meshOrgRotation = meshTransform.localRotation;
			meshOrgScale = meshTransform.localScale;
		}


		//=== Public ==========================================================

		//		public override void Setup()
		//		{
		//			base.Setup();
		//		}
		//
		public override void ApplyColors()
		{
			if (meshRenderer == null)
				return;

			meshRenderer.material.SetColor("_Color", MeshColor);
		}

		public override void ApplySizes()
		{
			meshTransform.localScale = new Vector3(
				meshOrgScale.x * MeshSize, 
				meshOrgScale.x * MeshSize, 
				meshOrgScale.x * MeshSize);
			meshTransform.localPosition = new Vector3(
				meshOrgPosition.x * MeshSize, 
				meshOrgPosition.y * MeshSize, 
				meshOrgPosition.z * MeshSize);
			meshTransform.localRotation = Quaternion.identity; //TODO
		}
	}
}
