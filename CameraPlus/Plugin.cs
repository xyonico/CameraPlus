using System;
using System.Collections;
using System.IO;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace CameraPlus
{
	public class Plugin : IPlugin
	{
		public readonly Config Config = new Config(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg"));
		private readonly WaitForSecondsRealtime _waitForSecondsRealtime = new WaitForSecondsRealtime(0.1f);
		
		private CameraPlusBehaviour _cameraPlus;
		private bool _init;
		
		public static Plugin Instance { get; private set; }
		public string Name => "CameraPlus";
		public string Version => "v2.0.1";

		public void OnApplicationStart()
		{
			if (_init) return;
			_init = true;

			Instance = this;
			
			SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
		}

		public void OnApplicationQuit()
		{
			SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
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

		private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			SharedCoroutineStarter.instance.StartCoroutine(DelayedOnSceneLoaded(scene));
		}

		private IEnumerator DelayedOnSceneLoaded(Scene scene)
		{
			yield return _waitForSecondsRealtime;
			
			if (scene.buildIndex < 1) yield break;
			if (_cameraPlus != null) Object.Destroy(_cameraPlus.gameObject);

			var mainCamera = Camera.main;
			if (mainCamera == null)
			{
				yield break;
			}

			var gameObj = new GameObject("CameraPlus");
			_cameraPlus = gameObj.AddComponent<CameraPlusBehaviour>();
			_cameraPlus.Init(mainCamera);
		}
	}
}