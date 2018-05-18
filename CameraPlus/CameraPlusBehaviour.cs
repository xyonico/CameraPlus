using System;
using UnityEngine;
using UnityEngine.XR;

namespace CameraPlus
{
	public class CameraPlusBehaviour : MonoBehaviour
	{
		public static Camera MainCamera;
		private Camera _cam;
		private float _fov;
		private float _posSmooth;
		private float _rotSmooth;
		
		private void Awake()
		{
			var gameObj = Instantiate(MainCamera.gameObject);
			gameObj.SetActive(false);
			gameObj.name = "Camera Plus";
			gameObj.tag = "Untagged";
			while (gameObj.transform.childCount > 0)
			{
				DestroyImmediate(gameObj.transform.GetChild(0).gameObject);
			}
			DestroyImmediate(gameObj.GetComponent("CameraRenderCallbacksManager"));
			DestroyImmediate(gameObj.GetComponent("AudioListener"));
			DestroyImmediate(gameObj.GetComponent("MeshCollider"));
			if (SteamVRCompatibility.IsAvailable)
			{
				DestroyImmediate(gameObj.GetComponent(SteamVRCompatibility.SteamVRCamera));
				DestroyImmediate(gameObj.GetComponent(SteamVRCompatibility.SteamVRFade));
			}

			_cam = gameObj.GetComponent<Camera>();
			_cam.stereoTargetEye = StereoTargetEyeMask.None;
			_cam.targetTexture = null;
			_cam.depth += 100;
			
			gameObj.SetActive(true);

			var camera = MainCamera.transform;
			transform.position = camera.position;
			transform.rotation = camera.rotation;
			
			gameObj.transform.parent = transform;
			gameObj.transform.localPosition = Vector3.zero;
			gameObj.transform.localRotation = Quaternion.identity;
			gameObj.transform.localScale = Vector3.one;
			
			ReadIni();
		}

		public void ReadIni()
		{
			_fov = Convert.ToSingle(Plugin.Ini.GetValue("fov", "", "90"));
			_posSmooth = Convert.ToSingle(Plugin.Ini.GetValue("positionSmooth", "", "10"));
			_rotSmooth = Convert.ToSingle(Plugin.Ini.GetValue("rotationSmooth", "", "10"));
			SetFOV();
		}

		private void LateUpdate()
		{
			var camera = MainCamera.transform;
			
			transform.position = Vector3.Lerp(transform.position, camera.position, _posSmooth * Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, camera.rotation, _rotSmooth * Time.deltaTime);
		}

		private void SetFOV()
		{
			if (_cam == null) return;
			_cam.fieldOfView = (float) (57.2957801818848 *
			                                  (2.0 * Mathf.Atan(Mathf.Tan((float) (_fov * (Math.PI / 180.0) * 0.5)) /
			                                                    MainCamera.aspect)));
		}
	}
}