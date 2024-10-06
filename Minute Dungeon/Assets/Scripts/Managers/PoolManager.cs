using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager> {

	Dictionary<int, Queue<GameObject>> poolDictionary = new Dictionary<int, Queue<GameObject>>();

	public void CreatePool(GameObject prefab, int poolSize) {
		int poolKey = prefab.GetInstanceID ();

		GameObject poolHolder = new GameObject (prefab.name + " pool");

		if (!poolDictionary.ContainsKey (poolKey)) {
			poolDictionary.Add (poolKey, new Queue<GameObject> ());

			for (int i = 0; i < poolSize; i++) {
				GameObject newObject = Instantiate(prefab, poolHolder.transform);
				newObject.SetActive(false);
				poolDictionary [poolKey].Enqueue (newObject);
			}
		}
	}

	public GameObject ReuseObject(GameObject prefab, Vector3 position) {
		int poolKey = prefab.GetInstanceID ();

		if (poolDictionary.ContainsKey (poolKey)) {
			GameObject objectToReuse = poolDictionary [poolKey].Dequeue ();
			poolDictionary [poolKey].Enqueue (objectToReuse);

			objectToReuse.SetActive (true);
			objectToReuse.transform.position = position;
			//rotation

			return objectToReuse;
		}
		return null;
	}
}
