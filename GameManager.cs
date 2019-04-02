using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject TilePrefab;

    public Vector2 gridWorldSize; // the size of the grid
    [HideInInspector]
    public float nodeRadius = 0.5f; //how much space the individual space covers
    [HideInInspector]
    protected float nodeDiameter;
    [HideInInspector]
    public int gridSizeX, gridSizeY;
    private GameObject tiles;
    public Tile[,] tileboard;
    public Unit[,] board;
    private GameObject[,] tilesObjs;
    public TextAsset mapConf = null;
    [HideInInspector]
    public PlayerController playerOne;
    [HideInInspector]
    public PlayerController playerTwo;
    public bool playerOneTurn = true;
    private PlayerController currentPlayer;
    private bool TheGameOver = false;
    public Text TheGameOverText;
    public Text TurnInfo;
    [HideInInspector]
    public int totalTurns = 1;
    public int MaxTurns = 1000;


    void Awake()
    {
        // Singleton pattern
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        Init();

    }

    void Init()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        tiles = GameObject.Find("Tiles");

        GenerateMap();
        getPlayers();
    }

    private void getPlayers()
    {
        // gets the first 2 that are active
        Component[] allplayers = GetComponents<PlayerController>();
        foreach (PlayerController p in allplayers)
        {
            if (p.enabled)
            {
                if (playerOne == null)
                {
                    playerOne = p;
                    p.whichplayer = 1;
                }
                else
                {
                    playerTwo = p;
                    p.whichplayer = 2;
                    break;
                }
            }
        }

        currentPlayer = playerOneTurn ? playerOne : playerTwo;
    }

    // Update is called once per frame
    void Update()
    {
        if (TheGameOver || totalTurns == MaxTurns)
        {
            TheGameOverText.enabled = true;

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else
        {
            if (playerOneTurn)
            {
                playerOne.TurnUpdate();
            }
            else
            {
                playerTwo.TurnUpdate();
            }
        }
    }

    public void NextTurn()
    {
        playerOneTurn = !playerOneTurn;
        currentPlayer = playerOneTurn ? playerOne : playerTwo;
        if (currentPlayer.PlayerUnitsDict.Count == 0)
        {
            playerOneTurn = !playerOneTurn;
            currentPlayer = playerOneTurn ? playerOne : playerTwo;
            TheGameOver = true;
        }
        totalTurns++;
        TurnInfo.text = "Turn: " + totalTurns;
    }

    private void GenerateMap()
    {
        tileboard = new Tile[gridSizeX, gridSizeY];
        tilesObjs = new GameObject[gridSizeX, gridSizeY];
        board = new Unit[gridSizeX, gridSizeY];
        string[] mapString = mapConf.text.TrimEnd('\n').Split('\n');
        string[] cost_line = null;
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridSizeX / 2 - Vector3.up * gridSizeY / 2;

        for (int j = 0; j < gridSizeY; j++)
        {
            cost_line = mapString[gridSizeY - 1 - j].Split(',');
            for (int i = 0; i < gridSizeX; i++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (i * nodeDiameter + nodeRadius) + Vector3.up * (j * nodeDiameter + nodeRadius);
                int value = int.Parse(cost_line[i]);
                Tile tile = ((GameObject)Instantiate(TilePrefab, worldPoint, Quaternion.identity)).GetComponent<Tile>();
                tile.gridPosition = new Vector2(i, j);
                tile.name = "T:(" + i + " , " + j + ")";
                tile.transform.SetParent(tiles.transform);
                tile.worldPosition = worldPoint;
                tile.HasWall = value < 0;
                tile.Walkable = true;
                if (tile.HasWall)
                {
                    tile.GetComponent<Renderer>().material.color = Color.black;
                }
                tileboard[i, j] = tile;
                board[i, j] = null;
            }
        }
    }

    public Tile TileFromBoardPoint(int x, int y)
    {
        return tileboard[x, y];
    }

    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public List<Tile> GetNeighbours(Tile node, int[,] neighbourhood)
    {
        List<Tile> neighbours = new List<Tile>();
        for (int i = 0; i < neighbourhood.GetLength(0); i++)
        {
            int checkX = (int)node.gridPosition.x + neighbourhood[i, 0];
            int checkY = (int)node.gridPosition.y + neighbourhood[i, 1];
            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                Tile neighbour = tileboard[checkX, checkY];
                if (neighbour.Walkable && !neighbour.HasWall)
                    neighbours.Add(neighbour);

            }
        }
        return neighbours;
    }
    
    public PlayerController GetAdversary(PlayerController player)
    {
        return player.id == playerOne.id ? playerTwo : playerOne;
    }


    public Vector2 BoardToWorldCoord(int grid_x, int grid_y)
    {
        Tile tmp = tileboard[grid_x, grid_y];
        float board_x = tmp.transform.position.x;
        float board_y = tmp.transform.position.y;
        return new Vector2(board_x, board_y);
    }



}
