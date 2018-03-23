using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Singleton class
public class LevelManager {

	public bool secondTryUsed = false;
	public bool showAdNow = false;


    private static LevelManager _instance;
    public static LevelManager instance {
		get {
			if (_instance == null)
				_instance = new LevelManager ();
			return _instance;
		}
	}

	// Use this for initialization
	private LevelManager()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;
		SceneManager.activeSceneChanged += OnActiveSceneChanged;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadLevel (string level) {
		//SceneManager.LoadScene(level);
		SceneManager.LoadScene(level);
	}

	public void GameOver ()
	{
		//Time.timeScale = 0f;
		if (secondTryUsed) {
			//secondTryUsed = false;
			SceneManager.LoadScene ("ContinueScene", LoadSceneMode.Additive);
		} else {
			//show ad?
			SceneManager.LoadScene ("ContinueScene", LoadSceneMode.Additive);
			//secondTryUsed = true;
		}
	}

	void OnSceneLoaded (Scene scene, LoadSceneMode mode)
	{
		Debug.Log("OnSceneLoaded: " + scene.name);
		
		if (scene.name == "ContinueScene") {
			Debug.Log("setting active scene to Continue Scene");
			SceneManager.SetActiveScene (SceneManager.GetSceneByName ("ContinueScene"));
		}
	}

	void OnSceneUnloaded (Scene scene) {
		Debug.Log ("Scene unloaded: " + scene.name);
	}

	void OnActiveSceneChanged(Scene scenePrev, Scene sceneNew) {

		Debug.Log("on ActiveSceneChanged called, scenePrev: " + scenePrev.name + ", sceneNew: " + sceneNew.name);
		if(sceneNew.name == "ContinueScene") {
			//Time.timeScale = 0f;
		} else if(sceneNew.name == "StartMenu") {
			Time.timeScale = 1f;
		} else if(sceneNew.name == "GameScene") {
			Time.timeScale = 1f;
		}

		if(sceneNew.name == "StartMenu" && scenePrev.name == "ContinueScene") {
			Debug.Log("back to start from game over");
			if (PlayerPrefs.GetInt("noAds",0) == 0) {
				showAdNow = true;
				//GameObject.FindObjectOfType<AdManager>().ShowAd();
			}	
		}
	}

	public void SecondTry ()
	{
		SceneManager.UnloadSceneAsync("ContinueScene");
		SceneManager.SetActiveScene(SceneManager.GetSceneByName("GameScene"));
	}

	IEnumerator unloadContinueScene() {
		yield return SceneManager.UnloadSceneAsync("ContinueScene");
	}

    public void preLoadGame () {
    	//Time.timeScale = 0f;
		GameController.preload = true;
    	SceneManager.LoadSceneAsync("GameScene", LoadSceneMode.Additive);
 
	}

	public void OnResume () {
		GameController.instance.unpauseScene();
		SceneManager.UnloadSceneAsync("PauseScene");
	}

	public void OnPause () {
		GameController.instance.pauseScene();
		SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);

	}
}


