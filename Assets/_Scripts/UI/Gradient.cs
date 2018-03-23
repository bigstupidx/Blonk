using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gradient : MonoBehaviour {
	public Color colorTop = Color.white;
	public Color colorBottom = Color.black;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		MeshFilter thisMeshFilter = GetComponent<MeshFilter>();
		thisMeshFilter.mesh.colors = new Color[]{colorBottom, colorTop, colorBottom, colorTop};
	}
}
