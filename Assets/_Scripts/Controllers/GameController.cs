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

public class GameController : MonoBehaviour {

	//Global State Objects
	public static GameController instance;
	public static GameState gameState;
	LevelManager levelManager = LevelManager.instance;
	public static bool preload = false;
	bool movingBlocks = true;
	bool animationsOn = true;
	public GameUI gameUI;
	public BoxCollider2D floorCollider;
	public CircleCollider2D[] ballCollider = new CircleCollider2D[1];
	public Rigidbody2D ball;
	public Rigidbody2D[] ballsArray;
	public Vector2 newBallVelocity;
	public Vector2 launchTrajectory;
	public Vector2 trajectoryStart;

	public GridElement[] newRandomBlocks = new GridElement[7];
	//bool skipNextLevelSound = false;

    void Start() {
    	if (preload) 
    		return;
		instance = this;
		ball = FindObjectOfType<Ball>().GetComponents<Rigidbody2D>()[0];
		ball.GetAttachedColliders(ballCollider);
		gameState = new GameState(preload);
		gameState.SaveLocal();

		if(gameState.gameLoadedFromFile) {
			animationsOn = false;
			gameUI.swipeAnimation.SetActive(false);
			gameState.gameLoadedFromFile = false;
		}

		gameState.timer = Parameters.TIME_FOR_MOVE;
		//ignore ball-on-ball collisions
		Physics2D.IgnoreLayerCollision (8, 8, true);
		// ignore block-on-floor collisions
		Physics2D.IgnoreLayerCollision (9, 10, true);

		//Initialize first ball
		ball.position = gameState.nextBallStart;


		gameUI.setScore(GameState.score); 
		gameUI.setHighScore(PlayerPrefs.GetInt("highScore",1));
		//Initialize trajectory line
		launchTrajectory = new Vector2 (ball.position.x, ball.position.y + 2f) - ball.position;
		launchTrajectory.Normalize();

		gameUI.tapAnimation.SetActive (false);
		gameUI.loseTutorialText.SetActive (false);

		loadBalls();

		//The StartMenu scene loads GameScene additively and sets preload, when that happens we don't play the game start sound
		if (preload) {
			preload = false;
		} else {
			gameUI.gameStartSoundAS.Play();
		}
	}

	void resetBalls() {
		//ballsArray = new Rigidbody2D[gameState.numBalls - 1];
		gameState.ballsInQueue = gameState.numBalls;
		ballsArray = new Rigidbody2D[gameState.ballsInQueue-1];
		gameUI.setBallsLeft(gameState.ballsInQueue); 

		ball.GetComponent<SpriteRenderer>().enabled = true;
		ball.GetComponent<Collider2D>().enabled = true;

		for(int i = 0; i < ballsArray.GetLength(0); i++) {
			ballsArray[i] = (Rigidbody2D)Instantiate(ball);
			ballsArray[i].position = ball.position;
		}
	}

	void loadBalls() {
		ballsArray = new Rigidbody2D[gameState.ballsInQueue-1];
		gameUI.setBallsLeft(gameState.ballsInQueue); 
		ball.GetComponent<SpriteRenderer>().enabled = true;
		ball.GetComponent<Collider2D>().enabled = true;

		for(int i = 0; i < ballsArray.GetLength(0); i++) {
			ballsArray[i] = (Rigidbody2D)Instantiate(ball);
			ballsArray[i].position = ball.position;
		}
	}

