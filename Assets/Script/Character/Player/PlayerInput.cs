using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	[Range(50, 300)]
	public float MouseSensitivity = 50f;

	public float Move_X;
	public float Move_Y;
	public float MouseX;
	public float MouseXRaw;
	public bool Action1IsPressed;
	public bool Action1WasPressed;
	public bool Action2IsPressed;
	public bool Action2WasPressed;
	public bool ActionPressed;

	private bool DebugEnabled;

	// Update is called once per frame
	void Update () {
		
		Move_X = Input.GetAxisRaw(CONSTANTS.INPUT.HORIZONTAL_AXIS);
		Move_Y = Input.GetAxisRaw(CONSTANTS.INPUT.VERTICAL_AXIS);
		MouseXRaw = Input.GetAxis(CONSTANTS.INPUT.MOUSE_X);
		MouseX = MouseXRaw * MouseSensitivity;

		Action1IsPressed = Input.GetMouseButton(CONSTANTS.INPUT.MOUSE_LEFT_BUTTON);
		Action1WasPressed = Input.GetMouseButtonDown(CONSTANTS.INPUT.MOUSE_LEFT_BUTTON);

		Action2IsPressed = Input.GetMouseButton(CONSTANTS.INPUT.MOUSE_RIGHT_BUTTON);
		Action2WasPressed = Input.GetMouseButton(CONSTANTS.INPUT.MOUSE_RIGHT_BUTTON);

		ActionPressed = Input.GetAxisRaw("Action_KeyBoard") == 1;

		// Habilitar o Debug das Variaveis de Controle
		if (Input.GetKeyDown(KeyCode.F1))
			DebugEnabled = !DebugEnabled;
	}

	void OnGUI()
	{
		if (DebugEnabled)
			DrawDebugMode();		
	}

	void DrawDebugMode()
	{
		float newLineSize = 15f;
		float LabelSize = 250f;
		float sizeXPercent = Screen.width * Mathf.Abs(((LabelSize / Screen.width) - 1));

		GUIStyle _guiStyle = new GUIStyle();
		_guiStyle.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		_guiStyle.fontStyle = FontStyle.Bold;

		Vector2 StarPosition = new Vector2(sizeXPercent, Screen.height * 0.10f);
		Vector2 CurrentPosition = StarPosition;

		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), "Player Input DEBUG MODE", _guiStyle);
		CurrentPosition.y += newLineSize * 2;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Move_X: {0}", Move_X.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Move_Y: {0}", Move_Y.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("MouseX Raw: {0}", MouseXRaw.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Mouse Sensitivy: {0}", MouseSensitivity.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("MouseX: {0}", MouseX.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Action1IsPressed: {0}", Action1IsPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Action1WasPressed: {0}", Action1WasPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Action2IsPressed: {0}", Action2IsPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("Action2WasPressed: {0}", Action2WasPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
		GUI.Label(new Rect(CurrentPosition.x, CurrentPosition.y, LabelSize, LabelSize), string.Format("ActionPressed: {0}", ActionPressed.ToString()), _guiStyle);
		CurrentPosition.y += newLineSize;
	}
}