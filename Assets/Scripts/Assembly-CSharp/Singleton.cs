using System;

public class Singleton<T> where T : class, new()
{
	private static T mUniqueInstance;

	public static T instance
	{
		get
		{
			if (mUniqueInstance == null)
			{
				mUniqueInstance = new T();
			}
			return mUniqueInstance;
		}
	}

	public Singleton()
	{
		if (mUniqueInstance != null)
		{
			throw new InvalidOperationException("Cannot manually instantiate a Singleton<> class.");
		}
	}

	public static bool IsAlive()
	{
		return mUniqueInstance != null;
	}
}
