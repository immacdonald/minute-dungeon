using UnityEngine;

public class DroppedItem : MonoBehaviour, IInteractable {
	private string item;
	[SerializeField] private AudioSource pickupSound;

	public void Initalize (string item) {
		this.item = item;

		Debug.Log($"Loading sprites/{item}");
		GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite>($"sprites/{item}");
		//gameObject.GetComponent<SpriteRenderer>().enabled = true;
		//gameObject.GetComponent<BoxCollider2D>().enabled = true;
	}

	public string Interact()
    {
		pickupSound.Play();
		gameObject.GetComponent<SpriteRenderer>().enabled = false;
		gameObject.GetComponent<BoxCollider2D>().enabled = false;
		return item;
	}
}
