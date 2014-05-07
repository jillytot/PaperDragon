using UnityEngine;
using System.Collections;

public enum ControlType {
	Player,
	AI,
}

public enum Behaviors {
	Idle,
	Roam,
	RunAway,
	Chase,
	Panic,
}

[RequireComponent(typeof(creatureStats))]
public class basicBehavior: MonoBehaviour {

	//All these variables direclty effect physical behavior
	public float speed = 0;
	private float newSpeed; //For fixing diagonal movment under analogue controls 
	public float maxSpeed = 5;
	public float walkingSpeed = 2.5f;
	public float acceleration = 1;
	public float deccelration = 1;
	public float jumpSpeed = 1;
	public float maxJumpSpeed = 1;
	public float rotationSpeed = 0.2f;
	
	bool runAway = false;
	bool triggerRunAway = false;
	
	CharacterController myController;

	[Range(-1, 1)]
	public float Horizontal; //raw value for Horizontal axis
	[Range(-1, 1)]
	public float Vertical; //raw value for Vertical axis



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
	//public bool hopper;
	bool moveToTarget;

	float horMax = 1;
	float horMin = -1;
	float verMax = 1;
	float verMin = -1;

	bool triggerWalk = false;
	bool walking = false;
	
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

	void aiController () {
		//Temp code for testing------------------------------------------------------------
		//Horizontal = 0.5f;
		//Vertical = 0.5f;
		//-----------------------------------------------------------------------------------------

		//ClampValues for input
		if (Horizontal >= horMax) { Horizontal = horMax;}
		if (Horizontal <= horMin) {Horizontal = horMin;}
		if (Vertical >= verMax) {Vertical = verMax;}
		if (Vertical <= verMin) {Vertical = verMin;}
		//TODO: Vertical and Horizontal 0 to upper or lower value should be a % of max speed, and acceleration should work based off of this... mabye
		moveDirection = new Vector3(Horizontal, 0, Vertical);
		newSpeed = speedMod();
		moveDirection *= newSpeed;
		rotateChildObject(moveDirection);
	}

	float speedMod () {
		
		//get the absolute value of each axis
		var horAbs = Mathf.Abs(Horizontal);
		var vertAbs = Mathf.Abs(Vertical);
		float angle = 0;
		//do some math...
		if (horAbs > vertAbs) { 
			angle = Mathf.Atan2(vertAbs, horAbs); 
		} else {
			angle = Mathf.Atan2(horAbs, vertAbs);
		}
		newSpeed = Mathf.Cos(angle);
		newSpeed *= speed;
		//It magically works!
		return newSpeed;
	}

	void randomWalk () {

		if (triggerWalk == false && walking == false && speed == 0) {
			Vertical = Random.Range(-1.0f, 1.0f);
			Horizontal = Random.Range(-1.0f, 1.0f);
			triggerWalk = true;
			walking = true;
			StartCoroutine("walkCycle");
		}
		speedControl();
		aiController();
	}

	IEnumerator walkCycle () {
		yield return new WaitForSeconds(Random.Range(0.5f, 3.5f));
		walking = false;
		yield return new WaitForSeconds(Random.Range(0.5f, 3.5f));
		triggerWalk = false;
	}
	

	// Update is called once per frame
	void Update () {

		switch (myStats.thisCreature) {
		case (CreatureType.bug):
			Debug.Log("I am just a little old bug");
			//speedControl();

			//current active behavior
			if (runAway == true) {
				runAwayBehavior();
			} else { randomWalk();}

			//passive behaviors
			onFireBehavior();
			break;

		case (CreatureType.hoppington):
			Debug.Log ("I am Mr. Hoppington, it's nice to meet you");
			speedControl();
			hopping();
			onFireBehavior();
			break;

		default:
			Debug.Log("There is no assigned creature behavior... watchoo gonna do?");
			break;
		}

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
			triggerRunAway = true;
			myHunter = other.GetComponent<Transform>();
		} 
	}
	
	void OnTriggerExit(Collider other) {	
		//If the hunter is not near, it's safe to resume normal behavior.
		var itsSafeNow = other.gameObject.CompareTag("hunter");
		if (itsSafeNow) {
			triggerRunAway = false;
		} 
		StartCoroutine("keepRunning");
	}

	IEnumerator keepRunning () {
		yield return new WaitForSeconds(1);
		if (triggerRunAway == false) {
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
		Debug.Log("I should be hopping");
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
		} else if (walking == true) {
			speed += acceleration * Time.deltaTime;
			if (speed > walkingSpeed) {
				speed = walkingSpeed;
			}
		} else {
			speed -= deccelration * Time.deltaTime;
			if (speed < 0) {
				speed = 0;
			}
		}
	}

		void speedUp () {
			speed += acceleration * Time.deltaTime;
			if (speed > maxSpeed) { 
				speed = maxSpeed;	
			}
		}
	void slowDown () {
		speed -= deccelration * Time.deltaTime;
		if (speed <= 0) {
			speed  = 0;
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

	void sensing () {

	}
}
