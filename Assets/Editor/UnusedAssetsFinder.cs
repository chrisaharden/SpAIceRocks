using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class UnusedAssetsFinder : EditorWindow
{
    private Vector2 scrollPosition;
    private List<string> unusedAssets = new List<string>();
    private bool includeScripts = true;
    private bool includeTextures = true;
    private bool includeMaterials = true;
    private bool includeAudio = true;
    private bool includePrefabs = true;
    private bool includeAnimations = true;  
    private bool includeAnimControllers = true;
    
    // Exclusion settings
    private bool excludeTextMeshPro = true;
    private string customExcludePath = "";
    private List<string> excludedPaths = new List<string>();
    private Vector2 excludedPathsScroll;
    private bool showExcludedPaths = false;

    // Search filter
    private string searchFilter = "";
    private List<string> filteredAssets = new List<string>();
    private bool showDeleteAllButton = true;

    [MenuItem("Tools/Unused Assets Finder")]
    public static void ShowWindow()
    {
        GetWindow<UnusedAssetsFinder>("Unused Assets Finder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Unused Assets Finder", EditorStyles.boldLabel);

        EditorGUILayout.Space();
        
        // Asset type filters
        GUILayout.Label("Asset Types to Check:", EditorStyles.boldLabel);
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            includeScripts = EditorGUILayout.Toggle("Scripts (.cs)", includeScripts);
            includeTextures = EditorGUILayout.Toggle("Textures (.png, .jpg)", includeTextures);
            includeMaterials = EditorGUILayout.Toggle("Materials (.mat)", includeMaterials);
            includeAudio = EditorGUILayout.Toggle("Audio (.mp3, .wav)", includeAudio);
            includePrefabs = EditorGUILayout.Toggle("Prefabs (.prefab)", includePrefabs);
            includeAnimations = EditorGUILayout.Toggle("Animations (.anim)", includeAnimations);
            includeAnimControllers = EditorGUILayout.Toggle("Animation Controllers (.controller)", includeAnimControllers);
        }

        EditorGUILayout.Space();

        // Exclusion settings
        GUILayout.Label("Folder Exclusions:", EditorStyles.boldLabel);
        excludeTextMeshPro = EditorGUILayout.Toggle("Exclude TextMesh Pro", excludeTextMeshPro);
        
        // Custom exclusion path
        EditorGUILayout.BeginHorizontal();
        customExcludePath = EditorGUILayout.TextField("Custom Path to Exclude:", customExcludePath);
        if (GUILayout.Button("Add", GUILayout.Width(60)))
        {
            if (!string.IsNullOrEmpty(customExcludePath) && !excludedPaths.Contains(customExcludePath))
            {
                excludedPaths.Add(customExcludePath);
                customExcludePath = "";
                GUI.FocusControl(null);
            }
        }
        EditorGUILayout.EndHorizontal();

        // Show/hide excluded paths
        showExcludedPaths = EditorGUILayout.Foldout(showExcludedPaths, "Current Exclusions");
        if (showExcludedPaths && excludedPaths.Count > 0)
        {
            excludedPathsScroll = EditorGUILayout.BeginScrollView(excludedPathsScroll, GUILayout.Height(100));
            for (int i = excludedPaths.Count - 1; i >= 0; i--)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(excludedPaths[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(60)))
                {
                    excludedPaths.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Find Unused Assets"))
        {
            FindUnusedAssets();
            UpdateFilteredList();
        }

        EditorGUILayout.Space();

        if (unusedAssets.Count > 0)
        {
            // Search filter
            EditorGUILayout.BeginHorizontal();
            GUI.SetNextControlName("SearchField");
            string newSearchFilter = EditorGUILayout.TextField("Search Filter:", searchFilter);
            if (newSearchFilter != searchFilter)
            {
                searchFilter = newSearchFilter;
                UpdateFilteredList();
            }
            if (GUILayout.Button("Clear", GUILayout.Width(60)))
            {
                searchFilter = "";
                GUI.FocusControl("SearchField");
                UpdateFilteredList();
            }
            EditorGUILayout.EndHorizontal();

            // Asset type summary
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField($"Total unused assets found: {unusedAssets.Count}");
                EditorGUILayout.LabelField($"Filtered results shown: {filteredAssets.Count}");
                
                // Show type breakdown
                var typeBreakdown = GetAssetTypeBreakdown(filteredAssets);
                foreach (var type in typeBreakdown)
                {
                    EditorGUILayout.LabelField($"{type.Key}: {type.Value}");
                }
            }
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            foreach (string asset in filteredAssets)
            {
                EditorGUILayout.BeginHorizontal();
                
                // Get icon for the asset type
                Texture icon = AssetDatabase.GetCachedIcon(asset);
                
                // Show icon and path
                GUILayout.Label(icon, GUILayout.Width(20), GUILayout.Height(20));
                
                // Highlight matching text if there's a search filter
                if (!string.IsNullOrEmpty(searchFilter))
                {
                    int startIndex = asset.ToLower().IndexOf(searchFilter.ToLower());
                    if (startIndex >= 0)
                    {
                        string before = asset.Substring(0, startIndex);
                        string highlight = asset.Substring(startIndex, searchFilter.Length);
                        string after = asset.Substring(startIndex + searchFilter.Length);
                        
                        EditorGUILayout.LabelField(before + highlight + after);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(asset);
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(asset);
                }
                
                if (GUILayout.Button("Select", GUILayout.Width(60)))
                {
                    Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(asset);
                }
                
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Delete Asset",
                        $"Are you sure you want to delete {Path.GetFileName(asset)}?",
                        "Delete", "Cancel"))
                    {
                        AssetDatabase.DeleteAsset(asset);
                        unusedAssets.Remove(asset);
                        UpdateFilteredList();
                        GUIUtility.ExitGUI();
                    }
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndScrollView();

            // Only show Delete All button when no filter is active
            if (string.IsNullOrEmpty(searchFilter) && showDeleteAllButton)
            {
                if (GUILayout.Button("Delete All Unused Assets"))
                {
                    if (EditorUtility.DisplayDialog("Delete All Unused Assets",
                        $"Are you sure you want to delete all {unusedAssets.Count} unused assets?",
                        "Delete All", "Cancel"))
                    {
                        DeleteAllUnusedAssets();
                    }
                }
            }
            // Show Delete Filtered Items button when filter is active
            else if (!string.IsNullOrEmpty(searchFilter) && filteredAssets.Count > 0)
            {
                if (GUILayout.Button($"Delete All Filtered Assets ({filteredAssets.Count} items)"))
                {
                    if (EditorUtility.DisplayDialog("Delete Filtered Assets",
                        $"Are you sure you want to delete all {filteredAssets.Count} filtered assets?",
                        "Delete Filtered", "Cancel"))
                    {
                        DeleteFilteredAssets();
                    }
                }
            }
        }
    }

    private Dictionary<string, int> GetAssetTypeBreakdown(List<string> assets)
    {
        var breakdown = new Dictionary<string, int>();
        
        foreach (string asset in assets)
        {
            string extension = Path.GetExtension(asset).ToLower();
            string type = GetAssetTypeDisplay(extension);
            
            if (breakdown.ContainsKey(type))
                breakdown[type]++;
            else
                breakdown[type] = 1;
        }
        
        return breakdown;
    }

    private string GetAssetTypeDisplay(string extension)
    {
        switch (extension)
        {
            case ".cs": return "Scripts";
            case ".png":
            case ".jpg":
            case ".jpeg": return "Textures";
            case ".mat": return "Materials";
            case ".mp3":
            case ".wav": return "Audio";
            case ".prefab": return "Prefabs";
            case ".anim": return "Animations";
            case ".controller": return "Animation Controllers";
            default: return "Other";
        }
    }

    private void UpdateFilteredList()
    {
        if (string.IsNullOrEmpty(searchFilter))
        {
            filteredAssets = new List<string>(unusedAssets);
        }
        else
        {
            filteredAssets = unusedAssets
                .Where(asset => asset.ToLower().Contains(searchFilter.ToLower()))
                .ToList();
        }
    }

    private void DeleteFilteredAssets()
    {
        foreach (string asset in filteredAssets.ToList())
        {
            AssetDatabase.DeleteAsset(asset);
            unusedAssets.Remove(asset);
        }
        UpdateFilteredList();
    }

    private bool IsExcludedPath(string assetPath)
    {
        // Check TextMesh Pro exclusion
        if (excludeTextMeshPro && assetPath.Contains("TextMesh Pro"))
            return true;

        // Check custom exclusions
        return excludedPaths.Any(excludePath => assetPath.Contains(excludePath));
    }

    private void FindUnusedAssets()
    {
        unusedAssets.Clear();
        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        HashSet<string> usedAssets = new HashSet<string>();

        // Get all scenes in build settings
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                CollectDependenciesRecursively(scene.path, usedAssets);
            }
        }

        // Check each asset
        foreach (string asset in allAssets)
        {
            if (!asset.StartsWith("Assets/"))
                continue;

            // Skip excluded paths
            if (IsExcludedPath(asset))
                continue;

            bool shouldCheck = false;
            string extension = Path.GetExtension(asset).ToLower();

            // Filter based on asset type
            if (includeScripts && extension == ".cs")
                shouldCheck = true;
            else if (includeTextures && (extension == ".png" || extension == ".jpg" || extension == ".jpeg"))
                shouldCheck = true;
            else if (includeMaterials && extension == ".mat")
                shouldCheck = true;
            else if (includeAudio && (extension == ".mp3" || extension == ".wav"))
                shouldCheck = true;
            else if (includePrefabs && extension == ".prefab")
                shouldCheck = true;
            else if (includeAnimations && extension == ".anim")  // New animation check
                shouldCheck = true;
            else if (includeAnimControllers && extension == ".controller")
                shouldCheck = true;

            if (shouldCheck && !usedAssets.Contains(asset))
            {
                unusedAssets.Add(asset);
            }
        }
    }

    private void CollectDependenciesRecursively(string assetPath, HashSet<string> collectedAssets)
    {
        if (string.IsNullOrEmpty(assetPath) || collectedAssets.Contains(assetPath))
            return;

        collectedAssets.Add(assetPath);

        foreach (string dependency in AssetDatabase.GetDependencies(assetPath, false))
        {
            CollectDependenciesRecursively(dependency, collectedAssets);
        }
    }

    private void DeleteAllUnusedAssets()
    {
        foreach (string asset in unusedAssets.ToList())
        {
            AssetDatabase.DeleteAsset(asset);
        }
        unusedAssets.Clear();
        filteredAssets.Clear();
    }
}