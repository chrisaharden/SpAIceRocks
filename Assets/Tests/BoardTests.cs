using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

// Explicitly add references to game scripts
using UnityEngine.UI;
using TMPro;

namespace SpaiceRocks.Tests
{
    public class BoardTests
    {
        private object board;
        private GameObject boardGameObject;
        private object gameManager;
        private GameObject gameManagerObject;

        [SetUp]
        public void Setup()
        {
            try 
            {
                // Dynamically load types
                var boardType = Type.GetType("Board") ?? 
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.Name == "Board");

                var gameManagerType = Type.GetType("GameManager") ?? 
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.Name == "GameManager");

                var uiManagerType = Type.GetType("UIManager") ?? 
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.Name == "UIManager");

                var audioManagerType = Type.GetType("AudioManager") ?? 
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.Name == "AudioManager");

                var tileConfigType = Type.GetType("TileConfig") ?? 
                    AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a => a.GetTypes())
                        .FirstOrDefault(t => t.Name == "TileConfig");

                if (boardType == null || gameManagerType == null || uiManagerType == null || 
                    audioManagerType == null || tileConfigType == null)
                {
                    throw new Exception("Could not find one or more required types");
                }

                // Create GameManager
                gameManagerObject = new GameObject();
                gameManager = gameManagerObject.AddComponent(gameManagerType);
                var gameManagerInstanceProperty = gameManagerType.GetProperty("Instance", 
                    BindingFlags.Public | BindingFlags.Static);
                gameManagerInstanceProperty.SetValue(null, gameManager);

                // Create Board
                boardGameObject = new GameObject();
                board = boardGameObject.AddComponent(boardType);

                // Create UIManager
                var uiManagerObject = new GameObject();
                var uiManager = uiManagerObject.AddComponent(uiManagerType);
                var uiInstanceProperty = uiManagerType.GetProperty("Instance", 
                    BindingFlags.Public | BindingFlags.Static);
                uiInstanceProperty.SetValue(null, uiManager);

                // Create AudioManager
                var audioManagerObject = new GameObject();
                var audioManager = audioManagerObject.AddComponent(audioManagerType);
                var audioInstanceProperty = audioManagerType.GetProperty("Instance", 
                    BindingFlags.Public | BindingFlags.Static);
                audioInstanceProperty.SetValue(null, audioManager);

                // Setup tile configurations
                var tileConfigsField = boardType.GetField("tileConfigs", 
                    BindingFlags.Public | BindingFlags.Instance);
                var tileConfigs = Array.CreateInstance(tileConfigType, 2);

                var tileTypeProperty = tileConfigType.GetProperty("tileType");
                var isLockedProperty = tileConfigType.GetProperty("isLocked");
                var tilePrefabProperty = tileConfigType.GetProperty("tilePrefab");

                var tileTypeEnum = tileConfigType.GetNestedType("TileType");

                // First tile config
                var tileConfig1 = ScriptableObject.CreateInstance(tileConfigType);
                tileTypeProperty.SetValue(tileConfig1, Enum.Parse(tileTypeEnum, "Type_00"));
                isLockedProperty.SetValue(tileConfig1, false);
                tilePrefabProperty.SetValue(tileConfig1, new GameObject());
                tileConfigs.SetValue(tileConfig1, 0);

                // Second tile config
                var tileConfig2 = ScriptableObject.CreateInstance(tileConfigType);
                tileTypeProperty.SetValue(tileConfig2, Enum.Parse(tileTypeEnum, "Type_01"));
                isLockedProperty.SetValue(tileConfig2, false);
                tilePrefabProperty.SetValue(tileConfig2, new GameObject());
                tileConfigs.SetValue(tileConfig2, 1);

                tileConfigsField.SetValue(board, tileConfigs);

                // Setup tool configurations
                var toolConfigsField = boardType.GetField("toolConfigs", 
                    BindingFlags.Public | BindingFlags.Instance);
                var toolConfigs = Array.CreateInstance(tileConfigType, 2);

                // First tool config
                var toolConfig1 = ScriptableObject.CreateInstance(tileConfigType);
                tileTypeProperty.SetValue(toolConfig1, Enum.Parse(tileTypeEnum, "TOOL_PLUS_CLEARER"));
                isLockedProperty.SetValue(toolConfig1, true);
                tilePrefabProperty.SetValue(toolConfig1, new GameObject());
                toolConfigs.SetValue(toolConfig1, 0);

                // Second tool config
                var toolConfig2 = ScriptableObject.CreateInstance(tileConfigType);
                tileTypeProperty.SetValue(toolConfig2, Enum.Parse(tileTypeEnum, "TOOL_COLUMN_CLEARER"));
                isLockedProperty.SetValue(toolConfig2, false);
                tilePrefabProperty.SetValue(toolConfig2, new GameObject());
                toolConfigs.SetValue(toolConfig2, 1);

                toolConfigsField.SetValue(board, toolConfigs);

                // Set board height
                var heightField = boardType.GetField("height", 
                    BindingFlags.Public | BindingFlags.Instance);
                heightField.SetValue(board, 6);

                // Mock canvas
                var canvasObject = new GameObject();
                var canvasField = boardType.GetField("canvas", 
                    BindingFlags.Public | BindingFlags.Instance);
                canvasField.SetValue(board, canvasObject.AddComponent<Canvas>());

                // Mock tile background
                var tileBackgroundField = boardType.GetField("tileBackground", 
                    BindingFlags.Public | BindingFlags.Instance);
                tileBackgroundField.SetValue(board, new GameObject());

                var textMoveAndFadePrefabField = boardType.GetField("textMoveAndFadePrefab", 
                    BindingFlags.Public | BindingFlags.Instance);
                textMoveAndFadePrefabField.SetValue(board, new GameObject());

                Debug.Log("Board Setup Completed Successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Setup failed with exception: {ex.Message}");
                Debug.LogError(ex.StackTrace);
                throw;
            }
        }

        [TearDown]
        public void Teardown()
        {
            if (boardGameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(boardGameObject);
            }
            if (gameManagerObject != null)
            {
                UnityEngine.Object.DestroyImmediate(gameManagerObject);
            }
        }

        [Test]
        public void GenerateBoard_CreatesCorrectBoardSize()
        {
            // Act
            var generateBoardMethod = board.GetType().GetMethod("GenerateBoard");
            generateBoardMethod.Invoke(board, null);

            // Use reflection to access tiles
            var tilesField = board.GetType().GetField("tiles");
            var tiles = tilesField.GetValue(board);

            // Assert using reflection
            Assert.IsNotNull(tiles, "Tiles array should not be null");
            Assert.AreEqual(6, ((Array)tiles).GetLength(0), "Board width should be 6");
            Assert.AreEqual(6, ((Array)tiles).GetLength(1), "Board height should be 6");
        }

        [Test]
        public void IsToolUnlocked_ReturnsCorrectStatus()
        {
            // Get the method using reflection
            var isToolUnlockedMethod = board.GetType().GetMethod("IsToolUnlocked");

            // Act and Assert
            Assert.IsFalse((bool)isToolUnlockedMethod.Invoke(board, new object[] { 0 }), "Locked tool should return false");
            Assert.IsTrue((bool)isToolUnlockedMethod.Invoke(board, new object[] { 1 }), "Unlocked tool should return true");
        }
    }
}
