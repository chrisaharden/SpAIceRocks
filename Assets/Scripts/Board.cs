using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 8;
    public int height = 8;
    public Tile[,] tiles;

    public GameObject matchEffectPrefab;
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

    public void GenerateBoard()
    {
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x - width / 2f + 0.5f, y - height / 2f + 0.5f);
                int randomIndex = Random.Range(0, tilePrefabs.Length);
                GameObject tile = Instantiate(tilePrefabs[randomIndex], pos, Quaternion.identity);
                tile.transform.localScale = new Vector3(0.25f, 0.25f, 1f); // Set tile scale to 25% width and height
                tile.name = $"Tile ({x},{y})";

                Tile tileComponent = tile.GetComponent<Tile>();
                tileComponent.x = x;
                tileComponent.y = y;
                tileComponent.type = (Tile.TileType)randomIndex;

                tiles[x, y] = tileComponent;
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSwapping)
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
                    selectedTile = null;
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

        // Animate the tiles moving to each other's positions
        Vector2 aPos = a.transform.position;
        Vector2 bPos = b.transform.position;
        float duration = 0.2f;

        for (float t = 0; t <= 1; t += Time.deltaTime / duration)
        {
            a.transform.position = Vector2.Lerp(aPos, bPos, t);
            b.transform.position = Vector2.Lerp(bPos, aPos, t);
            yield return null;
        }

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
        HashSet<Tile> matchedTiles = FindMatches();

        if (matchedTiles.Count > 0)
        {
            RemoveMatches(matchedTiles);
            StartCoroutine(RefillBoardCoroutine());
        }
        else
        {
            if (!HasPossibleMoves())
            {
                GameOver();
            }
            
            isSwapping = false;
        }
    }

    HashSet<Tile> FindMatches()
    {
        HashSet<Tile> matchedTiles = new HashSet<Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];

                // Check horizontal matches
                if (x <= width - 3)
                {
                    if (tile.type == tiles[x + 1, y].type && tile.type == tiles[x + 2, y].type)
                    {
                        matchedTiles.Add(tile);
                        matchedTiles.Add(tiles[x + 1, y]);
                        matchedTiles.Add(tiles[x + 2, y]);
                    }
                }

                // Check vertical matches
                if (y <= height - 3)
                {
                    if (tile.type == tiles[x, y + 1].type && tile.type == tiles[x, y + 2].type)
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
            tiles[tile.x, tile.y] = null;

            // Instantiate match effect at tile position
            GameObject effect = Instantiate(matchEffectPrefab, tile.transform.position, Quaternion.identity);
            Destroy(effect, 1f);

            // Trigger match animation before destroying tile
            tile.GetComponent<Animator>().SetTrigger("Match");
            Destroy(tile.gameObject, 0.5f);
        }

        GameManager.Instance.AddScore(matchedTiles.Count);
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
                    tile.transform.localScale = new Vector3(0.25f, 0.25f, 1f); // Set tile scale to 25% width and height
                    tile.name = $"Tile ({x},{y})";

                    Tile tileComponent = tile.GetComponent<Tile>();
                    tileComponent.x = x;
                    tileComponent.y = y;
                    tileComponent.type = (Tile.TileType)randomIndex;

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

                // Check horizontal swaps
                if (x < width - 1)
                {
                    Tile rightTile = tiles[x + 1, y];
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

                // Check vertical swaps
                if (y < height - 1)
                {
                    Tile aboveTile = tiles[x, y + 1];
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

        return false;
    }

    void GameOver()
    {
        audioSource.PlayOneShot(gameOverSound);
        UIManager.Instance.ShowGameOver();
    }
}
