using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRUIControls;

namespace CameraPlus
{
	public class CameraPlusBehaviour : MonoBehaviour
	{
		public static Camera MainCamera;
		public static float FOV;
		public static float PosSmooth;
		public static float RotSmooth;

		public static bool ThirdPerson;
		public static Vector3 ThirdPersonPos;
		public static Quaternion ThirdPersonRot;
		private static RenderTexture _renderTexture;
		private static Material _previewMaterial;
		private Camera _cam;
		private Camera _previewCam;
		private Transform _cameraCube;
		private const int Width = 256;

		private void Awake()
		{
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
			SceneManagerOnActiveSceneChanged(new Scene(), new Scene());
			var gameObj = Instantiate(MainCamera.gameObject);
			gameObj.SetActive(false);
			gameObj.name = "Camera Plus";
			gameObj.tag = "Untagged";
			while (gameObj.transform.childCount > 0) DestroyImmediate(gameObj.transform.GetChild(0).gameObject);
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

			var cameraCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
			cameraCube.SetActive(ThirdPerson);
			_cameraCube = cameraCube.transform;
			_cameraCube.localScale = new Vector3(0.15f, 0.15f, 0.22f);
			_cameraCube.name = "CameraCube";

			_previewCam = Instantiate(_cam.gameObject, _cameraCube).GetComponent<Camera>();
			
			if (_renderTexture == null && _previewMaterial == null)
			{
				_renderTexture = new RenderTexture(Width, (int) (Width / _cam.aspect), 24);
				_previewMaterial = new Material(Shader.Find("Hidden/BlitCopyWithDepth"));
				_previewMaterial.SetTexture("_MainTex", _renderTexture);
			}
			
			_previewCam.targetTexture = _renderTexture;

			var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			DestroyImmediate(quad.GetComponent<Collider>());
			quad.GetComponent<MeshRenderer>().material = _previewMaterial;
			quad.transform.parent = _cameraCube;
			quad.transform.localPosition = new Vector3(-1f * ((_cam.aspect - 1) / 2 + 1), 0, 0.22f);
			quad.transform.localEulerAngles = new Vector3(0, 180, 0);
			quad.transform.localScale = new Vector3(-1 * _cam.aspect, 1, 1);

			ReadIni();
		}

		private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
		{
			var pointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();
			if (pointer == null) return;
			ReflectionUtil.CopyComponent(pointer, typeof(CameraMoverPointer), pointer.gameObject);
			DestroyImmediate(pointer);
		}

		public void ReadIni()
		{
			FOV = Convert.ToSingle(Plugin.Ini.GetValue("fov", "", "90"));
			PosSmooth = Convert.ToSingle(Plugin.Ini.GetValue("positionSmooth", "", "10"));
			RotSmooth = Convert.ToSingle(Plugin.Ini.GetValue("rotationSmooth", "", "5"));

			ThirdPerson = Convert.ToBoolean(Plugin.Ini.GetValue("thirdPerson", "", "false"));
			_cameraCube.gameObject.SetActive(ThirdPerson);
			ThirdPersonPos = new Vector3(
				Convert.ToSingle(Plugin.Ini.GetValue("posx", "", "0")),
				Convert.ToSingle(Plugin.Ini.GetValue("posy", "", "2")),
				Convert.ToSingle(Plugin.Ini.GetValue("posz", "", "-1"))
			);
			
			ThirdPersonRot = new Quaternion(
				Convert.ToSingle(Plugin.Ini.GetValue("rotx", "", "0.25")),
				Convert.ToSingle(Plugin.Ini.GetValue("roty", "", "0")),
				Convert.ToSingle(Plugin.Ini.GetValue("rotz", "", "0")),
				Convert.ToSingle(Plugin.Ini.GetValue("rotw", "", "1"))
			);

			SetFOV();
		}

		private void LateUpdate()
		{
			var camera = MainCamera.transform;

			if (ThirdPerson)
			{
				transform.position = ThirdPersonPos;
				transform.rotation = ThirdPersonRot;
				_cameraCube.position = ThirdPersonPos;
				_cameraCube.rotation = ThirdPersonRot;
				return;
			}

			transform.position = Vector3.Lerp(transform.position, camera.position, PosSmooth * Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, camera.rotation, RotSmooth * Time.deltaTime);
		}

		private void SetFOV()
		{
			if (_cam == null) return;
			var fov = (float) (57.2957801818848 *
			                   (2.0 * Mathf.Atan(Mathf.Tan((float) (FOV * (Math.PI / 180.0) * 0.5)) /
			                                     MainCamera.aspect)));
			_cam.fieldOfView = fov;
			if (_previewCam == null) return;
			_previewCam.fieldOfView = fov;
		}
	}
}