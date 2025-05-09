using UnityEngine;

public class MenusFlow : Singleton<MenusFlow>
{
	private string mCurrentScene = "MainMenu";

	private string mPreviousScene;

	public string previousScene
	{
		get
		{
			return mPreviousScene;
		}
	}

	public void LoadScene(string scene)
	{
		mPreviousScene = mCurrentScene;
		mCurrentScene = scene;
		DoLoadScene(mCurrentScene);
	}

	public void LoadPreviousScene()
	{
		mCurrentScene = mPreviousScene;
		DoLoadScene(mCurrentScene);
		mPreviousScene = string.Empty;
	}

	private void DoLoadScene(string scene)
	{
		if (scene == "Store" || scene == "MenuWith3DLevel")
		{
			SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = true;
		}
		Application.LoadLevel(scene);
	}
}
