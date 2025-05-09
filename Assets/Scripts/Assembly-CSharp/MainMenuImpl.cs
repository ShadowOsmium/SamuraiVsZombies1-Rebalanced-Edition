using System.Collections;
using UnityEngine;

public class MainMenuImpl : SceneBehaviour
{
	private const float kSpeedStart = 1000f;

	private const float kSpeedEnd = 100f;

	private const float kSpeedDecrease = 2f;

	private const string kCloudSaveName = "save.data";

	private SUILayout mLayout;

	private SUIButton mGameCenter;

	private SUIButton mICloud;

	private TwoOptionDialog mICloudSupportDialog;

	private DialogHandler mDialogHandler;

	private SpriteScreenScroller mScrollBloodTop;

	private SpriteScreenScroller mScrollBloodBottom;

	public AudioClip ClickSFX;

	private float mBloodSpeed;

	private byte[] fileBuffer;

	private bool loadComplete;

	private YesNoDialog m_quitDialog;

	private SUIButton m_cheatsBtn;

	private IEnumerator Start()
	{
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeFromBlack();
		mLayout = new SUILayout("Layouts/StartScreen");
		mLayout.AnimateIn();
		((SUILabel)mLayout["version"]).text = Singleton<GameVersion>.instance.ToString() + ((!SingletonMonoBehaviour<ResourcesManager>.instance.hasOnlineBundleLoaded) ? string.Empty : "+");
		mGameCenter = (SUIButton)mLayout["gameCenter"];
		mGameCenter.onButtonPressed = delegate
		{
		};
		mICloud = (SUIButton)mLayout["iCloud"];
		mICloud.onButtonPressed = ShowICloudScreen;
		mGameCenter.visible = false;
		mICloud.visible = false;
		m_cheatsBtn = (SUIButton)mLayout["cheats"];
		m_cheatsBtn.scale = new Vector2(2f, 2f);
		if (/*Debug.isDebugBuild*/false)
		{
			m_cheatsBtn.onButtonPressed = ShowCheatsScreen;
		}
		else
		{
			m_cheatsBtn.visible = false;
		}
		mBloodSpeed = 1000f;
		mScrollBloodTop = new SpriteScreenScroller((SUISprite)mLayout["bloodTop"]);
		mScrollBloodTop.ScrollHorizontally(mBloodSpeed);
		mScrollBloodBottom = new SpriteScreenScroller((SUISprite)mLayout["bloodBottom"]);
		mScrollBloodBottom.ScrollHorizontally(0f - mBloodSpeed);
		((SUIButton)mLayout["startbutton"]).onButtonPressed = delegate
		{
			if (Singleton<Profile>.instance.newVersionDetected && Application.internetReachability != 0)
			{
				ShowNewVersionScreen();
			}
			else if (Singleton<Profile>.instance.zombieModeUnlocked)
			{
				GoToScene(delegate
				{
					Singleton<MenusFlow>.instance.LoadScene("PlayModeMenu");
				});
			}
			else if ((Singleton<Profile>.instance.waveToBeat == 1 && Singleton<Profile>.instance.GetWaveLevel(2) == 0) || Singleton<Profile>.instance.readyToStartBonusWave)
			{
				GoToScene(delegate
				{
					WaveManager.LoadNextWaveLevel();
				});
			}
			else
			{
				Singleton<Profile>.instance.tapFeatureAdsCnt = Singleton<Profile>.instance.tapFeatureAdsCnt + 1;
				Singleton<Profile>.instance.Save();
				GoToScene(delegate
				{
					Singleton<MenusFlow>.instance.LoadScene("Store");
				});
			}
		};
		((SUIButton)mLayout["options"]).onButtonPressed = delegate
		{
			GoToScene(delegate
			{
				Singleton<MenusFlow>.instance.LoadScene("Options");
			});
		};
		((SUIButton)mLayout["otherGames"]).onButtonPressed = onOtherGames;
		ApplicationUtilities.instance.ShowAds((ApplicationUtilities.AdPosition)17);
		if (!Singleton<Profile>.instance.hasReportedUser)
		{
			Singleton<Analytics>.instance.LogEvent("UniqueStartups", string.Empty);
			Singleton<Profile>.instance.hasReportedUser = true;
		}
		string[] osVersion = SystemInfo.operatingSystem.Substring(SystemInfo.operatingSystem.LastIndexOf(' ') + 1).Split('.');
		int majorOSVersion = 4;
		int.TryParse(osVersion[0], out majorOSVersion);
		IDialog specialDialog2 = null;
		specialDialog2 = Singleton<FreebiesManager>.instance.CheckForWelcomeBackGift();
		if (specialDialog2 != null)
		{
			((DailyRewardDialog)specialDialog2).onYesPressed = delegate
			{
				GoToScene(delegate
				{
					Singleton<MenusFlow>.instance.LoadScene("BoosterPackStore");
				});
			};
		}
		else
		{
			specialDialog2 = DailyRewardsManager.CheckForDailyReward();
		}
		if (specialDialog2 == null)
		{
			specialDialog2 = Singleton<FreebiesManager>.instance.CheckForGiveOnceGift();
		}
		if (specialDialog2 != null)
		{
			StartDialog(specialDialog2);
		}
		if (Application.internetReachability != 0)
		{
			yield return StartCoroutine(CheckVersion());
			if (Singleton<Profile>.instance.newVersionDetected)
			{
				ShowNewVersionScreen();
			}
		}
	}