    void FixedUpdate() {	


		if(preload)
			return;
		if(gameState.paused)
			return;
		
		//bool cache = FindObjectOfType<Canvas> ().overrideSorting;
		//FindObjectOfType<Canvas> ().overrideSorting = false;
		//FindObjectOfType<Canvas> ().overrideSorting = cache;
		//after last ball hits floor, scroll all blocks down by 1f over TIME_FOR_MOVE
		if (gameState.playEnded && !preload) {
			if(movingBlocks) {
				movingBlocks = moveBlocks();
			} else if(!movingBlocks) {

				GameState.score++;
				gameUI.setScore(GameState.score); 
				if (GameState.score > gameState.highScore) {
					gameState.highScore = GameState.score;
					PlayerPrefs.SetInt("highScore", gameState.highScore);
					//highScoreDisplay.text = "TOP\n" + gameState.highScore;
					gameUI.setHighScore(gameState.highScore);
					gameState.newHighScore = true;
				}

				eraseNonBlocks();
				if(isGameOver()) {
					gameState.gameOver = true;
					AudioSource.PlayClipAtPoint (gameUI.gameOverSound, transform.position);
					levelManager.GameOver ();
				}

				gameState.ballLaunchRequested = false;
				gameUI.ffButton.interactable = true;
				drawTrajectoryLine (ref launchTrajectory);
				resetBalls();
				gameState.playEnded = false;
				gameState.SaveLocal();
			}
		} else if (!gameState.ballLaunchRequested) { //Wait for and handle touch drags
			if (Input.touchCount > 0) {
				if (gameUI.swipeAnimation.activeSelf && animationsOn) {
					gameUI.tapAnimation.SetActive (true);
				}

				Touch touch = Input.GetTouch (0);
				switch (touch.phase) {
					case TouchPhase.Began:
						break;
					case TouchPhase.Moved:
						Vector2 currentPosition = Camera.main.ScreenToWorldPoint (touch.position);
						//Vector2 currentPosition = touch.position;
						if (currentPosition.y > 3.0f && currentPosition.y < 11.8f && gameState.ballsInQueue > 0) {

							float deltaXNorm = touch.deltaPosition.x / Screen.width;
							float deltaYNorm = touch.deltaPosition.y / Screen.height;

							launchTrajectory = new Vector2 (
							launchTrajectory.x + (deltaXNorm*11.8f*3.5f)/(float)Math.Pow(currentPosition.y,2), // / ((float)Math.Pow (currentPosition.y/Screen.height, 2)), 
							launchTrajectory.y + (deltaYNorm*11.8f*3.5f)/(float)Math.Pow(currentPosition.y,2) // / ((float)Math.Pow (currentPosition.y/Screen.height, 2))
							);
							launchTrajectory.Normalize ();
							drawTrajectoryLine (ref launchTrajectory);
						}
						break;
				}
			} else { 
				return;
			}
		} else if (gameState.ballLaunchRequested) { //wait for and handle firing
			if (!gameState.launchBegan) {
				newBallVelocity = launchTrajectory;
				if (newBallVelocity.normalized.y > Parameters.MIN_LAUNCH_Y) {
					gameState.ballLaunchRequested = true;
					gameUI.trajectoryLine.enabled = false;
					newBallVelocity.Normalize();
					newBallVelocity *= Parameters.BALL_SPEED;
					StartCoroutine(launchBalls(newBallVelocity, Parameters.INTER_BALL_DELAY));
                 }
            }
        }
	}

	bool moveBlocks() {
		if(gameState.timer > 0) {
			gameState.timer -= Time.deltaTime;
			float degree_of_movement = (Parameters.TIME_FOR_MOVE - gameState.timer) / Parameters.TIME_FOR_MOVE;
			for(int i = 0; i < 9; i++) {
				for(int j = 0; j < 7; j++) {
					if(gameState.blocks[i, j] != null && gameState.blocks[i, j].type != ElementType.Empty) {
						gameState.blocks[i, j].transform.position = new Vector2(gameState.blocks[i, j].transform.position.x, Parameters.startPositions[i] - (1f * degree_of_movement));
					}
				}
			}
			return true;
		} else {
			//finished scrolling rows, moving blocks down within array
			for (int i = 8; i > 0; i--) {
				for (int j = 0; j < 7; j++) {
					gameState.blocks [i, j] = gameState.blocks [i - 1, j];
				}
			}

			//empty first row
			for (int j = 0; j < 7; j++) {
				//gameState.blocks[0, j].EraseElement();
				gameState.blocks [0, j] = Instantiate(gameUI.gridElementPrefab);
			}
			gameState.timer = Parameters.TIME_FOR_MOVE;
			return false;
		}	
	}

