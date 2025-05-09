using UnityEngine;

public class OptionsImpl : SceneBehaviour
{
	private SUILayout mLayout;

	private YesNoDialog mConfirmDialog;

	private SUILayout mAboutDialog;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/Options");
		mLayout.AnimateIn();
		((SUIButton)mLayout["backBtn"]).onButtonPressed = delegate
		{
			GotoScene("MainMenu");
		};
		((SUIButton)mLayout["credits"]).onButtonPressed = delegate
		{
			GotoScene("Credits");
		};
		((SUIButton)mLayout["about"]).onButtonPressed = PopAboutInfo;
		((SUIButton)mLayout["privacy"]).onButtonPressed = ShowPrivacy;
		((SUIButton)mLayout["resetSave"]).onButtonPressed = PopResetDataConfirmDialog;
		((SUIButton)mLayout["resetSave"]).visible = false;
	}

	private void Update()
	{
		if (SceneBehaviourUpdate())
		{
			return;
		}
		if (mAboutDialog != null)
		{
			mAboutDialog.Update();
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				mAboutDialog.Destroy();
				mAboutDialog = null;
			}
		}
		else if (mConfirmDialog != null)
		{
			mConfirmDialog.Update();
			if (mConfirmDialog.isDone)
			{
				mConfirmDialog.Destroy();
				mConfirmDialog = null;
			}
		}
		else
		{
			mLayout.Update();
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				GotoScene("MainMenu");
			}
		}
	}

	private void GotoScene(string sceneName)
	{
		((SUIButton)mLayout["backBtn"]).onButtonPressed = null;
		((SUIButton)mLayout["credits"]).onButtonPressed = null;
		((SUIButton)mLayout["about"]).onButtonPressed = null;
		((SUIButton)mLayout["resetSave"]).onButtonPressed = null;
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			Singleton<MenusFlow>.instance.LoadScene(sceneName);
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void PopResetDataConfirmDialog()
	{
		mConfirmDialog = new YesNoDialog(Singleton<Localizer>.instance.Get("options_confirmreset"), false, delegate
		{
			Singleton<Profile>.instance.ResetData();
		}, delegate
		{
		});
		mConfirmDialog.priority = 500f;
	}

	private void PopAboutInfo()
	{
		string text = string.Empty;
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open("Text/" + Singleton<Localizer>.instance.Get("legal_file"));
		for (int i = 0; i < sDFTreeNode.attributeCount; i++)
		{
			string text2 = sDFTreeNode[i];
			if (text2 == "*version")
			{
				text2 = "v" + Singleton<GameVersion>.instance.ToString();
			}
			if (text != string.Empty)
			{
				text += "\n";
			}
			text += text2;
		}
		mAboutDialog = new SUILayout("Layouts/AboutDialog");
		((SUISprite)mAboutDialog["panel"]).scale = new Vector2(1f, 1.1f);
		((SUILabel)mAboutDialog["text"]).text = text;
		mAboutDialog.basePriority = 500f;
		((SUIButton)mAboutDialog["back"]).onButtonPressed = delegate
		{
			if (mAboutDialog != null)
			{
				mAboutDialog.Destroy();
				mAboutDialog = null;
			}
		};
		mAboutDialog.AnimateIn();
	}

	private void ShowPrivacy()
	{
		Application.OpenURL("http://www.glu.com/legal");
	}
}
