using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : Singleton<SeedManager> {

	public CustomRandom dungeonSeed;
	[SerializeField]
	private bool randomDungeonSeed;
	[SerializeField]
	private int dungeonSeedInt;


	public CustomRandom lootSeed;
	[SerializeField]
	private bool randomLootSeed;
	[SerializeField]
	private int lootSeedInt;


	private void Awake() {
		lootSeed = new CustomRandom(((randomLootSeed) ? Random.Range(-100000, 100000) : lootSeedInt));
		dungeonSeed = new CustomRandom(((randomDungeonSeed) ? Random.Range(-100000, 100000) : dungeonSeedInt));
	}
}
