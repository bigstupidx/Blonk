using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TheNextFlow.UnityPlugins;

public class PauseController : MonoBehaviour {

	LevelManager levelManager = LevelManager.instance;
	int soundSettingOn;
	public Button soundButton;
	public Sprite soundOnSprite;
	public Sprite soundOffSprite;
	public Button noAdsButton;
	public Sprite adFreeSprite;
	private bool homeConfirmed = false;

	float xOffset = Screen.width/8.0f;
	float yOffset = Screen.height/6.0f;

	// Use this for initialization
	void Start () {
		soundSettingOn = PlayerPrefs.GetInt ("soundSettingOn", 1);
		//AudioListener.pause = (soundSettingOn == 0) ? true : false;
		AudioListener.volume = (soundSettingOn == 0) ? 0f: 1f;
		soundButton.GetComponent<Image>().sprite = (soundSettingOn == 0) ? soundOffSprite : soundOnSprite;
		if (PlayerPrefs.GetInt("noAds",0) == 1) {
			noAdsButton.GetComponent<Image> ().sprite = adFreeSprite;
			noAdsButton.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update() {
		if(homeConfirmed) {
			levelManager.secondTryUsed = false;
			levelManager.LoadLevel("StartMenu");
		}
	}

	public void OnResumeDown () {
		levelManager.OnResume();
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

	public void OnHome () {
		
		//if (GUI.Button(new Rect(xOffset, yOffset * 2, 150, 50), "Open alert dialog with 2 buttons"))
        //{
            MobileNativePopups.OpenAlertDialog(
				"Are you sure you want to quit?", "",
                "Yes", "No",
                () => 	{ 
                			Debug.Log("Accept was pressed"); 
                			homeConfirmed = true;
							//levelManager.secondTryUsed = false;
							//levelManager.LoadLevel("StartMenu");
                		},
                () => 	{ 
                			Debug.Log("Cancel was pressed"); 
                		});
        //}

		//levelManager.secondTryUsed = false;
		//levelManager.LoadLevel("StartMenu");
	}
}
