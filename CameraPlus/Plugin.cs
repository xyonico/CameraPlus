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
		public static readonly Ini Ini = new Ini(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg"));
		private CameraPlusBehaviour _cameraPlus;
		private bool _init;
		private FileSystemWatcher _iniWatcher;

		public string Name => "CameraPlus";

		public string Version => "v1.1";

		public void OnApplicationStart()
		{
			if (_init) return;
			_init = true;
			SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
			if (!File.Exists(Path.Combine(Environment.CurrentDirectory, "cameraplus.cfg")))
			{
				Ini.WriteValue("fov", "90.0");
				Ini.WriteValue("positionSmooth", "10");
				Ini.WriteValue("rotationSmooth", "5");

				Ini.WriteValue("thirdPerson", "false");
				
				Ini.WriteValue("posx", "0");
				Ini.WriteValue("posy", "2");
				Ini.WriteValue("posz", "-1");

				Ini.WriteValue("rotw", "0.25");
				Ini.WriteValue("rotx", "0");
				Ini.WriteValue("roty", "0");
				Ini.WriteValue("rotz", "1");

				Ini.Save();
			}
			else
			{
				if (Ini.GetValue("thirdPerson", "", "missing") == "missing")
				{
					Ini.WriteValue("thirdPerson", "false");
					Ini.Save();
				}
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
			Ini.Save();
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

		private void IniWatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
		{
			if (_cameraPlus == null) return;
			Ini.Load();
			_cameraPlus.ReadIni();
		}
	}
}