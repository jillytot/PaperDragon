using UnityEngine;

using System.Collections;

public class dragonMovement : MonoBehaviour {

	public creatureStats mystats;
	creatureStats preyStats;
	public GameObject myWingController;
	wingController myWingPos;
	
	public float speed = 15.0F; //Max speed of the character
	float newSpeed; //Used to modify speed based on input
	
	public float jumpSpeed = 30.0F; //How high you can jump in relation to gravity
	public static float gravity = 40.0F; //how fast you fall
	public float turnSpeed = 0.2f; //how fast you the player turns

	bool gliding;
	public float liftRatio = 1; //at 0.01, gravity will be 99% effective, at 0.99, gravity will only be 1% effective

	Quaternion myRotation; //used to store direction of movement
	float Horizontal; //raw value for Horizontal axis
	float Vertical; //raw value for Vertical axis
	
	public Vector3 moveDirection = Vector3.zero; //initialize movement direction
	private Vector3 inputMagnitude; //store axis input
	private Vector3 lastMoveDirection; //record last movement.
	public Vector3 playerPos;

	CharacterController controller; //create instance of character controller

	//making it easier to edit controls later
	string myHorizontal = "Horizontal";
	string myVertical = "Vertical";
	string myFire1 = "Fire1";
	string myFire2 = "Fire2";
	string myFire3 = "Fire3";
	string myJump = "Jump";

	//used for determining pitch / yaw of dragon as it travels over terrain
	Vector3 storeNormal;

	//variables for controlling fire breath
	public GameObject fireBreath;
	public ParticleSystem dragonBreath;
	public ParticleSystem nomNom;
	float breathAngleMin = 0;
	float breathAngleMax = 35;
	float breathAngleCur;
	public bool fireOn = false;

	public GameObject myHead;
	GameObject thingInMyMouth;
	public Collider dragonHead;
	bool mouthIsFull;

	bool triggerMouthIsFull;
	Vector3 mouthOffset;
	int chewCount = 3;
	bool triggerFireDelay;

	void Awake() {
		controller = GetComponent<CharacterController>();
		myRotation = transform.rotation;
		dragonBreath.enableEmission = false;
		nomNom.enableEmission = false;
		mouthIsFull = false;
		fireOn = false;
		triggerFireDelay = false;	
		myWingPos = myWingController.GetComponent<wingController>();
	}
	
	void Start() {
		//trying to control the wideness and speed of the dragon breath programmatically
		breathAngleCur = (breathAngleMin + breathAngleMax) / 2;
		Debug.Log("Breath Angle: " + breathAngleCur);
	}
	
