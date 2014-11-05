using UnityEngine;
using System.Collections;

public class character_test_anim_behaviour : MonoBehaviour 
{
	public AudioClip footstepSound;
	public AudioClip stumbleSound;
	public float StumbleChanceOneIn;
	public float AngleMultiplier;
	CustomCharacterController cc;

	Animator anim;

	void Start () 
	{
		anim = GetComponent<Animator> ();
		cc = GetComponentInParent<CustomCharacterController> ();
	}

	void Update () 
	{
		//Rough code to change animation state for debug legs. Makes them run if the player presses forward button.
		bool move = Input.GetAxis ("Vertical") != 0 || Input.GetAxis ("Horizontal") != 0;
		float jump = Input.GetAxis ("Jump");

		if (move) 
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
		if (GetComponentInParent<CustomCharacterController> ().IsGrounded()) {
			var audioSource = GetComponentInChildren<AudioSource> ();

			if (Random.Range (0.0f, 1.0f) < 1 / cc.stumbleOneInBase) {
				audioSource.pitch = 1.0f;
				audioSource.volume = 1.0f;
				audioSource.PlayOneShot (stumbleSound);
				cc.isStumbling = true;
			} else {
				audioSource.pitch = 1.0f + Random.Range (-0.1f, 0.1f);
				audioSource.volume = 1.0f + Random.Range (-0.2f, 0.0f);
				audioSource.PlayOneShot (footstepSound);
				cc.isStumbling = false;
			}
		}
	}
}