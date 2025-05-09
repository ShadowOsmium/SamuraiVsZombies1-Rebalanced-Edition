using UnityEngine;

public class HeroControls
{
	public delegate void OnPlayerControlCallback();

	private Rect kMoveLeftTouchArea = new Rect(0f, 0f, SUIScreen.width / 2f, SUIScreen.height);

	private Rect kMoveRightTouchArea = new Rect(SUIScreen.width / 2f, 0f, SUIScreen.width / 2f, SUIScreen.height);

	public OnPlayerControlCallback onMoveLeft;

	public OnPlayerControlCallback onMoveRight;

	public OnPlayerControlCallback onDontMove;

	private bool mWasMoving;

	private KeyCode mCurrentKey = KeyCode.A, mHeldKey = KeyCode.None;

	private void UpdatePCControls()
	{
		//flips the else statements depending on the key you last pressed
		//this is so if you're holding d and you press a, you'll go backwards. and vice versa. instead of d just taking priority over a.
		//I did this in brawlers as well. it makes the controls feel a lot less clunky.
		if (mCurrentKey == KeyCode.A)
		{
			if (Input.GetKey(KeyCode.D))
			{
				onMoveRight();

				if (!Input.GetKey(mCurrentKey))
				{
					mCurrentKey = KeyCode.D;
				}
			}
			else if (Input.GetKey(KeyCode.A))
			{
				onMoveLeft();
				
				if (!Input.GetKey(mCurrentKey))
				{
					mCurrentKey = KeyCode.A;
				}
			}
			else
			{
				onDontMove();
			}
		}
		else if (mCurrentKey == KeyCode.D)
		{
			if (Input.GetKey(KeyCode.A))
			{
				onMoveLeft();
				
				if (!Input.GetKey(mCurrentKey))
				{
					mCurrentKey = KeyCode.A;
				}
			}
			else if (Input.GetKey(KeyCode.D))
			{
				onMoveRight();

				if (!Input.GetKey(mCurrentKey))
				{
					mCurrentKey = KeyCode.D;
				}
			}
			else
			{
				onDontMove();
			}
		}
	}

	private void UpdateMobileControls()
	{
		int num = 0;
		if (WeakGlobalInstance<SUIScreen>.instance.inputs.justTouched)
		{
			mWasMoving = false;
		}
		if (WeakGlobalInstance<SUIScreen>.instance.inputs.isTouching)
		{
			Vector2 position = WeakGlobalInstance<SUIScreen>.instance.inputs.position;
			if (mWasMoving || !WeakGlobalSceneBehavior<InGameImpl>.instance.HUD.IsTouchingHUD(position))
			{
				if (kMoveLeftTouchArea.Contains(position))
				{
					num = -1;
					mWasMoving = true;
				}
				else if (kMoveRightTouchArea.Contains(position))
				{
					num = 1;
					mWasMoving = true;
				}
			}
			else
			{
				mWasMoving = false;
			}
		}
		else
		{
			mWasMoving = false;
		}
		if (num < 0)
		{
			if (onMoveLeft != null)
			{
				onMoveLeft();
			}
		}
		else if (num > 0)
		{
			if (onMoveRight != null)
			{
				onMoveRight();
			}
		}
		else if (onDontMove != null)
		{
			onDontMove();
		}
	}

	public void Update()
	{
		if (Application.isMobilePlatform)
		{
			UpdateMobileControls();
		}
		else
		{
			UpdatePCControls();
		}
	}
}
