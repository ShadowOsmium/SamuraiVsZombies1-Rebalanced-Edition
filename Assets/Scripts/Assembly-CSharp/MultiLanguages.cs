using System.Runtime.InteropServices;
using UnityEngine;

public class MultiLanguages : MonoBehaviour
{
	public enum ESystemLanguage
	{
		ESL_English = 0,
		ESL_Chinese = 1,
		ESL_Japanese = 2,
		ESL_Korean = 3,
		ESL_Unknow = 4
	}

	public static bool isMultiLanguages = true;

	public static bool isCheatIAP;

	public static bool isTestChinese;

	public static bool isAsian
	{
		get
		{
			return isMultiLanguages && (GetCurrentLanguageToEnum() == ESystemLanguage.ESL_Chinese || GetCurrentLanguageToEnum() == ESystemLanguage.ESL_Japanese || GetCurrentLanguageToEnum() == ESystemLanguage.ESL_Korean);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	[DllImport("__Internal")]
	private static extern string _GetCurrentLanguageID();

	public static ESystemLanguage GetCurrentLanguageToEnum()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		ESystemLanguage result = ESystemLanguage.ESL_English;
		switch (systemLanguage)
		{
		default:
			return result;
		}
	}

	public static string GetCurrentLanguageToString()
	{
		ESystemLanguage currentLanguageToEnum = GetCurrentLanguageToEnum();
		string result = "Default";
		switch (currentLanguageToEnum)
		{
		case ESystemLanguage.ESL_Chinese:
			result = "Chinese";
			break;
		case ESystemLanguage.ESL_Korean:
			result = "Korean";
			break;
		case ESystemLanguage.ESL_Japanese:
			result = "Japanese";
			break;
		}
		return result;
	}
}
