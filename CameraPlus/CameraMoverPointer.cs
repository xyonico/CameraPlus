using System;
using System.Globalization;
using UnityEngine;
using VRUIControls;

namespace CameraPlus
{
	public class CameraMoverPointer : VRPointer
	{
		private Transform _cameraCube;
		private VRController _grabbingController;
		private Vector3 _grabPos;
		private Quaternion _grabRot;
		private Vector3 _realPos;
		private Quaternion _realRot;
		private const float MinDistance = 0.25f;

		public void Init(Transform cameraCube)
		{
			_cameraCube = cameraCube;
			_realPos = _cameraCube.position;
			_realRot = _cameraCube.rotation;
		}

		public override void OnEnable()
		{
			base.OnEnable();
			Plugin.ConfigChangedEvent += PluginOnConfigChangedEvent;
		}

		public override void OnDisable()
		{
			base.OnDisable();
			Plugin.ConfigChangedEvent -= PluginOnConfigChangedEvent;
		}

		private void PluginOnConfigChangedEvent()
		{
			_realPos = Plugin.Config.Position;
			_realRot = Quaternion.Euler(Plugin.Config.Rotation);
		}

		public override void Update()
		{
			base.Update();
			if (vrController != null)
				if (vrController.triggerValue > 0.9f)
				{
					if (_grabbingController != null) return;
					if (Physics.Raycast(vrController.position, vrController.forward, out var hit, _defaultLaserPointerLength))
					{
						if (hit.transform != _cameraCube) return;
						_grabbingController = vrController;
						_grabPos = vrController.transform.InverseTransformPoint(_cameraCube.position);
						_grabRot = Quaternion.Inverse(vrController.transform.rotation) * _cameraCube.rotation;
					}
				}

			if (_grabbingController == null || !(_grabbingController.triggerValue <= 0.9f)) return;
			if (_grabbingController == null) return;
			SaveToConfig();
			_grabbingController = null;
		}

		private void LateUpdate()
		{
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

			CameraPlusBehaviour.ThirdPersonPos = Vector3.Lerp(_cameraCube.position, _realPos,
				Plugin.Config.positionSmooth * Time.deltaTime);

			CameraPlusBehaviour.ThirdPersonRot = Quaternion.Slerp(_cameraCube.rotation, _realRot,
				Plugin.Config.rotationSmooth * Time.deltaTime).eulerAngles;
		}

		private void SaveToConfig()
		{
			var pos = _realPos;
			var rot = _realRot.eulerAngles;
			
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