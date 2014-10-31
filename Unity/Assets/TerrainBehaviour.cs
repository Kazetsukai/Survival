using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainBehaviour : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var mesh = GenerateMesh ();

		this.GetComponent<MeshFilter> ().mesh = mesh;
		this.GetComponent<MeshCollider> ().sharedMesh = mesh;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Mesh GenerateMesh()
	{
		const int Width = 100;
		const int Height = 100;

		int basis = Random.Range (0, 10000);

		var heightMap = new int[Width,Height];

		for (int x = 0; x < Width; x++) 
		{
			for (int y = 0; y < Height; y++) 
			{
				//if (Random.Range (0,3) == 0)
				heightMap[x,y] = (int)(Mathf.PerlinNoise((basis + x) * ((x / (Width * 10000f))), (basis + y) * ((y / (Height * 10000f)))) * (20 + x / 2f + y / 2f));
			}
		}

		
		/*for (int x = 0; x < Width; x++) 
		{
			for (int y = 0; y < Height; y++) 
			{
				heightMap[x,y] = (int)heightMap[x,y];
			}
		}*/
		

		int squares = (Width - 1) * (Height - 1);
		int totalVertices = squares * 6;
		var vertices = new Vector3[totalVertices];
		var uvs = new Vector2[totalVertices];

		var indices = new int[squares * 6];

		for (int x = 0; x < Width-1; x++) 
		{
			for (int y = 0; y < Height-1; y++) 
			{
				int xOffset = x;
				int zOffset = y;

				int i = (Width - 1) * y + x;
				int vI = i * 6;


				if (SwapCrease(x, y, heightMap))
				{
					vertices[vI] = new Vector3 (0 + xOffset, heightMap[x, y+1], 1 + zOffset);
					vertices[vI+1] = new Vector3 (1 + xOffset, heightMap[x+1, y+1], 1 + zOffset);
					vertices[vI+2] = new Vector3 (0 + xOffset, heightMap[x, y], 0 + zOffset);
					
					vertices[vI+3] = new Vector3 (0 + xOffset, heightMap[x, y], 0 + zOffset);
					vertices[vI+4] = new Vector3 (1 + xOffset, heightMap[x+1, y+1], 1 + zOffset);
					vertices[vI+5] = new Vector3 (1 + xOffset, heightMap[x+1, y], 0 + zOffset);

					uvs[vI] = new Vector2 (0, 1);
					uvs[vI+1] = new Vector2 (1, 1);
					uvs[vI+2] = new Vector2 (0, 0);
					
					uvs[vI+3] = new Vector2 (0, 0);
					uvs[vI+4] = new Vector2 (1, 1);
					uvs[vI+5] = new Vector2 (1, 0);
				}
				else
				{
					vertices[vI] = new Vector3 (0 + xOffset, heightMap[x, y], 0 + zOffset);
					vertices[vI+1] = new Vector3 (0 + xOffset, heightMap[x, y+1], 1 + zOffset);
					vertices[vI+2] = new Vector3 (1 + xOffset, heightMap[x+1, y], 0 + zOffset);
					
					vertices[vI+3] = new Vector3 (1 + xOffset, heightMap[x+1, y], 0 + zOffset);
					vertices[vI+4] = new Vector3 (0 + xOffset, heightMap[x, y+1], 1 + zOffset);
					vertices[vI+5] = new Vector3 (1 + xOffset, heightMap[x+1, y+1], 1 + zOffset);
					
					uvs[vI] = new Vector2 (0, 0);
					uvs[vI+1] = new Vector2 (0, 1);
					uvs[vI+2] = new Vector2 (1, 0);
					
					uvs[vI+3] = new Vector2 (1, 0);
					uvs[vI+4] = new Vector2 (0, 1);
					uvs[vI+5] = new Vector2 (1, 1);
				}
				
				indices[vI] = vI;
				indices[vI+1] = vI+1;
				indices[vI+2] = vI+2;
				
				indices[vI+3] = vI+3;
				indices[vI+4] = vI+4;
				indices[vI+5] = vI+5;
			}
		}

		var mesh = new Mesh ();

		mesh.vertices = vertices;
		mesh.uv = uvs;
		mesh.SetIndices (indices, MeshTopology.Triangles, 0);
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
	
		mesh.UploadMeshData (false);

		return mesh;
	}

	bool SwapCrease (int x, int y, int[,] heightMap)
	{
		return (heightMap[x,y+1] > heightMap[x,y] && heightMap[x,y+1] > heightMap[x+1,y+1] && heightMap[x,y+1] > heightMap[x+1,y]) ||
			   (heightMap[x+1,y] > heightMap[x,y] && heightMap[x+1,y+1] > heightMap[x,y+1] && heightMap[x+1,y+1] > heightMap[x+1,y+1]) ||
			   (heightMap[x,y+1] < heightMap[x,y] && heightMap[x,y+1] < heightMap[x+1,y+1] && heightMap[x,y+1] < heightMap[x+1,y]) ||
			   (heightMap[x+1,y] < heightMap[x,y] && heightMap[x+1,y+1] < heightMap[x,y+1] && heightMap[x+1,y+1] < heightMap[x+1,y+1]) ||
			   (heightMap[x,y] > heightMap[x,y+1] && heightMap[x,y] > heightMap[x+1,y] && heightMap[x+1,y+1] > heightMap[x+1,y] && heightMap[x+1,y+1] > heightMap[x,y+1]);
	}
}
