using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStare : MonoBehaviour
{
    private Camera cam;
    private void Awake() => cam = Camera.main;
    void Update() => transform.rotation = cam.transform.rotation;
}
