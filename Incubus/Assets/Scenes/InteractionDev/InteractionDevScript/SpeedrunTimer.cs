using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI textmeshPro;
	public static bool isASpeedrun = false;
	private bool started = false;
	private bool finished = false;
	private DateTime speedrunStart = DateTime.Now;
	private DateTime finishTime = DateTime.Now;
	private bool leaderboardUpdated = false;
	

    // Start is called before the first frame update
    void Start()
    {
        if (isASpeedrun)
		{
			textmeshPro.enabled = true;
		}
		else
		{
			textmeshPro.enabled = false;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (!isASpeedrun)
		{
			return;
		}

		if (!finished
			&& (Input.GetButtonDown("Horizontal")
			|| (Input.GetButtonDown("Vertical"))
			|| (Math.Abs(Input.GetAxis("Horizontal")) > 0.1f)
			|| (Math.Abs(Input.GetAxis("Vertical")) > 0.1f)))
		{
			StartSpeedrun();
		}

		UpdateUI();

        UpdateAchievement();
		
		if (finished && !leaderboardUpdated)
		{
			leaderboardUpdated = true;
			TimeSpan elapsedSpan = new TimeSpan(finishTime.Ticks - speedrunStart.Ticks);
			int finalTime = (int)elapsedSpan.TotalMilliseconds;
			StartCoroutine("UpdateLeaderboard", finalTime);
			//UpdateLeaderboard(finalTime);
		}
	}

	private void StartSpeedrun()
	{
		if (!started)
		{
			started = true;
			speedrunStart = DateTime.Now;
		}
	}

	private void UpdateUI()
	{
		long elapsedTicks = DateTime.Now.Ticks - speedrunStart.Ticks;
		TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);

		if (!started)
		{
			elapsedSpan = new TimeSpan(0);
		}
		else if (finished)
		{
			elapsedSpan = new TimeSpan(finishTime.Ticks - speedrunStart.Ticks);
		}

		string timeString = "";

		if (elapsedSpan.Hours < 1)
		{
			timeString = ""
			+ elapsedSpan.Minutes.ToString("D2") + ":"
			+ elapsedSpan.Seconds.ToString("D2") + ":"
			+ elapsedSpan.Milliseconds.ToString("D3");
		}
		else
		{
			timeString = "XX:XX:XX";
		}
		
		//Debug.Log(timeString);
		textmeshPro.SetText(timeString);
	}

	private void Finish()
	{
		if (!finished && TowerTrigger.NumberOfPiecesRemoved() >= 4)
		{
			finished = true;
			finishTime = DateTime.Now;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!isASpeedrun)
		{
			return;
		}

		if (other.tag == "speedrunFinish")
		{
			Finish();
		}
	}

    private void UpdateAchievement()
    {
        if (finished)
        {
            TimeSpan elapsedSpan = new TimeSpan(finishTime.Ticks - speedrunStart.Ticks);
            if (elapsedSpan.Minutes <= 12)
            {
                Achievement_Manager.Set_TWELVE_MINUTES();
            }
            if (elapsedSpan.Minutes <= 10)
            {
                Achievement_Manager.Set_TEN_MINUTES();
            }
            if (elapsedSpan.Minutes <= 8)
            {
                Achievement_Manager.Set_EIGHT_MINUTES();
            }
        }
    }

	private IEnumerator UpdateLeaderboard(int timeInMilliseconds)
	{
		Debug.Log("updating leaderboard");
		if (!SteamManager.Initialized)
		{
			Debug.Log("steammanager not initialized");
			yield break;
		}

		bool findLeaderboardCallCompleted = false;
		bool error = false;
		bool uploadedScoreCallCompleted = false;
		Steamworks.SteamLeaderboard_t speedrunLeaderboard = new Steamworks.SteamLeaderboard_t();
		Steamworks.LeaderboardScoreUploaded_t leaderboardScore = new Steamworks.LeaderboardScoreUploaded_t();

		Steamworks.SteamAPICall_t speedrunLeaderboardSearch = Steamworks.SteamUserStats.FindLeaderboard("speedrunlist");
		Steamworks.CallResult<Steamworks.LeaderboardFindResult_t> findLeaderboardCallResult = Steamworks.CallResult<Steamworks.LeaderboardFindResult_t>.Create();
		findLeaderboardCallResult.Set(speedrunLeaderboardSearch, (leaderboardFindResult, failure) =>
		{
			if (!failure && leaderboardFindResult.m_bLeaderboardFound == 1)
			{
				speedrunLeaderboard = leaderboardFindResult.m_hSteamLeaderboard;
				Debug.Log("speedrunLeaderboard found");
			}
			else
			{
				error = true;
			}

			findLeaderboardCallCompleted = true;
		});

		while (!findLeaderboardCallCompleted) yield return null;

		if (error)
		{
			
			Debug.Log("Error finding High Score leaderboard.");
			yield break;
		}

		Steamworks.SteamAPICall_t uploadedScoreCall = Steamworks.SteamUserStats.UploadLeaderboardScore(speedrunLeaderboard, Steamworks.ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodKeepBest, timeInMilliseconds, new int[0], 0);
		Steamworks.CallResult<Steamworks.LeaderboardScoreUploaded_t> leaderboardScoreUploadedCallResult = Steamworks.CallResult<Steamworks.LeaderboardScoreUploaded_t>.Create();
		leaderboardScoreUploadedCallResult.Set(uploadedScoreCall, (scoreUploadedResult, failure) => 
		{
			if (!failure && scoreUploadedResult.m_bSuccess == 1)
			{
				leaderboardScore = scoreUploadedResult;
				Debug.Log("leaderboardScore found");
			}
			else
			{
				error = true;
			}

			uploadedScoreCallCompleted = true;
		});

		while (!uploadedScoreCallCompleted) yield return null;

		if (error)
		{
			Debug.Log("Error uploading to High Score leaderboard.");
			yield break;
		}

		if (leaderboardScore.m_bScoreChanged == 1)
		{
			Debug.Log(String.Format("New high score! Global rank #{0}.", leaderboardScore.m_nGlobalRankNew));
		}
		else
		{
			Debug.Log(String.Format("A previous score was better. Global rank #{0}.", leaderboardScore.m_nGlobalRankNew));
		}
	}
}
