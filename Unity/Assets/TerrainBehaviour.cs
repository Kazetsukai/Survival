using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainBehaviour : MonoBehaviour {

	public GameObject TerrainPrototype;
	
	static readonly Vector3 distCentreToFlatEdge = new Vector3(0, 0, Mathf.Sqrt (3) / 2.0f);
	static readonly Vector3 distCentreToPointyEdge = new Vector3(0, 0, 1);

	// Use this for initialization
	void Start () {

		_maps [0] = new int[10, 10, 10];
		
		for (int d = 0; d < 10; d++)
		{
			for (int x = 0; x < 10; x++) 
			{
				for (int y = 0; y < 10; y++) 
				{
					_maps[0][d,y,x] = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) > d / 5f ? 1 : 0;
					/*
					if (d == 0)
					{
						_maps [0][d, y, x] = 1;
					}
					else if (_maps[0][d-1,y,x] == 1 || Random.Range(0, 20) == 0)
					{
						if (Random.Range (0, d+1) == 0)
							_maps[0][d,y,x] = 1;
					}
					*/
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

	Quaternion Rotate30 (int times)
	{
		return Quaternion.AngleAxis(30 * times, Vector3.up);
	}

	Vector2 UVify (Vector3 vert)
	{
		return new Vector2(vert.x, vert.z);
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
						Vector3 southEastVert;
						Vector3 southWestVert;
						Vector3 northWestVert;
						Vector3 northEastVert;
						Vector3 eastVert;
						Vector3 westVert;
						Vector3 northVert;
						Vector3 southVert;
						
						northWestVert = new Vector3 (0 + xOffset, 0 + yOffset, 1 + zOffset);
						northEastVert = new Vector3 (1 + xOffset, 0 + yOffset, 1 + zOffset);
						southWestVert = new Vector3 (0 + xOffset, 0 + yOffset, 0 + zOffset);
						southEastVert = new Vector3 (1 + xOffset, 0 + yOffset, 0 + zOffset);
						eastVert = new Vector3 (1 + xOffset, 0 + yOffset, 0.5f + zOffset);
						westVert = new Vector3 (0 + xOffset, 0 + yOffset, 0.5f + zOffset);
						northVert = new Vector3 (0.5f + xOffset, 0 + yOffset, 1 + zOffset);
						southVert = new Vector3 (0.5f + xOffset, 0 + yOffset, 0 + zOffset);
						centreVert = new Vector3 (0.5f + xOffset, 0 + yOffset, 0.5f + zOffset);
						
						if (!up)
						{
							if (leftUp || topUp || rightUp || bottomUp || leftTopUp || bottomLeftUp || rightTopUp || bottomRightUp)
								centreVert = new Vector3 (0.5f + xOffset, 0.5f + yOffset, 0.5f + zOffset);
							
							if (leftUp || topUp || leftTopUp)
								northWestVert += Vector3.up;
							
							if (leftUp || bottomUp || bottomLeftUp)
								southWestVert += Vector3.up;
							
							if (rightUp || topUp || rightTopUp)
								northEastVert += Vector3.up;
							
							if (rightUp || bottomUp || bottomRightUp)
								southEastVert += Vector3.up;
								
							if (topUp)
								northVert += Vector3.up;
							else if (leftTopUp || rightTopUp || leftUp || rightUp)
								northVert += Vector3.up / 2f;
								
							if (rightUp)
								eastVert += Vector3.up;
							else if (bottomRightUp || rightTopUp || topUp || bottomUp)
								eastVert += Vector3.up / 2f;
								
							if (leftUp)
								westVert += Vector3.up;
							else if (leftTopUp || bottomLeftUp || topUp || bottomUp)
								westVert += Vector3.up / 2f;
								
							if (bottomUp)
								southVert += Vector3.up;
							else if (bottomLeftUp || bottomRightUp || leftUp || rightUp)
								southVert += Vector3.up / 2f;
							
							// ---------
							// Top faces
							// ---------
							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southEastVert);
							uvs.Add (new Vector2 (0, 0));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southVert);
							uvs.Add (new Vector2 (0.5f, 0));
							indices.Add (vertices.Count - 1);
							
							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southVert);
							uvs.Add (new Vector2 (0.5f, 0));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southWestVert);
							uvs.Add (new Vector2 (1, 0));
							indices.Add (vertices.Count - 1);
							
							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southWestVert);
							uvs.Add (new Vector2 (0, 0));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (westVert);
							uvs.Add (new Vector2 (0, 0.5f));
							indices.Add (vertices.Count - 1);
							
							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (westVert);
							uvs.Add (new Vector2 (0, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northWestVert);
							uvs.Add (new Vector2 (0, 1));
							indices.Add (vertices.Count - 1);
							
							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northWestVert);
							uvs.Add (new Vector2 (0, 1));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northVert);
							uvs.Add (new Vector2 (0.5f, 1));
							indices.Add (vertices.Count - 1);
							
							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northVert);
							uvs.Add (new Vector2 (0.5f, 1));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northEastVert);
							uvs.Add (new Vector2 (1, 1));
							indices.Add (vertices.Count - 1);
							
							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northEastVert);
							uvs.Add (new Vector2 (1, 1));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (eastVert);
							uvs.Add (new Vector2 (1, 0.5f));
							indices.Add (vertices.Count - 1);
							
							
							vertices.Add (centreVert);
							uvs.Add (new Vector2 (0.5f, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (eastVert);
							uvs.Add (new Vector2 (1, 0.5f));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southEastVert);
							uvs.Add (new Vector2 (1, 0));
							indices.Add (vertices.Count - 1);
						}
						
						
						// ----------
						// Side faces
						// ----------
						
						// Bottom Side 1
						vertices.Add (southWestVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southWestVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southWestVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southVert + Vector3.down);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Bottom Side 2
						vertices.Add (southVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southEastVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southEastVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southEastVert + Vector3.down);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Left Side 1
						vertices.Add (northWestVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (westVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northWestVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northWestVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (westVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (westVert + Vector3.down);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Left Side 2
						vertices.Add (westVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southWestVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (westVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (westVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southWestVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southWestVert + Vector3.down);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Top Side 1
						vertices.Add (northEastVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northEastVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northEastVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northVert + Vector3.down);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Top Side 2
						vertices.Add (northVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northWestVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northWestVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northWestVert + Vector3.down);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Right Side 1
						vertices.Add (southEastVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (eastVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southEastVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (southEastVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (eastVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (eastVert + Vector3.down);
						uvs.Add (new Vector2 (1, 0));
						indices.Add (vertices.Count - 1);
						
						// Right Side 2
						vertices.Add (eastVert);
						uvs.Add (new Vector2 (0, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northEastVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (eastVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (eastVert + Vector3.down);
						uvs.Add (new Vector2 (0, 0));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northEastVert);
						uvs.Add (new Vector2 (1, 1));
						indices.Add (vertices.Count - 1);
						
						vertices.Add (northEastVert + Vector3.down);
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
