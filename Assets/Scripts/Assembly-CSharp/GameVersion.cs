using System;
using UnityEngine;

public class GameVersion : Singleton<GameVersion>
{
	public const string OBB_FILE_NAME_MASK = "main.{0}.com.glu.samuzombie.obb";

	public const int VersionCode = 340;

	public int major = 3;

	public int minor = 4;

	public int revision;

	public int bundleMajor = 3;

	public int bundleMinor = 4;

	public int bundleRevision;

	private BuildInfo m_buildInfo;

	public string onlineAssetBundlePath
	{
		get
		{
			string empty = string.Empty;
			empty = (Debug.isDebugBuild ? ((!ApplicationUtilities.IsBuildType("amazon")) ? "https://s3.amazonaws.com/griptonite/svz/android/DLC/google/stage/version{0}_{1}_{2}/Registry.assetbundle" : "https://s3.amazonaws.com/griptonite/svz/android/DLC/amazon/stage/version{0}_{1}_{2}/Registry.assetbundle") : ((!ApplicationUtilities.IsBuildType("amazon")) ? "https://d2lk18ssnvgdhj.cloudfront.net/svz/android/DLC/google/live/version{0}_{1}_{2}/Registry.assetbundle" : "https://d2lk18ssnvgdhj.cloudfront.net/svz/android/DLC/amazon/live/version{0}_{1}_{2}/Registry.assetbundle"));
			return string.Format(empty, bundleMajor.ToString(), bundleMinor.ToString(), bundleRevision.ToString());
		}
	}

	public string language
	{
		get
		{
			if (MultiLanguages.isMultiLanguages)
			{
				return MultiLanguages.GetCurrentLanguageToString();
			}
			return "Default";
		}
	}

	public string fontSet
	{
		get
		{
			if (MultiLanguages.isMultiLanguages)
			{
				return MultiLanguages.GetCurrentLanguageToString();
			}
			return "Default";
		}
	}

	public BuildInfo BuildInfo
	{
		get
		{
			return m_buildInfo;
		}
	}

	public GameVersion()
	{
		m_buildInfo = Resources.Load("BuildInfo") as BuildInfo;
		if (m_buildInfo == null)
		{
			m_buildInfo = ScriptableObject.CreateInstance<BuildInfo>();
			m_buildInfo.AppVersion = string.Format("{0}.{1}.{2}", major.ToString(), minor.ToString(), revision.ToString());
			m_buildInfo.BuildTag = "SvZ_" + DateTime.Now.ToString("yyyyMMdd-HHmm") + "_A";
		}
		if (Debug.isDebugBuild)
		{
			m_buildInfo.AppVersion += "D";
		}
	}

	public override string ToString()
	{
		return m_buildInfo.AppVersion;
	}
}
