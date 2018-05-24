using System;

namespace CameraPlus
{
	public static class SteamVRCompatibility
	{
		public static Type SteamVRCamera;
		public static Type SteamVRExternalCamera;
		public static Type SteamVRFade;
		public static bool IsAvailable => FindSteamVRAsset();

		private static bool FindSteamVRAsset()
		{
			SteamVRCamera = Type.GetType("SteamVR_Camera", false);
			SteamVRExternalCamera = Type.GetType("SteamVR_ExternalCamera", false);
			SteamVRFade = Type.GetType("SteamVR_Fade", false);
			return SteamVRCamera != null && SteamVRExternalCamera != null && SteamVRFade != null;
		}
	}
}