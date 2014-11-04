using UnityEngine;
using System.Collections;

public class character_test_anim_behaviour : MonoBehaviour 
{
	public AudioClip footstepSound;
	public AudioClip stumbleSound;
	public float StumbleChanceOneIn;
	public float AngleMultiplier;

	Animator anim;

	void Start () 
	{
		anim = GetComponent<Animator> ();
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
		if (GetComponentInParent<CharacterController> ().isGrounded) {
						var audioSource = GetComponentInChildren<AudioSource> ();

						var angle = GetComponentInParent<CharacterController> ().velocity.y;
						Debug.Log (GetVerticalMovementAngle ());
						var grassChanceToStumble = 1.0f / (StumbleChanceOneIn / Mathf.Max (AngleMultiplier * Mathf.Abs (angle), 1));

						if (Random.Range (0.0f, 1.0f) < grassChanceToStumble) {
								audioSource.pitch = 1.0f;
								audioSource.volume = 1.0f;
								audioSource.PlayOneShot (stumbleSound);
								SendMessageUpwards("Die");
						} else {
								audioSource.pitch = 1.0f + Random.Range (-0.1f, 0.1f);
								audioSource.volume = 1.0f + Random.Range (-0.2f, 0.0f);
								audioSource.PlayOneShot (footstepSound);
						}
				}
	}

	public float GetVerticalMovementAngle()
	{
		var direction = GetComponentInParent<CharacterController> ().velocity.normalized;
		if (Mathf.Abs(direction.y) <= 0.0001)
		{
			return 0;
		}
		return Mathf.PI / 2 - Mathf.Acos (Vector3.Dot (Vector3.up, direction));
	}
}
