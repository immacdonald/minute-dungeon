using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ValueNumberPair<T> {
	public T value;
	public int number;

	public ValueNumberPair(T value, int number) {
		this.value = value;
		this.number = number;
	}

}
