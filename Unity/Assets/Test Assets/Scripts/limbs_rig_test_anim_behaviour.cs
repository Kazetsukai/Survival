using UnityEngine;
using System.Collections;

public class limbs_rig_test_anim_behaviour : MonoBehaviour 
{
	public AudioClip footstepSound;

	Animator anim;
	void Start () 
	{
		anim = GetComponent<Animator> ();
	}

	void Update () 
	{
		//Rough code to change animation state for debug legs. Makes them run if the player presses forward button.
		float move = Input.GetAxis ("Vertical");
		float jump = Input.GetAxis ("Jump");

		if (move != 0) 
		{
			anim.SetBool("run", true);
		}
		else
		{
			anim.SetBool("run", false);
		}
	
		if (jump != 0) 
		{
			anim.SetBool("jump", true);
		}
		else
		{
			anim.SetBool("jump", false);
		}
	}

	public void PlayFootstep()
	{
		var audioSource = GetComponentInChildren<AudioSource> ();
		audioSource.pitch = 1.0f + Random.Range (-0.1f, 0.1f);
		audioSource.volume = 1.0f + Random.Range (-0.2f, 0.0f);
		audioSource.PlayOneShot(footstepSound);
	}
}
