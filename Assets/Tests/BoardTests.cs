using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Reflection;

public class BoardTests
{
    private object board;
    private GameObject boardGameObject;

    [SetUp]
    public void Setup()
    {
        try 
        {
            // Create a new GameObject and add the Board component
            boardGameObject = new GameObject();
            
            // Dynamically load Board type
            var boardType = Type.GetType("Board") ?? 
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == "Board");

            if (boardType == null)
            {
                throw new Exception("Could not find Board type");
            }

            board = boardGameObject.AddComponent(boardType);

            // Create mock GameManager
            var gameManagerType = Type.GetType("GameManager") ?? 
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == "GameManager");

            if (gameManagerType == null)
            {
                throw new Exception("Could not find GameManager type");
            }

            var gameManagerObject = new GameObject();
            var gameManager = gameManagerObject.AddComponent(gameManagerType);
            var instanceProperty = gameManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instanceProperty == null)
            {
                throw new InvalidOperationException("The GameManager 'Instance' property was not found.");
            }  
            instanceProperty.SetValue(null, gameManager);

            // Create mock UIManager
            var uiManagerType = Type.GetType("UIManager") ?? 
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == "UIManager");

            var uiManagerObject = new GameObject();
            var uiManager = uiManagerObject.AddComponent(uiManagerType);
            var uiInstanceProperty = uiManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instanceProperty == null)
            {
                throw new InvalidOperationException("The UIManager 'Instance' property was not found.");
            }  
            uiInstanceProperty.SetValue(null, uiManager);

            // Create mock AudioManager
            var audioManagerType = Type.GetType("AudioManager") ?? 
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == "AudioManager");

            var audioManagerObject = new GameObject();
            var audioManager = audioManagerObject.AddComponent(audioManagerType);
            var audioInstanceProperty = audioManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
            if (instanceProperty == null)
            {
                throw new InvalidOperationException("The AudioManager 'Instance' property was not found.");
            }              
            audioInstanceProperty.SetValue(null, audioManager);

            // Setup mock configurations for testing
            var tileConfigType = Type.GetType("TileConfig") ?? 
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.Name == "TileConfig");

            var tileTypeEnum = tileConfigType.GetNestedType("TileType");

            var tileConfigsField = boardType.GetField("tileConfigs", BindingFlags.Public | BindingFlags.Instance);
            var toolConfigsField = boardType.GetField("toolConfigs", BindingFlags.Public | BindingFlags.Instance);

            var tileConfigs = Array.CreateInstance(tileConfigType, 2);
            tileConfigs.SetValue(CreateMockTileConfig(tileTypeEnum.GetField("Type_00").GetValue(null), false, 10), 0);
            tileConfigs.SetValue(CreateMockTileConfig(tileTypeEnum.GetField("Type_01").GetValue(null), false, 20), 1);
            tileConfigsField.SetValue(board, tileConfigs);

            var toolConfigs = Array.CreateInstance(tileConfigType, 2);
            toolConfigs.SetValue(CreateMockTileConfig(tileTypeEnum.GetField("TOOL_PLUS_CLEARER").GetValue(null), true, 50), 0);
            toolConfigs.SetValue(CreateMockTileConfig(tileTypeEnum.GetField("TOOL_COLUMN_CLEARER").GetValue(null), false, 75), 1);
            toolConfigsField.SetValue(board, toolConfigs);

            // Set some default board parameters
            var heightField = boardType.GetField("height", BindingFlags.Public | BindingFlags.Instance);
            heightField.SetValue(board, 6);

            // Mock the canvas for text animations
            var canvasObject = new GameObject();
            var canvasField = boardType.GetField("canvas", BindingFlags.Public | BindingFlags.Instance);
            canvasField.SetValue(board, canvasObject.AddComponent<Canvas>());

            // Mock tile background
            var tileBackgroundField = boardType.GetField("tileBackground", BindingFlags.Public | BindingFlags.Instance);
            tileBackgroundField.SetValue(board, new GameObject());

            var textMoveAndFadePrefabField = boardType.GetField("textMoveAndFadePrefab", BindingFlags.Public | BindingFlags.Instance);
            textMoveAndFadePrefabField.SetValue(board, new GameObject());
        }
        catch (Exception ex)
        {
            Debug.LogError($"Setup failed: {ex.Message}");
            throw;
        }
    }

    [TearDown]
    public void Teardown()
    {
        // Use DestroyImmediate instead of Destroy in edit mode
        if (boardGameObject != null)
        {
            UnityEngine.Object.DestroyImmediate(boardGameObject);
        }
    }

    private object CreateMockTileConfig(object type, bool isLocked, int coinValue)
    {
        var tileConfigType = Type.GetType("TileConfig") ?? 
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == "TileConfig");

        var config = ScriptableObject.CreateInstance(tileConfigType);

        var tileTypeProperty = tileConfigType.GetProperty("tileType");
        tileTypeProperty.SetValue(config, type);

        var isLockedProperty = tileConfigType.GetProperty("isLocked");
        isLockedProperty.SetValue(config, isLocked);

        var tilePrefabProperty = tileConfigType.GetProperty("tilePrefab");
        tilePrefabProperty.SetValue(config, new GameObject());

        var coinValueProperty = tileConfigType.GetProperty("coinValue");
        coinValueProperty.SetValue(config, coinValue);

        return config;
    }

    [Test]
    public void GenerateBoard_CreatesCorrectBoardSize()
    {
        // Get the method using reflection
        var generateBoardMethod = board.GetType().GetMethod("GenerateBoard");
        generateBoardMethod.Invoke(board, null);

        // Use reflection to access tiles
        var tilesField = board.GetType().GetField("tiles", BindingFlags.Public | BindingFlags.Instance);
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

        // Modify tool configs
        var toolConfigsField = board.GetType().GetField("toolConfigs", BindingFlags.Public | BindingFlags.Instance);
        var toolConfigs = (Array)toolConfigsField.GetValue(board);

        var isLockedProperty = toolConfigs.GetType().GetElementType().GetProperty("isLocked");
        isLockedProperty.SetValue(toolConfigs.GetValue(0), true);
        isLockedProperty.SetValue(toolConfigs.GetValue(1), false);

        // Act and Assert
        Assert.IsFalse((bool)isToolUnlockedMethod.Invoke(board, new object[] { 0 }), "Locked tool should return false");
        Assert.IsTrue((bool)isToolUnlockedMethod.Invoke(board, new object[] { 1 }), "Unlocked tool should return true");
    }
}
