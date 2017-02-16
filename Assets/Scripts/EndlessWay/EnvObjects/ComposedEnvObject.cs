using System;
using DebugStuff;
using UnityEngine;

namespace EndlessWay
{
	public class ComposedEnvObject : EnvObject
	{
		public MeshRenderer[] meshRenderers;
		public Transform meshesRoot;
		public Vector2 selfAreaSize;

		private static Type _selfType;


		//=== Unity ===========================================================

		private void Awake()
		{
			if (_selfType == null)
				_selfType = GetType();

			if (meshRenderers == null)
			{
				meshRenderers = transform.GetComponentsInChildren<MeshRenderer>();
				if (meshRenderers.IsNull("meshRenderers", GetType()))
				{
					IsWrong = true;
					return;
				}
			}

			if (meshesRoot == null)
			{
				if (transform.childCount == 0)
				{
					Logs.LogError("<{0}> '{1}' IsWrong: meshesRoot not found", _selfType, name);
					IsWrong = true;
					return;
				}

				meshesRoot = transform.GetChild(0);
			}

			orgPosition = meshesRoot.localPosition;
			orgRotation = meshesRoot.localRotation;
			orgScale = meshesRoot.localScale;
		}


		//=== Public ==========================================================

		public override Vector2 GetOccupiedArea()
		{
			return IsWrong ? Vector2.one : new Vector2(meshesRoot.localScale.x * selfAreaSize.x, meshesRoot.localScale.z * selfAreaSize.y);
		}

		public override void ApplySizes()
		{
			if (IsWrong)
				return;

			meshesRoot.localScale = new Vector3(
				orgScale.x * MainSize,
				orgScale.y * MainSize,
				orgScale.z * MainSize);
			meshesRoot.localPosition = new Vector3(
				orgPosition.x * MainSize,
				orgPosition.y * MainSize,
				orgPosition.z * MainSize);
		}

		public override void ApplyColors()
		{
			if (IsWrong)
				return;

			for (int i = 0, len = meshRenderers.Length; i < len; i++)
			{
				meshRenderers[i].material.SetColor("_Color", MainColor);
			}

		}
	}
}
