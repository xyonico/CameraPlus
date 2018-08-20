using UnityEngine;
using VRUIControls;

namespace CameraPlus
{
	public class CameraMoverPointer : MonoBehaviour
	{
		protected const float MinScrollDistance = 0.25f;
		protected const float MaxLaserDistance = 50;

		protected VRPointer _vrPointer;
		protected CameraPlusBehaviour _cameraPlus;
		protected Transform _cameraCube;
		protected VRController _grabbingController;
		protected Vector3 _grabPos;
		protected Quaternion _grabRot;
		protected Vector3 _realPos;
		protected Quaternion _realRot;

		public virtual void Init(CameraPlusBehaviour cameraPlus, Transform cameraCube)
		{
			_cameraPlus = cameraPlus;
			_cameraCube = cameraCube;
			_realPos = Plugin.Instance.Config.Position;
			_realRot = Quaternion.Euler(Plugin.Instance.Config.Rotation);
			_vrPointer = GetComponent<VRPointer>();
		}

		protected virtual void OnEnable()
		{
			Plugin.Instance.Config.ConfigChangedEvent += PluginOnConfigChangedEvent;
		}

		protected virtual void OnDisable()
		{
			Plugin.Instance.Config.ConfigChangedEvent -= PluginOnConfigChangedEvent;
		}

		protected virtual void PluginOnConfigChangedEvent(Config config)
		{
			_realPos = config.Position;
			_realRot = Quaternion.Euler(config.Rotation);
		}

		protected virtual void Update()
		{
			if (_vrPointer.vrController != null)
				if (_vrPointer.vrController.triggerValue > 0.9f)
				{
					if (_grabbingController != null) return;
					if (Physics.Raycast(_vrPointer.vrController.position, _vrPointer.vrController.forward, out var hit, MaxLaserDistance))
					{
						if (hit.transform != _cameraCube) return;
						_grabbingController = _vrPointer.vrController;
						_grabPos = _vrPointer.vrController.transform.InverseTransformPoint(_cameraCube.position);
						_grabRot = Quaternion.Inverse(_vrPointer.vrController.transform.rotation) * _cameraCube.rotation;
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
				if (_grabPos.magnitude > MinScrollDistance)
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