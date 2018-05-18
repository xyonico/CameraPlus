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
		private CameraPlusBehaviour _cameraPlus;
		public static readonly Ini Ini = new Ini(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg"));
		private bool _init;
		private FileSystemWatcher _iniWatcher;
		
		public string Name
		{
			get { return "Camera+"; }
		}

		public string Version
		{
			get { return "v1.0"; }
		}
		
		public void OnApplicationStart()
		{
			if (_init) return;
			_init = true;
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
			if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg")))
			{
				Ini.WriteValue("fov", "90.0");
				Ini.WriteValue("positionSmooth", "10.0");
				Ini.WriteValue("rotationSmooth", "10.0");
				Ini.Save();
			}

			_iniWatcher = new FileSystemWatcher(Environment.CurrentDirectory)
			{
				NotifyFilter = NotifyFilters.LastWrite,
				Filter = "cameraplus.cfg",
				EnableRaisingEvents = true
			};
			_iniWatcher.Changed += IniWatcherOnChanged;
		}

		public void OnApplicationQuit()
		{
			SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
			_iniWatcher.Changed -= IniWatcherOnChanged;
		}

		private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
		{
			if (scene.buildIndex < 1) return;
			if (_cameraPlus != null)
			{
				Object.Destroy(_cameraPlus.gameObject);
			}
			var mainCamera = Object.FindObjectsOfType<Camera>().FirstOrDefault(x => x.CompareTag("MainCamera"));
			if (mainCamera == null)
			{
				return;
			}
			var gameObj = new GameObject("CameraPlus");
			CameraPlusBehaviour.MainCamera = mainCamera;
			_cameraPlus = gameObj.AddComponent<CameraPlusBehaviour>();
		}

		private void IniWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			if (_cameraPlus == null) return;
			Ini.Load();
			_cameraPlus.ReadIni();
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
	}
}