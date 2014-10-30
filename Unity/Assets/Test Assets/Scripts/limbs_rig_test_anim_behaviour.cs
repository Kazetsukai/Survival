using UnityEngine;
using System.Collections;

public class limbs_rig_test_anim_behaviour : MonoBehaviour 
{
	int walkingHash = Animator.StringToHash("Base Layer.run");

	Animator anim;
	void Start () {
		anim = GetComponent<Animator> ();
	}

	void Update () 
	{
		//Rough code to change animation state for debug legs. Makes them run if the player presses forward button.
		float move = Input.GetAxis ("Vertical");
		if (move != 0) 
		{
			anim.SetBool("run", true);
		}
		else
		{
			anim.SetBool("run", false);
		}
	}
}
