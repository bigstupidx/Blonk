using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using Random = UnityEngine.Random;

public class GameUI : MonoBehaviour {

	//Text Objects
	public TextMeshPro textPrefab;
	public TMP_FontAsset arialRoundFontAsset;
	public TMP_FontAsset audiowideFontAsset;
    public TextMeshProUGUI ballsLeftDisplay;


	private TextMeshPro scoreDisplay;
	private TextMeshPro highScoreDisplay;
	private TextMeshPro fireText;
	//UI Buttons
	public Button ffButton;
	public GameObject fireButton;
	public Button pauseButton;

	// Block/Grid Objects
	public GridElement gridElementPrefab;
	public GridElement squarePrefab;
	public GridElement diamondPrefab;
	public GridElement triangleAPrefab;
	public GridElement triangleBPrefab;
	public GridElement triangleCPrefab;
	public GridElement triangleDPrefab;
	public GridElement plusBallPrefab;
	public LineRenderer trajectoryLine;

	//Animation Objects
    public GameObject swipeAnimation;
    public GameObject tapAnimation;
    public GameObject loseTutorialText;

    //Sound Objects
	public AudioSource gameStartSoundAS;
	public AudioClip gameOverSound;

	// Use this for initialization
	void Start () {

		//initialize score display
		scoreDisplay = Instantiate (textPrefab);
		scoreDisplay.font = arialRoundFontAsset;
		scoreDisplay.fontSize = 5f;
		Color sdColor;
		ColorUtility.TryParseHtmlString ("#3a3a3b", out sdColor);
		scoreDisplay.color = sdColor;
		scoreDisplay.transform.position = new Vector3 (3.5f, 11.84f, -1f);
		scoreDisplay.text = "1";
		scoreDisplay.sortingLayerID = SortingLayer.NameToID ("FireButton");

		//initialize high score display
		highScoreDisplay = Instantiate (textPrefab);
		highScoreDisplay.font = arialRoundFontAsset;
		highScoreDisplay.fontSize = 3.5f;
		highScoreDisplay.color = Color.white;
		highScoreDisplay.transform.position = new Vector3 (3.5f + 2.85f, 11.83f, -1f);
		highScoreDisplay.SetText (PlayerPrefs.GetInt("highScore", 1).ToString());
		highScoreDisplay.sortingLayerID = SortingLayer.NameToID ("FireButton");

		// Initialize Button text ("FIRE")
		fireText = Instantiate (textPrefab);
		fireText.font = audiowideFontAsset;
		fireText.fontSize = 3f;
		fireText.transform.position = new Vector3 (fireButton.transform.position.x, fireButton.transform.position.y, fireButton.transform.position.z - 1);
		fireText.color = Color.black;
		fireText.alpha = 0.75f;
		fireText.SetText ("FIRE");
		fireText.sortingLayerID = SortingLayer.NameToID ("FireText");

		//Using sound controller for start sound to tune it a bit
		//gameStartSoundAS = GetComponent<AudioSource> ();
		trajectoryLine.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setScore(int score) {
		scoreDisplay.text = score.ToString ();
	}

	public void setHighScore(int highScore) {
		highScoreDisplay.text = highScore.ToString();
	}

	public void setBallsLeft (int ballsLeft) {
		ballsLeftDisplay.text = ballsLeft.ToString ();
	}

	public void playFireDown() {
		Animator anim = fireButton.GetComponentInChildren<Animator>();
		anim.Play("FireButtonDownAnim");
		fireText.transform.position = new Vector3(fireText.transform.position.x, fireText.transform.position.y - 0.05f, fireText.transform.position.z);
	}

	public void playFireUp() {
		Animator anim = fireButton.GetComponentInChildren<Animator>();
    	anim.Play("FireButtonUpAnim");
		fireText.transform.position = new Vector3 (fireText.transform.position.x, fireText.transform.position.y+0.05f, fireText.transform.position.z);
	}



}