	void Update() {
		
		//Get axis values for calculating movement
		Horizontal = Input.GetAxis(myHorizontal);
		Vertical = Input.GetAxis(myVertical);
		inputMagnitude =  new Vector3(Horizontal, 0, Vertical);
			if (inputMagnitude.sqrMagnitude != 0.0f) {
				lastMoveDirection = inputMagnitude;
			} 
			//Modifies speed based on axis input
			newSpeed = speedMod();
			//Ground Based Movement;
			if (controller.isGrounded) {
				gliding = false;
				//Get axis inputs and * by speed
				moveDirection = new Vector3(Horizontal, 0, Vertical);
				moveDirection *= newSpeed;
				RaycastHit hit;
				Physics.Raycast(transform.position, Vector3.down, out hit);
				Debug.DrawRay(transform.position, Vector3.up, Color.blue, 2);
				if (Physics.Raycast(transform.position, Vector3.down, 2)) {
					storeNormal = hit.normal;
					//Debug.Log("Normal hit: " + storeNormal);
					Debug.DrawRay(transform.position, storeNormal, Color.green, 2);
				}
				//Apply changes to each child of the game object
				foreach (Transform child in transform) {
					if (moveDirection.sqrMagnitude > 0) { 
						//myAnimation.SetBool("Run", true); //Changes avatar to running state
						var targetRotation = Quaternion.LookRotation(moveDirection, storeNormal); //set target towards direction of motion
						child.rotation = child.rotation.EaseTowards(targetRotation, turnSpeed); //rotate towards the direction of motion
						myRotation = child.rotation;
					}  else {
						//myAnimation.SetBool ("Run", false); 
					}
				}
				//How to jump!
				if (Input.GetButtonDown(myJump)) {
					moveDirection.y = jumpSpeed;
				}
				//Air based movement
			} else {
			if (Input.GetButtonDown(myJump) && gliding == false) {
				gliding = true;
			} else if (Input.GetButtonUp(myJump)) {
				gliding = false;
			}
				//Air Control
				//TODO: This is messy, it needs to be cleaned up.
				moveDirection.x = Horizontal;
				moveDirection.z = Vertical;
				Vector3 normalizeXZ = new Vector3(moveDirection.x, moveDirection.y, moveDirection.z);
				normalizeXZ *= newSpeed;
				Vector3 moveInAirDirection = new Vector3(normalizeXZ.x, moveDirection.y, normalizeXZ.z);
				moveDirection = transform.TransformDirection(moveInAirDirection);
				foreach (Transform child in transform) {
					if (moveDirection.sqrMagnitude > 0.5f) { 
						//myAnimation.SetBool("Run", true); //Changes avatar to running state
						Vector3 lookatMoveDirection = new Vector3(	moveDirection.x, 0, moveDirection.z);
						if (inputMagnitude.sqrMagnitude > 0.5f) {
							var targetRotation = Quaternion.LookRotation(lookatMoveDirection); //set target towards direction of motion
							child.rotation = child.rotation.EaseTowards(targetRotation, turnSpeed); //rotate towards the direction of motion
							myRotation = child.rotation;
						}
					}  else {
						//myAnimation.SetBool ("Run", false); 
					}
				}
			}
		breathControl();
		nomControl();
		//Controls Gravity
		glideControl();
		//move the player at the end of Update
		controller.Move(moveDirection * Time.deltaTime);
		playerPos = this.gameObject.transform.position;
		if (mouthIsFull == true) {
			thingInMyMouth.transform.position = myHead.transform.position;
			thingInMyMouth.transform.rotation = myHead.transform.rotation;
		}
	}

	void glideControl () {
		if (gliding == true) {
			myWingPos.wingPositions = WingPositions.glide;
			var adjustForGlide = gravity * liftRatio;
			moveDirection.y -= gravity * Time.deltaTime;
			moveDirection.y += adjustForGlide * Time.deltaTime;
		} else {
			myWingPos.wingPositions = WingPositions.defaultPos;
			moveDirection.y -= gravity * Time.deltaTime;
			Debug.Log("I'm Not Gliding!");
		}
	}

	void breathControl () {
		if (Input.GetButtonDown(myFire1) && mystats.fireStore > 0) {
			//fireBreath.SetActive(true);
			dragonBreath.enableEmission = true;
			fireOn = true;
			triggerFireDelay = false;
		} 
		if (Input.GetButtonUp(myFire1) || mystats.fireStore <= 0) {
				dragonBreath.enableEmission = false;
				fireOn = false;
			if (triggerFireDelay == false) {
				mystats.delayFireRefresh();
				triggerFireDelay = true;
			}
		}
		if (fireOn == true) {
			mystats.fireStore -= 10 * Time.deltaTime;
		}
	}

	void nomControl () {
		if (Input.GetButtonDown(myFire2) && mouthIsFull == true) {
			//fireBreath.SetActive(true);
			nomNom.enableEmission = true;
			nomNom.Play();
			chewCount -= 1;
		} 

		if (chewCount <= 0) {
			chewCount = 3;
			//If you eat the creature, add its calorie value to your calorie count
			mystats.calorieStore += preyStats.calorieValue;
			Destroy(thingInMyMouth);
			mouthIsFull = false;
		}
	}
	
	//Fix Unity's character controller!
	//This controls the speed while using a radial axis analogue stick so that it's constant in any degree at any velocity
	//With this method you don't need to normalize your movement vectors
	
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

	void OnTriggerEnter(Collider other) {
		var otherCreature = other.GetComponent<creatureStats>();
		if (otherCreature) {
			if (otherCreature.imDead == true && mouthIsFull == false) {
				thingInMyMouth = otherCreature.gameObject;
				mouthIsFull = true;
				thingInMyMouth.gameObject.GetComponent<CharacterController>().detectCollisions = false;
				preyStats = thingInMyMouth.gameObject.GetComponent<creatureStats>();
				preyStats.inMouth = true;
			}
		}
	}
}