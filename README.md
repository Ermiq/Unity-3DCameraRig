# CameraRig

Simply attach the `ActionCamera.cs` script to a GameObject and it will create the rig automagically on application start.  
Also, the pivot transform around which the camera should rotate could be set via the public `TargetToFollow` property.  
  
  
Designed for the camera construct that is made as following:

1. `CamRotationPivotH`:  
 a gameObject that could be attached to the root game object (player or something else) or the player object could be set via `TargetToFollow`
 it rotates around it's Y axis only in World space.  
  
2. `CamRotationPivotV`:  
 is a child of the `CamRotationPivotH`. It rotates around its X axis only in Local space.  
 This is how we avoid the Gimbal lock problem - by separating horizontal and vertical rotations.  
  
3. `Camera` component is attached to the game object that is a child of the `CamRotationPivotV`.  
 It could be rotated anyhow depending on different effects and states of the player.  
 The rotation if this object should be made in Local space (with `cam.transform.localRotation`) so this 'effects' rotation won't affect the upper level transforms.  
