using System;
using UnityEngine;

[RequireComponent(typeof(SoundThemePlayer))]
public class WeaponSwingSFX : MonoBehaviour
{
	[Serializable]
	public class AnimToSfxEvent
	{
		public string whenWielderStartsThisAnim;

		public string playThisSfxEvent;
	}

	private SoundThemePlayer mSfxPlayer;

	private TaggedAnimPlayer mAnimPlayer;

	public AnimToSfxEvent[] animToEventList;

	private void Start()
	{
		if (animToEventList != null && animToEventList.Length != 0)
		{
			mAnimPlayer = base.transform.root.GetComponentInChildren<TaggedAnimPlayer>();
			if (mAnimPlayer != null)
			{
				TaggedAnimPlayer taggedAnimPlayer = mAnimPlayer;
				taggedAnimPlayer.animStartedCallback = (TaggedAnimPlayer.TaggedAnimCallback)Delegate.Combine(taggedAnimPlayer.animStartedCallback, new TaggedAnimPlayer.TaggedAnimCallback(OnAnimStarted));
			}
			mSfxPlayer = GetComponent<SoundThemePlayer>();
		}
	}

	private void OnDestroy()
	{
		if (mAnimPlayer != null)
		{
			TaggedAnimPlayer taggedAnimPlayer = mAnimPlayer;
			taggedAnimPlayer.animStartedCallback = (TaggedAnimPlayer.TaggedAnimCallback)Delegate.Remove(taggedAnimPlayer.animStartedCallback, new TaggedAnimPlayer.TaggedAnimCallback(OnAnimStarted));
		}
	}

	private void OnAnimStarted(string newAnimName)
	{
		for (int i = 0; i < animToEventList.Length; i++)
		{
			if (newAnimName == animToEventList[i].whenWielderStartsThisAnim)
			{
				mSfxPlayer.PlaySoundEvent(animToEventList[i].playThisSfxEvent);
			}
		}
	}
}
