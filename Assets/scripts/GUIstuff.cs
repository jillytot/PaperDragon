using UnityEngine;
using System.Collections;

public class GUIstuff : MonoBehaviour {

	public TextMesh HPcount;
	public TextMesh Stamina;
	public TextMesh FireCount;
	public TextMesh Hunger;
	public TextMesh Calories;

	public GameObject trackPlayer;
	creatureStats player;

	// Use this for initialization
	void Start () {

		player = trackPlayer.GetComponent<creatureStats>();
	
	}
	
	// Update is called once per frame
	void Update () {

		HPcount.text = player.HP + "/" + player.maxHP;
		Stamina.text = player.stamina.ToString();
		FireCount.text = player.fireStore.ToString();
		Hunger.text = player.hunger.ToString();
 		Calories.text= player.calorieStore.ToString();
	
	}
}
