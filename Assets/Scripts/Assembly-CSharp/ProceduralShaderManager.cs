using System.Collections.Generic;

public class ProceduralShaderManager : WeakGlobalInstance<ProceduralShaderManager>
{
	private List<ShaderEvent> mEvents;

	public ProceduralShaderManager()
	{
		SetUniqueInstance(this);
		mEvents = new List<ShaderEvent>();
	}

	public void update()
	{
		for (int i = 0; i < mEvents.Count; i++)
		{
			if (mEvents[i].shouldDie)
			{
				mEvents.RemoveAt(i);
			}
			else
			{
				mEvents[i].resetToBaseValues();
			}
		}
		foreach (ShaderEvent mEvent in mEvents)
		{
			mEvent.update();
		}
	}

	public static void postShaderEvent(ShaderEvent shaderEvent)
	{
		WeakGlobalInstance<ProceduralShaderManager>.instance.mEvents.Add(shaderEvent);
	}
}
