using UnityEditor;
using UnityEngine;

namespace TheCubers
{
	class BlenderProcessor : AssetPostprocessor
	{

		void OnPostprocessModel(GameObject obj)
		{
			if (!assetPath.EndsWith(".blend"))
				return;

			int validLayerMask = LayerMask.NameToLayer("ValidGround");

			Transform root = obj.transform;
			Transform child;
			for (int i = 0; i < root.childCount; ++i)
			{
				child = root.GetChild(i);

				switch (child.name.ToLower())
				{
					case "collidersvalid":
						for (int j = 0; j < child.childCount; ++j)
						{
							processClider(child.gameObject, child, child.GetChild(j), false);
							Object.DestroyImmediate(child.GetChild(j).gameObject);
						}
						child.gameObject.layer = validLayerMask;
						break;
					case "colliders":
						for (int j = 0; j < child.childCount; ++j)
							processClider(obj, child, child.GetChild(j), true);
						Object.DestroyImmediate(child.gameObject);
						break;
				}
			}
		}

		private void processClider(GameObject root, Transform parent, Transform collider, bool rootIsRoot)
		{
			switch (collider.name.Substring(0, 3).ToLower())
			{
				case "box":
					var box = root.AddComponent<BoxCollider>();
					if (rootIsRoot)
					{
						box.center = collider.localPosition + parent.localPosition;
						box.center = new Vector3(box.center.x, box.center.z, box.center.y);

						box.size = Vector3.Scale(collider.localScale, parent.localScale) * 2f;
						box.size = new Vector3(box.size.x, box.size.z, box.size.y);
					}
					else
					{
						box.center = collider.localPosition;
						box.size = collider.localScale * 2f;
					}
					break;
				default:
					Debug.LogWarning("Collider type not supported: " + collider.name);
					break;
			}
		}

	}
}
