using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionCamera
{
	public class CameraRotator
	{
		ActionCamera actionCamera;

		Quaternion TargetRotationV;
		Quaternion TargetRotationH;

		float currentAngularSpeedV;
		float currentAngularSpeedH;

		public CameraRotator(ActionCamera manager)
		{
			this.actionCamera = manager;
		}

		public void UpdateRotation(float inputMouseX, float inputMouseY)
		{
			TargetRotationV = actionCamera.CamRotationPivotV.localRotation;
			TargetRotationH = actionCamera.CamRotationPivotH.rotation;

			if (actionCamera.smooth)
			{
				currentAngularSpeedV = actionCamera.SmoothDelay(currentAngularSpeedV, inputMouseY, actionCamera.smoothDelay, Time.deltaTime);
				currentAngularSpeedH = actionCamera.SmoothDelay(currentAngularSpeedH, inputMouseX, actionCamera.smoothDelay, Time.deltaTime);
			}
			else
			{
				currentAngularSpeedV = inputMouseY;
				currentAngularSpeedH = inputMouseX;
			}

			TargetRotationV *= Quaternion.Euler(-currentAngularSpeedV, 0f, 0f);
			TargetRotationH *= Quaternion.Euler(0f, currentAngularSpeedH, 0f);

			if (actionCamera.ClampVerticalRotation)
				TargetRotationV = ClampRotationAroundXAxis(TargetRotationV);

			actionCamera.CamRotationPivotV.localRotation = TargetRotationV;
			actionCamera.CamRotationPivotH.rotation = TargetRotationH;
		}

		/// <summary>
		/// Smoothly return "Camera" gameObject back to the original position after recoils, head bobs and other effects.
		/// </summary>
		public void CenterCamera()
		{
			if (actionCamera.Cam.transform.localRotation != Quaternion.identity)
				actionCamera.Cam.transform.localRotation = Quaternion.Slerp(
					actionCamera.Cam.transform.localRotation, Quaternion.identity, Time.deltaTime);
		}

		Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1.0f;

			float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
			angleX = Mathf.Clamp(angleX, actionCamera.MinimumX, actionCamera.MaximumX);
			q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

			return q;
		}
	}
}
