# CameraRig

Is designed for the camera construct that is made as following:

+-----+
|     | <CamRotationPivotH>
|     |  a gameObject is a child of the root game object (player or something else),
+-----+  it rotates around it's Y axis only in World space.
   |
   |
   +----+
   |    | <CamRotationPivotV> is a child of the <CamRotationPivotH>,
   +----+  it rotates around its X axis only in Local space.
      |
      |
      +----+
      |    | <Camera> component is attached to this game object
      +----+  that is a child of the <CamRotationPivotV>.
              It could be rotated anyhow depending on different effects and states of the player.
              The rotation should be made in Local space (with <cam.transform.localRotation>)
              so this 'effects' rotation won't affect the upper level transforms.

Simply attach the <ActionCamera.cs> script to a GameObject and it will create the rig automagically on application start.
Also, the pivot transform around which the camera should rotate could be set via the public <TargetToFollow> property.
