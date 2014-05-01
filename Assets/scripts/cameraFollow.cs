using UnityEngine;
using System.Collections;

public class cameraFollow : MonoBehaviour {

	public Transform myTarget;
	public Vector3 offset;
	public float altitude;
	public float cameraRange = 1;
	public float camSpeed = 1;
	 
	void Start () {
		offset = transform.position - myTarget.transform.position;
		altitude = transform.position.y;
	}

	void Update () {

		var trackTarget = new Vector3(myTarget.transform.position.x, myTarget.transform.position.y + altitude, myTarget.transform.position.z);
		//var offsetMagnitude = trackTarget + offset;
		transform.position = Vector3.Slerp(transform.position, trackTarget+ offset, camSpeed * Time.deltaTime);
		//if (offsetMagnitude.sqrMagnitude > offsetMagnitude +  cameraRange) <
	}
}
