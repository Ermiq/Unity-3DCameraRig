using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numb.ActionCamera {

	/// <summary>
	/// Is designed for the camera construct that is made as following:
	/// <CamRotationPivotH> gameObject is a root game object, and it considered to rotate around it's Y axis only (and its rotation is in World space).
	/// <CamRotationPivotV> is a child of the <CamRotationPivotH>, it rotates around its X axis only (its rotation is in Local space).
	/// <Camera> component is attached to the game object that is a child of the <CamRotationPivotV>. It could be rotated anyhow depending on different effects and states of the player.
	/// </summary>
	public class CameraManager : MonoBehaviour {

		[Header ("Example fields")]
		private bool CameraEnabled;
		public KeyCode PauseButton = KeyCode.Escape;

		public Transform player;
		public float playerHeight = 1.8f;
		
		public bool bIsFPSCameraModeActive = true;
		public Camera camMain;
		public Transform CamRotationPivotH;
		public Transform CamRotationPivotV;

		public float defaultFOV = 90f;
		private float targetFOV = 90f;

		public bool ClampVerticalRotation = true;
		public float MinimumX = -80F;
		public float MaximumX = 90F;

		public bool smooth = true;
		public float smoothTime = 60f;
		[Range (0.000001f, 0.1f)] public float smoothDelay = 0.00001f;
		[Range (0.1f, 10f)] public float XSensitivity = 6f;
		[Range (0.1f, 10f)] public float YSensitivity = 6f;

		private CameraRotator cameraRotator;
		private CameraMover cameraMover;
		
		private float inputMouseX;
		private float inputMouseY;

		void Awake () {
			CameraEnabled = false;
			ToggleCursorLock ();

			CamRotationPivotH = GameObject.Find ("CamRotationPivotH").transform;
			CamRotationPivotV = GameObject.Find ("CamRotationPivotV").transform;
			camMain = GameObject.Find ("Camera").transform.GetComponent<Camera> ();
		}

		void Start () {

			cameraMover = new CameraMover (this);
			cameraRotator = new CameraRotator (this);
		}

		public bool SwitchCameraMode () {
			bIsFPSCameraModeActive = !bIsFPSCameraModeActive;
			return bIsFPSCameraModeActive;
		}

		void ToggleCursorLock () {
			CameraEnabled = !CameraEnabled;
			Cursor.lockState = CameraEnabled ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !CameraEnabled;
		}

		void Update () {
			if (Input.GetKeyUp (PauseButton))
				ToggleCursorLock ();

			if (!CameraEnabled)
				return;

			if (camMain.fieldOfView != targetFOV)
				camMain.fieldOfView = Mathf.Lerp (camMain.fieldOfView, targetFOV, 1 - Mathf.Pow (0.01f, Time.deltaTime));

			inputMouseX = Input.GetAxisRaw ("Mouse X") * XSensitivity;
			inputMouseY = Input.GetAxisRaw ("Mouse Y") * YSensitivity;

			cameraMover.UpdateCameraPosition ();
			cameraMover.AdjustCameraPosition ();
			cameraRotator.UpdateRotation (inputMouseX, inputMouseY);

			if (Input.GetKey (KeyCode.U))
				smooth = !smooth;
		}
	}
}