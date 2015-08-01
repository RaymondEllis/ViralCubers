using UnityEditor;
using UnityEngine;

namespace TheCubers
{
	class BlenderProcessor : AssetPostprocessor
	{
		private struct blendinfo
		{
			public bool Collider;
			public bool Destroy;
			public bool HasName;
			public bool HasLayer;
			public bool HasTag;
		}

		private const char charStart = '-';
		private const char charSpace = '_';
		private const char charQuote = '-';

		void OnPostprocessModel(GameObject obj)
		{
			if (!assetPath.EndsWith(".blend"))
				return;

			Transform root = obj.transform;
			Transform child;
			for (int i = 0; i < root.childCount; ++i)
			{
				child = root.GetChild(i);
				// is process this child?
				if (child.name.Length == 0 || child.name[0] != charStart)
					continue;

				var info = parseName(child);
				if (info.Collider)
				{
					for (int j = 0; j < child.childCount; ++j)
					{
						if (info.Destroy)
							processCollider(obj, child, child.GetChild(j), true, true);
						else
						{
							processCollider(child.gameObject, child, child.GetChild(j), false, false);
							Object.DestroyImmediate(child.GetChild(j).gameObject);
						}
					}
				}

				if (info.Destroy)
					Object.DestroyImmediate(child.gameObject);
			}
		}

		// -Colliders Layer "VilidGround"
		private blendinfo parseName(Transform obj)
		{
			string str = obj.name;

			blendinfo info = new blendinfo();

			int i = 1;
			while (i < str.Length)
			{
				string arg = nextStr(str, ref i);
				switch (arg.ToLower())
				{
					case "":
						break;

					case "coll":
					case "collider":
					case "colliders":
						info.Collider = true;
						break;

					case "lay":
					case "layer":
						info.HasLayer = true;
						string layerstr = nextStr(str, ref i);
						int layer = LayerMask.NameToLayer(layerstr);
						if (layer == -1)
							Debug.LogWarning("Unknown layer: " + layerstr);
						else
							obj.gameObject.layer = layer;
						break;

					case "tag":
						info.HasTag = true;
						obj.gameObject.tag = nextStr(str, ref i);
						break;

					case "x":
					case "destroy":
						info.Destroy = true;
						break;

					case "name":
						info.HasName = true;
						obj.name = nextStr(str, ref i);
						break;

					default:
						Debug.LogWarning("Unknown model arg: " + arg);
						break;
				}
			}

			// destory if we havd collider but no layer or tag.
			if (info.Collider && !(info.HasLayer || info.HasTag))
				info.Destroy = true;

			// name if has no name.
			if (!info.HasName && !info.Destroy)
			{
				var tmp = new System.Text.StringBuilder();
				tmp.Append(charStart);

				if (info.Collider)
					tmp.Append("Collider");

				if (info.HasTag)
					tmp.Append(" " + obj.tag);
				if (info.HasLayer)
					tmp.Append(" " + LayerMask.LayerToName(obj.gameObject.layer));

				obj.name = tmp.ToString();
			}

			return info;
		}
		private string nextStr(string str, ref int i)
		{
			var tmp = new System.Text.StringBuilder();
			bool quote = false;
			bool start = true;
			while (i < str.Length)
			{
				if (start)
				{
					if (str[i] != charSpace)
						start = false;
					if (str[i] == charQuote)
						quote = true;
					else if (str[i] != charSpace)
						tmp.Append(str[i]);
					++i;
				}
				else
				{
					if (quote)
					{
						if (str[i] == charQuote)
						{
							++i;
							break;
						}
					}
					else if (str[i] == charSpace)
						break;

					tmp.Append(str[i]);
					++i;
				}
			}
			return tmp.ToString();
		}

		private void processCollider(GameObject root, Transform parent, Transform collider, bool transformWithParent, bool flipYZ)
		{
			switch (collider.name.Substring(0, 3).ToLower())
			{
				case "box":
					var box = root.AddComponent<BoxCollider>();
					if (transformWithParent)
					{
						box.center = collider.localPosition + parent.localPosition;
						box.size = Vector3.Scale(collider.localScale, parent.localScale) * 2f;
					}
					else
					{
						box.center = collider.localPosition;
						box.size = collider.localScale * 2f;
					}

					if (flipYZ)
					{
						box.center = new Vector3(box.center.x, box.center.z, box.center.y);
						box.size = new Vector3(box.size.x, box.size.z, box.size.y);
					}
					break;
				default:
					Debug.LogWarning("Collider type not supported: " + collider.name);
					break;
			}
		}

	}
}
