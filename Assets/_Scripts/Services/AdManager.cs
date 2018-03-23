using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdManager : MonoBehaviour {

	LevelManager levelManager = LevelManager.instance;
    [SerializeField] string gameID = "1720802";

    private void Awake() {
        Advertisement.Initialize(gameID, true);
        DontDestroyOnLoad(gameObject);
    }
    
    public void ShowAd(string zone = "") {

        #if UNITY_EDITOR
            StartCoroutine(WaitForAd());
        #endif

        if (string.Equals(zone,"")) {
            zone = null;
        }

        ShowOptions options = new ShowOptions();
        options.resultCallback = AdCallbackhandler;
		if (zone == "rewardedVideo") {
			options.resultCallback = RewardedHandler;
		}

        if (Advertisement.IsReady(zone)) {
            Advertisement.Show(zone, options);
        }
    }

	void RewardedHandler( ShowResult result) {
		switch (result) {
		//user watched all the way through
		case ShowResult.Finished:
			Debug.Log ("Ad Finished. Rewarding player...");
			GameController.gameState.gameOver = false;
			levelManager.SecondTry();
			break;
		case ShowResult.Skipped:
			Debug.Log("Ad skipped.");
			break;
		case ShowResult.Failed:
			Debug.Log("Something went wrong with ad");
			break;
		}
	}

    void AdCallbackhandler (ShowResult result) {
        switch (result) {
            //user watched all the way through
			case ShowResult.Finished:
				Debug.Log ("Ad Finished. Rewarding player...");
               	break;
            case ShowResult.Skipped:
                Debug.Log("Ad skipped.");
                break;
            case ShowResult.Failed:
                Debug.Log("Something went wrong with ad");
                break;
        }
    }

	//pause game while ad is playing in editor
    IEnumerator WaitForAd() {
        float currentTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return null;

        while(Advertisement.isShowing) {
            yield return null;
        }

        Time.timeScale = currentTimeScale;
    }


    // Not Used*****
    IEnumerator ShowAdWhenReady() {
        while(!Advertisement.IsReady()) {
            yield return null;
        }

        Advertisement.Show();
    }


}
