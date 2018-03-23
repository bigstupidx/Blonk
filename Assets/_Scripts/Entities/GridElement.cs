using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum ElementType {Empty, Square, Diamond, TriangleA, TriangleB, TriangleC, TriangleD, PlusBall};

public class GridElement : MonoBehaviour {

	public ElementType type;
	public int hitsLeft;

	public GridElement () {
		this.type = ElementType.Empty;
	}

	public GridElement (ElementType t) {
		this.type = t;
	}

	public virtual void EmptyElement() {
	}

	public virtual void EmptyElementImmediate () {
	}

	public virtual void EraseElement () {
	}

	// Use this for initialization
	void Start () {
		
	}

	void Awake () {
		if(!GameState.loadingGrid) {
			int score = GameState.score + 1;
			float isLargeBlock = Random.Range(0f, 1f);
			if(isLargeBlock < Parameters.LARGE_BLOCK_PROB) {
				hitsLeft = score * 3;
			} else {
				hitsLeft = score;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
