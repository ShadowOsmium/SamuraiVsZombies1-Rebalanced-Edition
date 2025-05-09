using System.Collections.Generic;
using UnityEngine;

public class CreditsImpl : SceneBehaviour
{
	private const float kScrollSpeed = 80f;

	private const float kBlankLineHeight = 64f;

	private readonly float kSpawnAtY = SUIScreen.height + 30f;

	private SUILayout mLayout;

	private List<string> mCredits = new List<string>(100);

	private List<SUILabel> mLabels = new List<SUILabel>(20);

	private int mNextLabelToSpawn;

	private float mNextLabelSpawnTimer;

	private void Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/Credits");
		mLayout.AnimateIn();
		((SUIButton)mLayout["continue"]).onButtonPressed = Quit;
		LoadCredits();
	}

	private void Update()
	{
		if (!SceneBehaviourUpdate())
		{
			mLayout.Update();
			UpdateScrolling();
			if (mLabels.Count == 0)
			{
				mNextLabelToSpawn = 0;
				mNextLabelSpawnTimer = 0f;
				UpdateScrolling();
			}
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				Quit();
			}
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

	private void LoadCredits()
	{
		string text = "Text/" + Singleton<Localizer>.instance.Get("credits_file");
		SDFTreeNode sDFTreeNode = SingletonMonoBehaviour<ResourcesManager>.instance.Open(text);
		if (sDFTreeNode == null)
		{
			Debug.Log("ERROR: Could not find the credit file: " + text);
			return;
		}
		string acc = string.Empty;
		OnSUIGenericCallback onSUIGenericCallback = delegate
		{
			if (acc != string.Empty)
			{
				mCredits.Add(acc);
				acc = string.Empty;
			}
		};
		for (int i = 0; i < sDFTreeNode.attributeCount; i++)
		{
			string formattedText = SUILayoutConv.GetFormattedText(sDFTreeNode[i]);
			if (formattedText == "-")
			{
				onSUIGenericCallback();
				mCredits.Add(string.Empty);
				continue;
			}
			if (formattedText.Length > 0 && formattedText[0] == '*')
			{
				onSUIGenericCallback();
				mCredits.Add(formattedText);
				continue;
			}
			if (acc != string.Empty)
			{
				acc += "\n";
			}
			acc += formattedText;
		}
		onSUIGenericCallback();
		Debug.Log("Number of labels found in the credit file: " + mCredits.Count);
	}

	private void UpdateScrolling()
	{
		float num = Time.deltaTime * 80f;
		for (int num2 = mLabels.Count - 1; num2 >= 0; num2--)
		{
			SUILabel sUILabel = mLabels[num2];
			sUILabel.position += new Vector2(0f, 0f - num);
			if (sUILabel.position.y + sUILabel.area.height < 0f)
			{
				sUILabel.Destroy();
				mLabels.RemoveAt(num2);
			}
		}
		mNextLabelSpawnTimer -= num;
		if (mNextLabelSpawnTimer <= 0f)
		{
			SpawnNextLabel();
		}
	}

	private void SpawnNextLabel()
	{
		if (mNextLabelToSpawn >= mCredits.Count)
		{
			return;
		}
		string text = mCredits[mNextLabelToSpawn];
		mNextLabelToSpawn++;
		if (text == string.Empty)
		{
			mNextLabelSpawnTimer += 64f;
			return;
		}
		bool flag = false;
		if (text.Length > 0 && text[0] == '*')
		{
			flag = true;
			text = text.Substring(1);
		}
		SUILabel sUILabel = new SUILabel("default64");
		if (flag)
		{
			sUILabel.fontColor = new Color(0.75f, 0f, 0f);
		}
		sUILabel.shadowColor = Color.black;
		sUILabel.shadowOffset = new Vector2(2f, 2f);
		sUILabel.anchor = TextAnchor.UpperCenter;
		sUILabel.alignment = TextAlignment.Center;
		sUILabel.text = text;
		sUILabel.position = new Vector2(SUIScreen.width / 2f, kSpawnAtY + mNextLabelSpawnTimer);
		sUILabel.priority = 5f;
		mNextLabelSpawnTimer += sUILabel.area.height;
		mLabels.Add(sUILabel);
	}
}
