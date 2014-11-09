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
					_maps[0][d,y,x] = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) > d / 10f ? 1 : 0;
					if (x > 3 && d == 3)
						_maps[0][d,y,x] = 0;
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

	static Vector2 UVify (Vector3 vert)
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
						
						var noShift = (x+1) % 2;
						var shift = x % 2;
						
						var up = IsFull (map, d+1, y, x);
						var top = IsFull (map, d, y+1, x);
						var bottom = IsFull (map, d, y-1, x);
						var topUp = IsFull (map, d+1, y+1, x);
						var leftTopUp = IsFull(map, d+1, y+noShift, x-1);
						var rightTopUp = IsFull(map, d+1, y+noShift, x+1);
						var bottomUp = IsFull (map, d+1, y-1, x);
						var leftBottomUp = IsFull (map, d+1, y-shift, x-1);
						var rightBottomUp = IsFull (map, d+1, y-shift, x+1);
						
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
						
						float baseY = yOffset - 1;
						
						if (!up)
						{
							if (topUp)
								northVert += Vector3.up;
							else if (leftTopUp || rightTopUp)
								northVert += Vector3.up / 2f;
							
							if (topUp || rightTopUp)
								northEastVert += Vector3.up;
							
							if (rightTopUp)
								eastNorthEastVert += Vector3.up;
							else if (topUp || rightBottomUp)
								eastNorthEastVert += Vector3.up / 2f;
							
							if (rightBottomUp || rightTopUp)
								eastVert += Vector3.up;
							
							if (rightBottomUp)
								eastSouthEastVert += Vector3.up;
							else if (bottomUp || rightTopUp)
								eastSouthEastVert += Vector3.up / 2f;
							
							if (bottomUp || rightBottomUp)
								southEastVert += Vector3.up;
							
							if (bottomUp)
								southVert += Vector3.up;
							else if (leftBottomUp || rightBottomUp)
								southVert += Vector3.up / 2f;
							
							if (leftBottomUp || bottomUp)
								southWestVert += Vector3.up;
							
							if (leftBottomUp)
								westSouthWestVert += Vector3.up;
							else if (leftTopUp || bottomUp)
								westSouthWestVert += Vector3.up / 2f;
							
							if (leftTopUp || leftBottomUp)
								westVert += Vector3.up;
							
							if (leftTopUp)
								westNorthWestVert += Vector3.up;
							else if (leftBottomUp || topUp)
								westNorthWestVert += Vector3.up / 2f;
							
							if (topUp || leftTopUp)
								northWestVert += Vector3.up;
							
							//if (topUp || leftTopUp || leftBottomUp || bottomUp || rightBottomUp || rightTopUp)
							//	centreVert.y += 0.5f;
								
							centreVert.y = (northVert.y + 
							                eastNorthEastVert.y + 
							                eastSouthEastVert.y + 
							                southVert.y + 
							                westSouthWestVert.y + 
							                westNorthWestVert.y) / 6.0f;
							
							// ---------
							// Top faces
							// ---------
							
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, northVert, northEastVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, northEastVert, eastNorthEastVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, eastNorthEastVert, eastVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, eastVert, eastSouthEastVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, eastSouthEastVert, southEastVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, southEastVert, southVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, southVert, southWestVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, southWestVert, westSouthWestVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, westSouthWestVert, westVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, westVert, westNorthWestVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, westNorthWestVert, northWestVert);
							GenerateTopFaceTriangle (vertices, uvs, indices, centreVert, northWestVert, northVert);	
							
							// ---------
							// Bottom faces
							// ---------
							
							GenerateBottomFaceTriangle (vertices, uvs, indices, baseY, centreVert, northEastVert, eastVert);
							GenerateBottomFaceTriangle (vertices, uvs, indices, baseY, centreVert, eastVert, southEastVert);
							GenerateBottomFaceTriangle (vertices, uvs, indices, baseY, centreVert, southEastVert, southWestVert);
							GenerateBottomFaceTriangle (vertices, uvs, indices, baseY, centreVert, southWestVert, westVert);
							GenerateBottomFaceTriangle (vertices, uvs, indices, baseY, centreVert, westVert, northWestVert);
							GenerateBottomFaceTriangle (vertices, uvs, indices, baseY, centreVert, northWestVert, northEastVert);				
						}

						// ----------
						// Side faces
						// ----------
						
						GenerateSideQuad(vertices, uvs, indices, baseY, southVert, southWestVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, southWestVert, westSouthWestVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, westSouthWestVert, westVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, westVert, westNorthWestVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, westNorthWestVert, northWestVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, northWestVert, northVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, northVert, northEastVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, northEastVert, eastNorthEastVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, eastNorthEastVert, eastVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, eastVert, eastSouthEastVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, eastSouthEastVert, southEastVert);
						GenerateSideQuad(vertices, uvs, indices, baseY, southEastVert, southVert);
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
	
	static void GenerateTopFaceTriangle (List<Vector3> vertices, List<Vector2> uvs, List<int> indices, Vector3 centreVert, Vector3 p1, Vector3 p2)
	{
		vertices.Add (centreVert);
		uvs.Add (UVify (centreVert));
		indices.Add (vertices.Count - 1);
		vertices.Add (p1);
		uvs.Add (UVify (p1));
		indices.Add (vertices.Count - 1);
		vertices.Add (p2);
		uvs.Add (UVify (p2));
		indices.Add (vertices.Count - 1);
	}
	
	static void GenerateBottomFaceTriangle (List<Vector3> vertices, List<Vector2> uvs, List<int> indices, float baseY, Vector3 centreVert, Vector3 p1, Vector3 p2)
	{
		vertices.Add (Drop(centreVert, baseY));
		uvs.Add (UVify (centreVert));
		indices.Add (vertices.Count - 1);
		vertices.Add (Drop(p2, baseY));
		uvs.Add (UVify (p2));
		indices.Add (vertices.Count - 1);
		vertices.Add (Drop(p1, baseY));
		uvs.Add (UVify (p1));
		indices.Add (vertices.Count - 1);
	}

	static void GenerateSideQuad (List<Vector3> vertices, List<Vector2> uvs, List<int> indices, float baseY, Vector3 p1, Vector3 p2)
	{
		vertices.Add (p1);
		uvs.Add (new Vector2 (1, 1));
		indices.Add (vertices.Count - 1);
		vertices.Add (Drop(p1, baseY));
		uvs.Add (new Vector2 (1, 0));
		indices.Add (vertices.Count - 1);
		vertices.Add (p2);
		uvs.Add (new Vector2 (0, 1));
		indices.Add (vertices.Count - 1);
		vertices.Add (p2);
		uvs.Add (new Vector2 (0, 1));
		indices.Add (vertices.Count - 1);
		vertices.Add (Drop(p1, baseY));
		uvs.Add (new Vector2 (1, 0));
		indices.Add (vertices.Count - 1);
		vertices.Add (Drop (p2, baseY));
		uvs.Add (new Vector2 (0, 0));
		indices.Add (vertices.Count - 1);
	}

	static Vector3 Drop (Vector3 p1, float baseY)
	{
		p1.y = baseY;
		return p1;
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
