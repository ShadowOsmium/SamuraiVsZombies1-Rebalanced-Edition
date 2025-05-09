using System.Collections.Generic;
using UnityEngine;

internal class FontSetFinder : Singleton<FontSetFinder>
{
	private string mBasePath = string.Empty;

	private SDFTreeNode mConfig;

	public string basePath
	{
		get
		{
			return mBasePath;
		}
	}

	public FontSetFinder()
	{
		mConfig = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/FontsConfig");
		string text = string.Empty;
		if (text == string.Empty)
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<string, string> attribute in mConfig.to("AllResolutions").attributes)
			{
				string value = attribute.Value;
				list.Add(int.Parse(value));
			}
			text = AutoFindClosestResolutionFolderPath(list);
		}
		Debug.Log("Using font REZ: " + text.ToString());
		mBasePath = Singleton<Localizer>.instance.LocalizeFontPath("Fonts/" + text + "/");
		string fontSet = Singleton<GameVersion>.instance.fontSet;
		mConfig = mConfig.to(fontSet);
		if (mConfig != null)
		{
			mConfig = mConfig.to(text);
		}
	}

	public float GetScaleFactor(string fontName)
	{
		if (mConfig == null || !mConfig.hasChild(fontName))
		{
			return 1f;
		}
		SDFTreeNode sDFTreeNode = mConfig.to(fontName);
		if (sDFTreeNode.hasAttribute("scale"))
		{
			return float.Parse(sDFTreeNode["scale"]);
		}
		return 1f;
	}

	public string GetPrefab(string fontName)
	{
		if (mConfig == null || !mConfig.hasChild(fontName))
		{
			return string.Empty;
		}
		SDFTreeNode sDFTreeNode = mConfig.to(fontName);
		if (sDFTreeNode.hasAttribute("prefab"))
		{
			return sDFTreeNode["prefab"];
		}
		return string.Empty;
	}

	private string AutoFindClosestResolutionFolderPath(List<int> allRez)
	{
		int i = 0;
		string result = allRez[i].ToString();
		int num = Mathf.Abs(Screen.height - allRez[i]);
		for (; i < allRez.Count; i++)
		{
			int num2 = Mathf.Abs(Screen.height - allRez[i]);
			if (num2 < num)
			{
				num = num2;
				result = allRez[i].ToString();
			}
		}
		return result;
	}
}
