using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ItemInteraction : MonoBehaviour {

	public float MaxInteractDistance = 3.0f;
	public float InteractBubbleSize = 0.8f;
    public Material SelectionMaterial;
    public GameObject SelectedObject;

    private GameObject _selectionObject;

	// Use this for initialization
	void Start () {
        _selectionObject = new GameObject();
        _selectionObject.AddComponent<MeshRenderer>();
        _selectionObject.AddComponent<MeshFilter>();
        _selectionObject.renderer.material = SelectionMaterial;
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit pointHit;
		Ray ray = new Ray (transform.position, transform.forward);

        Collider objectToSelect = null;

		if (Physics.Raycast (ray, out pointHit, MaxInteractDistance)) {
            if (pointHit.collider.gameObject.GetComponents<MonoBehaviour>().Any(m => m is IInteraction))
                objectToSelect = pointHit.collider;
            else
            {
                var detectedObjects = Physics.OverlapSphere(pointHit.point, InteractBubbleSize);
                var interactableObjects = detectedObjects.Where(o => o.GetComponents<MonoBehaviour>().Any(m => m is IInteraction));
                objectToSelect = interactableObjects.OrderBy(c => (pointHit.point - c.transform.position).magnitude).FirstOrDefault();
            }
		}

        SelectedObject = objectToSelect == null ? null : objectToSelect.gameObject;

        ShowSelection(SelectedObject);
	}

    private void ShowSelection(GameObject selectedObject)
    {
        MeshFilter meshFilter = null;

        if (selectedObject != null && selectedObject.renderer != null)
            meshFilter = selectedObject.GetComponent<MeshFilter>();

        if (meshFilter != null && meshFilter.mesh != null)
        {
            _selectionObject.renderer.enabled = true;

            _selectionObject.transform.position = selectedObject.transform.position;
            _selectionObject.transform.rotation = selectedObject.transform.rotation;
            _selectionObject.transform.localScale = selectedObject.transform.lossyScale * 1.01f;

            _selectionObject.GetComponent<MeshFilter>().mesh = meshFilter.mesh;
        }
        else
        {
            _selectionObject.renderer.enabled = false;
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
