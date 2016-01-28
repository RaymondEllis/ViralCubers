using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

/// <summary>
/// Creates scene thumbnailes while in Unity editor.
/// </summary>
[CustomEditor(typeof(Camera))]
public class ThumbGen : Editor
{
	private int size = 128;

	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Generate Thumb"))
		{
			Camera cam = (Camera)target;
			CameraClearFlags clearFlags = cam.clearFlags;
			cam.clearFlags = CameraClearFlags.Depth;

			RenderTexture renTex = new RenderTexture(size, size, 1);
			cam.targetTexture = renTex;
			cam.Render();
			cam.targetTexture = null;

			Texture2D tex = new Texture2D(size, size);

			RenderTexture.active = renTex;
			tex.ReadPixels(new Rect(0, 0, size, size), 0, 0);
			RenderTexture.active = null;

			byte[] data = tex.EncodeToPNG();

			string scenePath = Path.GetFullPath(Path.GetDirectoryName(EditorSceneManager.GetActiveScene().path));
			string sceneName = EditorSceneManager.GetActiveScene().name;
			string file = scenePath + "/Thumbnails/" + sceneName + ".png";
			File.WriteAllBytes(file, data);
			Debug.Log("Thumbnail saved to: " + file);

			cam.clearFlags = clearFlags;
		}

		base.OnInspectorGUI();
	}
}
