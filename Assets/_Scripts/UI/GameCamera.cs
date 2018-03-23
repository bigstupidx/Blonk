using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		if (Screen.currentResolution.width == 1125 || Screen.width == 1125) {
			foreach (Camera c in Camera.allCameras) {
				c.orthographicSize = 7.58f;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
