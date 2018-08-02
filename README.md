# CameraPlus
Plugin for Beat Saber for a smoother and wider FOV camera.

[Video Comparison](https://youtu.be/MysLXKSXGTY)  
[Third Person Preview](https://youtu.be/ltIhpt-n6b8)

# Installing
1. Use the mod installer: https://github.com/Umbranoxio/BeatSaberModInstaller/releases
		It is the easiest method, it will do all these steps below in 1 click.
	
### To install manually:
	1b. Make sure that Beat Saber is not running.
	2b. Extract the contents of the zip into Beat Saber's installation folder.
		For Oculus Home: \Oculus Apps\Software\hyperbolic-magnetism-beat-saber\
		For Steam: \steamapps\common\Beat Saber\
		(The folder that contains Beat Saber.exe)
	3b. Done! You've installed the Custom Avatar Plugin.
# Usage
Press <kbd>F1</kbd> to toggle between first and third person.

After you run the game once, a `cameraplus.cfg` file is created within the Beat Saber folder.  
Edit that file to configure CameraPlus:

`fov=90.0` Horizontal field of view of the camera  
`fps=90.0` Frame rate of the camera  
`antiAliasing=2` Anti-aliasing setting for the camera (1, 2, 4 or 8 only)  
`renderScale` The resolution scale of the camera relative to game window (similar to supersampling for VR)  
`positionSmooth=10.0` How much position should smooth **(SMALLER NUMBER = SMOOTHER)**  
`rotationSmooth=5.0` How much rotation should smooth **(SMALLER NUMBER = SMOOTHER)**  
`thirdPerson=false` Whether third person camera is enabled  

`posx=0` X position of third person camera  
`posy=0` Y position of third person camera  
`posz=0` Z position of third person camera  
`angx=0` X rotation of third person camera  
`angy=0` Y rotation of third person camera  
`angz=0` Z rotation of third person camera  

If you need help, ask us at the Beat Saber Mod Group Discord Server:  
https://discord.gg/BeatSaberMods
