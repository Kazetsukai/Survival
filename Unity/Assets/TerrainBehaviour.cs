using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainBehaviour : MonoBehaviour {

	public GameObject TerrainPrototype;

	// Use this for initialization
	void Start () {
		var mesh = GenerateMesh (_maps[0]);

		var terrainPatch = (GameObject)Instantiate (TerrainPrototype);
		terrainPatch.GetComponent<MeshFilter> ().mesh = mesh;
		terrainPatch.GetComponent<MeshCollider> ().sharedMesh = mesh;
	}


	// Update is called once per frame
	void Update () {
	
	}

	Mesh GenerateMesh(int[,,] map)
	{
		int Depth = map.GetLength(0);
		int Height = map.GetLength(1);
		int Width = map.GetLength(2);

		List<Vector3> vertices = new List<Vector3> ();
		List<Vector2> uvs = new List<Vector2> ();
		List<int> indices = new List<int> ();

		for (int x = 0; x < Width; x++) 
		{
			for (int y = 0; y < Height; y++)
			{
				int xOffset = x;
				int zOffset = y;

				if (map[0,y,x] == 1)
				{
					vertices.Add (new Vector3 (0 + xOffset, 1, 1 + zOffset));
					uvs.Add (new Vector2 (0, 1));
					indices.Add (vertices.Count-1);

					vertices.Add (new Vector3 (1 + xOffset, 1, 1 + zOffset));
					uvs.Add (new Vector2 (1, 1));
					indices.Add (vertices.Count-1);

					vertices.Add (new Vector3 (0 + xOffset, 1, 0 + zOffset));
					uvs.Add (new Vector2 (0, 0));
					indices.Add (vertices.Count-1);


					vertices.Add (new Vector3 (0 + xOffset, 1, 0 + zOffset));
					uvs.Add (new Vector2 (0, 0));
					indices.Add (vertices.Count-1);

					vertices.Add (new Vector3 (1 + xOffset, 1, 1 + zOffset));
					uvs.Add (new Vector2 (1, 1));
					indices.Add (vertices.Count-1);

					vertices.Add (new Vector3 (1 + xOffset, 1, 0 + zOffset));
					uvs.Add (new Vector2 (1, 0));
					indices.Add (vertices.Count-1);
				}
			}
		}

		var mesh = new Mesh ();

		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.SetIndices (indices.ToArray(), MeshTopology.Triangles, 0);
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	
		mesh.UploadMeshData (false);

		return mesh;
	}
	
	int[][,,] _maps = new int[][,,] {
		new int[,,] {{
				{0,0,1,0,0},
				{0,1,1,1,0},
				{0,0,1,0,0}
			}},		
		
		new int[,,] {{
				{1,0,0,0,0},
				{1,0,1,1,0},
				{1,0,0,0,0}
			}},
		
		new int[,,] {
			{
				{0,1,0},
				{1,1,0},
				{0,0,0}
			},{
				{0,0,0},
				{0,1,0},
				{0,1,0}
			},
		},
		
		new int[,,] {
			{
				{1},
			},{
				{1},
			},{
				{1},
			}
		},
		
		new int[,,] {
			{
				{1,0},
				{0,0},
			},{
				{0,1},
				{0,0},
			},{
				{0,0},
				{1,1},
			}
		},
	};
}
