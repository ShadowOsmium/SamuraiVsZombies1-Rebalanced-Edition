using UnityEngine;

public class BuildInfo : ScriptableObject
{
	public string m_appVersion;

	public string m_buildTag;

	public string AppVersion
	{
		get
		{
			return m_appVersion;
		}
		set
		{
			m_appVersion = value;
		}
	}

	public string BuildTag
	{
		get
		{
			return m_buildTag;
		}
		set
		{
			m_buildTag = value;
		}
	}
}
