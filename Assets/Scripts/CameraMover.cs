using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Numb.ActionCamera
{
	/// <summary>
	/// Switching FPS/TPS modes, aiming down sights mode, keeps camera at the position in front of character's head, etc.
	/// </summary>
	/// <code>
	/// new CameraMover (CameraManager manager)
	/// </code>
	[Serializable]
	public class CameraMover
	{
		CameraManager camManager;

		Vector3 cameraLocalPosition;
		Vector3 cameraPivotPositionRaw;
		Vector3 cameraPivotPosition;
		Vector3 vDirectionFromCameraToPivot;
		// Avoiding obstacles in 3rd person mode
		float distanceToObstacles;
		RaycastHit obstaclesHitInfo;

		// For 3rd person mode
		public float defaultHorizontalOffset = 0f;
		public float maxDistanceFromCamera = 4f;
		
		public CameraMover(CameraManager manager)
		{
			camManager = manager;
		}

		/// <summary>
		/// Updates camera prefered position (for FPS mode, 3rd person mode and for aiming down sights).
		/// </summary>
		public void UpdateCameraPosition()
		{
			// 1st person position
			if (camManager.bIsFPSCameraModeActive)
				cameraLocalPosition = Vector3.zero;
			// 3rd person position
			else
			{
				UpdateDistanceToObstacles(maxDistanceFromCamera);
				cameraLocalPosition.Set(defaultHorizontalOffset, 0f, -distanceToObstacles);
			}
		}

		/// <summary>
		/// Moves the camera to preferred position.
		/// </summary>
		public void AdjustCameraPosition()
		{
			cameraPivotPositionRaw = camManager.player.position + Vector3.up * camManager.playerHeight + camManager.player.forward * 0.2f;
			cameraPivotPosition = cameraPivotPositionRaw;

			camManager.CamRotationPivotH.position = cameraPivotPosition;
			camManager.camMain.transform.localPosition = cameraLocalPosition;
		}

		///<summary>
		/// Calculates the distance from the character's back to the closest obstacle on the scene (to move 3rd persson camera closer to the character if needed)
		///</summary>
		private void UpdateDistanceToObstacles(float distance)
		{
			vDirectionFromCameraToPivot = camManager.camMain.transform.position - camManager.CamRotationPivotH.position;
			Physics.Raycast(camManager.CamRotationPivotH.position, vDirectionFromCameraToPivot.normalized, out obstaclesHitInfo, distance,
				Physics.AllLayers, QueryTriggerInteraction.Ignore);
			if (obstaclesHitInfo.transform && obstaclesHitInfo.transform.root != camManager.player)
				distanceToObstacles = Vector3.Distance(camManager.CamRotationPivotH.position, obstaclesHitInfo.point) - 0.1f;
			else
				distanceToObstacles = distance;
		}
	}
}