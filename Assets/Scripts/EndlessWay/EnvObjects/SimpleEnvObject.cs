using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	/// <summary>
	/// Объект с одним мешем
	/// </summary>
	public class SimpleEnvObject : EnvObject
	{
		public MeshRenderer meshRenderer;


		//=== Props ===========================================================

		private Transform meshTransform { get { return meshRenderer == null ? null : meshRenderer.transform; } }


		//=== Unity ===========================================================

		private void Awake()
		{
			if (meshRenderer == null)
				meshRenderer = transform.GetComponentInChildren<MeshRenderer>();
			if (meshRenderer.IsNull("meshRenderer", GetType()))
			{
				IsWrong = true;
				return;
			}

			orgPosition = meshTransform.localPosition;
			orgRotation = meshTransform.localRotation;
			orgScale = meshTransform.localScale;
		}


		//=== Public ==========================================================

		public override Vector2 GetOccupiedArea()
		{
			return IsWrong ? Vector2.one : new Vector2(meshTransform.localScale.x, meshTransform.localScale.z);
		}

		public override void ApplySizes()
		{
			if (IsWrong)
				return;

			meshTransform.localScale = new Vector3(
				orgScale.x * MainSize,
				orgScale.y * MainSize,
				orgScale.z * MainSize);
			meshTransform.localPosition = new Vector3(
				orgPosition.x * MainSize,
				orgPosition.y * MainSize,
				orgPosition.z * MainSize);
			meshTransform.localRotation = Quaternion.identity; //TODO
		}

		public override void ApplyColors()
		{
			if (IsWrong)
				return;

			meshRenderer.material.SetColor("_Color", MainColor);
		}

	}
}
