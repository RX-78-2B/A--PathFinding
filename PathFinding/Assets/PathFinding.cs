﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour {

	public Transform seeker, target;
	Grid grid;
	bool isLooped = true;
	int count = 1;
	private int algomode = 0;
	void Awake() {
		grid = GetComponent<Grid> ();
	}

	void Start(){
		//FindPath (seeker.position, target.position);
	}
	void Update() {
		if (Input.GetButtonDown ("Jump")) {
			count++;
			Debug.Log ("Number of Loop =" + count);
		}
		if (Input.GetButtonDown ("Fire1")) {
			count--;
			count = Mathf.Max (count, 0);
			Debug.Log ("Number of Loop =" + count);
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			count+=10;
			Debug.Log ("Number of Loop =" + count);
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			count-=10;
			count = Mathf.Max (count, 0);
			Debug.Log ("Number of Loop =" + count);
		}
		if (Input.GetKeyDown (KeyCode.R)) {
			count = 1;
			Debug.Log ("Number of Loop =" + count);
		}
		if (Input.GetKeyDown (KeyCode.Q)) {
			algomode = 0;
			Debug.Log ("Use A* Algorithm" );
		}
		else if (Input.GetKeyDown (KeyCode.W)) {
			algomode = 1;
			Debug.Log ("Use Uniform Cost Search Algorithm" );
		}
		FindPath(seeker.position, target.position);
	}

	void FindPath(Vector3 startPos, Vector3 targetPos) {
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		List<Node> openSet = new List<Node>();
		HashSet<Node> closedSet = new HashSet<Node>();
		grid.path = null;
		openSet.Add(startNode);
		int k = 0;
		while (openSet.Count > 0 && k< count) {
			Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) {
//				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost) {
//					if (openSet[i].hCost < node.hCost)
//						node = openSet[i];
//				}
				// Uniform cost Search
				/*if (openSet[i].gCost < node.gCost )
					node = openSet[i];*/
				// Heuristic Search
				/*if (openSet[i].hCost < node.hCost)
					node = openSet[i];*/
				if (algomode == 0) {
					if (openSet [i].fCost < node.fCost) {
						node = openSet [i];
					}	
				}
				else if (openSet[i].gCost < node.gCost) {
					node = openSet[i];
				}
			}
		
			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode) {
				RetracePath(startNode,targetNode);
				return;
			}

			foreach (Node neighbour in grid.GetNeighbours(node)) {
				if (!neighbour.walkable || closedSet.Contains(neighbour)) {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
			// For DrawPath
			grid.OpenPath = openSet;
			grid.ClosePath = closedSet;
			k++;
			grid.minNode = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) {
//				if (openSet[i].fCost < grid.minNode.fCost || openSet[i].fCost == grid.minNode.fCost) {
//					if (openSet[i].hCost < grid.minNode.hCost)
//						grid.minNode = openSet[i];
//				}
				if (algomode == 0) {
					if (openSet [i].fCost < grid.minNode.fCost) {
						grid.minNode = openSet [i];
					}	
				}
				else if (openSet[i].gCost < grid.minNode.gCost) {
					grid.minNode = openSet[i];
				}
			}

		}
	}

	void RetracePath(Node startNode, Node endNode) {
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		grid.path = path;
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}

}
