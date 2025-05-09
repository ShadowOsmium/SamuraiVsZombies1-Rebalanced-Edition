using System;
using System.Collections.Generic;
using UnityEngine;

public class GameCenterManager : MonoBehaviour
{
	public static event Action<string> loadPlayerDataFailed;

	public static event Action<List<GameCenterPlayer>> playerDataLoaded;

	public static event Action playerAuthenticated;

	public static event Action<string> playerFailedToAuthenticate;

	public static event Action playerLoggedOut;

	public static event Action<string> loadCategoryTitlesFailed;

	public static event Action<List<GameCenterLeaderboard>> categoriesLoaded;

	public static event Action<string> reportScoreFailed;

	public static event Action<string> reportScoreFinished;

	public static event Action<string> retrieveScoresFailed;

	public static event Action<List<GameCenterScore>> scoresLoaded;

	public static event Action<string> retrieveScoresForPlayerIdFailed;

	public static event Action<List<GameCenterScore>> scoresForPlayerIdLoaded;

	public static event Action<string> reportAchievementFailed;

	public static event Action<string> reportAchievementFinished;

	public static event Action<string> loadAchievementsFailed;

	public static event Action<List<GameCenterAchievement>> achievementsLoaded;

	public static event Action<string> resetAchievementsFailed;

	public static event Action resetAchievementsFinished;

	public static event Action<string> retrieveAchievementMetadataFailed;

	public static event Action<List<GameCenterAchievementMetadata>> achievementMetadataLoaded;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void loadPlayerDataDidFail(string error)
	{
		if (GameCenterManager.loadPlayerDataFailed != null)
		{
			GameCenterManager.loadPlayerDataFailed(error);
		}
	}

	public void loadPlayerDataDidLoad(string jsonFriendList)
	{
		List<GameCenterPlayer> obj = GameCenterPlayer.fromJSON(jsonFriendList);
		if (GameCenterManager.playerDataLoaded != null)
		{
			GameCenterManager.playerDataLoaded(obj);
		}
	}

	public void playerDidLogOut()
	{
		if (GameCenterManager.playerLoggedOut != null)
		{
			GameCenterManager.playerLoggedOut();
		}
	}

	public void playerDidAuthenticate()
	{
		if (GameCenterManager.playerAuthenticated != null)
		{
			GameCenterManager.playerAuthenticated();
		}
	}

	public void playerAuthenticationFailed(string error)
	{
		if (GameCenterManager.playerFailedToAuthenticate != null)
		{
			GameCenterManager.playerFailedToAuthenticate(error);
		}
	}

	public void loadCategoryTitlesDidFail(string error)
	{
		if (GameCenterManager.loadCategoryTitlesFailed != null)
		{
			GameCenterManager.loadCategoryTitlesFailed(error);
		}
	}

	public void categoriesDidLoad(string jsonCategoryList)
	{
		List<GameCenterLeaderboard> obj = GameCenterLeaderboard.fromJSON(jsonCategoryList);
		if (GameCenterManager.categoriesLoaded != null)
		{
			GameCenterManager.categoriesLoaded(obj);
		}
	}

	public void reportScoreDidFail(string error)
	{
		if (GameCenterManager.reportScoreFailed != null)
		{
			GameCenterManager.reportScoreFailed(error);
		}
	}

	public void reportScoreDidFinish(string category)
	{
		if (GameCenterManager.reportScoreFinished != null)
		{
			GameCenterManager.reportScoreFinished(category);
		}
	}

	public void retrieveScoresDidFail(string category)
	{
		if (GameCenterManager.retrieveScoresFailed != null)
		{
			GameCenterManager.retrieveScoresFailed(category);
		}
	}

	public void retrieveScoresDidLoad(string jsonScoresList)
	{
		List<GameCenterScore> obj = GameCenterScore.fromJSON(jsonScoresList);
		if (GameCenterManager.scoresLoaded != null)
		{
			GameCenterManager.scoresLoaded(obj);
		}
	}

	public void retrieveScoresForPlayerIdDidFail(string error)
	{
		if (GameCenterManager.retrieveScoresForPlayerIdFailed != null)
		{
			GameCenterManager.retrieveScoresForPlayerIdFailed(error);
		}
	}

	public void retrieveScoresForPlayerIdDidLoad(string jsonScoresList)
	{
		List<GameCenterScore> obj = GameCenterScore.fromJSON(jsonScoresList);
		if (GameCenterManager.scoresForPlayerIdLoaded != null)
		{
			GameCenterManager.scoresForPlayerIdLoaded(obj);
		}
	}

	public void reportAchievementDidFail(string error)
	{
		if (GameCenterManager.reportAchievementFailed != null)
		{
			GameCenterManager.reportAchievementFailed(error);
		}
	}

	public void reportAchievementDidFinish(string identifier)
	{
		if (GameCenterManager.reportAchievementFinished != null)
		{
			GameCenterManager.reportAchievementFinished(identifier);
		}
	}

	public void loadAchievementsDidFail(string error)
	{
		if (GameCenterManager.loadAchievementsFailed != null)
		{
			GameCenterManager.loadAchievementsFailed(error);
		}
	}

	public void achievementsDidLoad(string jsonAchievmentList)
	{
		List<GameCenterAchievement> obj = GameCenterAchievement.fromJSON(jsonAchievmentList);
		if (GameCenterManager.achievementsLoaded != null)
		{
			GameCenterManager.achievementsLoaded(obj);
		}
	}

	public void resetAchievementsDidFail(string error)
	{
		if (GameCenterManager.resetAchievementsFailed != null)
		{
			GameCenterManager.resetAchievementsFailed(error);
		}
	}

	public void resetAchievementsDidFinish(string emptyString)
	{
		if (GameCenterManager.resetAchievementsFinished != null)
		{
			GameCenterManager.resetAchievementsFinished();
		}
	}

	public void retrieveAchievementsMetadataDidFail(string error)
	{
		if (GameCenterManager.retrieveAchievementMetadataFailed != null)
		{
			GameCenterManager.retrieveAchievementMetadataFailed(error);
		}
	}

	public void achievementMetadataDidLoad(string jsonAchievementDescriptionList)
	{
		List<GameCenterAchievementMetadata> obj = GameCenterAchievementMetadata.fromJSON(jsonAchievementDescriptionList);
		if (GameCenterManager.achievementMetadataLoaded != null)
		{
			GameCenterManager.achievementMetadataLoaded(obj);
		}
	}
}
