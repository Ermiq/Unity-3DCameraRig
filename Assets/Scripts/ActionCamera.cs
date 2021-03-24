using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionCamera
{

	/// <summary>
	/// Is designed for the camera construct that is made as following:
	/// <CamRotationPivotH> gameObject is a root game object, and it considered to rotate around it's Y axis only (and its rotation is in World space).
	/// <CamRotationPivotV> is a child of the <CamRotationPivotH>, it rotates around its X axis only (its rotation is in Local space).
	/// <Camera> component is attached to the game object that is a child of the <CamRotationPivotV>. It could be rotated anyhow depending on different effects and states of the player.
	/// </summary>
	public class ActionCamera : MonoBehaviour
	{
		private bool CameraEnabled;
		private float targetFOV = 90f;
		private CameraRotator cameraRotator;
		private CameraMover cameraMover;
		private float inputMouseX;
		private float inputMouseY;

		public Transform TargetToFollow;
		
		public KeyCode PauseButton = KeyCode.Escape;

		public float Height = 2f;

		public bool FPSMode;
		public Camera Cam;
		public Transform CamRotationPivotH;
		public Transform CamRotationPivotV;

		public float defaultFOV = 90f;
		
		public bool ClampVerticalRotation = true;
		public float MinimumX = -80F;
		public float MaximumX = 90F;

		public bool smooth = true;
		[Range(1f, 10f)] public float smoothDelay = 4f;
		[Range(0.1f, 10f)] public float XSensitivity = 6f;
		[Range(0.1f, 10f)] public float YSensitivity = 6f;
		
		void Awake()
		{
			BuildRig();

			CameraEnabled = false;
			ToggleCursorLock();
		}

		void Start()
		{
			if (!TargetToFollow)
				TargetToFollow = transform;
			
			CamRotationPivotH = GameObject.Find("CamRotationPivotH").transform;
			CamRotationPivotV = GameObject.Find("CamRotationPivotV").transform;
			Cam = GameObject.Find("Cam").transform.GetComponent<Camera>();

			cameraMover = new CameraMover(this);
			cameraRotator = new CameraRotator(this);
		}

		void BuildRig()
		{
			GameObject go = new GameObject();
;
			GameObject pivotH = GameObject.Instantiate(go, new Vector3(0, Height, 0), Quaternion.identity, transform);
			pivotH.name = "CamRotationPivotH";
			//pivotH.transform.localPosition = new Vector3(0, Height, 0);
			
			GameObject pivotV = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity, pivotH.transform);
			pivotV.name = "CamRotationPivotV";
			pivotV.transform.localPosition = Vector3.zero;

			GameObject cam = GameObject.Instantiate(go, Vector3.zero, Quaternion.identity, pivotV.transform);
			cam.name = "Cam";
			cam.AddComponent<Camera>();
		}

		public bool SwitchCameraMode()
		{
			FPSMode = !FPSMode;
			return FPSMode;
		}

		void ToggleCursorLock()
		{
			CameraEnabled = !CameraEnabled;
			Cursor.lockState = CameraEnabled ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !CameraEnabled;
		}

		void Update()
		{
			if (Input.GetKeyUp(PauseButton))
				ToggleCursorLock();

			if (!CameraEnabled)
				return;

			if (Cam.fieldOfView != targetFOV)
				Cam.fieldOfView = SmoothDelay(Cam.fieldOfView, targetFOV, smoothDelay, Time.deltaTime);

			inputMouseX = Input.GetAxisRaw("Mouse X") * XSensitivity;
			inputMouseY = Input.GetAxisRaw("Mouse Y") * YSensitivity;

			cameraMover.UpdateCameraPosition();
			cameraMover.AdjustCameraPosition();
			cameraRotator.UpdateRotation(inputMouseX, inputMouseY);

			if (Input.GetKey(KeyCode.U))
				smooth = !smooth;
		}

		public float SmoothDelay(float source, float target, float delay, float dt)
		{
			// Old method used Mathf.Pow and the delay param had to be like 0.0001f and the smaller it was the less smoothing.
			//return Mathf.Lerp(source, target, 1 - Mathf.Pow(delay, dt));

			// With Mathf.Exp method the delay could be like 1 to 10, the less the value the more smooth.
			return Mathf.Lerp(source, target, 1f - Mathf.Exp(-delay * dt));
		}

		public Vector3 SmoothDelay(Vector3 source, Vector3 target, float delay, float dt)
		{
			return Vector3.Lerp(source, target, 1f - Mathf.Exp(-delay * dt));
		}
	}
}
