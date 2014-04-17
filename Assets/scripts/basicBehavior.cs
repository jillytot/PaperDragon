using UnityEngine;
using System.Collections;

[RequireComponent(typeof(creatureStats))]

public class basicBehavior: MonoBehaviour {

	//variables for controlling creature speed
	public float speed = 0;
	public float maxSpeed = 5;
	public float acceleration = 1;
	public float deccelration = 1;

	//variables for controlling jumping
	public float jumpSpeed = 0;
	public float maxJumpSpeed = 5;
	public float jumpAccel = 50;
	public float jumpDeccel = 50;

	//moving will trigger further update behavior
	bool groundMovement = false;

	//running away will trigger specific behavior for fleeing
	bool runAway = false;

	//character controller is in charge of the character
	CharacterController myController;
	public Vector3 moveDirection = Vector3.zero; //initialize movement direction
	public Vector3 targetDestination;

	//for basic running away
	Vector3 runFromTarget;
	Transform myHunter;
	Vector3 storeNormal;
	Vector3 targetPosition;

	//variables for controlling getting burned
	public ParticleSystem imOnFire;
	bool fireOn;
	bool waitForNexFire;
	bool burning;
	public Material crispy; //Material for being burned to a crisp

	//creature stats are required for every creature
	internal creatureStats myStats;

	Material storeMat; //store my original material
	Transform myChild; //access transform of the child (All the graphics are on the child object, and not this main object

	public bool iCanHop;
	
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
		groundMovement = false;
		
		foreach (Transform child in transform) {
			
			myChild = child;
			
		}
		
		storeMat = myChild.renderer.material;
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (myController.isGrounded) {
			
			moveOnGround ();

			hopping();

			onDeath();
			
			if (myStats.imDead == false) {



				moveDirection = Vector3.Lerp(transform.position, targetPosition, 1);
				moveDirection *= speed;



			}
		}
		
		
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
		
		if (myStats.imDead == false) {

			//hopping();
			
			//Do the movement last, after all the other calculations are done
			moveDirection.y -= dragonMovement.gravity * Time.deltaTime;
			myController.Move(moveDirection * Time.deltaTime);
			
		} 
	}
	
	void OnTriggerEnter(Collider other) {
		
		//if a hunter character is near, then get away from them STAT!
		var runForIt = other.gameObject.CompareTag("hunter");
		
		if (runForIt) {
			
			Debug.Log("Run AWAY!!!");
			runAway = true; 
			groundMovement = true;
			myHunter = other.GetComponent<Transform>();
			
		} 
	}
	
	void OnTriggerExit(Collider other) {
		
		//If the hunter is not near, it's safe to resume normal behavior.
		var itsSafeNow = other.gameObject.CompareTag("hunter");
		
		if (itsSafeNow) {
			
			runAway = false;
			groundMovement = false;
			
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

	void basicRunAway () {

		runFromTarget = transform.position - myHunter.transform.position;
		
		if (runFromTarget.sqrMagnitude != 0) {
			
			targetPosition = runFromTarget;
			targetPosition.Normalize();
			speed += acceleration * Time.deltaTime;
			
			if (speed > maxSpeed) { 
				
				speed = maxSpeed;
				
			}
		}
	}

	void hopping () {

		if (iCanHop) {

		// computing the vertical velocity to make sure it stays in the air long enough to hit the target
		float timeOfFlight = Vector3.Distance(targetPosition, transform.position) / speed;
		moveDirection.y = 0.5f * dragonMovement.gravity * timeOfFlight + (targetPosition.y - transform.position.y)/timeOfFlight;

			}
		}
	

	void moveOnGround () {

		if (groundMovement == true) {
			
			RaycastHit hit;
			Physics.Raycast(transform.position, Vector3.down, out hit);
			Debug.DrawRay(transform.position, Vector3.down, Color.blue, 2);
			
			if (Physics.Raycast(transform.position, Vector3.down, 2)) {
				
				storeNormal = hit.normal;
				
			}
			
			if (runAway == true) {
				
				targetDestination = new Vector3 (runFromTarget.x, 0, runFromTarget.z);
				
			} else { 
				
				targetDestination = Vector3.zero;
				
			}
			
			var targetRotation = Quaternion.LookRotation(targetDestination, storeNormal); //set target towards direction of motion
			
			//don't rotate me if i am dead
			if (myStats.imDead == false) {
				
				foreach (Transform child in transform) {
					
					child.rotation = child.rotation.EaseTowards(targetRotation, .2f); //rotate towards the direction of motion
					
				}
			}
			
			if (runAway == true) {
				
				basicRunAway();
				
			}
			
		} else {
			
			
			speed -= acceleration * Time.deltaTime;
			
			if (speed < 0) {
				
				speed = 0;
			}
		}
	}
}
