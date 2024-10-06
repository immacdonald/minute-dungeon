using UnityEngine;

public class DroppedItem : MonoBehaviour, IInteractable {
	private string item;
	[SerializeField] private AudioSource pickupSound;
	private ParticleSystem particles;

	public void Initalize (string item) {
		this.item = item;

		GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite>($"sprites/{item}");
		gameObject.GetComponent<SpriteRenderer>().enabled = true;
		particles = GetComponent<ParticleSystem>();
	}

	public string Interact()
    {
		particles.Play();
		pickupSound.Play();
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		return item;
	}
}
