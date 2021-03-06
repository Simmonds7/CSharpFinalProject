﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace C_Sharp_Final_Project
{
    class Scene
    {
        public static void SetUpSceneLevel(string sceneFilePath)
        {
            string[] sceneDecode = File.ReadAllLines(sceneFilePath);

            if (sceneDecode[0].ToLower().Equals("game:"))
            {
                for (int i = 1; i < sceneDecode.Length; i++)
                    DecodeLineToGame(sceneDecode[i]);
            } 
        }

        public static void SetUpSceneScreen(string sceneFilePath, Screen main)
        {
            string[] sceneDecode = File.ReadAllLines(sceneFilePath);

            if (sceneDecode[0].ToLower().Equals("screen:"))
            {
                for (int i = 1; i < sceneDecode.Length; i++)
                    DecodeLineToScreen(sceneDecode[i], main);
            }
        }

        private static void DecodeLineToScreen(string line, Screen main)
        {
           
            line = line.Replace(" ", string.Empty);
            string[] tempStrings;

            switch (line[0])
            {
                case 'B':// draw button
                    line = line.Substring(2);
                    tempStrings = line.Split(',').ToArray();
                  
                    if (tempStrings[0].ToLower().Equals("game"))
                    {
                        main.MainButtons.Add(new Button(200, 50, int.Parse(tempStrings[1]), int.Parse(tempStrings[2]),
                            "Textures/Button_Begin.png", () => main.StartGame()));
                    } else if (tempStrings[0].ToLower().Equals("level"))
                    {
                        main.MainButtons.Add(new Button(200, 50, int.Parse(tempStrings[1]), int.Parse(tempStrings[2]),
                            "Textures/Button_Level.png", () => main.LevelScreen()));
                    } else if (tempStrings[0].ToLower().Equals("quit"))
                    {
                        main.MainButtons.Add(new Button(200, 50, int.Parse(tempStrings[1]), int.Parse(tempStrings[2]),
                            "Textures/Button_Quit.png", () => main.Clean()));
                    } else
                    {
                        main.MainButtons.Add(new Button(75, 75, int.Parse(tempStrings[1]), int.Parse(tempStrings[2]),
                            "Textures/Button_" + tempStrings[0] + ".png", () => main.LoadLevel(tempStrings[0] + ".txt")));
                    }

                    break;
                default:
                    Console.WriteLine("Unrecognize symbol for screen: " + line[0]);
                    break;
            }
        }

        private static void DecodeLineToGame(string line)
        {
            line = line.Replace(" ", string.Empty);
            int[] tempIndexes;

            switch (line[0])
            {
                case 'L':// Draw Level
                    line = line.Substring(2);
                    tempIndexes = line.Split('x').Select(x => int.Parse(x)).ToArray();
                    Game.Grid = new Grid(Screen.Width, Screen.Height, tempIndexes[0], tempIndexes[1]);
                    Game.Walls.Add(new Tile(0, 0, Game.Grid.numTileWidth, Game.Grid.numTileHeight, 3));
                    break;
                case 'W': //Draw Walls
                    line = line.Substring(2);
                    tempIndexes = line.Split(',').Select(x => int.Parse(x)).ToArray();
                    Game.Walls.Add(new Tile(tempIndexes[0], tempIndexes[1], tempIndexes[2], tempIndexes[3], 1));
                    foreach (Node node in Game.Grid.TileNodes(tempIndexes[0], tempIndexes[1], tempIndexes[2], tempIndexes[3], 1))
                        node.walkable = false;
                    break;
                case 'E': //Draw Enemy
                    line = line.Substring(2);
                    tempIndexes = line.Split(',').Select(x => int.Parse(x)).ToArray();
                    if (Game.Grid.ConvertTileUnitsIntoPixels(tempIndexes[0], tempIndexes[1]) != null)
                    {
                        Game.Enemies.Add(new Enemy(Game.Grid.ConvertTileUnitsIntoPixels(tempIndexes[0], tempIndexes[1]).GetValueOrDefault(), 
                            64, 64, Component.ChooseRandomEnemyType()));
                    } else
                    {
                        Console.WriteLine("Enemy out of bound: " + tempIndexes[0] + "," + tempIndexes[1] + ";" + 
                            Game.Grid.numTileWidth + "," + Game.Grid.numTileHeight);
                    }

                    break;
                case 'P': //Draw Player
                    line = line.Substring(2);
                    tempIndexes = line.Split(',').Select(x => int.Parse(x)).ToArray();
                    if (Game.Grid.ConvertTileUnitsIntoPixels(tempIndexes[0], tempIndexes[1]) != null)
                    {
                        Game.Player = new Player(Game.Grid.ConvertTileUnitsIntoPixels(tempIndexes[0], tempIndexes[1]).GetValueOrDefault(),
                            56, 36, "Textures/Player.png");
                    }
                    else
                    {
                        Console.WriteLine("Player out of bound: " + tempIndexes[0] + "," + tempIndexes[1] + ";" +
                            Game.Grid.numTileWidth + "," + Game.Grid.numTileHeight);
                    }
                    break;
                case 'G':
                    line = line.Substring(2);
                    tempIndexes = line.Split(',').Select(x => int.Parse(x)).ToArray();

                    Game.Walls.Add(new Tile(tempIndexes[0], tempIndexes[1], tempIndexes[0], tempIndexes[1], 5));

                    break;
                default:
                    Console.WriteLine("Unrecognize symbol: " + line[0]);
                    break;
            }
        }
        /*  IMPORTANT:
            ALL PROPERTIES are in TILES UNITS not actual PIXEL UNITS

            Drawing scenes: add "screen:" on top of file
                *NOTE: When creating level button png, make sure to name it: "Button_[level txt name].png" 
                For Example: "Button_level1.png"

            **NOTE do not actually put the ".png" or ".txt" for the name, this is just the file type.

            Drawing levels: add "game:" on top of file
                NOTE: When creating levels, make sure to name it like: "level[level number].txt"
                For Example: "level1.txt"

            **NOTE: ALWAYS DRAW PLAYER FIRST
            Drawing a player: P> x, y
            Drawing a wall: W> topleftx, toplefty, bottomrightx, bottomrighty
            Drawing an enemy: E> x, y
            Drawing a goal: G> x, y
        */
    }
}
