using UnityEngine;
using System.Collections;

[RequireComponent(typeof(creatureStats))]

public class basicBehavior: MonoBehaviour {

	//All these variables direclty effect physical behavior
	public float speed = 0;
	public float maxSpeed = 5;
	public float acceleration = 1;
	public float deccelration = 1;
	public float jumpSpeed = 1;
	public float maxJumpSpeed = 1;
	public float rotationSpeed = 0.2f;
	
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
	
	internal creatureStats myStats;
	public Material crispy;
	Material storeMat;
	Transform myChild;

	public Transform player;
	public bool hopper;
	bool moveToTarget;
	
	// Use this for initialization
	void Start () {
		
		//myStats = new creatureStats();
		myStats = this.gameObject.GetComponent<creatureStats>();
		myController = GetComponent<CharacterController>();
		runAway = false;
		runFromTarget = Vector3.zero;
		targetPosition = Vector3.zero;
		fireOn = false;
		imOnFire.enableEmission = false;
		waitForNexFire = false;
		burning = false;
		moveToTarget = false;
		
		foreach (Transform child in transform) {		
			myChild = child;
		}
		storeMat = myChild.renderer.material;
	}
	
	// Update is called once per frame
	void Update () {

		//runAwayBehavior();
		speedControl();
		hopping();
		onFireBehavior();

		if (myStats.imDead == false) {
			//Do the movement last, after all the other calculations are done
			moveDirection.y -= dragonMovement.gravity * Time.deltaTime;
			myController.Move(moveDirection * Time.deltaTime);
		}  else if (myStats.inMouth == false) {
			moveDirection = Vector3.zero;
			moveDirection.y -= dragonMovement.gravity * Time.deltaTime;
			myController.Move(moveDirection * Time.deltaTime);
		}
		onDeath();
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

	void onFireBehavior () {

		//If im on fire, turn on the fire particles, and trigger other fire behaviors
		if (fireOn == true) {
			imOnFire.enableEmission = true;
			StartCoroutine("putOutFire");
			fireOn = false;
			waitForNexFire = true;
		}
		
		//How to take damage while on fire
		if (burning == true && myStats.imDead == false) {
			myStats.HP -= 0.25f * Time.deltaTime;
			//Debug.Log("bugHP: " + myStats.HP);
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

	void runAwayBehavior() {

		if (myController.isGrounded) {
			
			if (runAway == true) {	
				rotateChildObject(runFromTarget);	
				runFromTarget = transform.position - myHunter.transform.position;
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
			}
			
			//onDeath();
			if (myStats.imDead == false) {
				moveDirection = Vector3.Lerp(transform.position, targetPosition, 1);
				moveDirection *= speed;
			}
		}
	}

	void hopping () {
		jumpSpeed = speed * 2;
	if (jumpSpeed > maxJumpSpeed) {
			jumpSpeed = maxJumpSpeed;
		} 
		var myTarget = player.transform.position - transform.position;
		var offsetToTarget = transform.position - player.transform.position;
		//TODO:Move target selection to it's own function
		if (offsetToTarget.sqrMagnitude < myStats.senseRadius * myStats.senseRadius) {
			moveToTarget = true;
			Debug.Log ("Target is close");
		} else { 
			moveToTarget = false;
			Debug.Log ("Target is far");
		}
			if (myController.isGrounded) {
				rotateChildObject(myTarget);
				targetPosition = myTarget;
				if (myStats.imDead == false) {
//					var lerpXZ = new Vector3(targetPosition.x, 0, targetPosition.z);
//					var tooDeeLerp = new Vector3(transform.position.x, 0, transform.position.z);
//					var lerpThatWay = Vector3.Lerp(tooDeeLerp, lerpXZ, speed);
					var hopThisWay = myTarget.normalized;
					moveDirection = hopThisWay;
					moveDirection *= speed;
					moveDirection.y = jumpSpeed;
			}
		}
		rotateChildObject(myTarget);
	}

	void speedControl() {
		//manages acceleration / decceleration 
			if (moveToTarget == true) {
				speed += acceleration * Time.deltaTime;
				if (speed > maxSpeed) { 
					speed = maxSpeed;	
				}
		} else {
			speed -= deccelration * Time.deltaTime;
			if (speed < 0) {
				speed = 0;
			}
		}
	}

	void rotateChildObject(Vector3 rotationTarget) { //rotates the child object
	
		if (myController.isGrounded) {

			RaycastHit hit;
			Physics.Raycast(transform.position, Vector3.down, out hit);
			Debug.DrawRay(transform.position, Vector3.down, Color.blue, 2);
			if (Physics.Raycast(transform.position, Vector3.down, 2)) {
				storeNormal = hit.normal;
			}
			var targetXZ = new Vector3 (rotationTarget.x, 0, rotationTarget.z);
			var targetRotation = Quaternion.LookRotation(targetXZ, storeNormal); //set target towards direction of motion
			if (myStats.imDead == false) { //don't rotate me if i am dead
				foreach (Transform child in transform) {
					child.rotation = child.rotation.EaseTowards(targetRotation, rotationSpeed); //rotate towards the direction of motion
				}
			}
		} else if (myStats.thisCreature == CreatureType.hoppington) {
			var targetXZ = new Vector3 (rotationTarget.x, 0, rotationTarget.z);
			var targetRotation = Quaternion.LookRotation(targetXZ); //set target towards direction of motion
			if (myStats.imDead == false) { //don't rotate me if i am dead
				foreach (Transform child in transform) {
					child.rotation = child.rotation.EaseTowards(targetRotation, .2f); //rotate towards the direction of motion
				}
			}
		}
	}

	Vector3 getMeATarget () {
		var targetPlayer = player.transform.position;
		return targetPlayer;
	}
}
