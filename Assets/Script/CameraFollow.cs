using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private void FixedUpdate() {
        if (GameObject.FindGameObjectWithTag("Player") != null) {
            Vector3 desirePosition = target.position + offset;
            Vector3 SmoothedPosition = Vector3.Lerp(transform.position, desirePosition, smoothSpeed);
            transform.position = SmoothedPosition;
        }
    }
}
