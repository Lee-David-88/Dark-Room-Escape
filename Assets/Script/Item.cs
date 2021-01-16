using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

    public Camera puzzleCam;
    public Text instructionText;

    private void OnTriggerEnter(Collider other) {
        instructionText.enabled = true;

        if (Input.GetKeyDown(KeyCode.E)) {
            Camera.main.enabled = false;
        }
        if (Input.GetKey(KeyCode.Escape)) {
            Camera.main.enabled = true;
        }
    }
    void OnTriggerStay(Collider other) {
        if (Input.GetKeyDown(KeyCode.E)) {
            Camera.main.enabled = false;
        }
        if(Input.GetKey(KeyCode.Escape)) {
            Camera.main.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        instructionText.enabled = false;
    }
}
