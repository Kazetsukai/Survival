using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainBehaviour : MonoBehaviour {
	
	static readonly Vector3 distCentreToFlatEdge = new Vector3(0, 0, Mathf.Sqrt (3) / 2.0f);
	static readonly Vector3 distCentreToPointyEdge = new Vector3(0, 0, 1);
	
	public int MapWidth = 50;
	public int MapHeight = 50;
	public int MapDepth = 20;

	public GameObject Terrain;	//Create terrain empty object to hold all the generated terrain objects in (so that unity hierarchy does not fill up)
	public GameObject[] TerrainTypes;
	
	// Use this for initialization
	void Start () {		

		Terrain = new GameObject("Terrain");

		_maps [0] = new int[MapDepth, MapHeight, MapWidth];
		
		for (int d = 0; d < MapDepth; d++)
		{
			for (int x = 0; x < MapWidth; x++) 
			{
				for (int y = 0; y < MapHeight; y++) 
				{
					int terrainTypeI = (int)(Mathf.PerlinNoise((x+300+d) * 0.04f, (y+103+d) * 0.04f) * TerrainTypes.Length);
					if (terrainTypeI >= TerrainTypes.Length)
					{
						terrainTypeI = 0;
					}
					
					_maps[0][d,y,x] = Mathf.PerlinNoise(x * 0.1f, y * 0.1f) > d / (10f + x / 10f) ? terrainTypeI : 0;
						
					//if (x > 3 && d == 3)
						//_maps[0][d,y,x] = 0;
					if (d == 0)
						_maps[0][d,y,x] = terrainTypeI;	
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
		
		// Dig the first corner down by four levels, except for a raised section in the corner
		for (int x = 0; x < 15; x++)
		{
			for (int y = 0; y < 15; y++)
			{
				int d;
				for (d = 5; d < MapDepth; d++)
				{
					if (_maps[0][d,y,x] == 0)
					{
						
						if (x + y < 10)
						{
							_maps[0][d-1,y,x] = _maps[0][d-3,y,x];
							_maps[0][d-2,y,x] = _maps[0][d-3,y,x];
							_maps[0][d,y,x] = _maps[0][d-3,y,x];
							_maps[0][d+1,y,x] = _maps[0][d-3,y,x];
							_maps[0][d+2,y,x] = _maps[0][d-3,y,x];
							_maps[0][d+3,y,x] = _maps[0][d-3,y,x];
						}
						else
						{
							_maps[0][d-1,y,x] = 0;
							_maps[0][d-2,y,x] = 0;
							_maps[0][d-3,y,x] = 0;
							_maps[0][d-4,y,x] = 0;
						}
						break;
					}
				}
			}
		}
		
		// Dig a cave
		for (int i = 0; i < 10; i++)
		{
			int x = 2 + (i/3);
			int y = 2 + i;
			
			_maps[0][1, y, x] = 0;
			_maps[0][1+1, y, x] = 0;
			_maps[0][1+2, y, x] = 0;
		}
		
		
		
		int vertices = 0;
		foreach (var map in _maps) {
			foreach (var mapChunk in ChunkMap(map))
			{
				for (int i = 1; i < TerrainTypes.Length; i++)
				{
					var mesh = GenerateMesh (mapChunk.Map, i);
				
					vertices += mesh.vertexCount;
				
					var terrainPatch = (GameObject)Instantiate (TerrainTypes[i]);
					terrainPatch.transform.parent = Terrain.transform; //Add the terrain patch to the Terrain hierarchy object
					terrainPatch.transform.position = terrainPatch.transform.position + mapChunk.StartPos;
					terrainPatch.GetComponent<MeshFilter> ().mesh = mesh;
					terrainPatch.GetComponent<MeshCollider> ().sharedMesh = mesh;
				}
			}
		}
		Debug.Log (vertices);
	}

	public class MapChunk
	{
		public int[,,] Map;
		public Vector3 StartPos;
	}

	IEnumerable<MapChunk> ChunkMap (int[,,] map)
	{
		var chunkSize = 10;
		int posX = 0;
		int posY = 0;
	
		for (int x = 0; x < map.GetLength(2); x += chunkSize-2)
		{
			for (int y = 0; y < map.GetLength (1); y += chunkSize-2)
			{
				var mapChunk = new int[map.GetLength (0), chunkSize, chunkSize];
				for (int xI = 0; xI < chunkSize; xI++)
				{
					for (int yI = 0; yI < chunkSize; yI++)
					{
						for (int d = 0; d < map.GetLength(0); d++)
						{
							if (x + xI >= map.GetLength(2) || y + yI >= map.GetLength(1))
								mapChunk[d, yI, xI] = 0;
							else
								mapChunk[d, yI, xI] = map[d, y+yI, x+xI];
						}
					}
				}
				
				yield return new MapChunk() { StartPos = new Vector3(posX * (chunkSize-2) * 4 * 1.5f, 0, posY * (chunkSize-2) * Mathf.Sqrt (3) * 4), Map = mapChunk };
				posY++;
			}
			posY = 0;
			posX++;
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
	
	Mesh GenerateMesh(int[,,] map, int terrainType)
	{
		int Depth = map.GetLength(0);
		int Height = map.GetLength(1);
		int Width = map.GetLength(2);
		
		List<Vector3> vertices = new List<Vector3> ();
		List<Vector2> uvs = new List<Vector2> ();
		List<int> indices = new List<int> ();
		
		for (int d = 0; d < Depth; d++) 
		{
			for (int x = 1; x < Width-1; x++) 
			{
				for (int y = 1; y < Height-1; y++) 
				{
					float xOffset = x * 1.5f;
					float zOffset = (y + (x % 2 == 0 ? 0.5f : 0.0f)) * Mathf.Sqrt (3);
					float yOffset = d * 1;
					
					if (map[d,y,x] == terrainType) {
						
						var noShift = (x+1) % 2;
						var shift = x % 2;
						
						var up = IsFull (map, d+1, y, x);
						var down = IsFull (map, d-1, y, x);
						
						var top = IsFull (map, d, y+1, x);
						var bottom = IsFull (map, d, y-1, x);
						var leftTop = IsFull (map, d, y+noShift, x-1);
						var rightTop = IsFull (map, d, y+noShift, x+1);
						var leftBottom = IsFull (map, d, y-shift, x-1);
						var rightBottom = IsFull (map, d, y-shift, x+1);
						
						var topUp = IsFull (map, d+1, y+1, x) && !IsFull (map, d+2, y+1, x);
						var leftTopUp = IsFull(map, d+1, y+noShift, x-1) && !IsFull (map, d+2, y+noShift, x-1);
						var rightTopUp = IsFull(map, d+1, y+noShift, x+1) && !IsFull (map, d+2, y+noShift, x+1);
						var bottomUp = IsFull (map, d+1, y-1, x) && !IsFull (map, d+2, y-1, x);
						var leftBottomUp = IsFull (map, d+1, y-shift, x-1) && !IsFull (map, d+2, y-shift, x-1);
						var rightBottomUp = IsFull (map, d+1, y-shift, x+1) && !IsFull (map, d+2, y-shift, x+1);
						
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
											
						}
						
						if (!down)
						{
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
						if (!bottom)
						{
							GenerateSideQuad(vertices, uvs, indices, baseY, southEastVert, southVert);
							GenerateSideQuad(vertices, uvs, indices, baseY, southVert, southWestVert);
						}
						if (!leftBottom)
						{
							GenerateSideQuad(vertices, uvs, indices, baseY, southWestVert, westSouthWestVert);
							GenerateSideQuad(vertices, uvs, indices, baseY, westSouthWestVert, westVert);
						}
						if (!leftTop)
						{
							GenerateSideQuad(vertices, uvs, indices, baseY, westVert, westNorthWestVert);
							GenerateSideQuad(vertices, uvs, indices, baseY, westNorthWestVert, northWestVert);
						}
						if (!top)
						{
							GenerateSideQuad(vertices, uvs, indices, baseY, northWestVert, northVert);
							GenerateSideQuad(vertices, uvs, indices, baseY, northVert, northEastVert);
						}
						if (!rightTop)
						{
							GenerateSideQuad(vertices, uvs, indices, baseY, northEastVert, eastNorthEastVert);
							GenerateSideQuad(vertices, uvs, indices, baseY, eastNorthEastVert, eastVert);
						}
						if (!rightBottom)
						{
							GenerateSideQuad(vertices, uvs, indices, baseY, eastVert, eastSouthEastVert);
							GenerateSideQuad(vertices, uvs, indices, baseY, eastSouthEastVert, southEastVert);
						}
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