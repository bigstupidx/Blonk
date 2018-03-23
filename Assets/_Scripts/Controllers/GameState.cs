using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Random = UnityEngine.Random;

public class GameState {

	public bool playEnded = true;
	public bool ballLaunchRequested = false;
	public bool launchBegan = false;
	public bool gameOver = false;
	public bool paused = false;
	public static bool secondTryUsed = false;
	public static bool loadingGrid = false;
	public static bool adWatched = false;
	public bool gameLoadedFromFile = false;

	//Block variables
	public GridElement[,] blocks = new GridElement[9,7];
	public GridElement[] newRandomBlocks = new GridElement[7];
	public float timer;

	//Ball variables
	public int ballsInQueue = 1; //balls waiting to be launched
	public int ballsLeft = 1; //balls left to hit floor for play to end
	public Vector2 newBallVelocity;
	public Vector2 launchTrajectory;
	public Vector2 trajectoryStart;
	public int numBalls = 1;
	public Vector2 nextBallStart;
	public int ballsToAdd = 0;

	//Scoring
	public static int score = 0;
	public int highScore;
	public bool newHighScore = false;
	GameStateSaver saveData;

	public GameState(bool preload) {
		saveData = new GameStateSaver();
		if(preload) {
			//initialize new game
			for(int i = 0; i < blocks.GetLength(0); i++) {
				for(int j = 0; j < blocks.GetLength(1); j++) {
					blocks[i, j] = GameObject.Instantiate<GridElement>(GameController.instance.gameUI.gridElementPrefab, GameController.instance.transform);
					//blocks [i, j] = GameController.instance.gameObject.AddComponent<GridElement>();
				}
			}

			newRandomBlocks = GameController.instance.generateRandomBlocks();
			for(int i = 0; i < 7; i++) {
				blocks[0, i] = newRandomBlocks[i];
			}
			nextBallStart = new Vector2(3.5f, GameController.instance.floorCollider.bounds.max.y + GameController.instance.ballCollider[0].radius + Parameters.EPSILON);
			highScore = PlayerPrefs.GetInt("highScore", 1);
		} else if(!Load()) {
				//initialize new game
				score = 0;
				for(int i = 0; i < blocks.GetLength(0); i++) {
					for(int j = 0; j < blocks.GetLength(1); j++) {
						blocks[i, j] = GameObject.Instantiate<GridElement>(GameController.instance.gameUI.gridElementPrefab, GameController.instance.transform);
						//blocks [i, j] = GameController.instance.gameObject.AddComponent<GridElement>();
					}
				}

				newRandomBlocks = GameController.instance.generateRandomBlocks();
				for(int i = 0; i < 7; i++) {
					blocks[0, i] = newRandomBlocks[i];
				}
				nextBallStart = new Vector2(3.5f, GameController.instance.floorCollider.bounds.max.y + GameController.instance.ballCollider[0].radius + Parameters.EPSILON);
				highScore = PlayerPrefs.GetInt("highScore", 1);
		} else {
			gameLoadedFromFile = true;
		}
	}

