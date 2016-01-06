﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
	
	[Range(0, 10)]
	public float CameraOffSetZ;
	[Range(0, 10)]
	public float CameraOffSetY;
	public float CameraOffSetHeight = 4;
	public float FixCameraRate;
	public float FixBackCameraRate;

	float _fixCameraOffSetZ;
	float _fixCameraOffSetY;
	bool _isPlayerVisibileNow;

	RaycastHit _rayCastHit;
	Camera _camera;

	// Use this for initialization
	void Start () {	
		_camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {

		PositionCamera(CameraOffSetY, CameraOffSetZ);

		if (!IsPlayerVisible())
		{
			if (!_isPlayerVisibileNow){
				_fixCameraOffSetY += FixCameraRate * Time.deltaTime;
				_fixCameraOffSetZ -= FixCameraRate * Time.deltaTime;
			}

			PositionCamera(CameraOffSetY + _fixCameraOffSetY, CameraOffSetZ + _fixCameraOffSetZ);

			_isPlayerVisibileNow = IsPlayerVisible();
		}
		else
		{
			// Limite para corrigir o eixo Y
			if (_fixCameraOffSetY < 0.1)
				_fixCameraOffSetY = 0;
			else
			{
				_fixCameraOffSetY -= FixBackCameraRate * Time.deltaTime;
			}

			// Limite para corrigir o eixo Z
			if (_fixCameraOffSetZ > -0.1)
				_fixCameraOffSetZ = 0;
			else
			{
				_fixCameraOffSetZ += FixBackCameraRate * Time.deltaTime;
			}
		}

		PositionCamera(CameraOffSetY + _fixCameraOffSetY, CameraOffSetZ + _fixCameraOffSetZ);
	}

	void PositionCamera(float cameraOffSetY_, float cameraOffSetZ_)
	{
		/* Atualizacao da Camera */
		Vector3 newCameraPosition = new Vector3(ApplicationModel.Instance.CurrentPlayer.transform.position.x + ApplicationModel.Instance.CurrentPlayer.transform.forward.x * -1 * cameraOffSetZ_,
			ApplicationModel.Instance.CurrentPlayer.transform.position.y + cameraOffSetY_,
			ApplicationModel.Instance.CurrentPlayer.transform.position.z + ApplicationModel.Instance.CurrentPlayer.transform.forward.z * -1 * cameraOffSetZ_);
		
		_camera.transform.position = newCameraPosition + Vector3.up;

		// Rotaciona a camera para o personagem
		_camera.transform.LookAt(ApplicationModel.Instance.CurrentPlayer.transform.position);
		// Aplica o Offset da Camera

		//Aplica o Offset da Camera na altura, para o personagem ficar abaixo do centro da câmera
		_camera.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y + CameraOffSetHeight, _camera.transform.position.z);
	}

	bool IsPlayerVisible()
	{
		Vector3 _cameraPosition = _camera.transform.position;
		Vector3 _rayDirection =  ApplicationModel.Instance.CurrentPlayer.transform.position - _cameraPosition;

		if (Physics.Raycast(_cameraPosition, _rayDirection, out _rayCastHit, 500f))
		{
			Debug.DrawRay(_cameraPosition, _rayCastHit.point - _cameraPosition, Color.blue);

			if (_rayCastHit.collider != null && _rayCastHit.collider.CompareTag(CONSTANTS.TAGS.PLAYER)){
				Debug.DrawRay(_cameraPosition, _rayCastHit.point - _cameraPosition, Color.green);
				return true;
			}
			else{
				Debug.DrawRay(_cameraPosition, _rayDirection * _rayCastHit.distance, Color.red);
			}
		}

		return false;
	}
}
