using System;
using UnityEngine;

namespace CameraPlus
{
	/// <summary>
	/// This is the monobehaviour that goes on the camera that handles
	/// displaying the actual feed from the camera to the screen.
	/// </summary>
	public class ScreenCameraBehaviour : MonoBehaviour
	{
		private Camera _cam;
		private RenderTexture _renderTexture;
		
		public void SetRenderTexture(RenderTexture renderTexture)
		{
			_renderTexture = renderTexture;
		}

		private void Awake()
		{
			_cam = gameObject.AddComponent<Camera>();
			_cam.clearFlags = CameraClearFlags.Nothing;
			_cam.cullingMask = 0;
			_cam.depth = -1000;
			_cam.stereoTargetEye = StereoTargetEyeMask.None;
		}
		
		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			if (_renderTexture == null) return;
			Graphics.Blit(_renderTexture, dest);
		}
	}
}