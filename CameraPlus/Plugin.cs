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
		//public static readonly Ini Ini = new Ini(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg"));
		private CameraPlusBehaviour _cameraPlus;
		private bool _init;
		private FileSystemWatcher _configWatcher;
		public static readonly Config Config = new Config(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg"));

		public static event Action ConfigChangedEvent;

		public string Name => "CameraPlus";

		public string Version => "v1.3";

		public void OnApplicationStart()
		{
			if (_init) return;
			_init = true;
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
			

			_configWatcher = new FileSystemWatcher(Environment.CurrentDirectory)
			{
				NotifyFilter = NotifyFilters.LastWrite,
				Filter = "cameraplus.cfg",
				EnableRaisingEvents = true
			};
			_configWatcher.Changed += ConfigWatcherOnChanged;
		}

		public void OnApplicationQuit()
		{
			SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
			_configWatcher.Changed -= ConfigWatcherOnChanged;
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
			CameraPlusBehaviour.MainCamera = mainCamera;
			_cameraPlus = gameObj.AddComponent<CameraPlusBehaviour>();
		}

		private void ConfigWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			Config.Load();
			
			if (ConfigChangedEvent != null)
			{
				ConfigChangedEvent();
			}
		}
	}
}