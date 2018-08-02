using UnityEngine;
using VRUIControls;

namespace CameraPlus
{
	public class CameraMoverPointer : VRPointer
	{
		protected CameraPlusBehaviour _cameraPlus;
		protected Transform _cameraCube;
		protected VRController _grabbingController;
		protected Vector3 _grabPos;
		protected Quaternion _grabRot;
		protected Vector3 _realPos;
		protected Quaternion _realRot;
		protected const float MinDistance = 0.25f;

		public virtual void Init(CameraPlusBehaviour cameraPlus,Transform cameraCube)
		{
			_cameraPlus = cameraPlus;
			_cameraCube = cameraCube;
			_realPos = Plugin.Instance.Config.Position;
			_realRot = Quaternion.Euler(Plugin.Instance.Config.Rotation);
		}

		public override void OnEnable()
		{
			base.OnEnable();
			Plugin.Instance.Config.ConfigChangedEvent += PluginOnConfigChangedEvent;
		}

		public override void OnDisable()
		{
			base.OnDisable();
			Plugin.Instance.Config.ConfigChangedEvent -= PluginOnConfigChangedEvent;
		}

		protected virtual void PluginOnConfigChangedEvent(Config config)
		{
			_realPos = config.Position;
			_realRot = Quaternion.Euler(config.Rotation);
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

		protected virtual void LateUpdate()
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

			_cameraPlus.ThirdPersonPos = Vector3.Lerp(_cameraCube.position, _realPos,
				Plugin.Instance.Config.positionSmooth * Time.deltaTime);

			_cameraPlus.ThirdPersonRot = Quaternion.Slerp(_cameraCube.rotation, _realRot,
				Plugin.Instance.Config.rotationSmooth * Time.deltaTime).eulerAngles;
		}

		protected virtual void SaveToConfig()
		{
			var pos = _realPos;
			var rot = _realRot.eulerAngles;

			var config = Plugin.Instance.Config;
			
			config.posx = pos.x;
			config.posy = pos.y;
			config.posz = pos.z;
			
			config.angx = rot.x;
			config.angy = rot.y;
			config.angz = rot.z;
			
			config.Save();
		}
	}
}