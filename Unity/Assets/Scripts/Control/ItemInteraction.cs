using UnityEngine;
using System.Collections;
using System.Linq;

public class ItemInteraction : MonoBehaviour {

	public float MaxInteractDistance = 3.0f;
	public float InteractBubbleSize = 0.8f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit pointHit;
		Ray ray = new Ray (transform.position, transform.forward);

		if (Physics.Raycast (ray, out pointHit, MaxInteractDistance)) {
			
			var detectedObjects = Physics.OverlapSphere(pointHit.point, InteractBubbleSize);

			var interactableObjects = detectedObjects.Where(o => o.GetComponents<MonoBehaviour>().Any(m => m is IInteraction));

			foreach (var obj in interactableObjects)
			{
				obj.renderer.material.color = Color.yellow;
			}
		}
	}

	void OnDrawGizmos()
	{
		RaycastHit pointHit;
		Ray ray = new Ray (transform.position, transform.forward);
		Gizmos.DrawRay (ray);

		if (Physics.Raycast (ray, out pointHit, MaxInteractDistance)) {
			
			var detectedObjects = Physics.OverlapSphere(pointHit.point, InteractBubbleSize);
			Gizmos.DrawWireSphere (pointHit.point, InteractBubbleSize);
			
		}

	}
}
