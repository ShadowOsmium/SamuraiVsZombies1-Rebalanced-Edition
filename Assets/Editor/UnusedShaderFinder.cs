using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class UnusedShaderFinder : Editor
{
	[MenuItem("Unused Shaders/Search")]
	public static void Search()
	{
		string[] allShaders = AssetDatabase.FindAssets("t:Shader");
		List<string> foundShaders = new List<string>();

		foreach (string file in Directory.GetFiles("Assets/", "*", SearchOption.AllDirectories))
		{
			if (Path.GetExtension(file) == ".meta")
			{
				continue;
			}

			string content = File.ReadAllText(file);

			foreach (string shader in allShaders)
			{
				if (content.Contains(shader))
				{
					if (!foundShaders.Contains(shader))
					{
						foundShaders.Add(shader);
					}
				}
			}
		}

		string[] allShaderNames = allShaders.Select(x => AssetDatabase.LoadAssetAtPath<Shader>(AssetDatabase.GUIDToAssetPath(x)).name).ToArray();

		foreach (string file in Directory.GetFiles("Assets/Scripts/", "*", SearchOption.AllDirectories))
		{
			if (Path.GetExtension(file) == ".meta")
			{
				continue;
			}

			string content = File.ReadAllText(file);

			foreach (string shader in allShaderNames)
			{
				if (content.Contains(shader))
				{
					if (!foundShaders.Contains(shader))
					{
						foundShaders.Add(shader);
					}
				}
			}
		}

		Debug.LogError(string.Join("\n", allShaders.Where(x => !foundShaders.Contains(x)).Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray()));
	}
}
