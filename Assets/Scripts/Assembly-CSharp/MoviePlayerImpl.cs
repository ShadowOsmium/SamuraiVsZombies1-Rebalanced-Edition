public class MoviePlayerImpl : SceneBehaviour
{
	private SUILayout mLayout;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/MoviePlayer");
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = onContinueButton;
	}

	private void Update()
	{
		if (!SceneBehaviourUpdate())
		{
			mLayout.Update();
		}
	}

	private void onContinueButton()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			if (Singleton<MenusFlow>.instance.previousScene == "Options")
			{
				Singleton<MenusFlow>.instance.LoadPreviousScene();
			}
			else
			{
				Singleton<MenusFlow>.instance.LoadScene("Store");
			}
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}
}
