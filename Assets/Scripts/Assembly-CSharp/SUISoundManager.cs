using UnityEngine;

public class SUISoundManager : Singleton<SUISoundManager>
{
	private SDFTreeNode mSoundInfos;

	private GameObject mDefaultGOEmitter;

	public SUISoundManager()
	{
		mDefaultGOEmitter = new GameObject("DefaultSoundEmitter");
		Object.DontDestroyOnLoad(mDefaultGOEmitter);
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Registry/Config");
		if (sDFTreeNode.hasChild("DefaultSounds"))
		{
			mSoundInfos = sDFTreeNode.to("DefaultSounds");
		}
		else
		{
			Debug.Log("WARNING: Could not find the DefaultSounds section in the config file.");
		}
	}

	public void Play(string soundID)
	{
		Play(soundID, mDefaultGOEmitter);
	}

	public void Play(string soundID, GameObject target)
	{
		if (mSoundInfos == null)
		{
			Debug.Log("ERROR: Trying to play a sound without a sound config file.");
			return;
		}
		string text = soundID;
		if (mSoundInfos.hasAttribute(soundID))
		{
			text = mSoundInfos[soundID];
		}
		if (!(text == string.Empty) && !(text.ToLower() == "none"))
		{
			ForcePlay(text, AcquireAudioSource(target));
		}
	}

	private AudioSource AcquireAudioSource(GameObject obj)
	{
		AudioSource component = obj.GetComponent<AudioSource>();
		if (component != null)
		{
			return component;
		}
		obj.AddComponent<AudioSource>();
		return obj.GetComponent<AudioSource>();
	}

	private void ForcePlay(string soundFile, AudioSource target)
	{
		if (!(target == null))
		{
			AudioClip audioClip = (AudioClip)Resources.Load(soundFile, typeof(AudioClip));
			if (audioClip != null)
			{
				target.PlayOneShot(audioClip);
			}
			else
			{
				Debug.Log("Warning: could not find sound file: '" + soundFile + "'");
			}
		}
	}
}
