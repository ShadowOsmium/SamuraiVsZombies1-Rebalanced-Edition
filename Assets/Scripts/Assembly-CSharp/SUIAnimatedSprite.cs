using System.Collections.Generic;
using UnityEngine;

public class SUIAnimatedSprite : SUISprite
{
	public class Command
	{
		public string action;

		public int param;

		public float delay;

		public Command(string _action, int _param, float _delay)
		{
			action = _action;
			param = _param;
			delay = _delay;
		}
	}

	private delegate void PlayFunc();

	public delegate void OnCustomCommandCallback(Command cmd);

	private List<string> mFrameFiles;

	private List<Command> mCommands = new List<Command>();

	private int mCurrentFrame = -1;

	private int mCurrentCommand;

	private float mDelayTimer;

	private float mTimer;

	private PlayFunc mPlayFunc;

	public OnCustomCommandCallback onCustomCommand;

	public int frame
	{
		get
		{
			return mCurrentFrame;
		}
		set
		{
			int num = Mathf.Max(0, value);
			num = Mathf.Min(value, numFrames - 1);
			if (num != mCurrentFrame)
			{
				mCurrentFrame = num;
				base.texture = mFrameFiles[mCurrentFrame];
			}
		}
	}

	public int numFrames
	{
		get
		{
			return mFrameFiles.Count;
		}
	}

	public bool isPlaying
	{
		get
		{
			return mPlayFunc != null;
		}
	}

	public SUIAnimatedSprite(List<string> frameFiles)
	{
		mFrameFiles = frameFiles;
		if (mFrameFiles == null)
		{
			mFrameFiles = new List<string>();
		}
		else
		{
			frame = 0;
		}
	}

	public SUIAnimatedSprite(string folderPath, int firstFrame, int lastFrame)
	{
		mFrameFiles = new List<string>();
		for (int i = firstFrame; i <= lastFrame; i++)
		{
			string item = folderPath + "/" + string.Format("{0:000}", i);
			mFrameFiles.Add(item);
		}
		frame = 0;
	}

	public override void Update()
	{
		if (mPlayFunc != null)
		{
			mPlayFunc();
		}
		base.Update();
	}

	public void Play(float delay)
	{
		mPlayFunc = NormalPlayUpdate;
		mDelayTimer = delay;
		mTimer = delay;
		frame = 0;
	}

	public void PlayCommands()
	{
		mPlayFunc = CommandsPlayUpdate;
		mCurrentCommand = 0;
		mTimer = SUIScreen.deltaTime;
		RunCurrentCommand();
	}

	public void Stop()
	{
		mPlayFunc = null;
	}

	public void ClearCommands()
	{
		mCommands.Clear();
	}

	public void AddCommand(Command cmd)
	{
		mCommands.Add(cmd);
	}

	public void AddCommand(string action, int param, float delay)
	{
		mCommands.Add(new Command(action, param, delay));
	}

	private void NormalPlayUpdate()
	{
		mTimer -= SUIScreen.deltaTime;
		if (mTimer <= 0f)
		{
			if (frame == numFrames - 1)
			{
				mPlayFunc = null;
				return;
			}
			frame++;
			mTimer = mDelayTimer + mTimer;
		}
	}

	private void CommandsPlayUpdate()
	{
		mTimer -= SUIScreen.deltaTime;
		while (isPlaying && mTimer <= 0f)
		{
			RunCurrentCommand();
		}
	}

	private void RunCurrentCommand()
	{
		if (mCurrentCommand < 0 || mCurrentCommand >= mCommands.Count)
		{
			mPlayFunc = null;
			return;
		}
		Command command = mCommands[mCurrentCommand];
		if (command.action == "set")
		{
			frame = command.param;
			mCurrentCommand++;
		}
		else if (command.action == "jump")
		{
			mCurrentCommand = FindTagIndex(command.param);
		}
		else if (command.action == "tag")
		{
			mCurrentCommand++;
		}
		else
		{
			if (onCustomCommand != null)
			{
				onCustomCommand(command);
			}
			mCurrentCommand++;
		}
		mTimer = command.delay + mTimer;
	}

	private int FindTagIndex(int tagID)
	{
		for (int i = 0; i < mCommands.Count; i++)
		{
			if (mCommands[i].action == "tag" && mCommands[i].param == tagID)
			{
				return i;
			}
		}
		return -1;
	}
}
