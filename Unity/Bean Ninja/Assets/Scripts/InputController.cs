using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputState))]
public class InputController : MonoBehaviour {

	public float deadZone;
	
	InputState inputState;
	
	void Start () {
		inputState = GetComponent<InputState>();
	}

	void Update () {
		inputState.leftDown = Input.GetAxis("Horizontal") < -deadZone;
		inputState.rightDown = Input.GetAxis("Horizontal") > deadZone;
		inputState.jumpDown = Input.GetButton("Jump");
	}
}
