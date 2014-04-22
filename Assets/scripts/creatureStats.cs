using UnityEngine;
using System.Collections;

public enum CreatureType {
	notCreature,
	dragon,
	bug,
	hoppington,
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
	public bool inMouth;

	public float senseRadius =1;



	// Use this for initialization
	void Start () {

		fireTimer = Time.time -1;
		delayFire = false;
		//initialize dragon specific stats
		if (thisCreature == CreatureType.dragon) {
			Debug.Log("I am a dragon!");
		}
		creatureObject = this.gameObject;
		if (this.gameObject.GetComponent<dragonMovement>() != null) {
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
		Debug.DrawRay(transform.position, Vector3.forward, Color.red, 2);
		if (thisCreature == CreatureType.dragon) {
			//special behavior just for dragons
			dragonVariables();
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

	public void delayFireRefresh () {
		fireTimer = Time.time + delayRestore;
	}
}
