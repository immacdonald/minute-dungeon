using UnityEngine;
using UnityEngine.Tilemaps;

public class Room : MonoBehaviour {

	private Vector2Int sectionSize;
	private int entranceY;
	private int exitY;
	private int tunnelHeight;
	private int borderSize;

	[SerializeField]
	private TileBase dungeonTile;

	public void Initalize(Vector2Int sectionSize, int entranceY, int exitY, int tunnelHeight, int borderSize) {
		this.sectionSize = sectionSize;
		this.entranceY = entranceY;
		this.exitY = exitY;
		this.tunnelHeight = tunnelHeight;
		this.borderSize = borderSize;

		GenerateDungeonMesh(this.sectionSize.x, this.sectionSize.y, this.entranceY, this.exitY, this.borderSize, this.tunnelHeight);
	}

	public void GenerateDungeonMesh(int horizontalSize, int verticalSize, int entranceY, int exitY, int borderSize, int tunnelHeight)
	{
		GameObject dungeonGO = new GameObject("DungeonTilemap");
		dungeonGO.transform.SetParent(this.transform);
		dungeonGO.layer = 6;
		Tilemap tilemap = dungeonGO.AddComponent<Tilemap>();
		dungeonGO.AddComponent<TilemapRenderer>();
		dungeonGO.AddComponent<TilemapCollider2D>();

		// Define the size including borders
		horizontalSize += 2 * borderSize;
		verticalSize += 2 * borderSize;

		// Loop through each position in the grid and set tiles
		for (int y = 0; y < verticalSize; y++)
		{
			for (int x = 0; x < horizontalSize; x++)
			{
				// Check if we are in the border
				bool isBorder = x < borderSize || y < borderSize || x >= horizontalSize - borderSize || y >= verticalSize - borderSize;

				// Create wall tiles at borders, except for the entrance and exit tunnels
				if (isBorder)
				{
					// Entrance (gap in the bottom-left)
					if (x <= borderSize && y >= entranceY && y < entranceY + tunnelHeight)
					{
						//tilemap.SetTile(new Vector3Int(x, y, 0), dungeonTile);
					}
					// Exit (gap in the top-right)
					else if (x >= horizontalSize - borderSize && y >= exitY && y < exitY + tunnelHeight)
					{
						//tilemap.SetTile(new Vector3Int(x, y, 0), dungeonTile);
					}
					// Place wall tile if not entrance or exit
					else
					{
						tilemap.SetTile(new Vector3Int(x, y, 0), dungeonTile);
					}
				}
			}
		}

		GenerateDungeonBackground(horizontalSize, verticalSize);
	}

	private void GenerateDungeonBackground(int horizontalSize, int verticalSize)
	{
		Vector3[] vertices = new Vector3[horizontalSize * verticalSize * 6];
		int[] triangles = new int[horizontalSize * verticalSize * 6];
		Vector2[] uvs = new Vector2[horizontalSize * verticalSize * 6];
		for (int y = 0; y < verticalSize; y++)
		{
			for (int x = 0; x < horizontalSize; x++)
			{
				int cq = (y * horizontalSize + x);
				vertices[cq * 6] = new Vector3(x, y, 0); //top-left
				vertices[cq * 6 + 1] = new Vector3(x + 1, y, 0); //top-right
				vertices[cq * 6 + 2] = new Vector3(x, y - 1, 0); //bottom-left
				vertices[cq * 6 + 3] = new Vector3(x, y - 1, 0); //bottom-left
				vertices[cq * 6 + 4] = new Vector3(x + 1, y, 0); //top-right
				vertices[cq * 6 + 5] = new Vector3(x + 1, y - 1, 0); //bottom-right
				triangles[cq * 6] = cq * 6;
				triangles[cq * 6 + 1] = cq * 6 + 1;
				triangles[cq * 6 + 2] = cq * 6 + 2;
				triangles[cq * 6 + 3] = cq * 6 + 3;
				triangles[cq * 6 + 4] = cq * 6 + 4;
				triangles[cq * 6 + 5] = cq * 6 + 5;
				uvs[cq * 6] = new Vector2(x / (float)horizontalSize, (y + 1) / (float)verticalSize); //top-left
				uvs[cq * 6 + 1] = new Vector2((x + 1) / (float)horizontalSize, (y + 1) / (float)verticalSize); //top-right
				uvs[cq * 6 + 2] = new Vector2(x / (float)horizontalSize, y / (float)verticalSize); //bottom-left
				uvs[cq * 6 + 3] = new Vector2(x / (float)horizontalSize, y / (float)verticalSize); //bottom-left
				uvs[cq * 6 + 4] = new Vector2((x + 1) / (float)horizontalSize, (y + 1) / (float)verticalSize); //top-right
				uvs[cq * 6 + 5] = new Vector2((x + 1) / (float)horizontalSize, y / (float)verticalSize); //bottom-right
			}
		}

		GameObject background = transform.GetChild(1).gameObject;
		MeshRenderer mr = background.GetComponent<MeshRenderer>();

		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		background.GetComponent<MeshFilter>().sharedMesh = mesh;
		background.isStatic = true;
		mr.material.SetTextureScale("_MainTex", new Vector2(horizontalSize, verticalSize));
	}

	public void CreateTileEntity(GameObject entity, Vector2Int position)
    {
		GameObject tileEntity = Instantiate(entity, transform);
		tileEntity.transform.localPosition = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);

    }

}