	private IEnumerator CoQuit()
	{
		yield return new WaitForSeconds(0.5f);
		Application.Quit();
	}

	private void Update()
	{
		if (SceneBehaviourUpdate())
		{
			return;
		}
		mScrollBloodTop.Update();
		mScrollBloodBottom.Update();
		if (mBloodSpeed > 100f)
		{
			mBloodSpeed = (mBloodSpeed - 100f) * Mathf.Clamp(1f - SUIScreen.deltaTime * 2f, 0f, 0.98f) + 100f;
			mScrollBloodTop.speed = mBloodSpeed;
			mScrollBloodBottom.speed = 0f - mBloodSpeed;
		}
		if (mDialogHandler != null)
		{
			mDialogHandler.Update();
			if (mDialogHandler.isDone)
			{
				mDialogHandler.Destroy();
				mDialogHandler = null;
			}
			return;
		}
		mLayout.Update();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			m_quitDialog = new YesNoDialog(Singleton<Localizer>.instance.Get("quit_confirm"), false, delegate
			{
				StartCoroutine(CoQuit());
			}, delegate
			{
			});
			m_quitDialog.priority = 500f;
			StartDialog(m_quitDialog);
		}
	}

	private IEnumerator CheckVersion()
	{
		if (!Singleton<Profile>.instance.newVersionDetected)
		{
			WWW www = (ApplicationUtilities.IsBuildType("amazon") ? ((!Debug.isDebugBuild) ? new WWW(Singleton<Localizer>.instance.Get("amazon_version_file_location_live")) : new WWW(Singleton<Localizer>.instance.Get("amazon_version_file_location_stage"))) : ((!Debug.isDebugBuild) ? new WWW(Singleton<Localizer>.instance.Get("version_file_location_live")) : new WWW(Singleton<Localizer>.instance.Get("version_file_location_stage"))));
			yield return www;
			if (www == null)
			{
				yield break;
			}
			if (Debug.isDebugBuild)
			{
				Debug.Log("*** Version Found:" + Singleton<Profile>.instance.latestDetectedOnlineVersion);
			}
			Singleton<Profile>.instance.latestDetectedOnlineVersion = www.text;
		}
		if (string.IsNullOrEmpty(Singleton<Profile>.instance.latestDetectedOnlineVersion))
		{
			yield break;
		}
		string[] version = Singleton<Profile>.instance.latestDetectedOnlineVersion.Split('.');
		if (version.Length >= 3)
		{
			if (Singleton<GameVersion>.instance.major < int.Parse(version[0]) || (Singleton<GameVersion>.instance.major == int.Parse(version[0]) && Singleton<GameVersion>.instance.minor < int.Parse(version[1])) || (Singleton<GameVersion>.instance.major == int.Parse(version[0]) && Singleton<GameVersion>.instance.minor == int.Parse(version[1]) && Singleton<GameVersion>.instance.revision < int.Parse(version[2])))
			{
				Singleton<Profile>.instance.newVersionDetected = true;
			}
			else
			{
				Singleton<Profile>.instance.newVersionDetected = false;
			}
		}
		else
		{
			Singleton<Profile>.instance.newVersionDetected = false;
		}
		Singleton<Profile>.instance.Save();
	}

	private void ShowNewVersionScreen()
	{
		TwoOptionDialog twoOptionDialog;
		if (ApplicationUtilities.IsBuildType("amazon"))
		{
			twoOptionDialog = new TwoOptionDialog(SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get("New_Version_Notification_Message_Text_Amazon")), false, Singleton<Localizer>.instance.Get("ok"), delegate
			{
				OnClickNewVersion(true);
			}, Singleton<Localizer>.instance.Get("exit"), delegate
			{
				OnClickNewVersion(false);
			}, "Layouts/TwoOptionDialogWithoutBackButton");
			twoOptionDialog.SetBackKey(delegate
			{
				OnClickNewVersion(true);
			});
		}
		else
		{
			twoOptionDialog = new TwoOptionDialog(SUILayoutConv.GetFormattedText(Singleton<Localizer>.instance.Get("New_Version_Notification_Message_Text_Google")), false, Singleton<Localizer>.instance.Get("ok"), delegate
			{
				OnClickNewVersion(true);
			}, Singleton<Localizer>.instance.Get("exit"), delegate
			{
				OnClickNewVersion(false);
			}, "Layouts/TwoOptionDialogWithoutBackButton");
			twoOptionDialog.SetBackKey(delegate
			{
				OnClickNewVersion(true);
			});
		}
		twoOptionDialog.priority = 500f;
		StartDialog(twoOptionDialog);
	}

	private void ShowICloudScreen()
	{
		mICloudSupportDialog = new TwoOptionDialog(Singleton<Localizer>.instance.Get("iCloud_Message_Text"), false, Singleton<Localizer>.instance.Get("iCloud_Load_Button_Text"), ICloudLoadButton, Singleton<Localizer>.instance.Get("iCloud_Save_Button_Text"), ICloudSaveButton, null);
		StartDialog(mICloudSupportDialog);
	}

	private void ICloudLoadButton()
	{
		mICloudSupportDialog.DisableButtons();
		StartCoroutine(ICloudLoad());
	}

	private void ICloudSaveButton()
	{
		mICloudSupportDialog.DisableButtons();
		ICloudSave();
	}

	private IEnumerator ICloudLoad()
	{
		if (Application.internetReachability != 0)
		{
			Debug.Log("iCloudLoad(): Internet Found");
			if (!FileManager.CloudStorageAvailable)
			{
				Debug.Log("iCloudLoad(): iCloud Not Availible");
				if (mDialogHandler != null)
				{
					mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("No_iCloud_Notification_Message_Text"), false, delegate
					{
					}, null)
					{
						priority = 9999f
					});
				}
				mICloudSupportDialog.Close();
				yield break;
			}
			SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = true;
			Debug.Log("iCloudLoad(): Cloud Storage Found");
			string cloudSavePath = null;
			bool fileExists = true;
			FileManager.FindFilePathInCloud("save.data", delegate(FileData fileData)
			{
				if (fileData.Exists)
				{
					cloudSavePath = fileData.Path;
				}
				fileExists = fileData.Exists;
			});
			float timeToWaitForLoad = 5f;
			while (string.IsNullOrEmpty(cloudSavePath))
			{
				Debug.Log(string.Format("Waiting on FileManager.FindFilePathInCloud; time = {0}, fileExists = {1}", timeToWaitForLoad, fileExists));
				timeToWaitForLoad -= Time.deltaTime;
				if (timeToWaitForLoad > 0f)
				{
					yield return new WaitForEndOfFrame();
					continue;
				}
				SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = false;
				if (mDialogHandler != null)
				{
					mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("iCloud_Timed_Out_Message_Text"), false, delegate
					{
					}, null)
					{
						priority = 9999f
					});
				}
				mICloudSupportDialog.Close();
				yield break;
			}
			Debug.Log(string.Format("cloudSavePath = {0}, fileExists = {0}", cloudSavePath, fileExists));
			if (fileExists)
			{
				Debug.Log("iCloudLoad(): File Found In iCloud");
				fileBuffer = null;
				loadComplete = false;
				FileManager.LoadFile(cloudSavePath, delegate(FileData result)
				{
					Debug.Log("ICloudLoad(): LoadFile result = " + result.ToString());
					fileBuffer = result.Data;
					loadComplete = true;
				});
				timeToWaitForLoad = 5f;
				while (!loadComplete)
				{
					Debug.Log("Waiting on FileManager.LoadFile; time = " + timeToWaitForLoad);
					timeToWaitForLoad -= Time.deltaTime;
					if (timeToWaitForLoad > 0f)
					{
						yield return new WaitForEndOfFrame();
						continue;
					}
					SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = false;
					if (mDialogHandler != null)
					{
						mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("iCloud_Timed_Out_Message_Text"), false, delegate
						{
						}, null)
						{
							priority = 9999f
						});
					}
					mICloudSupportDialog.Close();
					yield break;
				}
				Debug.Log("iCloudLoad(): Load Complete");
				Singleton<Profile>.instance.saveData.SetWithByteArray(fileBuffer);
				SavedData sd = new SavedData(Singleton<Profile>.instance.saveFilePath);
				sd.SetWithByteArray(fileBuffer);
				sd.Save();
				Debug.Log("iCloudLoad(): New Save Created");
				string msg2 = Singleton<Localizer>.instance.Get("iCloud_Data_Loaded_Message_Text");
				SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = false;
				if (mDialogHandler != null)
				{
					mDialogHandler.PushCreator(() => new YesNoDialog(msg2, false, delegate
					{
					}, null)
					{
						priority = 9999f
					});
				}
				mICloudSupportDialog.Close();
			}
			else
			{
				Debug.Log("iCloudLoad(): File Not Found In iCloud");
				string msg = Singleton<Localizer>.instance.Get("iCloud_File_Not_Found_Notification_Message_Text");
				SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = false;
				if (mDialogHandler != null)
				{
					mDialogHandler.PushCreator(() => new YesNoDialog(msg, false, delegate
					{
					}, null)
					{
						priority = 9999f
					});
				}
				mICloudSupportDialog.Close();
			}
		}
		else
		{
			if (mDialogHandler != null)
			{
				mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("No_Internet_Notification_Message_Text"), false, delegate
				{
				}, null)
				{
					priority = 9999f
				});
			}
			mICloudSupportDialog.Close();
		}
		SingletonMonoBehaviour<WaitingIconBetweenScenes>.instance.visible = false;
		mICloudSupportDialog.Close();
	}

	private void ICloudSave()
	{
		if (Application.internetReachability != 0)
		{
			Debug.Log("ICLOUDSAVE: Internet reachability found");
			if (FileManager.CloudStorageAvailable)
			{
				string text = FileManager.ConvertLocalPathToCloudPath(Singleton<Profile>.instance.saveFilePath);
				Debug.Log("ICLOUDSAVE: cloudAutoSavePath = " + text);
				if (text == Singleton<Profile>.instance.saveFilePath)
				{
					return;
				}
				if (string.IsNullOrEmpty(text))
				{
					if (mDialogHandler != null)
					{
						mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("No_iCloud_Notification_Message_Text"), false, delegate
						{
						}, null)
						{
							priority = 9999f
						});
					}
					mICloudSupportDialog.Close();
					return;
				}
				Debug.Log("ICLOUDSAVE: SaveCloudFile([cloudAutoSavePath])");
				try
				{
					Singleton<Profile>.instance.SaveCloudFile(text);
				}
				catch
				{
					if (mDialogHandler != null)
					{
						mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("No_iCloud_Notification_Message_Text"), false, delegate
						{
						}, null)
						{
							priority = 9999f
						});
					}
					mICloudSupportDialog.Close();
					return;
				}
				Debug.Log("ICLOUDSAVE: Queue an async upload to the cloud");
				FileManager.SaveFile(text, Singleton<Profile>.instance.cloudSaveData.GetAsByteArray());
				Debug.Log("ICLOUDSAVE: Presenting dialog to inform player");
				if (mDialogHandler != null)
				{
					mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("iCloud_Save_Started_Notification_Message_Text"), false, delegate
					{
					}, null)
					{
						priority = 9999f
					});
				}
				mICloudSupportDialog.Close();
				return;
			}
			if (mDialogHandler != null)
			{
				mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("No_iCloud_Notification_Message_Text"), false, delegate
				{
				}, null)
				{
					priority = 9999f
				});
			}
			mICloudSupportDialog.Close();
			return;
		}
		if (mDialogHandler != null)
		{
			mDialogHandler.PushCreator(() => new YesNoDialog(Singleton<Localizer>.instance.Get("No_Internet_Notification_Message_Text"), false, delegate
			{
			}, null)
			{
				priority = 9999f
			});
		}
		mICloudSupportDialog.Close();
	}

	private void DisableButtons()
	{
		((SUIButton)mLayout["startbutton"]).onButtonPressed = null;
		((SUIButton)mLayout["options"]).onButtonPressed = null;
		((SUIButton)mLayout["otherGames"]).onButtonPressed = null;
	}

	private void onOtherGames()
	{
		if (Application.internetReachability != 0)
		{
			Singleton<PlayHavenTowerControl>.instance.InvokeContent("more_games");
			return;
		}
		YesNoDialog yesNoDialog = new YesNoDialog(Singleton<Localizer>.instance.Get("No_Internet_Notification_Message_Text"), false, delegate
		{
		}, null);
		yesNoDialog.priority = 500f;
		StartDialog(yesNoDialog);
	}

	private void StartDialog(IDialog d)
	{
		if (mDialogHandler != null)
		{
			mDialogHandler.Destroy();
			mDialogHandler = null;
		}
		mDialogHandler = new DialogHandler(499f, d);
	}

	private void GoToScene(OnSUIGenericCallback menuJump)
	{
		ApplicationUtilities.instance.HideAds();
		DisableButtons();
		WeakGlobalInstance<SUIScreen>.instance.fader.onFadingDone = delegate
		{
			menuJump();
		};
		WeakGlobalInstance<SUIScreen>.instance.fader.FadeToBlack();
		base.GetComponent<AudioSource>().PlayOneShot(ClickSFX);
		WeakGlobalInstance<SUIScreen>.instance.inputs.processInputs = false;
	}

	private void OnClickNewVersion(bool update)
	{
		if (update)
		{
			Application.OpenURL(ApplicationUtilities.MarketURL);
		}
		else
		{
			StartCoroutine(CoQuit());
		}
	}

	private void ShowCheatsScreen()
	{
		GoToScene(delegate
		{
			Singleton<MenusFlow>.instance.LoadScene("cheats");
		});
	}
}
