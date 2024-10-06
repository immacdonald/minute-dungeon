using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour, IInteractable
{
    [SerializeField] public bool hasGravity = false;
    [SerializeField] private float rotationSpeed = 4;
    [SerializeField] private float waveSpeed = 1;
    [SerializeField] [Range(0, 2)] private float heightLimit = 1;
    public string type;
    Vector3 rotation = Vector3.zero;
    Vector3 position;
    private float startingY;
    [SerializeField] private ParticleSystem destructionParticles;
    private AudioSource collectSound;

    private void Start()
    {
        if (!hasGravity)
        {
            position = transform.position;
            startingY = position.y;
        }
        TryGetComponent(out collectSound);
    }

    private void Update()
    {
        if (!hasGravity)
        {
            rotation.z += rotationSpeed * GameManager.DeltaTime;
            transform.eulerAngles = rotation;
            position.y = startingY + Mathf.Sin(GameManager.GameTime * waveSpeed) * heightLimit;
            transform.position = position;
        }
    }

    public string Interact()
    {
        Debug.Log($"Interacted with {type}");
        StartCoroutine(DestructionDelay());
        return type;
    }

    private IEnumerator DestructionDelay()
    {
        if (collectSound) collectSound.Play();
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