	public void SavePermanent() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/savedGame.dat");
		bf.Serialize(file, saveData);
		file.Close();
	}

	public void SaveLocal() {
		saveData.playEnded = false;
		//saveData.ballLaunchRequested = false;
		saveData.launchBegan = false;
		saveData.gameOver = false;
		saveData.paused = false;
		saveData.secondTryUsed = secondTryUsed;
		saveData.ballsInQueue = ballsInQueue;
		saveData.nextBallStartX = nextBallStart.x;
		saveData.nextBallStartY = nextBallStart.y;
		saveData.ballsToAdd = ballsToAdd;
		saveData.numBalls = numBalls;
		saveData.ballsLeft = ballsLeft;
		saveData.score = score;
		saveData.newHighScore = newHighScore;
		saveGrid(ref saveData);

	}

	public bool Load() {
		if(File.Exists(Application.persistentDataPath + "/savedGame.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			Debug.Log("DATAPATH: " + Application.persistentDataPath);
			FileStream file = File.Open(Application.persistentDataPath + "/savedGame.dat", FileMode.Open);
			GameStateSaver data = (GameStateSaver)bf.Deserialize(file);

			this.playEnded = false;
			this.paused = false;
			GameState.secondTryUsed = data.secondTryUsed;
			this.ballsInQueue = data.ballsInQueue;
			this.nextBallStart = new Vector2(data.nextBallStartX, data.nextBallStartY);
			this.ballsToAdd = data.ballsToAdd;
			this.numBalls = data.numBalls;
			this.ballsLeft = data.ballsLeft;
			score = data.score;
			this.newHighScore = data.newHighScore;
			loadingGrid = true;
			loadGrid(data);
			loadingGrid = false;

			file.Close();
			File.Delete(Application.persistentDataPath + "/savedGame.dat");
			return true;
		} else {
			return false;
		}
	}

	void saveGrid(ref GameStateSaver data) {

		for (int i = 0; i < data.gridSerial.GetLength(0); i++) {
			for (int j = 0; j < data.gridSerial.GetLength(1); j++) {
				data.gridSerial[i,j] = (int)blocks[i,j].type;
				data.gridHits[i,j] = blocks[i,j].hitsLeft;
			}
		}
	}

	void loadGrid(GameStateSaver data) {
		//reset blocks array
		/*
		for(int i = 0; i < blocks.GetLength(0); i++) {
			for(int j = 0; j < blocks.GetLength(1); j++) {	
				blocks[i, j].EraseElement();
			}
		}*/

		//populate array with loaded data
		GridElement nextBlock;
		for(int i = 0; i < blocks.GetLength(0); i++) {
			for(int j = 0; j < blocks.GetLength(1); j++) {
				ElementType elementType = (ElementType)data.gridSerial[i, j];
				switch(elementType) {
					case (ElementType.Empty):
						nextBlock = GameController.instance.gameUI.gridElementPrefab;
						blocks[i,j] = (GridElement)GameObject.Instantiate(nextBlock, new Vector2((float)j+0.5f,11f - (float)i), Quaternion.identity, GameController.instance.transform);
						blocks[i,j].type = ElementType.Empty;
						break;
					case (ElementType.Square):
						nextBlock = GameController.instance.gameUI.squarePrefab;
						blocks[i,j] = (Block)GameObject.Instantiate(nextBlock, new Vector2((float)j+0.5f,11f - (float)i), Quaternion.identity, GameController.instance.transform);
						blocks[i,j].type = ElementType.Square;
						blocks[i,j].hitsLeft = data.gridHits[i,j];
						break;
					case (ElementType.Diamond):
						nextBlock = GameController.instance.gameUI.diamondPrefab;
						blocks[i,j] = (Block)GameObject.Instantiate(nextBlock, new Vector2((float)j+0.5f,11f - (float)i), Quaternion.identity, GameController.instance.transform);
						blocks[i,j].type = ElementType.Diamond;
						blocks[i,j].hitsLeft = data.gridHits[i,j];
						break;
					case (ElementType.TriangleA):
						nextBlock = GameController.instance.gameUI.triangleAPrefab;
						blocks[i,j] = (Block)GameObject.Instantiate(nextBlock, new Vector2((float)j+0.5f,11f - (float)i), Quaternion.identity, GameController.instance.transform);
						blocks[i,j].type = ElementType.TriangleA;
						blocks[i,j].hitsLeft = data.gridHits[i,j];
						break;
					case (ElementType.TriangleB):
						nextBlock = GameController.instance.gameUI.triangleBPrefab;
						blocks[i,j] = (Block)GameObject.Instantiate(nextBlock, new Vector2((float)j+0.5f,11f - (float)i), Quaternion.identity, GameController.instance.transform);
						blocks[i,j].type = ElementType.TriangleB;
						blocks[i,j].hitsLeft = data.gridHits[i,j];
						break;
					case (ElementType.TriangleC):
						nextBlock = GameController.instance.gameUI.triangleCPrefab;
						blocks[i,j] = (Block)GameObject.Instantiate(nextBlock, new Vector2((float)j+0.5f,11f - (float)i), Quaternion.identity, GameController.instance.transform);
						blocks[i,j].type = ElementType.TriangleC;
						blocks[i,j].hitsLeft = data.gridHits[i,j];
						break;
					case (ElementType.TriangleD):
						nextBlock = GameController.instance.gameUI.triangleDPrefab;
						blocks[i,j] = (Block)GameObject.Instantiate(nextBlock, new Vector2((float)j+0.5f,11f - (float)i), Quaternion.identity, GameController.instance.transform);
						blocks[i,j].type = ElementType.TriangleD;
						blocks[i,j].hitsLeft = data.gridHits[i,j];
						break;
					case (ElementType.PlusBall):
						nextBlock = GameController.instance.gameUI.plusBallPrefab;
						blocks[i,j] = (PlusBall)GameObject.Instantiate(nextBlock, new Vector2((float)j+0.5f,11f - (float)i), Quaternion.identity, GameController.instance.transform);
						blocks[i,j].type = ElementType.PlusBall;
						break;
					default:
						Debug.Log("load grid default switch case should never happen");
						Debug.Assert(false);
						nextBlock = GameController.instance.gameUI.squarePrefab;
						break;
				}
			}
		}
	}
}




[System.Serializable]
class GameStateSaver {

	public bool playEnded;
	//public bool ballLaunchRequested;
	public bool launchBegan;
	public bool gameOver;
	public bool paused;
	public bool secondTryUsed;

	//Ball variables
	public int ballsInQueue;
	public int numBalls;
	public int ballsLeft;
	//public Vector2 nextBallStart;
	public float nextBallStartX;
	public float nextBallStartY;
	public int ballsToAdd = 0;

	//Scoring
	public int score = 0;
	public bool newHighScore;

	public int[,] gridSerial = new int[9,7];
	public int[,] gridHits = new int[9,7];
}