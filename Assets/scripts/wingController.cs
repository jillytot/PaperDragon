using UnityEngine;
using System.Collections;

public enum WingPositions {
	defaultPos,
	glide,
	flap,
}

public class wingController : MonoBehaviour {
	
	//Dragon Wings
	public GameObject leftWing;
	public GameObject rightWing;
	public WingPositions wingPositions;

	//Rotate objects along the following axis
	float x;
	float y;
	float z;
	
	//set speed for rotation
	public float rotationSpeed;
	//DefaultWingPositions
	
	void Update () {

		switch (wingPositions) {
		case WingPositions.defaultPos:
			x = -35; y = 0; z = 0;
			break;
		case WingPositions.glide:
			x = 0; y = 20; z = 50;
			break;
		case WingPositions.flap:
			x = 0; y = 0; z = 80;
			break;
		default:
			Debug.Log("There is no assigned wing position");
			break;
		}
		leftWing.transform.localRotation = flap (leftWing, rotationSpeed, x,y, z);
		rightWing.transform.localRotation = flap (rightWing, rotationSpeed, x,y * -1,z * -1);
	}

	Quaternion flap (GameObject thisWing, float flapSpeed, float x, float y, float z) {
		Quaternion newRot = Quaternion.Euler(x, y, z);
		var flapTo = Quaternion.Slerp(thisWing.transform.localRotation, newRot, flapSpeed * Time.fixedDeltaTime);
		return flapTo;
	}
}
