using System.Runtime.InteropServices;
using UnityEngine;

public class NUF
{
	public delegate void OnNeverShowAlertAgainDelegate();

	[DllImport("__Internal")]
	private static extern int _ScheduleNotification(int tD, string mB, string bT, string cSF);

	[DllImport("__Internal")]
	private static extern int _CancelAllLocalNotification();

	[DllImport("__Internal")]
	private static extern int _PresentLocalNotificationNow(string mB, string bT, string cSF);

	[DllImport("__Internal")]
	private static extern bool _IsAppStorePaymentsAllowed();

	[DllImport("__Internal")]
	private static extern void _PresentRateMeAlert(string title, string messageBody, string cancelButton, string acceptButton, string url, string neverButton, OnNeverShowAlertAgainDelegate neverShowAgainCallback);

	[DllImport("__Internal")]
	private static extern void _GotoURL(string url);

	[DllImport("__Internal")]
	private static extern void _allowPortrait(bool allow);

	public static int ScheduleNotification(int timeDelay, string messageBody, string buttonText, string customSoundFilename)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _ScheduleNotification(timeDelay, messageBody, buttonText, customSoundFilename);
		}
		return 1;
	}

	public static int CancelAllLocalNotification()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _CancelAllLocalNotification();
		}
		return 1;
	}

	public static int PresentLocalNotificationNow(string messageBody, string buttonText, string customSoundFilename)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _PresentLocalNotificationNow(messageBody, buttonText, customSoundFilename);
		}
		return 1;
	}

	public static bool IsAppStorePaymentsAllowed()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _IsAppStorePaymentsAllowed();
		}
		return true;
	}

	public static void PresentRateMeAlert(string title, string messageBody, string cancelButton, string acceptButton, string url, string neverButton, OnNeverShowAlertAgainDelegate neverAgainCallback)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_PresentRateMeAlert(title, messageBody, cancelButton, acceptButton, url, neverButton, neverAgainCallback);
		}
	}

	public static void AllowPortrait(bool allow)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_allowPortrait(allow);
		}
	}

	public static void GotoURL(string url)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_GotoURL(url);
		}
	}
}
