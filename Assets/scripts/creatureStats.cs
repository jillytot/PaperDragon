using UnityEngine;
using System.Collections;

public enum CreatureType {

	notCreature,
	dragon,
	bug,

}

//This script will eventually manage stats for all creatures in the game
public class creatureStats : MonoBehaviour {

	public float HP; //current HP, when HP reaches 0, you die
	public int maxHP; //Maximum HP available

	public int stamina; //stamina is used to do actions
	public int maxStamina; //maximum stamina for creature
	public int staminaRestoreRate; //The rate at which stamina restores itself

	public int hunger; //When you are hungry, your stats will restore slower, and you may start taking damage (need to think on this more)
	public int strength; //Used to calculate attack strength among other things

	public float fireRestoreRate; //rate at which dragon fire restores it'self
	public float fireStore; //amount of fire available to use
	public float maxFireStore; //Maximum fire reserve this creature can have

	public float calorieStore; //current calaories the player has for use
	public float calorieBurnRate; //the rate at which calories burn

	public float calorieValue; //This is how many calories this creature provides when consumed

	public CreatureType thisCreature; //Determines what kind of creature this is

	public bool imDead;

	// Use this for initialization
	void Start () {

		//initialize dragon specific stats
		if (thisCreature == CreatureType.dragon) {
			
			Debug.Log("I am a dragon!");
			
		}

		imDead = false;
	}
	
	// Update is called once per frame
	void Update () {

		//clamp HP baseline to 0;
		if (HP < 0) {

			HP = 0;
			imDead = true;


		} 

		Debug.DrawRay(transform.position, Vector3.forward, Color.red, 2);

		if (thisCreature == CreatureType.dragon) {

			//special behavior just for dragons
			dragonVariables();
			
		}
	}	

	void dragonVariables () {


			calorieStore -= calorieBurnRate * Time.deltaTime;
			Debug.Log ("Calories: " + calorieStore);

	}
}
