using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDungeon : MonoBehaviour {
	[SerializeField]
	private DungeonSettings dungeonSettings;
	public Room roomPrefab;
	public GameObject platformPrefab;

	public GameObject GenerateNextRun()
    {
		GameObject dungeonHolder = new();
		Vector2 lastExitPos = Vector2.zero;
		for (int i = 0; i < dungeonSettings.numberOfRooms; i++)
		{
			lastExitPos = Generate(lastExitPos, i, dungeonHolder.transform);
		}

		return dungeonHolder;
	}

	Vector2 Generate (Vector2 lastExitPos, int roomIndex, Transform holder) {
		//getting basic room setup
		Room room = Instantiate(roomPrefab);
		room.transform.name = "Room " + (roomIndex + 1);
		room.transform.SetParent(holder);

		//find/generate all the needed vectors
		Vector2Int roomSize = new Vector2Int (SeedManager.Instance.dungeonSeed.Next (dungeonSettings.minRoomSize.x, dungeonSettings.maxRoomSize.x), SeedManager.Instance.dungeonSeed.Next (dungeonSettings.minRoomSize.y, dungeonSettings.maxRoomSize.y));
		Vector2Int sectionSize = roomSize + new Vector2Int(dungeonSettings.borderSize * 2, dungeonSettings.borderSize * 2);
		Vector2Int entrancePos = new Vector2Int (dungeonSettings.borderSize, roomIndex > 0 ? SeedManager.Instance.dungeonSeed.Next (dungeonSettings.borderSize, roomSize.y + 2 - dungeonSettings.tunnelHeight) : dungeonSettings.borderSize);
		Vector2Int exitPos = new Vector2Int(sectionSize.x - dungeonSettings.borderSize, SeedManager.Instance.dungeonSeed.Next(dungeonSettings.borderSize, roomSize.y + 2 - dungeonSettings.tunnelHeight));

		//initalize room and set position
		room.Initalize (sectionSize, entrancePos.y, exitPos.y, dungeonSettings.tunnelHeight, dungeonSettings.borderSize);
		room.transform.position = new Vector2(lastExitPos.x, lastExitPos.y - ((roomIndex > 0) ? entrancePos.y : 0));

		List<Vector2> validSpawnLocations = new List<Vector2> ();

		if (dungeonSettings.generatePlatforms) {
			Vector2[] points = PhantomUtil.GetPointsBetweenLocations (entrancePos + new Vector2(6, entrancePos.y <= dungeonSettings.borderSize ? 4 : 0), exitPos + new Vector2Int(dungeonSettings.borderSize + 3, 0), Mathf.RoundToInt(Vector2Int.Distance(entrancePos, exitPos) / 7), 0, false);
			if (points != null) {
				for (int j = 0; j < points.Length; j++) {
					/*int n = SeedManager.Instance.dungeonSeed.Next (dungeonSettings.minPlatformSize, maxPlatformSize);
					for (int k = Mathf.RoundToInt(-n / 2); k < Mathf.RoundToInt(n / 2) + ((n % 2 == 0) ? 0 : 1); k++) {
						room.CreateTile (points [j].x + k, points [j].y, "brick_2");
					}*/
					Instantiate(platformPrefab, room.transform.TransformPoint(points[j] + new Vector2(0.5f, 0.5f)), Quaternion.identity, room.transform);
				//	validSpawnLocations.Add (new Vector2 (points [j].x - 2, points [j].y + 1));
					validSpawnLocations.Add (new Vector2 (points [j].x - 1, points [j].y + 1));
					validSpawnLocations.Add (new Vector2 (points [j].x, points [j].y + 1));
					validSpawnLocations.Add (new Vector2 (points [j].x + 1, points [j].y + 1));
				//	validSpawnLocations.Add (new Vector2 (points [j].x + 2, points [j].y + 1));

				}
			}
		}
		for (int x = dungeonSettings.borderSize; x < roomSize.x + dungeonSettings.borderSize; x++) {
			validSpawnLocations.Add (new Vector2 (x, dungeonSettings.borderSize));
		}

		for(int i = 0; i < validSpawnLocations.Count; i++) {
			if (dungeonSettings.showSpawningSpace) {
				PhantomUtil.VisualizeLocation (room.transform.TransformPoint (validSpawnLocations[i]), Color.magenta);
			}
			for (int j = 0; j < dungeonSettings.spawnables.Length; j++) {
				if (SeedManager.Instance.dungeonSeed.Next (1, 100) <= dungeonSettings.spawnables[j].odds) {
					room.CreateTileEntity (dungeonSettings.spawnables[j].prefab, new Vector2Int((int)validSpawnLocations[i].x, (int)validSpawnLocations[i].y));
					break;
				}
			}
		}



		return room.transform.TransformPoint (new Vector2 (sectionSize.x + dungeonSettings.borderSize * 2, exitPos.y));
	}
}

public struct ValueNumberValueSet<T, U> {
	public readonly T value1;
	public readonly int number;
	public readonly U value2;

	public ValueNumberValueSet(T value1, int number, U value2) {
		this.value1 = value1;
		this.number = number;
		this.value2 = value2;
	}
}
