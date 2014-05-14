using UnityEngine;
using System.Collections;

public class bodySegment : MonoBehaviour {

	public GameObject leader;
	public GameObject follower;

	Transform leaderTransform;
	Transform followerTransform;

	Vector3 frontPos;
	Quaternion frontRot;
	Vector3 backPos;
	Quaternion backRot;
	Vector3 curPos;
	Quaternion curRot;

	basicBehavior head;

	public bool hinge;
	bool tail;
	bool followHead;

	// Use this for initialization
	void Start () {
		if (follower == null) {
			tail = true;
		}

		curPos = transform.position;
		curRot = transform.rotation;

		if (leader.gameObject.GetComponent<basicBehavior>()) {
			followHead = true;
			head = leader.gameObject.GetComponent<basicBehavior>();

		} else {
			leaderTransform = leader.gameObject.GetComponent<Transform>();
		}

		if (tail == true) { } else {
			followerTransform = follower.gameObject.GetComponent<Transform>();
		}
	}
	
	// Update is called once per frame
	void Update () {

		if (followHead == true) {

			frontRot = head.myRot;
			frontPos = head.transform.localPosition;
		} else if (hinge == false) {
			frontPos = leaderTransform.transform.localPosition;
			frontRot = leaderTransform.transform.localRotation;
		} else {
			frontPos = leaderTransform.transform.position;
			frontRot = leaderTransform.transform.rotation;
		}  	if (tail == true) { 

			float x = (frontPos.x + curPos.x);
			float z = (frontPos.z + curPos.z);
			curPos = new Vector3(x, curPos.y, z);
			//transform.position = curPos;
			transform.position = Vector3.MoveTowards(transform.position, frontPos, 1.0f);

		} else {

			backPos = followerTransform.transform.position;
			backRot = followerTransform.transform.rotation;

			float x = (frontPos.x + backPos.x) /2;
			float z = (frontPos.z + backPos.z) / 2;

			curPos = new Vector3(x, curPos.y, z);

			if (hinge = false) {
			transform.localPosition = curPos;
			} else { 
				transform.position = curPos;
			}
		}
	}
}
