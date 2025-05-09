public class PaperdollImpl : SceneBehaviour
{
	private SUILayout mLayout;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/PaperdollLayout");
		mLayout.AnimateIn();
	}

	private void Update()
	{
		if (!SceneBehaviourUpdate())
		{
			mLayout.Update();
		}
	}

	private void Quit()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			Singleton<MenusFlow>.instance.LoadPreviousScene();
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		mLayout.AnimateOut();
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}
}
