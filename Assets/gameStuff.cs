using UnityEngine;
using System.Collections;

public class gameStuff : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.Tab)) {

			Time.timeScale = 0.1f;

		} else {

			Time.timeScale = 1f;

		}
	}
}
