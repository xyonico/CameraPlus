using System;
using System.Globalization;
using UnityEngine;
using VRUIControls;

namespace CameraPlus
{
	public class CameraMoverPointer : VRPointer
	{
		private Transform _grabbedCamera;
		private VRController _grabbingController;
		private Vector3 _grabPos;
		private Quaternion _grabRot;
		private Vector3 _realPos;
		private Quaternion _realRot;
		private const float MinDistance = 0.25f;

		public override void Update()
		{
			base.Update();
			if (vrController != null)
				if (vrController.triggerValue > 0.9f)
				{
					if (_grabbingController != null) return;
					if (Physics.Raycast(vrController.position, vrController.forward, out var hit, _defaultLaserPointerLength))
					{
						if (hit.transform.name != "CameraCube") return;
						_grabbedCamera = hit.transform;
						_grabbingController = vrController;
						_grabPos = vrController.transform.InverseTransformPoint(_grabbedCamera.position);
						_grabRot = Quaternion.Inverse(vrController.transform.rotation) * _grabbedCamera.rotation;
					}
				}

			if (_grabbingController == null || !(_grabbingController.triggerValue <= 0.9f)) return;
			if (_grabbingController == null) return;
			SaveToConfig();
			_grabbingController = null;
		}

		private void LateUpdate()
		{
			if (_grabbedCamera == null) return;
			if (_grabbingController != null)
			{
				var diff = _grabbingController.verticalAxisValue * Time.deltaTime;
				if (_grabPos.magnitude > MinDistance)
				{
					_grabPos -= Vector3.forward * diff;
				}
				else
				{	
					_grabPos -= Vector3.forward * Mathf.Clamp(diff, float.MinValue, 0);
				}
				_realPos = _grabbingController.transform.TransformPoint(_grabPos);
				_realRot = _grabbingController.transform.rotation * _grabRot;
			}

			_grabbedCamera.position = Vector3.Lerp(_grabbedCamera.position, _realPos,
				Plugin.Config.positionSmooth * Time.deltaTime);

			_grabbedCamera.rotation = Quaternion.Slerp(_grabbedCamera.rotation, _realRot,
				Plugin.Config.rotationSmooth * Time.deltaTime);

			CameraPlusBehaviour.ThirdPersonPos = _grabbedCamera.position;
			CameraPlusBehaviour.ThirdPersonRot = _grabbedCamera.eulerAngles;
		}

		private void SaveToConfig()
		{
			var pos = _grabbedCamera.position;
			var rot = _grabbedCamera.eulerAngles;
			
			Plugin.Config.posx = pos.x;
			Plugin.Config.posy = pos.y;
			Plugin.Config.posz = pos.z;
			
			Plugin.Config.angx = rot.x;
			Plugin.Config.angy = rot.y;
			Plugin.Config.angz = rot.z;
			
			Plugin.Config.Save();
		}
	}
}