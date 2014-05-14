using UnityEngine;
using System.Collections;

public class gameStuff : MonoBehaviour {

	//creatureStats playerStats;
	//public static GameObject thisPlayer;
	//Vector3 initialScale;

	//public GameObject caliorieBar;

	// Use this for initialization
	void Start () {

		//playerStats = thisPlayer.gameObject.GetComponent<creatureStats>();
		//initialScale = new Vector3(1,1,1);
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey(KeyCode.Tab)) {

			Time.timeScale = 0.1f;

		} else {

			Time.timeScale = 1f;

		}

//		var calorieStore = playerStats.calorieStore;
//		var rescale = initialScale.x * calorieStore * 0.01f;
//		caliorieBar.transform.localScale = new Vector3(rescale, initialScale.y, initialScale.z);
	}




}
