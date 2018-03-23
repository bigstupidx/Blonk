using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public class Ball : MonoBehaviour {

	//public BoxCollider2D floorCollider;
	//public Sprite ballSprite;
	//public TextMeshProUGUI scoreText;
	//public Rigidbody2D box;

	// Use this for initialization
	void Start () {
		//initialize to 0 gravity to avoid triggering floor at start
		GetComponent<Rigidbody2D>().gravityScale = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

}
