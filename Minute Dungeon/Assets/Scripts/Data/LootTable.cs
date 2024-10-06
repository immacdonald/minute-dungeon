using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootTable
{
	public readonly LootTableItem[] lootTableItems;

	public LootTable(LootTableItem[] lootTableItems)
	{
		this.lootTableItems = lootTableItems;
	}

	public ValueNumberPair<string>[] GetLootFromTable()
	{
		List<ValueNumberPair<string>> droppedItems = new List<ValueNumberPair<string>>();
		for (int i = 0; i < lootTableItems.Length; i++)
		{
			int itemNumDropped = 0;
			for (int j = 0; j < lootTableItems[i].maxAmount; j++)
			{
				if (j < lootTableItems[i].minAmount && lootTableItems[i].dropChance >= SeedManager.Instance.lootSeed.Next(0, 100))
				{
					itemNumDropped++;
				}
				else if (itemNumDropped >= lootTableItems[i].minAmount)
				{
					if (lootTableItems[i].dropChancePerExtra >= SeedManager.Instance.lootSeed.Next(0, 100))
					{
						itemNumDropped++;
					}
					else
					{
						break;
					}
				}
			}
			if (itemNumDropped > 0)
			{
				droppedItems.Add(new ValueNumberPair<string>(lootTableItems[i].loot, itemNumDropped));
			}
		}
		return droppedItems.ToArray();
	}
}
