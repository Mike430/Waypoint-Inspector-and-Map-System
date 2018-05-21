# Waypoint Inspector and Map System

This repository provides and demonstrates four scripts for unity which allow for quickly building and editing waypoing maps for AIs to navigate.


![alt text](https://github.com/Mike430/RobotMountainPlatformer/blob/master/GITHUB_Image_3D_Scene.PNG)

### These scripts are:
(For the unity editor, which you should store in Assets/Editor)
- WayPointDBItem.cs
- WayPointMapInspector.cs
 
(As components to game objectes)
- WayPointMap.cs
- WayPoint.cs

### Instructions for usage:
- All waypoints must be created through the map. There is no way to add a stray waypoint from the scene to a map currently.
- Create an empty game object and attage the WayPointMap script component to it. You may notice that it should now have an 'i' Icon in the scene.
- Through this game object you may add or remove waypoint game objects from the scene.
- After you've created all your waypoints you may select them through the map's waypoint inspector tool and position them in the scene however you like and even parent them to other objects.
- Waypoint object transformations are not parented to the WayPointMap, to allow for flexibility with dynamic environments.
- Please do NOT delete waypoints from the scene outside of the waypoint inspect.

![alt text](https://github.com/Mike430/RobotMountainPlatformer/blob/master/GITHUB_Image_Inspector.PNG)

A sample AI agent, an A-Star Pathfinder and first person camera has been provided along with a few different scenes.

Please note that this is currently a hobbiest project and is not currently robust enough for production usage.