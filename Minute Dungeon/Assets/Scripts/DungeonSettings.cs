using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDungeonSettings", menuName = "Minute Dungeon/Dungeon Settings", order = 1)]
public class DungeonSettings : ScriptableObject
{
	[Header("Dungeon Size Settings")]
	[Range(1, 100)]
	public int numberOfRooms = 5;
	public Vector2Int minRoomSize = new Vector2Int(10, 10);
	public Vector2Int maxRoomSize = new Vector2Int(20, 20);
	public int tunnelHeight = 5;
	public int borderSize = 2;


	[Header("Platform Settings")]
	public bool generatePlatforms;
	[Range(1, 8)]
	public int minPlatformSize = 2;
	[Range(1, 8)]
	public int maxPlatformSize = 6;

	//All chance vars are x/100
	[Header("Tile Entities")]
	public Spawnable[] spawnables;

	[Header("Debug")]
	public bool debugCriticalPlatforms;
	public bool debugSpawningSpace;
	public bool showSpawningSpace;
	
}

[System.Serializable]
public struct Spawnable
{
	public string name;
	public GameObject prefab;
	public int odds;

    public Spawnable(string name, GameObject prefab, int odds)
    {
        this.name = name;
        this.prefab = prefab;
        this.odds = odds;
    }
}