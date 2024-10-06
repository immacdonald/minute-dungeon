using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : MonoBehaviour, IHittable 
{
	public LootTable lootTable;
	[SerializeField] private AudioSource breakSound;

	void Start()
	{
		lootTable = new LootTable(new LootTableItem[] {
			//new LootTableItem ("emerald", 1, 2, 50, 25),
			new LootTableItem ("coin", 1, 4, 100, 65),
			new LootTableItem ("silver_coin", 0, 3, 80, 80)
		});
	}

	public void Hit()
	{
		Debug.Log("Hit Vase");
		Shatter();
	}

	void Shatter() {
		breakSound.Play();
		WorldItemManager.Instance.CreateItemsInWorld(lootTable.GetLootFromTable(), transform.position, new Vector2(12, 12));
		StartCoroutine(DestroySequence());
	}

	private IEnumerator DestroySequence()
    {
		GetComponent<BoxCollider2D>().enabled = false;
		GetComponent<SpriteRenderer>().enabled = false;
		yield return new WaitForSeconds(1);
		Destroy(gameObject);
	}
}
