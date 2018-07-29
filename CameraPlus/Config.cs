using System;
using System.IO;
using UnityEngine;

namespace CameraPlus
{
	public class Config
	{
		public string FilePath { get; }
		
		public float fov = 90;
		public float fps = 60;
		public float positionSmooth = 10;
		public float rotationSmooth = 5;
		
		public bool thirdPerson = false;
		public bool thirdPersonPreview = false;
		
		public float posx;
		public float posy = 2;
		public float posz = -1.2f;

		public float angx = 15;
		public float angy;
		public float angz;

		public Config(string filePath)
		{
			FilePath = filePath;

			if (File.Exists(FilePath))
			{
				Load();
				var text = File.ReadAllText(FilePath);
				if (!text.Contains("rotx")) return;
				
				var oldRotConfig = new OldRotConfig();
				ConfigSerializer.LoadConfig(oldRotConfig, FilePath);
		
				var euler = new Quaternion(oldRotConfig.rotx, oldRotConfig.roty, oldRotConfig.rotz, oldRotConfig.rotw)
					.eulerAngles;
				angx = euler.x;
				angy = euler.y;
				angz = euler.z;
				
				Save();
			}
			else
			{
				Save();
			}
		}

		public void Save()
		{
			ConfigSerializer.SaveConfig(this, FilePath);
		}

		public void Load()
		{
			ConfigSerializer.LoadConfig(this, FilePath);
		}
		
		public class OldRotConfig
		{
			public float rotx;
			public float roty;
			public float rotz;
			public float rotw;
		}
	}
}