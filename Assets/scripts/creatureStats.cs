using UnityEngine;
using System.Collections;

public enum CreatureType {

	notCreature,
	dragon,
	bug,

}

//This script will eventually manage stats for all creatures in the game
public class creatureStats : MonoBehaviour {

	public int HP;
	public int maxHP;

	public int stamina;
	public int maxStamina;

	public int hunger;
	public int strength;

	public int fireStore;
	public int calorieStore;
	public int calorieCount;

	public CreatureType thisCreature;
	public static float timeTick = 0.25f;
	float storeTick;

	// Use this for initialization
	void Start () {

		Debug.Log("TEST");

		HP = maxHP;
		storeTick = timeTick;

		//initialize dragon specific stats
		if (thisCreature == CreatureType.dragon) {
			
			Debug.Log("I am a dragon!");
			
		}
	}
	
	// Update is called once per frame
	void Update () {

		Debug.DrawRay(transform.position, Vector3.forward, Color.red, 2);

		if (timeTick > 0) {

			timeTick -= Time.deltaTime;

		} 

		if (timeTick <= 0) {

			Debug.Log("Tick Tock!");
			timeTick = storeTick;

		}
	}	
}
