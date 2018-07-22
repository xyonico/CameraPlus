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
					RaycastHit hit;
					if (Physics.Raycast(vrController.position, vrController.forward, out hit, _defaultLaserPointerLength))
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
			SaveToIni();
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
				CameraPlusBehaviour.PosSmooth * Time.deltaTime);
			_grabbedCamera.rotation = Quaternion.Slerp(_grabbedCamera.rotation, _realRot,
				CameraPlusBehaviour.RotSmooth * Time.deltaTime);

			CameraPlusBehaviour.ThirdPersonPos = _grabbedCamera.position;
			CameraPlusBehaviour.ThirdPersonRot = _grabbedCamera.rotation;
		}

		private void SaveToIni()
		{
			var ini = Plugin.Ini;
			var pos = _grabbedCamera.position;
			var rot = _grabbedCamera.rotation;
			ini.WriteValue("posx", pos.x.ToString(CultureInfo.InvariantCulture));
			ini.WriteValue("posy", pos.y.ToString(CultureInfo.InvariantCulture));
			ini.WriteValue("posz", pos.z.ToString(CultureInfo.InvariantCulture));
			ini.WriteValue("rotx", rot.x.ToString(CultureInfo.InvariantCulture));
			ini.WriteValue("roty", rot.y.ToString(CultureInfo.InvariantCulture));
			ini.WriteValue("rotz", rot.z.ToString(CultureInfo.InvariantCulture));
			ini.WriteValue("rotw", rot.w.ToString(CultureInfo.InvariantCulture));
		}
	}
}