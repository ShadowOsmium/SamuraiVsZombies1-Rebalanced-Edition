using Debug = UnityEngine.Debug;

public class Analytics : Singleton<Analytics>
{
	private const string google_flurry_live_id = "SMEMR3VMDANCMUCP3Y2M";

	private const string google_flurry_stage_id = "9ZZ6E6KUR7DVH3V42YWU";

	private const string amazon_flurry_live_id = "U6CX3T2ZYVKWMUBB9FFL";

	private const string amazon_flurry_stage_id = "WAUBTWCJ4CMFE68XAA34";

	public static string FLURRY_ID
	{
		get
		{
			if (ApplicationUtilities.IsBuildType("amazon"))
			{
				if (Debug.isDebugBuild)
				{
					return "WAUBTWCJ4CMFE68XAA34";
				}
				return "U6CX3T2ZYVKWMUBB9FFL";
			}
			if (Debug.isDebugBuild)
			{
				return "9ZZ6E6KUR7DVH3V42YWU";
			}
			return "SMEMR3VMDANCMUCP3Y2M";
		}
	}

	public Analytics()
	{
	}

	public void LogEvent(string eventTypeId, string info = "", int eventValue = 0, int eventRefrence = 0)
	{
		Debug.Log(eventTypeId + ": " + info + ", " + eventValue + ", " + eventRefrence);
	}

	public void StartSession()
	{
	}

	public void EndSession()
	{
	}
}
