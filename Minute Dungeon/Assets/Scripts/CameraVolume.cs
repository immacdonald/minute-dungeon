using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraVolume : MonoBehaviour
{
    public static bool DEBUG_VOLUMES = false;
    public static int DEFAULT_CAMERA_SIZE = 9;
    public bool useDefaultSize = true;
    public int cameraSize = 9;
    public PolygonCollider2D volume;

    void Start() {
        if(!DEBUG_VOLUMES) {
            GetComponent<SpriteRenderer>().color = Color.clear;
        }

        volume = GetComponent<PolygonCollider2D>();
    }

    public int GetCameraSize()
    {
        return useDefaultSize ? DEFAULT_CAMERA_SIZE : cameraSize;
    }
}
