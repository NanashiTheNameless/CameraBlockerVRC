# CameraBlockerVRC Modded By NamelessNanashi

## Original by orels1

## Installation

- [Open the listing website](<https://CameraBlockerVRC.NamelessNanashi.dev/>) and download the package in a format you prefer
- Either Drag & Drop the `.unitypackage` into your project if you downloaded that
- **OR** unpack the Zip file into your `Packages` folder, e.g. `C:\UnityProjects\MyProject\Packages\dev.namelessnanashi.camerablocker`
- Check out the package-specific Usage section in the relevant Readme, e.g. Project View in Unity -> Packages -> CameraBlockerVRC -> README

## Package included

- Camera Blocker (CameraBlockerVRC): A simple shader to make an area of the scene render completely black in player cameras. Helpful for game worlds and other scenarios where you want to disallow "spying"

## Testing Avatars

### All Testing Avatars Are compiled with the latest version at *compiletime* and have GoGoLoco v1.8.6 with flight (cross compiled, shader only works on PC/PCVR)

- [AntiCamera Light, Excellent, 2.5x2.5x2.5](<https://vrchat.com/home/avatar/avtr_b748efcb-13fa-4ba0-83e3-4de776e1850a>)

- [AntiCamera Medium, Medium, 5x5x5](<https://vrchat.com/home/avatar/avtr_cc7c8872-c0b2-4c36-a642-14feace3df47>)

- [AntiCamera Extreme, Very Poor, 100x100x100](<https://vrchat.com/home/avatar/avtr_00f598f6-0af9-4bab-85c6-cfc257916f98>)

## Usage

- Drag and Drop the Camera Blocker Container prefab in the main folder (or from the `Runtime` folder into your scene)
- Position and scale to fit around the area you want to be hidden
- There is a visualizer cube in the prefab which gets removed on building your world, you can keep it if it helps you position the object, or disable it if you don't need it

## Details

- The prefab, by default, exists on the `Player` layer, that is most likely what you want, as usually, as a world creator, you don't want people spying on others using drone cameras or personal mirrors.
- There are 3 parts to the prefab:
  - Blocker Writer
  - Blocker Clearer
  - Camera Blocker
  - You want to make sure that all of them are present, and that the Writer is always contained by the Clearer, otherwise the blocker can be overwriting things you don't want to be blocked. If you just dragged and drop the prefab - you should be fine as is.
- You can replace the blocker meshes if you need (by default they use  unity cubes), but make sure that the normals of the mesh point in the same direction as the unity cube (outwards), or the blocker will work the wrong way around!

## Technical Details

- The core idea is to render over everything but only INSIDE some area, which is a bit more non-trivial than you might think. So I decided to use stencils to make it work
- Blocker Writer writes stencil id 69 over everything while being completely transparent and invisible to anything but the handheld cameras. It uses Cull Front to only render the backfaces (as I'm using the detault cube mesh which has normals facing outward
- Blocker Clearer renders one render queue later and writes stencil id 0 onto everything. It is also a default cube with Cull Back, so when looking from the outside, it will replace all those written 69s with 0s
- Finally, blocker uses stencil id 69 to determine if it should render. If the stencil id is anything but 69 it will not render anything. It is rendered last in the order
- All shaders have ZTest Always which make them always render on top of everything so that stencil ids propagate orrectly
- All shaders assign NaN to vertex position (to skip rendering completely) whenever the current camera is anything but the handheld cam
