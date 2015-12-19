using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	[Range(50, 300)]
	public float MouseSensitivity = 50f;

	public float Move_X;
	public float Move_Y;
	public float MouseX;
	public bool Action1IsPressed;
	public bool Action1WasPressed;
	public bool Action2IsPressed;
	public bool Action2WasPressed;

	// Update is called once per frame
	void Update () {
		
		Move_X = Input.GetAxisRaw(CONSTANTS.INPUT.HORIZONTAL_AXIS);
		Move_Y = Input.GetAxisRaw(CONSTANTS.INPUT.VERTICAL_AXIS);
		MouseX = Input.GetAxis(CONSTANTS.INPUT.MOUSE_X) * MouseSensitivity;

		Action1IsPressed = Input.GetMouseButton(CONSTANTS.INPUT.MOUSE_LEFT_BUTTON);
		Action1WasPressed = Input.GetMouseButtonDown(CONSTANTS.INPUT.MOUSE_LEFT_BUTTON);

		Action2IsPressed = Input.GetMouseButton(CONSTANTS.INPUT.MOUSE_RIGHT_BUTTON);
		Action2WasPressed = Input.GetMouseButton(CONSTANTS.INPUT.MOUSE_RIGHT_BUTTON);

	}
}
