using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainBehaviour : MonoBehaviour {

	public GameObject TerrainPrototype;

	// Use this for initialization
	void Start () {

		_maps [0] = new int[10, 20, 20];
		
		for (int d = 0; d < 10; d++)
		{
			for (int x = 0; x < 20; x++) 
			{
				for (int y = 0; y < 20; y++) 
				{
					if (d == 0)
					{
						_maps [0][d, y, x] = 1;
					}
					else if (_maps[0][d-1,y,x] == 1 || Random.Range(0, 20) == 0)
					{
						if (Random.Range (0, d+1) == 0)
							_maps[0][d,y,x] = 1;
					}
				}
			}
		}

		int pos = 0;
		foreach (var map in _maps) {
			var mesh = GenerateMesh (map);

			var terrainPatch = (GameObject)Instantiate (TerrainPrototype);
			terrainPatch.transform.position = terrainPatch.transform.position + Vector3.right * pos;
			terrainPatch.GetComponent<MeshFilter> ().mesh = mesh;
			terrainPatch.GetComponent<MeshCollider> ().sharedMesh = mesh;

			pos += 4 * map.GetLength(1);
		}
	}


	// Update is called once per frame
	void Update () {
	
	}

	bool IsFull (int[,,] map, int d, int y, int x)
	{
		if (d < 0 || y < 0 || x < 0 || 
			d >= map.GetLength (0) || y >= map.GetLength (1) || x >= map.GetLength (2)) 
		{
			return false;
		}
		return map [d, y, x] != 0;
	}

	Mesh GenerateMesh(int[,,] map)
	{
		int Depth = map.GetLength(0);
		int Height = map.GetLength(1);
		int Width = map.GetLength(2);

		List<Vector3> vertices = new List<Vector3> ();
		List<Vector2> uvs = new List<Vector2> ();
		List<int> indices = new List<int> ();

		for (int d = 0; d < Depth; d++) 
		{
			for (int x = 0; x < Width; x++) 
			{
				for (int y = 0; y < Height; y++) 
				{
					int xOffset = x;
					int zOffset = y;
					int yOffset = d;

					if (IsFull(map, d, y, x)) {
						
						var up = IsFull (map, d+1, y, x);
						var left = IsFull (map, d, y, x-1);
						var right = IsFull (map, d, y, x+1);
						var top = IsFull (map, d, y+1, x);
						var bottom = IsFull (map, d, y-1, x);
						var leftUp = IsFull (map, d+1, y, x-1);
						var topUp = IsFull (map, d+1, y+1, x);
						var leftTopUp = IsFull(map, d+1, y+1, x-1);
						var rightUp = IsFull (map, d+1, y, x+1);
						var rightTopUp = IsFull(map, d+1, y+1, x+1);
						var bottomUp = IsFull (map, d+1, y-1, x);
						var bottomLeftUp = IsFull (map, d+1, y-1, x-1);
						var bottomRightUp = IsFull (map, d+1, y-1, x+1);

						Vector3 centreVert;
						Vector3 bottomRightVert;
						Vector3 bottomLeftVert;
						Vector3 topLeftVert;
						Vector3 topRightVert;
						Vector3 bottomRightDownVert;
						Vector3 bottomLeftDownVert;
						Vector3 topLeftDownVert;
						Vector3 topRightDownVert;

						bottomRightDownVert = new Vector3 (1 + xOffset, yOffset - 1, 0 + zOffset);
						bottomLeftDownVert = new Vector3 (0 + xOffset, yOffset - 1, 0 + zOffset);
						topLeftDownVert = new Vector3 (0 + xOffset, yOffset - 1, 1 + zOffset);
						topRightDownVert = new Vector3 (1 + xOffset, yOffset - 1, 1 + zOffset);
						topLeftVert = new Vector3 (0 + xOffset, 0 + yOffset, 1 + zOffset);
						topRightVert = new Vector3 (1 + xOffset, 0 + yOffset, 1 + zOffset);
						bottomLeftVert = new Vector3 (0 + xOffset, 0 + yOffset, 0 + zOffset);
						bottomRightVert = new Vector3 (1 + xOffset, 0 + yOffset, 0 + zOffset);
						centreVert = new Vector3 (0.5f + xOffset, 0 + yOffset, 0.5f + zOffset);

						if (!up)
						{
							if (leftUp || topUp || rightUp || bottomUp || leftTopUp || bottomLeftUp || rightTopUp || bottomRightUp)
								centreVert = new Vector3 (0.5f + xOffset, 0.5f + yOffset, 0.5f + zOffset);

							if (leftUp || topUp || leftTopUp)
								topLeftVert = new Vector3 (0 + xOffset, 1 + yOffset, 1 + zOffset);

							if (leftUp || bottomUp || bottomLeftUp)
								bottomLeftVert = new Vector3 (0 + xOffset, 1 + yOffset, 0 + zOffset);

							if (rightUp || topUp || rightTopUp)
								topRightVert = new Vector3 (1 + xOffset, 1 + yOffset, 1 + zOffset);

							if (rightUp || bottomUp || bottomRightUp)
								bottomRightVert = new Vector3 (1 + xOffset, 1 + yOffset, 0 + zOffset);

							// ---------
							// Top faces
							// ---------

							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);

							vertices.Add (bottomRightVert);
							uvs.Add (new Vector2 (0, 0));
							indices.Add (vertices.Count - 1);

							vertices.Add (bottomLeftVert);
							uvs.Add (new Vector2 (1, 0));
							indices.Add (vertices.Count - 1);

							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);

							vertices.Add (bottomLeftVert);
							uvs.Add (new Vector2 (0, 0));
							indices.Add (vertices.Count - 1);

							vertices.Add (topLeftVert);
							uvs.Add (new Vector2 (0, 1));
							indices.Add (vertices.Count - 1);


							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (topLeftVert);
							uvs.Add (new Vector2 (0, 1));
							indices.Add (vertices.Count - 1);

							vertices.Add (topRightVert);
							uvs.Add (new Vector2 (1, 1));
							indices.Add (vertices.Count - 1);


							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (topRightVert);
							uvs.Add (new Vector2 (1, 1));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (bottomRightVert);
							uvs.Add (new Vector2 (1, 0));
							indices.Add (vertices.Count - 1);
						}

						
						// ----------
						// Side faces
						// ----------
						
						// Bottom Side 1
						vertices.Add (bottomLeftVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (bottomRightVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (bottomLeftDownVert);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						// Bottom Side 2
						vertices.Add (bottomLeftDownVert);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (bottomRightVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (bottomRightDownVert);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Left Side 1
						vertices.Add (topLeftVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (bottomLeftVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (topLeftDownVert);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						// Left Side 2
						vertices.Add (topLeftDownVert);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (bottomLeftVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (bottomLeftDownVert);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Top Side 1
						vertices.Add (topRightVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (topLeftVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (topRightDownVert);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						// Top Side 2
						vertices.Add (topRightDownVert);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (topLeftVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (topLeftDownVert);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);

						// Right Side 1
						vertices.Add (bottomRightVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (topRightVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (bottomRightDownVert);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						// Right Side 2
						vertices.Add (bottomRightDownVert);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (topRightVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (topRightDownVert);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
					}
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
				{0,0,0,0,0},
				{0,0,1,0,0},
				{0,0,0,0,0}
			},{
				{0,0,1,0,0},
				{0,1,0,1,0},
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

		new int[,,] {
			{
				{1,1},
				{1,0},
			},{
				{1,1},
				{0,0},
			},{
				{1,0},
				{0,0},
			}
		},
	};
}
