using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class Block : GridElement {

	//public int hitsLeft = -1;
	public TextMeshPro textPrefab;
	private TextMeshPro maxHitsDisplay;
	public TMP_FontAsset blockFont;
	public AudioClip hitSound;

	public ParticleSystem myParticleSystem;
	private ParticleSystem.ColorOverLifetimeModule colorModule;
	private float fixedSaturation = 1f;
	private float fixedBrightness = 0.92f;
	private float startHue = 0.1f;
	private Component[] blockComponents;
	private Animator hitAnimator;
	private float FONT_START = 3.5f;

	Block() {
		type = ElementType.Square;
	}

	// Use this for initialization
	void Start() {

		//determine random hit number
		print("*****LOADING GRID*****   " + GameState.loadingGrid);


		//Render color and hits left number
		GetComponent<Renderer>().material.color = Color.HSVToRGB(System.Math.Min(1f,(startHue+(float)hitsLeft/2)/100f),fixedSaturation, fixedBrightness);
		blockComponents = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in blockComponents)
            if (r.name == "highlight")
				r.material.color = Color.HSVToRGB(System.Math.Min(1f,startHue+(float)hitsLeft/100f),fixedSaturation, fixedBrightness);
		
		maxHitsDisplay = Instantiate(textPrefab);
		//maxHitsDisplay = gameObject.AddComponent<TextMeshPro>(textPrefab);
		maxHitsDisplay.font = blockFont;
		maxHitsDisplay.fontSize = FONT_START;
		Color c;
		ColorUtility.TryParseHtmlString ("#3A3A3BFF", out c);
		maxHitsDisplay.color = c;
		maxHitsDisplay.sortingLayerID = SortingLayer.NameToID("HitsText");
		hitAnimator = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//move text along with block

		if (hitsLeft < 100) {
			maxHitsDisplay.fontSize = FONT_START;
		} else if (hitsLeft >= 100) {
			maxHitsDisplay.fontSize = 2.5f;
			if (hitsLeft >= 1000) {
				maxHitsDisplay.fontSize = 1.5f;
			}
		}
		
		switch (type) {
				case (ElementType.Square):
					maxHitsDisplay.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z-1);
					break;
				case (ElementType.Diamond):
					maxHitsDisplay.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z-1);
					break;
				case (ElementType.TriangleA):
					maxHitsDisplay.transform.position = new Vector3(this.transform.position.x-0.22f, this.transform.position.y+0.22f, this.transform.position.z-1);
					break;
				case (ElementType.TriangleB):
					maxHitsDisplay.transform.position = new Vector3(this.transform.position.x+0.2f, this.transform.position.y+0.2f, this.transform.position.z-1);
					break;
				case (ElementType.TriangleC):
					maxHitsDisplay.transform.position = new Vector3(this.transform.position.x-0.16f, this.transform.position.y-0.14f, this.transform.position.z-1);
					break;
				case (ElementType.TriangleD):
					maxHitsDisplay.transform.position = new Vector3(this.transform.position.x+0.18f, this.transform.position.y-0.17f, this.transform.position.z-1);
					break;
				default:
					break;
		}


		//maxHitsDisplay.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z-1);
		maxHitsDisplay.SetText(hitsLeft.ToString());
		GetComponent<Renderer>().material.color = Color.HSVToRGB(System.Math.Min(1f,startHue+(float)hitsLeft/100f),fixedSaturation, fixedBrightness);

		foreach (Renderer r in blockComponents)
            if (r.name == "highlight")
				r.material.color = Color.HSVToRGB(System.Math.Min(1f,startHue+(float)hitsLeft/100f),fixedSaturation, fixedBrightness+0.08f);
	}

	void OnCollisionExit2D (Collision2D collision)
	{
		AudioSource.PlayClipAtPoint(hitSound,transform.position, 1f);
		hitAnimator.SetTrigger("Hit");
		hitsLeft--;
		if (hitsLeft <= 0) {
			EmptyElement();
		}
	}

	void PlayBreakEffect () {
		ParticleSystem breakEffect = Instantiate(myParticleSystem,transform.position, Quaternion.identity);
		ParticleSystem.MainModule pModule = breakEffect.main;
		//pModule.startColor = GetComponent<SpriteRenderer>().color;
		pModule.startColor = GetComponent<Renderer>().material.color;
	}

	//breaks blocks with explosion effect
	public override void EmptyElement () {
		PlayBreakEffect();
		type = ElementType.Empty;
		Destroy(maxHitsDisplay.gameObject);
		Destroy(gameObject);
	}

	//removes element without explosion
	public override void EraseElement () {
		type = ElementType.Empty;
		Destroy(maxHitsDisplay.gameObject);
		Destroy(gameObject);
	}
}
