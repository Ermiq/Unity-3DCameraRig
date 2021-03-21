using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numb.ActionCamera
{
	public class CameraRotator
	{
		CameraManager camManager;

		Quaternion TargetRotationV;
		Quaternion TargetRotationH;

		float currentAngularSpeedV;
		float currentAngularSpeedH;

		public CameraRotator(CameraManager manager)
		{
			this.camManager = manager;
		}

		public static float SmoothDelay(float source, float target, float delay, float dt)
		{
			return Mathf.Lerp(source, target, 1 - Mathf.Pow(delay, dt));
		}

		public void UpdateRotation(float inputMouseX, float inputMouseY)
		{
			TargetRotationV = camManager.CamRotationPivotV.localRotation;
			TargetRotationH = camManager.CamRotationPivotH.rotation;

			if (camManager.smooth)
			{
				currentAngularSpeedV = SmoothDelay(currentAngularSpeedV, inputMouseY, camManager.smoothDelay, Time.deltaTime);
				currentAngularSpeedH = SmoothDelay(currentAngularSpeedH, inputMouseX, camManager.smoothDelay, Time.deltaTime);
			}
			else
			{
				currentAngularSpeedV = inputMouseY;
				currentAngularSpeedH = inputMouseX;
			}

			TargetRotationV *= Quaternion.Euler(-currentAngularSpeedV, 0f, 0f);
			TargetRotationH *= Quaternion.Euler(0f, currentAngularSpeedH, 0f);

			if (camManager.ClampVerticalRotation)
				TargetRotationV = ClampRotationAroundXAxis(TargetRotationV);

			camManager.CamRotationPivotV.localRotation = TargetRotationV;
			camManager.CamRotationPivotH.rotation = TargetRotationH;
		}

		/// <summary>
		/// Smoothly return "Camera" gameObject back to the original position after recoils, head bobs and other effects.
		/// </summary>
		public void CenterCamera()
		{
			if (camManager.camMain.transform.localRotation != Quaternion.identity)
				camManager.camMain.transform.localRotation = Quaternion.Slerp(
					camManager.camMain.transform.localRotation, Quaternion.identity, Time.deltaTime);
		}

		Quaternion ClampRotationAroundXAxis(Quaternion q)
		{
			q.x /= q.w;
			q.y /= q.w;
			q.z /= q.w;
			q.w = 1.0f;

			float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
			angleX = Mathf.Clamp(angleX, camManager.MinimumX, camManager.MaximumX);
			q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

			return q;
		}
	}
}