using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IHittable
{
    [SerializeField] private Vector3 startingPosition;
    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] private float moveSpeed;
    private float progress;
    private int direction = 1;
    private SpriteRenderer spriteRenderer;
    public LootTable lootTable;

    private void Start()
    {
        transform.Translate(new Vector3(0, 0.5f, 0));
        startingPosition = transform.position;
        offsetPosition = new Vector3(Random.Range(-2, 2), 0, 0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        lootTable = new LootTable(new LootTableItem[] {
			new LootTableItem ("coin", 2, 5, 100, 50),
            new LootTableItem ("silver_coin", 1, 3, 100, 60)
        });
    }



    private void Update()
    {
        progress += GameManager.DeltaTime * moveSpeed / 10f * direction;

        if (progress > 1)
        {
            progress = 1;
            direction = -1;
        }
        else if (progress < 0)
        {
            progress = 0;
            direction = 1;
        }

        transform.position = Vector3.Lerp(startingPosition, startingPosition + offsetPosition, progress);

        spriteRenderer.flipX = direction == -1;
    }

    public void Hit()
    {
        WorldItemManager.Instance.CreateItemsInWorld(lootTable.GetLootFromTable(), transform.position, new Vector2(12, 12));
        Destroy(gameObject);
    }
}