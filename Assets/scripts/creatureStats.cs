using UnityEngine;
using System.Collections;

public enum CreatureType {
	notCreature,
	dragon,
	bug,
	hoppington,
	velvetdear
}

//This script will eventually manage stats for all creatures in the game
public class creatureStats : MonoBehaviour {

	//Wait just a little bit after an action is done before begining to restore a variable;
	public float delayRestore = 5.0f;
	float fireTimer;
	public bool delayFire = false;

	GameObject creatureObject;
	dragonMovement playerDragon;
	bool fireOn;

	public float myScale = 1; //scale relative to player, used to calculate some stats
	public float HP; //current HP, when HP reaches 0, you die
	public float maxHP; //Maximum HP available

	public float stamina; //stamina is used to do actions
	public int maxStamina; //maximum stamina for creature
	public float staminaRestoreRate; //The rate at which stamina restores itself

	public int hunger; //When you are hungry, your stats will restore slower, and you may start taking damage (need to think on this more)
	public int strength; //Used to calculate attack strength among other things

	public float fireRestoreRate; //rate at which dragon fire restores it'self
	public float fireStore; //amount of fire available to use
	public float maxFireStore; //Maximum fire reserve this creature can have

	public float calorieStore; //current calaories the player has for use
	public float calorieBurnRate; //the rate at which calories burn

	public float calorieValue; //This is how many calories this creature provides when consumed

	public CreatureType thisCreature; //Determines what kind of creature this is

	public bool imDead; //returns true if dead
	public bool inMouth; //returns true of the creature has an object in it's mouth

	public float senseRadius =1; //The area around in the creature in which it can sense it's surroundings

	bool delayStamina; //delay the restoration of stamina
	public float caloriesToStamina; //how many calories are burned to restore stamina
	//float storeStaminaRestoreRate;
	public float staminaBurnFlapping = 25; //amount of stamina used for flapping wings
	
	void Start () {

		fireTimer = Time.time -1;
		delayFire = false;
		delayStamina = false;
		//initialize dragon specific stats
		if (thisCreature == CreatureType.dragon) {
			Debug.Log("I am a dragon!");
		}
		creatureObject = this.gameObject;
		if (creatureObject.GetComponent<dragonMovement>() != null) {
			playerDragon = this.gameObject.GetComponent<dragonMovement>();
		}
		maxHP *= myScale;
	
		HP = maxHP;
		imDead = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {

		//clamp HP baseline to 0;
		if (HP < 0) {
			HP = 0;
			imDead = true;
		} 
		if (thisCreature == CreatureType.dragon) {
			//special behavior just for dragons
			dragonVariables();
			creatureVariables();
		}
	}	
	//If i am a dragon, do this shit! 
	void dragonVariables () {
		calorieStore -= calorieBurnRate * Time.deltaTime;
		if (Time.time < fireTimer) {
			delayFire = true;
		} else { 
			delayFire = false; 
		}
		if (calorieStore > 0 && fireStore < maxFireStore && playerDragon.fireOn == false && delayFire == false) {
			//Debug.Log("Derp Test");
			fireStore += fireRestoreRate * Time.deltaTime;
			calorieStore -= calorieBurnRate * Time.deltaTime;
		}
		//clamp all values
		if (calorieStore <= 0) {
			calorieStore = 0;
		}
		if (fireStore <= 0) { fireStore = 0; }
		if (fireStore >= maxFireStore) { fireStore = maxFireStore;}
	}

	public void creatureVariables () {
		if (calorieStore > 0 && stamina < maxStamina && delayStamina == false) {
			stamina += staminaRestoreRate * Time.deltaTime;
			calorieStore -= caloriesToStamina * Time.deltaTime;
		} else if (calorieStore <= 0 && stamina < maxStamina && delayStamina == false) {
			stamina += (staminaRestoreRate / 4) * Time.deltaTime;
		}
		//clampStamina
		if (stamina <= 0) {stamina = 0;}
		if (stamina >= maxStamina) {stamina = maxStamina;}
	}

	public void delayFireRefresh () {
		fireTimer = Time.time + delayRestore;
	}
}
