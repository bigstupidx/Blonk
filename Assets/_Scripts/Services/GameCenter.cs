using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;

public class GameCenter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Social.Active.
		Social.localUser.Authenticate(ProcessAuthentication);
		DontDestroyOnLoad(transform.gameObject);
	}

	void ProcessAuthentication (bool success)
	{
		if (success) {
			print ("Authenticated, checking achievements");
			Social.LoadAchievements (ProcessLoadedAchievements);
		} else {
			print ("Failed to authenticate");
		}
	}

	// This function gets called when the LoadAchievement call completes
	void ProcessLoadedAchievements (IAchievement[] achievements) {
    	if (achievements.GetLength(0) == 0) {
        	print("Error: no achievements found");
    	} else {
        	print("Got " + achievements.Length + " achievements");
        }
 
    	// You can also call into the functions like this
    	/*
    	Social.ReportProgress("highscore", (double)PlayerPrefs.GetInt("highScore", 1), result => {
    		if (result){
    			print("Successfully reported achievement progress");
    		} else {
    			print("Failed to report achievement");
    		}
    	});
    	*/

		//Report high score to GameCenter
		int highScore = PlayerPrefs.GetInt("highScore",1);
		Social.ReportScore((long)highScore, "grp.com.minimal.blonk.highscore", result => {
			if (result) {
				print ("score submission successful");
			} else {
				print ("score submission failed");
			}
		});

    }

    public void OnGameCenterButton() {
    	Social.ShowLeaderboardUI();
    }

	// Update is called once per frame
	void Update () {
		
	}

}
