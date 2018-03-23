using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class PlusBall : GridElement {
	bool ballAddSemaphore = true;
	public ParticleSystem myParticleSystem;

	PlusBall() {
		type = ElementType.PlusBall;	
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D trigger)
	{
		//FindObjectOfType<FloorCollider> ().current.ballsToAdd++;
		if (ballAddSemaphore) {
			ballAddSemaphore = false;
			GameController.gameState.ballsToAdd++;
			GameController.gameState.SaveLocal();
			EmptyElement ();
		}
	}

	public override void EmptyElement () {
		type = ElementType.Empty;
		ParticleSystem breakEffect = Instantiate(myParticleSystem,transform.position, Quaternion.identity);
		ParticleSystem.MainModule pModule = breakEffect.main;
		Destroy(gameObject);
	}

	public override void EraseElement () {
		type = ElementType.Empty;
		Destroy(gameObject);
	}
}
