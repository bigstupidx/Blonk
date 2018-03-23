using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SocialPlatforms;

public class GameOverController : MonoBehaviour {

	LevelManager levelManager = LevelManager.instance;
	//AdManager adManager;
	float timeLeft = 10;
	public TextMeshPro textPrefab;
	public TMP_FontAsset scoreFont;
	private TextMeshPro timerText;
	public SpriteRenderer scoreSprite;
	//public SpriteRenderer keepGoingSprite;
	//public SpriteRenderer replaySprite;
	private int finalScore;
	private TextMeshPro scoreText;
	private int highScore;
	private TextMeshPro highScoreText;
	private bool newHighScore;
	public Button continueButton;
	public Button replayButton;
	public Button noAdsButton;
	public Sprite adFreeSprite;
	public SpriteRenderer newHighScoreDisplay;
	bool offerSecondTry;
	bool continueInProgress = false;
	AudioSource clockAS;

	// Use this for initialization
	void Start () {
		timerText = Instantiate(textPrefab);
		timerText.font = scoreFont;
		timerText.fontSize = 10f;
		timerText.SetText("");
		timerText.color = Color.white;
		timerText.transform.position = new Vector3(3.5f, 8.75f, -3f);
		timerText.sortingLayerID = SortingLayer.NameToID("StartMenu");
		timerText.sortingOrder = 2;
		finalScore = GameState.score;
		highScore = PlayerPrefs.GetInt("highScore",1);
		newHighScore = GameController.gameState.newHighScore;

		highScoreText = Instantiate(textPrefab);
		highScoreText.font = scoreFont;
		highScoreText.fontSize = 3.5f;
		highScoreText.color = Color.white;
		highScoreText.transform.position = new Vector3(3.5f + 2.85f, 11.83f, -3f);
		highScoreText.SetText(highScore.ToString());
		highScoreText.sortingLayerID = SortingLayer.NameToID("StartMenu");

		replayButton.interactable = false;
		clockAS = GetComponent<AudioSource>();
		clockAS.volume = 0.3f;

		if (PlayerPrefs.GetInt("noAds",0) == 1) {
			noAdsButton.GetComponent<Image> ().sprite = adFreeSprite;
			noAdsButton.enabled = false;
		}


		//replayButton.GetComponent<Image>().enabled = false;
		offerSecondTry = !GameState.secondTryUsed;

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (continueInProgress)
			return;

		if (offerSecondTry) {

			//disable score sprite
			//show keep going sprite
			//generate timer text

			scoreSprite.enabled = false;

			timeLeft -= Time.deltaTime;
			print ("timeLeft: " + timeLeft);
			print ("Time.timeScale: " + Time.timeScale);
			if (timeLeft < 0) {
				timerText.SetText ("");
				offerSecondTry = false;
				timeLeft = 10;
				clockAS.Stop ();
			} else {
				timerText.SetText (((int)timeLeft).ToString ());
				if (!clockAS.isPlaying) {
					clockAS.Play ();
				}
			}
		} else {
			scoreSprite.enabled = true;

			scoreText = Instantiate (textPrefab);
			scoreText.font = scoreFont;
			scoreText.fontSize = 10f;
			scoreText.color = Color.white;
			scoreText.transform.position = new Vector3 (3.5f, 9.25f, -3f);
			scoreText.sortingLayerID = SortingLayer.NameToID ("StartMenu");
			scoreText.sortingOrder = 2;
			scoreText.SetText (finalScore.ToString ());
			highScoreText.SetText (highScore.ToString ());

			continueButton.interactable = false;
			continueButton.GetComponent<Image> ().enabled = false;
			replayButton.interactable = true;
			replayButton.GetComponent<Image> ().enabled = true;
			GameController.instance.gameUI.pauseButton.enabled = false;

			if (newHighScore) {
				newHighScoreDisplay.enabled = true;
			}
		}
	}

	public void OnContinue ()
	{ 	
		continueInProgress = true;
		GameState.secondTryUsed = true;
		if (PlayerPrefs.GetInt ("noAds", 0) == 0) {
			FindObjectOfType<AdManager> ().ShowAd ("rewardedVideo");
			clockAS.Stop ();
		} else {
			GameController.gameState.gameOver = false;
			levelManager.SecondTry();
		}
	}

	public void OnHome () {
		GameState.secondTryUsed = false;
		if (PlayerPrefs.GetInt("noAds",0) == 0) {
			//adManager.ShowAd ("rewardedVideo");
			FindObjectOfType<AdManager>().ShowAd ();
		}
		levelManager.LoadLevel("StartMenu");
	}

	public void OnReplay ()
	{
		GameState.secondTryUsed = false;
		levelManager.LoadLevel("GameScene");
	}

	public void rateGame() {
        #if UNITY_ANDROID
         Application.OpenURL("market://details?id=YOUR_ID");
        #elif UNITY_IPHONE
         Application.OpenURL(Parameters.APPLE_APP_URL);
        #endif
    }

	public void OnGameCenter() {
		Social.ShowLeaderboardUI();
	}

	public void OnFacebookLike() {
		Application.OpenURL("https://www.facebook.com/blonkgame");
	}
}
