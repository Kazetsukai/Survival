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

        // Find object being pointed at, or failing that: something nearby
		if (Physics.Raycast (ray, out pointHit, MaxInteractDistance)) {
            if (pointHit.collider.gameObject.GetComponents<MonoBehaviour>().Any(m => m is IInteraction))
                objectToSelect = pointHit.collider;
            else
            {
                var detectedObjects = Physics.OverlapSphere(pointHit.point, InteractBubbleSize);
                var interactableObjects = detectedObjects.Where(o => GetInteractableObject(o.gameObject) != null);
                objectToSelect = interactableObjects.OrderBy(c => (pointHit.point - c.transform.position).magnitude).FirstOrDefault();
            }
		}

        SelectedObject = objectToSelect == null ? null : GetInteractableObject(objectToSelect.gameObject);

        ShowSelection(SelectedObject);

        if (Input.GetButtonDown("Fire1"))
        {
            if (SelectedObject != null)
            {
                var interaction = GetInteractions(SelectedObject).First();
                if (interaction != null)
                    interaction.Interact();
            }
        }
	}

    private IEnumerable<IInteraction> GetInteractions(GameObject o)
    {
        return o.GetComponents<MonoBehaviour>().Where(m => m is IInteraction).Select(m => m as IInteraction);
    }

    // Get the lowest object in the tree of parents that has interactions
    private GameObject GetInteractableObject(GameObject o)
    {
        if (o == null)
            return null;

        var interactions = o.GetComponents<MonoBehaviour>().Where(m => m is IInteraction);

        if (interactions.Any())
            return o;
        else if (o.transform.parent != null)
            return GetInteractableObject(o.transform.parent.gameObject);
        else
            return null;
    }

    private Mesh GetMesh(GameObject o)
    {
        var meshFilter = o.GetComponent<MeshFilter>();
        if (meshFilter != null)
            return meshFilter.sharedMesh;
        else
            return null;
    }

    private void ShowSelection(GameObject selectedObject)
    {
        Mesh mesh = null;

        if (selectedObject != null)
            mesh = GetMesh(selectedObject);

        if (mesh != null)
        {
            _selectionObject.renderer.enabled = true;

            _selectionObject.transform.position = selectedObject.transform.position;
            _selectionObject.transform.rotation = selectedObject.transform.rotation;
            _selectionObject.transform.localScale = selectedObject.transform.lossyScale * 1.01f;

            _selectionObject.GetComponent<MeshFilter>().mesh = mesh;
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
