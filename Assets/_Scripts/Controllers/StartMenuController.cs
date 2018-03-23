using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.GameCenter;
using Random = UnityEngine.Random;
using System.IO;

public class StartMenuController : MonoBehaviour {

	LevelManager levelManager = LevelManager.instance;
	int soundSettingOn;
	public Button soundButton;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
	public Button noAdsButton;
	public Sprite adFreeSprite;
	public AdManager adManager;
	public Rigidbody2D title;
	public Rigidbody2D rate;
	float TITLE_SPEED = 1.5f;
	float RATE_SPEED = 0.4f;
	float TITLE_ANGULAR = 10f;

	// Use this for initialization
	void Start() {
		//DELETE BEFORE BUILDING
		PlayerPrefs.DeleteAll();
		if (File.Exists(Application.persistentDataPath + "/savedGame.dat"))
			File.Delete(Application.persistentDataPath + "/savedGame.dat");
	
		// ignore block-on-floor collisions
		Physics2D.IgnoreLayerCollision (0, 5, true);
		Physics2D.IgnoreLayerCollision (5, 9, true);
		levelManager.secondTryUsed = false;
		levelManager.preLoadGame();
		soundSettingOn = PlayerPrefs.GetInt("soundSettingOn", 1);
		//AudioListener.pause = (soundSettingOn == 0) ? true : false;
		AudioListener.volume = (soundSettingOn == 0) ? 0f : 1f;
		soundButton.GetComponent<Image>().sprite = (soundSettingOn == 0) ? soundOffSprite : soundOnSprite;
		if(PlayerPrefs.GetInt("noAds", 0) == 1) {
			noAdsButton.GetComponent<Image>().sprite = adFreeSprite;
			noAdsButton.enabled = false;
		}

		if((PlayerPrefs.GetInt("noAds", 0) == 0) && levelManager.showAdNow) {
			GameObject.FindObjectOfType<AdManager>().ShowAd();
		}



		//title.velocity = new Vector2 (TITLE_SPEED, TITLE_SPEED);
		//title.angularVelocity = 10f;
		//rate.velocity = new Vector2 (RATE_SPEED, RATE_SPEED);
		//rate.angularVelocity = 5f;

		rate.AddTorque (0.5f);

	}
	
	// Update is called once per frame
	void Update () {

		/*
		if (rate.velocity.magnitude < RATE_SPEED - 0.1f) {
			Vector2 v = rate.velocity.normalized;
			rate.velocity = v * RATE_SPEED;
		}
		*/
		if (rate.angularVelocity < 10f) {
			rate.AddTorque (10f);
		}

		/*
		if (title.velocity.magnitude < TITLE_SPEED - 0.6f) {
			Vector2 v = title.velocity.normalized;
			title.velocity = new Vector2 (Random.Range(0.5f,1f)*TITLE_SPEED, Random.Range(0.5f,1f)*TITLE_SPEED);
		}

		if (title.angularVelocity < TITLE_ANGULAR - 5f) {
			title.angularVelocity = Random.Range (-1f, 1f) * TITLE_ANGULAR;
		}
		*/
	}

	public void OnPlayButton() {
		GameController.preload = false;
		levelManager.LoadLevel("GameScene");
	}

	public void OnSoundButton ()
	{
		soundSettingOn = PlayerPrefs.GetInt("soundSettingOn",1);
		if (soundSettingOn == 0) {
			//turn sound on
			//AudioListener.pause = false;
			AudioListener.volume = 1f;
			soundButton.GetComponent<Image>().sprite = soundOnSprite;
			PlayerPrefs.SetInt("soundSettingOn", 1);
		} else {
			//turn sound off
			//AudioListener.pause = true;
			AudioListener.volume = 0f;
			soundButton.GetComponent<Image>().sprite = soundOffSprite;
			PlayerPrefs.SetInt("soundSettingOn", 0);
		}
	}

	public void rateGame() {
        #if UNITY_ANDROID
         Application.OpenURL("market://details?id=YOUR_ID");
        #elif UNITY_IPHONE
		Application.OpenURL(Parameters.APPLE_APP_URL);
        #endif
    }
}
