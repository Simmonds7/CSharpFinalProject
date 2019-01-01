﻿using System;
using System.Collections.Generic;
using System.Windows;

namespace C_Sharp_Final_Project
{
    class Pathfinder
    {
        List<Node> nodePath;

        public void FindPath(Vector start, Vector target, double distFromTarget)
        {
            bool success = false;

            Node startNode = Game.Grid.NodeFromWorld(start);
            Node targetNode = Game.Grid.NodeFromWorld(target);

            Heap<Node> openSet = new Heap<Node>(Game.Grid.numNodeHeight * Game.Grid.numNodeWidth);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (Component.DistanceOfPoints(targetNode.worldPosition, currentNode.worldPosition) <= distFromTarget &&
                    !Raycaster.IsWallsBlockView(currentNode.worldPosition, targetNode.worldPosition, Game.Walls))
                {
                    targetNode = currentNode; 
                    success = true;
                    break;
                }

                foreach (Node neighbor in Game.Grid.PossibleNodeNeighbors(currentNode, 1))
                {
                    
                    if (closedSet.Contains(neighbor) || neighbor.endPoint)
                        continue;
                    else
                    {
                        bool subNeighborEndPoint = false;
                        foreach (Node subNeighbor in Game.Grid.PossibleNodeNeighbors(currentNode, 2))
                            if (subNeighbor.endPoint)
                            {
                                subNeighborEndPoint = true;
                                break;
                            }
                        if (subNeighborEndPoint)
                            continue;
                    }
                    
                    int moveCost = currentNode.gCost + NodeDistance(currentNode, neighbor);
                    if (moveCost < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = moveCost;
                        neighbor.hCost = NodeDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;
                        if (!openSet.Contains(neighbor))
                            openSet.Add(neighbor);
                        else
                            openSet.UpdateItem(neighbor);
                    }
                }
            }
            if (success)
                RetracePath(startNode, targetNode, distFromTarget);
            Game.Pathmanager.FinishedrocessingPath(nodePath, success);
        }
        
        private void RetracePath(Node startNode, Node targetNode, double distFromTarget)
        {
            nodePath = new List<Node>();
            Node currentNode = targetNode;
            while(currentNode != startNode)
            {
                nodePath.Add(currentNode);
                currentNode = currentNode.parent;
            }

            nodePath.Reverse();

            foreach (Node node in nodePath)
                node.path = true;
        }

        private int NodeDistance(Node nodea, Node nodeb)
        {
            Vector dist = new Vector(Math.Abs(nodea.gridPosition.X - nodeb.gridPosition.X),
                                     Math.Abs(nodea.gridPosition.Y - nodeb.gridPosition.Y));

            return dist.X > dist.Y ? (int) (14 * dist.Y + 10 * (dist.X - dist.Y)) : (int) (14 * dist.X + 10 * (dist.Y - dist.X));
        }
    }
}
