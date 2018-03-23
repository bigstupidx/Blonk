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

public class FloorCollider : MonoBehaviour {

	GameController gameController;
	GameState gameState = GameController.gameState;
	public GameUI gameUI; 

	void Start () {
	}

	//When ball hits floor
	void OnTriggerEnter2D (Collider2D trigger)
	{
		if (GameController.preload) return;
		print("****Entered FloorCollider trigger****");
		gameController = GameController.instance;
		gameState = GameController.gameState;
		Debug.Assert (gameState.ballsLeft <= gameState.numBalls);
		Debug.Assert (gameState.ballsLeft >= 1);
		Debug.Assert (gameState.numBalls <= GameState.score);


		if (gameState.ballsLeft <= 1) { //end the play, generate boxes, update score

			//If this is the last ball, set nextBallStart position
			if (gameState.ballsLeft == 1) {
				gameState.nextBallStart = new Vector2 (trigger.transform.position.x, GetComponent<BoxCollider2D>().bounds.max.y + gameController.ballCollider[0].radius + Parameters.EPSILON);
			}

			gameUI.ffButton.interactable = false;
			if (trigger.name != "Ball") {
				Destroy (trigger.gameObject);
			} else {
				gameController.ball.GetComponent<SpriteRenderer>().enabled = false;
				gameController.ball.GetComponent<Collider2D>().enabled = false;
			}

			gameController.nextLevel(); //generate random boxes


		
			gameController.ball.velocity = new Vector2 (0f, 0f);
			gameController.ball.position = gameState.nextBallStart;
			gameController.ball.gravityScale = 0f;
			gameState.numBalls += gameState.ballsToAdd;
			gameState.ballsToAdd = 0;
			gameState.ballsLeft = gameState.numBalls;
			gameState.SaveLocal();

			StartCoroutine (endPlayAfter (0.5f));
			gameUI.loseTutorialText.SetActive(false);

		} else { //play has not ended, destroy balls

			//if not original ball, destroy
			if (trigger.name != "Ball") {
				Destroy (trigger.gameObject);

			} else { //if original ball, make invisible 
				gameController.ball.GetComponent<SpriteRenderer>().enabled = false;
				gameController.ball.GetComponent<Collider2D>().enabled = false;
			}

			gameState.ballsLeft--;
			gameState.SaveLocal();
		}
	}

	IEnumerator endPlayAfter(float seconds) {
		yield return new WaitForSecondsRealtime(seconds);
		gameState.playEnded = true;
	}
}


