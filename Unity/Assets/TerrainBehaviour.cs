using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainBehaviour : MonoBehaviour {

	public GameObject TerrainPrototype;
	
	static readonly Vector3 distCentreToFlatEdge = new Vector3(0, 0, Mathf.Sqrt (3) / 2.0f);
	static readonly Vector3 distCentreToPointyEdge = new Vector3(0, 0, 1);

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
					float xOffset = x * 1.5f;
					float zOffset = (y + (x % 2 == 0 ? 0.5f : 0.0f)) * Mathf.Sqrt (3);
					float yOffset = d * 1;

					if (IsFull(map, d, y, x)) {
						
						var up = IsFull (map, d+1, y, x);
						var top = IsFull (map, d, y+1, x);
						var bottom = IsFull (map, d, y-1, x);
						var topUp = IsFull (map, d+1, y+1, x);
						var leftTopUp = IsFull(map, d+1, y+1, x-1);
						var rightTopUp = IsFull(map, d+1, y+1, x+1);
						var bottomUp = IsFull (map, d+1, y-1, x);
						var leftBottomUp = IsFull (map, d+1, y-1, x-1);
						var rightBottomUp = IsFull (map, d+1, y-1, x+1);
						
						Vector3 centreVert = new Vector3 (xOffset, yOffset, zOffset);
						
						Vector3 northVert = 			centreVert + Rotate30(0) * distCentreToFlatEdge;
						Vector3 northEastVert = 		centreVert + Rotate30(1) * distCentreToPointyEdge;
						Vector3 eastNorthEastVert = 	centreVert + Rotate30(2) * distCentreToFlatEdge;
						Vector3 eastVert = 				centreVert + Rotate30(3) * distCentreToPointyEdge;
						Vector3 eastSouthEastVert = 	centreVert + Rotate30(4) * distCentreToFlatEdge;
						Vector3 southEastVert = 		centreVert + Rotate30(5) * distCentreToPointyEdge;
						Vector3 southVert = 			centreVert + Rotate30(6) * distCentreToFlatEdge;
						Vector3 southWestVert = 		centreVert + Rotate30(7) * distCentreToPointyEdge;
						Vector3 westSouthWestVert = 	centreVert + Rotate30(8) * distCentreToFlatEdge;
						Vector3 westVert = 				centreVert + Rotate30(9) * distCentreToPointyEdge;
						Vector3 westNorthWestVert = 	centreVert + Rotate30(10) * distCentreToFlatEdge;
						Vector3 northWestVert = 		centreVert + Rotate30(11) * distCentreToPointyEdge;

						if (!up)
						{
							if (topUp)
							{
								northVert += Vector3.up;
							}
							
							if (topUp || rightTopUp)
							{
								northEastVert += Vector3.up;
							}
							
							if (rightTopUp)
							{
								eastNorthEastVert += Vector3.up;
							}
							
							if (rightBottomUp || rightTopUp)
							{
								eastVert += Vector3.up;
							}
							
							if (rightBottomUp)
							{
								eastSouthEastVert += Vector3.up;
							}
							
							if (bottomUp || rightBottomUp)
							{
								southEastVert += Vector3.up;
							}
							
							if (bottomUp)
							{
								southVert += Vector3.up;
							}
							
							if (leftBottomUp || bottomUp)
							{
								southWestVert += Vector3.up;
							}
							
							if (leftBottomUp)
							{
								westSouthWestVert += Vector3.up;
							}
							
							if (leftTopUp || leftBottomUp)
							{
								westVert += Vector3.up;
							}
							
							if (leftTopUp)
							{
								westNorthWestVert += Vector3.up;
							}
							
							if (topUp || leftTopUp)
							{
								northWestVert += Vector3.up;
							}
							
							
							// ---------
							// Top faces
							// ---------
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northVert);
							uvs.Add (UVify(northVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northEastVert);
							uvs.Add (UVify(northEastVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northEastVert);
							uvs.Add (UVify(northEastVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (eastNorthEastVert);
							uvs.Add (UVify(eastNorthEastVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (eastNorthEastVert);
							uvs.Add (UVify(eastNorthEastVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (eastVert);
							uvs.Add (UVify(eastVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (eastVert);
							uvs.Add (UVify(eastVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (eastSouthEastVert);
							uvs.Add (UVify(eastSouthEastVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (eastSouthEastVert);
							uvs.Add (UVify(eastSouthEastVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southEastVert);
							uvs.Add (UVify(southEastVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southEastVert);
							uvs.Add (UVify(southEastVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southVert);
							uvs.Add (UVify(southVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southVert);
							uvs.Add (UVify(southVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southWestVert);
							uvs.Add (UVify(southWestVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (southWestVert);
							uvs.Add (UVify(southWestVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (westSouthWestVert);
							uvs.Add (UVify(westSouthWestVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (westSouthWestVert);
							uvs.Add (UVify(westSouthWestVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (westVert);
							uvs.Add (UVify(westVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (westVert);
							uvs.Add (UVify(westVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (westNorthWestVert);
							uvs.Add (UVify(westNorthWestVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (westNorthWestVert);
							uvs.Add (UVify(westNorthWestVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northWestVert);
							uvs.Add (UVify(northWestVert));
							indices.Add (vertices.Count - 1);
							
							//---
							
							vertices.Add (centreVert);
							uvs.Add (UVify(centreVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northWestVert);
							uvs.Add (UVify(northWestVert));
							indices.Add (vertices.Count - 1);
							
							vertices.Add (northVert);
							uvs.Add (UVify(northVert));
							indices.Add (vertices.Count - 1);
							
							
							/*vertices.Add (centreVert);
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
							*/
						}

						/*
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
						*/
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
