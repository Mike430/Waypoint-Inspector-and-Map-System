# Waypoint Inspector and Map System

This repository provides and demonstrates four scripts for unity which allow for quickly building and editing waypoint maps for AIs to navigate.


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
- Create an empty game object and attach the WayPointMap script component to it. You may notice that it should now have an 'i' Icon in the scene.
- Through this game object you may add or remove waypoint game objects from the scene.
- After you've created all your waypoints you may select them through the map's waypoint inspector tool and position them in the scene however you like and even parent them to other objects.
- Waypoint object transformations are not parented to the WayPointMap, to allow for flexibility with dynamic environments.
- Waypoint connections are best handled directly through the WayPointMap inspector, the index of a connection matches a waypoint's index in the scene.
- Examining an object in the scene will hilight it's connections and give it a position handle. There is no undo functionality for this method of positioning, so...
- through the same inspector you may select the waypoint of interest for full control.
- Please do NOT delete WayPoint objects from the scene outside of the WayPointMap inspector.

![alt text](https://github.com/Mike430/RobotMountainPlatformer/blob/master/GITHUB_Image_Inspector.PNG)

A sample AI agent, an A-Star Pathfinder and first person camera has been provided along with a few different scenes.

Please note that this is currently a hobbyist project and is not currently robust enough for production usage.