	bool isGameOver() {
		for (int i = 0; i < 7; i++) {
			if (gameState.blocks [8, i].type != ElementType.Empty) {
				for (int j = 0; j < 7; j++) {
					if (gameState.blocks [8, j] != null) {
						gameState.blocks [8, j].EraseElement ();
					}

					if (gameState.blocks [7, j] != null) {
						gameState.blocks [7, j].EraseElement ();
					}
				}
				return true;
			}
		}
		return false;
	}

	public void eraseNonBlocks() {
		for (int i = 0; i < 7; i++) {
			if (gameState.blocks [8, i].type == ElementType.PlusBall) {
				gameState.blocks [8, i].EraseElement ();
			}
		}
	}

	public void setBallsState (bool state) {
		ball.GetComponent<SpriteRenderer>().enabled = state;
		ball.GetComponent<Collider2D> ().enabled = state;

		for (int i = 0; i < ballsArray.GetLength (0); i++) {
			ballsArray[i].GetComponent<SpriteRenderer> ().enabled = state;;
			ballsArray [i].GetComponent<Collider2D> ().enabled = state;
		}
	}

    public void OnFireButtonDown ()
	{
		if (gameState.paused) return;

		//if first tutorial animation is running, run second set of tutorial animations
		if (gameUI.swipeAnimation.activeSelf && animationsOn) {
			gameUI.tapAnimation.SetActive(false);
			gameUI.swipeAnimation.SetActive(false);
			gameUI.loseTutorialText.SetActive(true);
		}

		gameUI.playFireDown();

		if (!gameState.playEnded) {
			gameState.ballLaunchRequested = true;
        }
    }

    public void OnFireButtonUp() {
    	if (gameState.paused) return;
		gameState.ballLaunchRequested = false;
		gameUI.playFireUp();
    }

    IEnumerator launchBalls(Vector2 v, float delay) {
        
		while(gameState.ballsInQueue > 1 && gameState.ballLaunchRequested) { 
			gameState.launchBegan = true;
			ballsArray[gameState.ballsInQueue-2].velocity = v;
			ballsArray[gameState.ballsInQueue-2].gravityScale = Parameters.GRAVITY_SCALE;
			gameState.ballsInQueue--;
			gameUI.setBallsLeft(gameState.ballsInQueue);
			gameState.SaveLocal();
            yield return new WaitForSecondsRealtime(delay);
        }

        //original ball gets launched last
		if (gameState.ballsInQueue == 1 && gameState.ballLaunchRequested) {
			gameState.launchBegan = true;
			ball.velocity = v;
			ball.gravityScale = Parameters.GRAVITY_SCALE;
			gameState.ballsInQueue--;
			gameUI.setBallsLeft(gameState.ballsInQueue);
			gameState.ballLaunchRequested = false;
			gameState.SaveLocal();
        }

		gameState.launchBegan = false;
    }

	public void nextLevel() {
		newRandomBlocks = generateRandomBlocks ();
		for (int i = 0; i < 7; i++) {
			gameState.blocks [0, i] = newRandomBlocks [i];
		}
		movingBlocks = true;
	}

