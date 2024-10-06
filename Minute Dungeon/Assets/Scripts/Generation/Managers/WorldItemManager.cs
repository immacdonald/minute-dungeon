using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager : Singleton<WorldItemManager> {
	[SerializeField]
	public GameObject droppedItemPrefab;

	private void Start() {
		PoolManager.Instance.CreatePool (droppedItemPrefab, 15);
	}

	public void CreateItemInWorld(string item, Vector2 location, Vector2 maxSpread) {
		GameObject droppedItem = PoolManager.Instance.ReuseObject (droppedItemPrefab, location);
		droppedItem.GetComponent<DroppedItem>().Initalize (item);
		droppedItem.GetComponent<Rigidbody2D> ().AddForce (new Vector2(SeedManager.Instance.lootSeed.Next(-maxSpread.x, maxSpread.x) * 250, SeedManager.Instance.lootSeed.Next(0, maxSpread.y) * 250));
	}

	public void CreateItemsInWorld(ValueNumberPair<string>[] items, Vector2 location, Vector2 maxSpread) {
		for (int i = 0; i < items.Length; i++) {
			for (int j = 0; j < items[i].number; j++)
            {
				CreateItemInWorld(items[i].value, location, maxSpread);
			}
		}
	}
}
