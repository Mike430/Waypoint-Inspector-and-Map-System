using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
	public class pathNode
	{
		public float G; // The sum cost of getting to this way point from the starting waypoint
		public float H; // The guessed distance from this waypoint to the end waypoint
		public float F; // The sum of G and H
		public WayPoint Myself;
		public pathNode Parent;

		public void addDigits() { F = G + H; }
	}

    public static class AStar
	{
		public static List<WayPoint> findPath(Vector3 startPos, Vector3 endPos, WayPointMap wayPointMap)
		{
			List<pathNode> openList = new List<pathNode>();
			List<pathNode> closedList = new List<pathNode>();
			List<WayPoint> finalPath = new List<WayPoint>();
            
			WayPoint startWaypoint = wayPointMap.ClosestWaypoint(startPos);
			WayPoint endWaypoint = wayPointMap.ClosestWaypoint(endPos);

			pathNode chosenNode = null;
			pathNode first = new pathNode();
			first.Parent = null;
			first.Myself = startWaypoint;
			first.F = 0.0f;
			first.G = 0.0f;
			first.H = 0.0f;

			openList.Add(first);

			while (openList.Count != 0)
			{
				int indexOfLowestFNode = 0;
				for (int i = 0; i < openList.Count; ++i)
				{
					openList[i].addDigits ();
					if(openList[i].F < openList[indexOfLowestFNode].F)
					{
						indexOfLowestFNode = i;
					}
				}

				chosenNode = openList [indexOfLowestFNode];
				closedList.Add (chosenNode);
				openList.RemoveAt (indexOfLowestFNode);

				for (int i = 0; i < openList.Count; ++i)
				{
					closedList.Add (openList[i]);
				}

				// END CONDITION
				if (chosenNode.Myself == endWaypoint)
				{
					// reverse the queue and return
					while(chosenNode != null)
					{
						finalPath.Add (chosenNode.Myself);
						chosenNode = chosenNode.Parent;
					}
					return finalPath;
				}

				// STANDARD OPERATION
				List<WayPoint> connections = chosenNode.Myself.m_Connections;

				for (int i = 0; i < connections.Count; ++i)
				{
					bool connectionAlreadyUsed = false;
					for (int j = 0; j < closedList.Count; ++j)
					{
						if (connections [i] == closedList [j].Myself)
						{
							connectionAlreadyUsed = true;
							break;
						}
					}
					if (connectionAlreadyUsed)
					{
						continue;
					}

					pathNode validConnection = new pathNode ();
					validConnection.Myself = connections [i];
					validConnection.Parent = chosenNode;
					validConnection.G = (validConnection.Myself.transform.position - chosenNode.Myself.transform.position).magnitude;
					validConnection.H = (validConnection.Myself.transform.position - endWaypoint.transform.position).magnitude;
					validConnection.addDigits();
					openList.Add(validConnection);
				}
			}

			return finalPath;
		}
	}
}