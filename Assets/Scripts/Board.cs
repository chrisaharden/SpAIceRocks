using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TileConfig
{
    public Tile.TileType tileType;
    public bool isLocked;
    public int coinValue;
    public int purchasePrice;
    public int value; // Add this property
}

public class Board : MonoBehaviour
{
    private int width = 8;
    public int height = 6;
    public Tile[,] tiles;
    public int movesRemaining = 20;
    
    [Header("Tile Configuration")]
    [Tooltip("Configure each tile type's properties. This array should match the tilePrefabs array order.")]
    public TileConfig[] tileConfigs;
    public GameObject[] tilePrefabs;

    public GameObject tileBackground;

    public AudioClip swapSound;
    public AudioClip matchSound;
    public AudioClip gameOverSound;
    public AudioClip robotRemovalSound;

    public AudioSource audioSource;
    private Tile selectedTile;
    private bool isSwapping;

    public void UnlockTileType(int tileIndex)
    {
        if (tileIndex >= 0 && tileIndex < tileConfigs.Length)
        {
            TileConfig config = tileConfigs[tileIndex];
            config.isLocked = false;
            tileConfigs[tileIndex] = config;

            // Update any existing tiles of this type on the board
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tiles[x, y] != null && (int)tiles[x, y].type == tileIndex)
                    {
                        tiles[x, y].isLocked = false;
                    }
                }
            }
        }
    }
    
    void Start()
    {
        //audioSource = GetComponent<AudioSource>();
        //GenerateBoard();
    }

    public void UpdateBoardSize(int level)
    {
        // Calculate new width based on level (starting at min, max is the second param)
        int newWidth = Mathf.Min(8 + (level - 1), 12);
        width = newWidth;
        ClearBoard();
        GenerateBoard();
    }

    public void ClearBoard()
    {
        if (tiles != null)
        {
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (tiles[x, y] != null)
                    {
                        Destroy(tiles[x, y].gameObject);
                    }
                }
            }
        }

        // Clear background grid
        foreach (Transform child in transform)
        {
            if (child.gameObject.name.StartsWith("GridBackground"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreateBackgroundGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x - width / 2f + 0.5f, y - height / 2f + 0.5f);
                GameObject background = Instantiate(tileBackground, pos, Quaternion.identity, transform);
                background.name = $"GridBackground ({x},{y})";
                background.transform.position = new Vector3(pos.x, pos.y, 0.05f);
            }
        }
    }

    public void GenerateBoard()
    {
        // Validate tile configurations
        if (tileConfigs == null || tileConfigs.Length == 0)
        {
            Debug.LogError("Tile configurations are not set up!");
            return;
        }

        // Create a list of unlocked tile configurations
        List<TileConfig> unlockedConfigs = new List<TileConfig>();
        List<GameObject> unlockedPrefabs = new List<GameObject>();
        
        for (int i = 0; i < tileConfigs.Length; i++)
        {
            if (!tileConfigs[i].isLocked)
            {
                unlockedConfigs.Add(tileConfigs[i]);
                unlockedPrefabs.Add(tilePrefabs[i]);
            }
        }

        // Ensure we have at least one unlocked tile type
        if (unlockedConfigs.Count == 0)
        {
            Debug.LogError("No unlocked tile configurations available!");
            return;
        }

        movesRemaining = 20;
        tiles = new Tile[width, height];

        CreateBackgroundGrid();

        // If we have collected robots, choose a random position to place one
        bool shouldPlaceRobot = GameManager.Instance.robotsCollected > 0 && GameManager.Instance.robotPrefabs != null && GameManager.Instance.robotPrefabs.Length > 0;
        Vector2Int robotPosition = new Vector2Int(-1, -1);
        
        if (shouldPlaceRobot)
        {
            robotPosition.x = Random.Range(0, width);
            robotPosition.y = Random.Range(0, height);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x - width / 2f + 0.5f, y - height / 2f + 0.5f);
                GameObject tilePrefab;
                TileConfig config;
                
                // If this is the robot position and we should place a robot
                if (shouldPlaceRobot && x == robotPosition.x && y == robotPosition.y)
                {
                    // Get the most recently collected robot
                    int lastRobotIndex = (GameManager.Instance.robotsCollected - 1) % GameManager.Instance.robotPrefabs.Length;
                    tilePrefab = GameManager.Instance.robotPrefabs[lastRobotIndex];
                    config = new TileConfig { tileType = Tile.TileType.Robot, isLocked = false, coinValue = 0, purchasePrice = 0 };
                }
                else
                {
                    // Place a regular unlocked tile
                    int randomIndex = Random.Range(0, unlockedConfigs.Count);
                    tilePrefab = unlockedPrefabs[randomIndex];
                    config = unlockedConfigs[randomIndex];
                }

                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.name = $"Tile ({x},{y})";

                Tile tileComponent = tile.GetComponent<Tile>();
                tileComponent.x = x;
                tileComponent.y = y;
                tileComponent.type = config.tileType;
                tileComponent.isLocked = config.isLocked;
                tileComponent.coinValue = config.coinValue;
                tileComponent.purchasePrice = config.purchasePrice;

                tiles[x, y] = tileComponent;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSwapping && movesRemaining > 0)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tile tile = GetTileAtPosition(mousePos);

            if (tile != null)
            {
                if (selectedTile == null)
                {
                    if (!tile.isLocked)
                        selectedTile = tile;
                }
                else if (selectedTile != tile)
                {
                    if (!tile.isLocked && AreAdjacent(selectedTile, tile))
                    {
                        StartCoroutine(SwapTilesCoroutine(selectedTile, tile));
                        movesRemaining--;
                        UIManager.Instance.UpdateMoves(movesRemaining);
                    }
                    else
                    {
                        selectedTile = !tile.isLocked ? tile : null;
                    }
                }
            }
        }
    }

    // Check if two tiles are adjacent (horizontally or vertically)
    private bool AreAdjacent(Tile a, Tile b)
    {
        return (Mathf.Abs(a.x - b.x) == 1 && a.y == b.y) || // Horizontally adjacent
               (Mathf.Abs(a.y - b.y) == 1 && a.x == b.x);   // Vertically adjacent
    }

    Tile GetTileAtPosition(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x + width / 2f - 0.5f);
        int y = Mathf.RoundToInt(pos.y + height / 2f - 0.5f);

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y];
        }
        return null;
    }

    IEnumerator SwapTilesCoroutine(Tile a, Tile b)
    {
        isSwapping = true;

        audioSource.PlayOneShot(swapSound);

        // Store original positions including z
        Vector3 aPos = a.transform.position;
        Vector3 bPos = b.transform.position;
        
        // Move tiles forward in z-space during animation
        aPos.z = -1;
        bPos.z = -1;
        
        float duration = 0.2f;

        for (float t = 0; t <= 1; t += Time.deltaTime / duration)
        {
            Vector3 newAPos = Vector3.Lerp(a.transform.position, bPos, t);
            Vector3 newBPos = Vector3.Lerp(b.transform.position, aPos, t);
            
            // Maintain forward z-position during animation
            newAPos.z = -1;
            newBPos.z = -1;
            
            a.transform.position = newAPos;
            b.transform.position = newBPos;
            yield return null;
        }

        // Reset z positions after swap
        aPos.z = 0;
        bPos.z = 0;
        a.transform.position = bPos;
        b.transform.position = aPos;

        tiles[a.x, a.y] = b;
        tiles[b.x, b.y] = a;

        int tempX = a.x;
        int tempY = a.y;
        a.x = b.x;
        a.y = b.y;
        b.x = tempX;
        b.y = tempY;

        yield return new WaitForSeconds(0.2f);

        CheckForMatches();
    }

    void CheckForMatches()
    {
        HashSet<Tile> matchedTiles = new HashSet<Tile>();

        // Check for robot matches only if a robot tile was involved in the swap
        if (selectedTile != null && (selectedTile.type == Tile.TileType.Robot || 
            (tiles[selectedTile.x, selectedTile.y] != null && tiles[selectedTile.x, selectedTile.y].type == Tile.TileType.Robot)))
        {
            matchedTiles.UnionWith(FindRobotMatches());
        }
        else
        {
            matchedTiles.UnionWith(FindMatches());
        }
        selectedTile = null;

        if (matchedTiles.Count > 0)
        {
            RemoveMatches(matchedTiles);
            StartCoroutine(RefillBoardCoroutine());
        }
        else
        {
            if (!HasPossibleMoves() || movesRemaining <= 0)
            {
                GameOver();
            }
            
            isSwapping = false;
        }
    }

    HashSet<Tile> FindRobotMatches()
    {
        HashSet<Tile> matchedTiles = new HashSet<Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                if (tile != null && tile.type == Tile.TileType.Robot)
                {
                    bool hasAdjacentMatch = false;
                    // Add adjacent tiles (up, down, left, right)
                    if (x > 0 && tiles[x - 1, y] != null && tiles[x - 1, y].type != Tile.TileType.Robot) 
                    {
                        matchedTiles.Add(tiles[x - 1, y]);
                        hasAdjacentMatch = true;
                    }
                    if (x < width - 1 && tiles[x + 1, y] != null && tiles[x + 1, y].type != Tile.TileType.Robot) 
                    {
                        matchedTiles.Add(tiles[x + 1, y]);
                        hasAdjacentMatch = true;
                    }
                    if (y > 0 && tiles[x, y - 1] != null && tiles[x, y - 1].type != Tile.TileType.Robot) 
                    {
                        matchedTiles.Add(tiles[x, y - 1]);
                        hasAdjacentMatch = true;
                    }
                    if (y < height - 1 && tiles[x, y + 1] != null && tiles[x, y + 1].type != Tile.TileType.Robot) 
                    {
                        matchedTiles.Add(tiles[x, y + 1]);
                        hasAdjacentMatch = true;
                    }
                    
                    // If the robot matched with any adjacent tiles, add it to be removed
                    if (hasAdjacentMatch)
                    {
                        matchedTiles.Add(tile);
                    }
                }
            }
        }

        return matchedTiles;
    }

    HashSet<Tile> FindMatches()
    {
        HashSet<Tile> matchedTiles = new HashSet<Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                
                // Skip robot tiles and null tiles for regular matching
                if (tile == null || tile.type == Tile.TileType.Robot)
                    continue;

                // Check horizontal matches
                if (x <= width - 3)
                {
                    if (tiles[x + 1, y] != null && tiles[x + 2, y] != null &&
                        tile.type == tiles[x + 1, y].type && tile.type == tiles[x + 2, y].type)
                    {
                        matchedTiles.Add(tile);
                        matchedTiles.Add(tiles[x + 1, y]);
                        matchedTiles.Add(tiles[x + 2, y]);
                    }
                }

                // Check vertical matches
                if (y <= height - 3)
                {
                    if (tiles[x, y + 1] != null && tiles[x, y + 2] != null &&
                        tile.type == tiles[x, y + 1].type && tile.type == tiles[x, y + 2].type)
                    {
                        matchedTiles.Add(tile);
                        matchedTiles.Add(tiles[x, y + 1]);
                        matchedTiles.Add(tiles[x, y + 2]);
                    }
                }
            }
        }

        return matchedTiles;
    }

    void RemoveMatches(HashSet<Tile> matchedTiles)
    {
        bool containsRobot = false;
        int totalCoins = 0;

        foreach (Tile tile in matchedTiles)
        {
            if (tile != null)
            {
                if (tile.type == Tile.TileType.Robot)
                {
                    containsRobot = true;
                }
                else
                {
                    totalCoins += tile.coinValue;
                }
            }
        }

        if (containsRobot && robotRemovalSound != null)
        {
            audioSource.PlayOneShot(robotRemovalSound);
            audioSource.PlayOneShot(matchSound);
        }
        else
        {
            audioSource.PlayOneShot(matchSound);
        }

        foreach (Tile tile in matchedTiles)
        {
            if (tile == null)
                continue;
                
            tiles[tile.x, tile.y] = null;

            // Get the animator component and play the shrink animation
            Animator animator = tile.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("ShrinkAnimation");
            }

            // Destroy the tile after animation duration
            Destroy(tile.gameObject, 0.5f);
        }

        GameManager.Instance.AddItemsCollected(matchedTiles);
    }

    IEnumerator RefillBoardCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        // Create lists of unlocked configurations for refilling
        List<TileConfig> unlockedConfigs = new List<TileConfig>();
        List<GameObject> unlockedPrefabs = new List<GameObject>();
        
        for (int i = 0; i < tileConfigs.Length; i++)
        {
            if (!tileConfigs[i].isLocked)
            {
                unlockedConfigs.Add(tileConfigs[i]);
                unlockedPrefabs.Add(tilePrefabs[i]);
            }
        }

        // First pass: Shift existing tiles down and create new ones at the top
        for (int x = 0; x < width; x++)
        {
            int emptySpaces = 0;
            // Count empty spaces from bottom to top
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    emptySpaces++;
                }
                else if (emptySpaces > 0)
                {
                    // Move this tile down by the number of empty spaces
                    tiles[x, y - emptySpaces] = tiles[x, y];
                    tiles[x, y] = null;
                    Vector2 newPos = new Vector2(x - width / 2f + 0.5f, (y - emptySpaces) - height / 2f + 0.5f);
                    tiles[x, y - emptySpaces].transform.position = newPos;
                    tiles[x, y - emptySpaces].y = y - emptySpaces;
                }
            }

            // Fill empty spaces at the top with unlocked tiles
            for (int i = 0; i < emptySpaces; i++)
            {
                int y = height - 1 - i;
                Vector2 pos = new Vector2(x - width / 2f + 0.5f, y - height / 2f + 0.5f);
                int randomIndex = Random.Range(0, unlockedConfigs.Count);
                GameObject tile = Instantiate(unlockedPrefabs[randomIndex], pos, Quaternion.identity);
                tile.name = $"Tile ({x},{y})";

                Tile tileComponent = tile.GetComponent<Tile>();
                tileComponent.x = x;
                tileComponent.y = y;
                
                // Use the unlocked configuration for this tile type
                TileConfig config = unlockedConfigs[randomIndex];
                tileComponent.type = config.tileType;
                tileComponent.isLocked = config.isLocked;
                tileComponent.coinValue = config.coinValue;
                tileComponent.purchasePrice = config.purchasePrice;

                tiles[x, y] = tileComponent;
            }
        }

        yield return new WaitForSeconds(0.5f);

        CheckForMatches();
    }

    bool HasPossibleMoves()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                if (tile == null || tile.isLocked) continue;

                // Check horizontal swaps
                if (x < width - 1)
                {
                    Tile rightTile = tiles[x + 1, y];
                    if (rightTile != null && !rightTile.isLocked)
                    {
                        tiles[x, y] = rightTile;
                        tiles[x + 1, y] = tile;
                        if (FindMatches().Count > 0)
                        {
                            tiles[x, y] = tile;
                            tiles[x + 1, y] = rightTile;
                            return true;
                        }
                        tiles[x, y] = tile;
                        tiles[x + 1, y] = rightTile;
                    }
                }

                // Check vertical swaps
                if (y < height - 1)
                {
                    Tile aboveTile = tiles[x, y + 1];
                    if (aboveTile != null && !aboveTile.isLocked)
                    {
                        tiles[x, y] = aboveTile;
                        tiles[x, y + 1] = tile;
                        if (FindMatches().Count > 0)
                        {
                            tiles[x, y] = tile;
                            tiles[x, y + 1] = aboveTile;
                            return true;
                        }
                        tiles[x, y] = tile;
                        tiles[x, y + 1] = aboveTile;
                    }
                }
            }
        }

        return false;
    }

    void GameOver()
    {
        audioSource.PlayOneShot(gameOverSound);
        GameManager.Instance.GameOver();
    }
}
