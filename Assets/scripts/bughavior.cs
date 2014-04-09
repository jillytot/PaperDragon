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

	// Use this for initialization
	void Start () {

		myController = GetComponent<CharacterController>();
		runAway = false;
		runFromTarget = Vector3.zero;
		targetPosition = Vector3.zero;
		fireOn = false;
		imOnFire.enableEmission = false;
	
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

			//moveDirection = Vector3.MoveTowards(transform.position, targetPosition, speed);
			moveDirection = Vector3.Lerp(transform.position, targetPosition, 1);
			//var smoothMove = Vector3.SmoothDamp(transform.position, moveDirection, ref targetPosition, 0);
			//moveDirection = smoothMove;
			moveDirection *= speed;

		}

			if (fireOn == true) {

				imOnFire.enableEmission = true;
		
			}

		moveDirection.y -= dragonMovement.gravity * Time.deltaTime;
		myController.Move(moveDirection * Time.deltaTime);
	}

	void OnTriggerEnter(Collider other) {

		var runForIt = other.gameObject.CompareTag("hunter");

		if (runForIt) {

			Debug.Log("Run AWAY!!!");
			runAway = true; 
			myHunter = other.GetComponent<Transform>();




		} 
	}

	void OnTriggerExit(Collider other) {

		var itsSafeNow = other.gameObject.CompareTag("hunter");

		if (itsSafeNow) {

			runAway = false;

		}
	}

	void OnParticleCollision(GameObject other) {

		var fire = other.gameObject.CompareTag("fire");

		if (fire) {

			Debug.Log("OH GOD IM ON FIRE HELP ME PLEASE OH GOD");
			fireOn = true;

		}
	}
}
