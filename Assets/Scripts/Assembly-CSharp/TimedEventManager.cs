using System.Collections.Generic;
using UnityEngine;

public class TimedEventManager : WeakGlobalInstance<TimedEventManager>
{
	private class TimeEvent
	{
		public int uid;

		public float timer;

		public OnSUIGenericCallback callback;
	}

	private List<TimeEvent> mTimers = new List<TimeEvent>();

	private int mNextUID = 1;

	public TimedEventManager()
	{
		SetUniqueInstance(this);
	}

	public void Update()
	{
		for (int num = mTimers.Count - 1; num >= 0; num--)
		{
			TimeEvent timeEvent = mTimers[num];
			timeEvent.timer -= Time.deltaTime;
			if (timeEvent.timer <= 0f)
			{
				if (timeEvent.callback != null)
				{
					timeEvent.callback();
				}
				mTimers.RemoveAt(num);
			}
		}
	}

	public int Add(float delay, OnSUIGenericCallback callback)
	{
		int num = mNextUID;
		mNextUID++;
		TimeEvent timeEvent = new TimeEvent();
		timeEvent.uid = num;
		timeEvent.timer = delay;
		timeEvent.callback = callback;
		mTimers.Add(timeEvent);
		return num;
	}

	public void Stop(int timerID)
	{
		foreach (TimeEvent mTimer in mTimers)
		{
			if (mTimer.uid == timerID)
			{
				mTimers.Remove(mTimer);
				break;
			}
		}
	}
}
