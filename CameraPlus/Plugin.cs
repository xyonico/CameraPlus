using System;
using System.IO;
using System.Linq;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CameraPlus
{
	public class Plugin : IPlugin
	{
		public readonly Config Config = new Config(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg"));
		
		private CameraPlusBehaviour _cameraPlus;
		private bool _init;
		
		public static Plugin Instance { get; private set; }
		public string Name => "CameraPlus";
		public string Version => "v2.0.0";

		public void OnApplicationStart()
		{
			if (_init) return;
			_init = true;

			Instance = this;
			
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
		}

		public void OnApplicationQuit()
		{
			SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
			Config.Save();
		}

		public void OnLevelWasLoaded(int level)
		{
		}

		public void OnLevelWasInitialized(int level)
		{
		}

		public void OnUpdate()
		{
		}

		public void OnFixedUpdate()
		{
		}

		private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
		{
			if (scene.buildIndex < 1) return;
			if (_cameraPlus != null) Object.Destroy(_cameraPlus.gameObject);

			var mainCamera = Object.FindObjectsOfType<Camera>().FirstOrDefault(x => x.CompareTag("MainCamera"));
			if (mainCamera == null) return;

			var gameObj = new GameObject("CameraPlus");
			_cameraPlus = gameObj.AddComponent<CameraPlusBehaviour>();
			_cameraPlus.Init(mainCamera);
		}
	}
}