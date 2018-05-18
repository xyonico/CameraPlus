using System;

namespace CameraPlus
{
	public class SteamVRCompatibility
	{
		// Token: 0x0600032D RID: 813 RVA: 0x00012124 File Offset: 0x00010324
		private static bool FindSteamVRAsset()
		{
			SteamVRCamera = Type.GetType("SteamVR_Camera", false);
			SteamVRExternalCamera = Type.GetType("SteamVR_ExternalCamera", false);
			SteamVRFade = Type.GetType("SteamVR_Fade", false);
			return SteamVRCamera != null && SteamVRExternalCamera != null && SteamVRFade != null;
		}

		// Token: 0x040002C1 RID: 705
		public static bool IsAvailable
		{
			get { return FindSteamVRAsset(); }
		}

		// Token: 0x040002C2 RID: 706
		public static Type SteamVRCamera;

		// Token: 0x040002C3 RID: 707
		public static Type SteamVRExternalCamera;

		// Token: 0x040002C4 RID: 708
		public static Type SteamVRFade;
	}
}