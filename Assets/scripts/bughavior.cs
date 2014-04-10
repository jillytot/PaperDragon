using UnityEngine;
using System.Collections;

public class bughavior : MonoBehaviour {

	public float speed = 0;
	public float maxSpeed = 5;
	public float acceleration = 1;
	public float deccelration = 1;
	//float timeStep = 0.1f;

	bool runAway = false;

	CharacterController myController;
	public Vector3 moveDirection = Vector3.zero; //initialize movement direction

	Vector3 runFromTarget;
	Transform myHunter;
	Vector3 storeNormal;
	Vector3 targetPosition;

	public ParticleSystem imOnFire;
	bool fireOn;
	bool waitForNexFire;
	bool burning;

	public creatureStats myStats;
	public Material crispy;
	Material storeMat;
	Transform myChild;

	// Use this for initialization
	void Start () {

		myStats = new creatureStats();
		//myStats = this.gameObject.GetComponent<creatureStats>();

		myController = GetComponent<CharacterController>();
		runAway = false;
		runFromTarget = Vector3.zero;
		targetPosition = Vector3.zero;
		fireOn = false;
		imOnFire.enableEmission = false;
		waitForNexFire = false;
		burning = false;

		foreach (Transform child in transform) {

			myChild = child;

		}

		storeMat = myChild.renderer.material;

	
	}
	
	// Update is called once per frame
	void Update () {

		if (myController.isGrounded) {

			if (runAway == true) {


				RaycastHit hit;
				Physics.Raycast(transform.position, Vector3.down, out hit);
				Debug.DrawRay(transform.position, Vector3.down, Color.blue, 2);
				
				if (Physics.Raycast(transform.position, Vector3.down, 2)) {
					
					storeNormal = hit.normal;
					
				}
			//transform.Translate(Vector3.forward * Time.deltaTime * speed);
			//moveDirection = runFromTarget;
			//moveDirection = Vector3.MoveTowards(transform.position, runFromTarget, 10f);
			var runFromXZ = new Vector3 (runFromTarget.x, 0, runFromTarget.z);
			var targetRotation = Quaternion.LookRotation(runFromXZ, storeNormal); //set target towards direction of motion

			 foreach (Transform child in transform) {

			child.rotation = child.rotation.EaseTowards(targetRotation, .2f); //rotate towards the direction of motion

			}
			//myRotation = child.rotation;


				runFromTarget = transform.position - myHunter.transform.position;
				//runFromTarget -= myHunter.transform.position;

				if (runFromTarget.sqrMagnitude != 0) {

					targetPosition = runFromTarget;
					targetPosition.Normalize();
					speed += acceleration * Time.deltaTime;

					if (speed > maxSpeed) { 

						speed = maxSpeed;

					}


				}

		} else {


			speed -= acceleration * Time.deltaTime;

				if (speed < 0) {

					speed = 0;

				}

			//targetPosition = Vector3.zero;

			} 

			onDeath();
			moveDirection = Vector3.Lerp(transform.position, targetPosition, 1);
			moveDirection *= speed;

		}

			//If im on fire, turn on the fire particles, and trigger other fire behaviors
			if (fireOn == true) {

				imOnFire.enableEmission = true;
				StartCoroutine("putOutFire");
				fireOn = false;
				waitForNexFire = true;
		
			}

		//How to take damage while on fire
		if (burning = true && myStats.imDead == false) {

			myStats.HP -= 0.1f * Time.deltaTime;
			Debug.Log("bugHP: " + myStats.HP);

		}

		//Do the movement last, after all the other calculations are done
		moveDirection.y -= dragonMovement.gravity * Time.deltaTime;
		myController.Move(moveDirection * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other) {

		//if a hunter character is near, then get away from them STAT!
		var runForIt = other.gameObject.CompareTag("hunter");

		if (runForIt) {

			Debug.Log("Run AWAY!!!");
			runAway = true; 
			myHunter = other.GetComponent<Transform>();

		} 
	}

	void OnTriggerExit(Collider other) {

		//If the hunter is not near, it's safe to resume normal behavior.
		var itsSafeNow = other.gameObject.CompareTag("hunter");

		if (itsSafeNow) {

			runAway = false;

		}
	}
	
	void OnParticleCollision(GameObject other) {

		//If im hit with fire particles, i am on fire
		var fire = other.gameObject.CompareTag("fire");

		if (fire && waitForNexFire == false) {

			Debug.Log("OH GOD IM ON FIRE HELP ME PLEASE OH GOD");
			fireOn = true;
			burning = true;

		}
	}

	IEnumerator putOutFire () {

		yield return new WaitForSeconds(3);
		imOnFire.enableEmission = false;
		waitForNexFire = false;
		burning = false;

	}

	void onDeath () {

		//TODO: create an enum for types of attacks, so the creature knows what it does from and unique behavior can used for each instance
		//attack type enum can be passed into this function to determine what to do

		if (myStats.imDead == true) {

			maxSpeed = 0;
			myChild.renderer.material = crispy;
	
		}
	}
}
