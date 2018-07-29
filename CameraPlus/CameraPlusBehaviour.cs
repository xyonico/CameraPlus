using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using VRUIControls;

namespace CameraPlus
{
	public class CameraPlusBehaviour : MonoBehaviour
	{
		public static Camera MainCamera;

		public static bool ThirdPerson
		{
			get { return _thirdPerson; }
			set
			{
				_thirdPerson = value;
				_cameraCube.gameObject.SetActive(_thirdPerson);
			}
		}

		private static bool _thirdPerson;
		
		public static Vector3 ThirdPersonPos;
		public static Vector3 ThirdPersonRot;
		private static Texture2D _screenTexture;
		private static Material _previewMaterial;
		private static Camera _cam;
		private static Transform _cameraCube;
		private static bool _stopRenderLoop;
		private static GameObject _cameraPreviewQuad;

		private static bool ShouldRenderPreview
		{
			get
			{
				return Plugin.Config.thirdPersonPreview && IsVisibleToCamera(_cameraPreviewQuad.transform, MainCamera);
			}
		}

		private void Awake()
		{
			XRSettings.showDeviceView = false;
			
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
			_cam.enabled = false;

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
			
			if (_previewMaterial == null)
			{
				_previewMaterial = new Material(Shader.Find("Hidden/BlitCopyWithDepth"));
			}
			
			if (_screenTexture == null)
			{
				_screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
				_previewMaterial.SetTexture("_MainTex", _screenTexture);
			}

			var quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			DestroyImmediate(quad.GetComponent<Collider>());
			quad.GetComponent<MeshRenderer>().material = _previewMaterial;
			quad.transform.parent = _cameraCube;
			quad.transform.localPosition = new Vector3(-1f * ((_cam.aspect - 1) / 2 + 1), 0, 0.22f);
			quad.transform.localEulerAngles = new Vector3(0, 180, 0);
			quad.transform.localScale = new Vector3(_cam.aspect, 1, 1);
			_cameraPreviewQuad = quad;

			ReadConfig();

			StartCoroutine(RenderCameraRoutine());
		}

		public void ReadConfig()
		{
			ThirdPerson = Plugin.Config.thirdPerson;
			ThirdPersonPos = new Vector3(Plugin.Config.posx, Plugin.Config.posy, Plugin.Config.posz);
			ThirdPersonRot = new Vector3(Plugin.Config.angx, Plugin.Config.angy, Plugin.Config.angz);

			_cameraPreviewQuad.gameObject.SetActive(Plugin.Config.thirdPersonPreview);

			SetFOV();
		}

		private IEnumerator RenderCameraRoutine()
		{
			var waitForFrame = new WaitForEndOfFrame();
			var nextRender = 0f;
			while (!_stopRenderLoop)
			{

				while (Time.realtimeSinceStartup < nextRender)
				{
					yield return waitForFrame;
				}
				
				if (_screenTexture.width != Screen.width || _screenTexture.height != Screen.height)
				{
					_screenTexture.Resize(Screen.width, Screen.height);
				}
				
				_cam.Render();

				if (ShouldRenderPreview)
				{
					_screenTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.width), 0, 0);
					_screenTexture.Apply(false);
				}

				nextRender = Time.realtimeSinceStartup + 1 / Plugin.Config.fps;
			}
		}

		private static void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
		{
			var pointer = Resources.FindObjectsOfTypeAll<VRPointer>().FirstOrDefault();
			if (pointer == null) return;
			ReflectionUtil.CopyComponent(pointer, typeof(CameraMoverPointer), pointer.gameObject);
			DestroyImmediate(pointer);
		}

		private void LateUpdate()
		{
			var camera = MainCamera.transform;

			if (ThirdPerson)
			{
				transform.position = ThirdPersonPos;
				transform.eulerAngles = ThirdPersonRot;
				_cameraCube.position = ThirdPersonPos;
				_cameraCube.eulerAngles = ThirdPersonRot;
				return;
			}

			transform.position = Vector3.Lerp(transform.position, camera.position,
				Plugin.Config.positionSmooth * Time.deltaTime);
			
			transform.rotation = Quaternion.Slerp(transform.rotation, camera.rotation,
				Plugin.Config.rotationSmooth * Time.deltaTime);
		}

		private static void SetFOV()
		{
			if (_cam == null) return;
			var fov = (float) (57.2957801818848 *
			                   (2.0 * Mathf.Atan(Mathf.Tan((float) (Plugin.Config.fov * (Math.PI / 180.0) * 0.5)) /
			                                     MainCamera.aspect)));
			_cam.fieldOfView = fov;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				ThirdPerson = !ThirdPerson;
				if (!ThirdPerson)
				{
					transform.position = MainCamera.transform.position;
					transform.rotation = MainCamera.transform.rotation;
				}

				Plugin.Config.thirdPerson = ThirdPerson;
				Plugin.Config.Save();
			}
		}
		
		public static bool IsVisibleToCamera(Transform transform, Camera camera)
		{
			var visTest = camera.WorldToViewportPoint(transform.position);
			return visTest.x >= 0 && visTest.y >= 0 && visTest.x <= 1 && visTest.y <= 1 && visTest.z >= 0;
		}
	}
}