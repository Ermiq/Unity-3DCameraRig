using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionCamera {

	/// <summary>
	/// Is designed for the camera rig that is made as following:
	/// <CamRotationPivotH> gameObject is a root game object, and it's considered to rotate around it's Y axis in World space.
	/// <CamRotationPivotV> is a child of the <CamRotationPivotH>, it rotates around its X axis in Local space.
	/// <Camera> component is attached to a game object that is a child of the <CamRotationPivotV>. It could be rotated anyhow
	///	depending on different effects and states of the player (recoil, head bob, sprint shake).
	/// </summary>
	public class CameraManager : MonoBehaviour
	{
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
		public float MinimumX = -80f;
		public float MaximumX = 90f;

		public bool smooth = true;
		public float smoothTime = 60f;
		[Range (0.000001f, 0.1f)] public float smoothDelay = 0.00001f;
		[Range (0.1f, 10f)] public float XSensitivity = 6f;
		[Range (0.1f, 10f)] public float YSensitivity = 6f;

		private bool CameraEnabled;
		
		private CameraRotator cameraRotator;
		private CameraMover cameraMover;
		
		private float inputMouseX;
		private float inputMouseY;

		void Awake ()
		{
			ToggleCursorLock ();

			CamRotationPivotH = GameObject.Find ("CamRotationPivotH").transform;
			CamRotationPivotV = GameObject.Find ("CamRotationPivotV").transform;
			camMain = GameObject.Find ("Camera").transform.GetComponent<Camera> ();
		}

		void Start ()
		{
			cameraMover = new CameraMover (this);
			cameraRotator = new CameraRotator (this);
		}

		public bool SwitchCameraMode ()
		{
			bIsFPSCameraModeActive = !bIsFPSCameraModeActive;
			return bIsFPSCameraModeActive;
		}

		void ToggleCursorLock ()
		{
			CameraEnabled = !CameraEnabled;
			Cursor.lockState = CameraEnabled ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !CameraEnabled;
		}

		void Update ()
		{
			if (Input.GetKeyUp (PauseButton))
				ToggleCursorLock ();
			
			if (Input.GetKeyUp (KeyCode.U))
				smooth = !smooth;

			if (!CameraEnabled)
				return;

			if (camMain.fieldOfView != targetFOV)
				camMain.fieldOfView = Mathf.Lerp (camMain.fieldOfView, targetFOV, 1f - Mathf.Pow (0.01f, Time.deltaTime));

			cameraMover.UpdateCameraPosition ();
			cameraMover.AdjustCameraPosition ();
			
			inputMouseX = Input.GetAxisRaw ("Mouse X") * XSensitivity;
			inputMouseY = Input.GetAxisRaw ("Mouse Y") * YSensitivity;
			cameraRotator.UpdateRotation (inputMouseX, inputMouseY);
		}
	}
}