	public GridElement[] generateRandomBlocks ()
	{
		//Determine a random number of new blocks
		int numBlocks = (int)Random.Range (2, 6);
		GridElement[] randomBlocks = new GridElement[7];
		for (int i = 0; i < randomBlocks.GetLength (0); i++) {
			randomBlocks [i] = (GridElement)Instantiate (gameUI.gridElementPrefab);
		}

		//insert new blocks in random positions
		int nextRandomPosition;
		for (int i = 0; i < numBlocks; i++) {
			int f = Random.Range (0, 7);
			nextRandomPosition = f;
			while (randomBlocks [nextRandomPosition].type != ElementType.Empty) {
				f = Random.Range (0, 7);
				nextRandomPosition = f;
			}

			GridElement nextBlock;
			//GridElement nextBlock = blockPrefab;
			int elementType = Random.Range (0, 6);
			switch (elementType) {
				case (0):
					nextBlock = gameUI.squarePrefab;
					break;
				case (1):
					nextBlock = gameUI.diamondPrefab;
					break;
				case (2):
					nextBlock = gameUI.triangleAPrefab;
					break;
				case (3):
					nextBlock = gameUI.triangleBPrefab;
					break;
				case (4):
					nextBlock = gameUI.triangleCPrefab;
					break;
				case (5):
					nextBlock = gameUI.triangleDPrefab;
					break;
				default:
					print("randomGenerator default switch case should never happen");
					Debug.Assert(false);
					nextBlock = gameUI.squarePrefab;
					break;
			}

			randomBlocks[nextRandomPosition] = (Block) Instantiate(nextBlock, new Vector2((float)nextRandomPosition+0.5f,11f), Quaternion.identity);
		}

		//insert one new ball in a random one of the remaining spaces
		int s = Random.Range(0, 7);
		nextRandomPosition = s;
		while (randomBlocks[nextRandomPosition].type != ElementType.Empty) {
			int f = Random.Range(0, 7);
			nextRandomPosition = f;
		}
		randomBlocks[nextRandomPosition] = (PlusBall) Instantiate(gameUI.plusBallPrefab, new Vector2((float)nextRandomPosition+0.5f,11f), Quaternion.identity);

		return randomBlocks;

	}



	IEnumerator launchWithDelay (Rigidbody2D[] b, Vector2 v, float seconds)
	{
		yield return new WaitForSecondsRealtime (seconds);
		for (int i = 0; i < b.GetLength(0); i++) {
			b[i].velocity = v;
			b[i].gravityScale = Parameters.GRAVITY_SCALE;
			yield return new WaitForSecondsRealtime (seconds);
		}
	}

	public void OnFastForward ()
	{
		if (gameState.playEnded) {
			return;
		}

		Vector2 zero = new Vector2 (0f, 0f);
		if (ball.velocity != zero) {
			ball.velocity = ball.velocity.normalized*Parameters.FF_SPEED;
		}

		for (int i = 0; i < ballsArray.GetLength(0); i++) {
			if ((ballsArray[i] != null) && ballsArray[i].velocity != zero) {
				ballsArray[i].velocity = ballsArray[i].velocity.normalized*Parameters.FF_SPEED;
			}	
		}
	}

	void drawTrajectoryLine (ref Vector2 traj) {

		//limit trajectory to a minimum angle
		if (traj.normalized.y < Parameters.MIN_LAUNCH_Y) {
			if (traj.x < 0) {
				traj = Parameters.MIN_LAUNCH_LEFT;
			} else {
				traj = Parameters.MIN_LAUNCH_RIGHT;
			}
		}

		RaycastHit2D trajectoryHit;

		Vector2 ballPosition = ball.position + traj.normalized * (ballCollider[0].radius + Parameters.EPSILON);
		if (Physics2D.Raycast (ballPosition, traj)) {
			trajectoryHit = Physics2D.Raycast (ballPosition, traj);
			gameUI.trajectoryLine.SetPositions (new Vector3[] {
				(Vector3) ballPosition,
				(Vector3)(trajectoryHit.point)
			});
		}

		gameUI.trajectoryLine.enabled = true;
	}

	public void OnPauseDown ()
	{
		if (!gameState.paused) {
			print ("Pause button pressed");
			gameState.paused = true;
			levelManager.OnPause ();
		}

	}

	public void pauseScene () {
		Time.timeScale = 0f;
	}

	public void unpauseScene () {

		gameState.paused = false;
		Time.timeScale = 1f;
	}

	void OnApplicationQuit() {
		//if (movingBlocks playEnded)
		if(gameState != null && !gameState.gameOver && !preload) {
			gameState.SavePermanent();
		}
	}
}
