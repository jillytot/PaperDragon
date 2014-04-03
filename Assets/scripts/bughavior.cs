using UnityEngine;
using System.Collections;

public class bughavior : MonoBehaviour {

	public float speed = 1;
	bool runAway = false;

	// Use this for initialization
	void Start () {

		runAway = false;
	
	}
	
	// Update is called once per frame
	void Update () {

		if (runAway == true) {

			transform.Translate(Vector3.forward * Time.deltaTime * speed);

		}
	}

	void OnTriggerEnter(Collider other) {

		var runForIt = other.gameObject.CompareTag("hunter");

		if (runForIt) {

			Debug.Log("Run AWAY!!!");
			runAway = true; 

		} else { runAway = false;

		}
	}
}
