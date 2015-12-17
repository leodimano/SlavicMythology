using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	[Range(50, 300)]
	public float MouseSensitivity = 50f;

	[HideInInspector]
	public float Move_X;
	[HideInInspector]
	public float Move_Y;
	[HideInInspector]
	public float MouseX;

	// Update is called once per frame
	void Update () {
	
		Move_X = Input.GetAxisRaw(CONSTANTS.INPUT.HORIZONTAL_AXIS);
		Move_Y = Input.GetAxisRaw(CONSTANTS.INPUT.VERTICAL_AXIS);
		MouseX = Input.GetAxis(CONSTANTS.INPUT.MOUSE_X) * MouseSensitivity;

	}
}
