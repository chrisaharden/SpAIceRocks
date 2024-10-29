using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private int width = 8;
    public int height = 6;
    public Tile[,] tiles;
    public int movesRemaining = 20;
    
    public GameObject matchEffectPrefab;
    public GameObject gridBlockBackground;
    public AudioClip swapSound;
    public AudioClip matchSound;
    public AudioClip gameOverSound;

    public GameObject[] tilePrefabs;

    public AudioSource audioSource;
    private Tile selectedTile;
    private bool isSwapping;
    
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
                GameObject background = Instantiate(gridBlockBackground, pos, Quaternion.identity, transform);
                background.name = $"GridBackground ({x},{y})";
                background.transform.position = new Vector3(pos.x, pos.y, 0.05f);
            }
        }
    }

    public void GenerateBoard()
    {
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
                Tile.TileType tileType;
                
                // If this is the robot position and we should place a robot
                if (shouldPlaceRobot && x == robotPosition.x && y == robotPosition.y)
                {
                    // Get the most recently collected robot
                    int lastRobotIndex = (GameManager.Instance.robotsCollected - 1) % GameManager.Instance.robotPrefabs.Length;
                    tilePrefab = GameManager.Instance.robotPrefabs[lastRobotIndex];
                    tileType = Tile.TileType.Robot;
                }
                else
                {
                    // Place a regular tile
                    int randomIndex = Random.Range(0, tilePrefabs.Length);
                    tilePrefab = tilePrefabs[randomIndex];
                    // Ensure we're using the correct tile type based on the prefab index
                    // The enum order should match: Type_00, Type_1, Type_02,...
                    tileType = (Tile.TileType)randomIndex;
                    
                    // Make sure we don't accidentally set it as Robot type
                    if ((int)tileType >= (int)Tile.TileType.Robot)
                    {
                        tileType = Tile.TileType.Type_00; // Default to Yellow if we somehow get an invalid index
                    }
                }

                GameObject tile = Instantiate(tilePrefab, pos, Quaternion.identity);
                tile.name = $"Tile ({x},{y})";

                Tile tileComponent = tile.GetComponent<Tile>();
                tileComponent.x = x;
                tileComponent.y = y;
                tileComponent.type = tileType;

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
                    selectedTile = tile;
                }
                else if (selectedTile != tile)
                {
                    StartCoroutine(SwapTilesCoroutine(selectedTile, tile));
                    //selectedTile = null;
                    movesRemaining--;
                    UIManager.Instance.UpdateMoves(movesRemaining);
                }
            }
        }
    }

    Tile GetTileAtPosition(Vector2 pos)
    {
        int x = Mathf.RoundToInt(pos.x + width / 2f - 0.5f);
        int y = Mathf.RoundToInt(pos.y + height / 2f - 0.5f);

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return tiles[x, y];
        }
        else
        {
            return null;
        }
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
        audioSource.PlayOneShot(matchSound);

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

        GameManager.Instance.AddItemsCollected(matchedTiles.Count);
    }

    IEnumerator RefillBoardCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    ShiftTilesDown(x, y);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y] == null)
                {
                    Vector2 pos = new Vector2(x - width / 2f + 0.5f, y - height / 2f + 0.5f);
                    int randomIndex = Random.Range(0, tilePrefabs.Length);
                    GameObject tile = Instantiate(tilePrefabs[randomIndex], pos, Quaternion.identity);
                    tile.name = $"Tile ({x},{y})";

                    Tile tileComponent = tile.GetComponent<Tile>();
                    tileComponent.x = x;
                    tileComponent.y = y;
                    
                    // Ensure we don't create Robot type tiles during refill
                    Tile.TileType newType = (Tile.TileType)randomIndex;
                    if ((int)newType >= (int)Tile.TileType.Robot)
                    {
                        newType = Tile.TileType.Type_00;
                    }
                    tileComponent.type = newType;

                    tiles[x, y] = tileComponent;
                }
            }
        }

        CheckForMatches();
    }

    void ShiftTilesDown(int x, int startY)
    {
        for (int y = startY; y < height - 1; y++)
        {
            tiles[x, y] = tiles[x, y + 1];

            if (tiles[x, y] != null)
            {
                Vector2 pos = new Vector2(x - width / 2f + 0.5f, y - height / 2f + 0.5f);
                tiles[x, y].transform.position = pos;
                tiles[x, y].y = y;
            }
        }
    }

    bool HasPossibleMoves()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                if (tile == null) continue;

                // Check horizontal swaps
                if (x < width - 1)
                {
                    Tile rightTile = tiles[x + 1, y];
                    if (rightTile != null)
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
                    if (aboveTile != null)
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
