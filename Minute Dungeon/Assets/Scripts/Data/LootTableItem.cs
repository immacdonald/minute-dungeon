using System.Collections;
using System.Collections.Generic;

public struct LootTableItem  {
	public readonly string loot;
	public readonly int minAmount;
	public readonly int maxAmount;
	public readonly int dropChance;
	public readonly int dropChancePerExtra;

	public LootTableItem(string loot, int minAmount, int maxAmount, int dropChance, int dropChancePerExtra) {
		this.loot = loot;
		this.minAmount = minAmount;
		this.maxAmount = maxAmount;
		this.dropChance = dropChance;
		this.dropChancePerExtra = dropChancePerExtra;
	}

}
