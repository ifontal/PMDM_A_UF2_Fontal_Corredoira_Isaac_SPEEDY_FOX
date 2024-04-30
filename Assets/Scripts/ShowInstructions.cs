using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShowInstructions : MonoBehaviour
{
    GameObject instructions;
    private void Start() {
        instructions = GameObject.Find("GUI").transform.Find("Instructions").gameObject;
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.I)) {
            instructions.SetActive(!instructions.activeSelf);
        }
    }
}
