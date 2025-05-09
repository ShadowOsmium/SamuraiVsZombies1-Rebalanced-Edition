using UnityEngine;
using Debug = UnityEngine.Debug;

public class Localizer : Singleton<Localizer>
{
	private SDFTreeNode mData;

	private string mLocalizedSpritePath = "Sprites/Localized/" + Singleton<GameVersion>.instance.language + "/";

	private string mLocalizedFontPath = "Fonts/" + Singleton<GameVersion>.instance.fontSet + "/";

	public bool Has(string id)
	{
		CacheData();
		return mData.hasAttribute(id);
	}

	public string Get(string id)
	{
		CacheData();
		if (mData.hasAttribute(id))
		{
			return mData[id];
		}
		Debug.Log("ERROR: Unable to find localized text id: " + id);
		return "*MISSING*";
	}

	public string Parse(string source)
	{
		if (source.Length > 0 && source[0] == '@')
		{
			if (!Application.isMobilePlatform)
			{
				if (Singleton<GameVersion>.instance.language == "Default" && source.Substring(1).StartsWith("tutorial_ingame") || source.Substring(1).StartsWith("waveselect_inst"))
				{
					return Get(source.Substring(1) + "_pc");
				}
			}
			return Get(source.Substring(1));
		}
		return source;
	}

	public string LocalizeSpritePath(string originalPath)
	{
		return originalPath.Replace("Sprites/Localized/", mLocalizedSpritePath);
	}

	public string LocalizeFontPath(string originalPath)
	{
		return originalPath.Replace("Fonts/", mLocalizedFontPath);
	}

	private void CacheData()
	{
		if (mData == null)
		{
			mData = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Text/" + Singleton<GameVersion>.instance.language);
		}
	}
}
