using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public MoveMaker moveMaker;

    public enum TypeUnit 
    {
        Protector,
        Mage,
        Assassin,
        Warrior
    };

    public List<TypeUnit> units;
    public List<Vector2> startPositions;
    [HideInInspector]
    public Vector2 moveDestination;
    [HideInInspector]
    public Vector2 moveStart;
    [HideInInspector]
    public Vector3 theMovement;
    [HideInInspector]
    public Vector3 moveAmount;
    public float moveSpeed = 1.0f;

    [HideInInspector]
    public bool interactive = false;
    public Color agentColor = Color.white;
    [HideInInspector]
    public Text UitextInfo;
    [HideInInspector]
    public string baseinfo;
    private bool animated;
    public int id = 0;
    // para animations...
    [HideInInspector]
    public bool IsIdle;
    [HideInInspector]
    public bool Moving;
    [HideInInspector]
    public bool IsAtacking;
    [HideInInspector]
    public bool UseAnimations = false;
    [HideInInspector]
    public bool OnMovement = false;

    public GameObject canvasRef { get; private set; }
    public int ExpandedNodes;

    [HideInInspector]
    public List<GameObject> unitsObjects;
    [HideInInspector]
    public List<Unit> PlayersUnits;
    [HideInInspector]
    public Dictionary<string, GameObject> PlayerUnitsDict;
    [HideInInspector]
    public Dictionary<string, Unit> PlayerUnitsInfoDict;
    [HideInInspector]
    public Dictionary<string, GameObject> PlayersUnitsBarsDict;


    [HideInInspector]
    public Tile source;
    [HideInInspector]
    public Tile destination;
    [HideInInspector]
    public bool doattack;
    [HideInInspector]
    public int whichplayer;
    [HideInInspector]
    public bool updateboard;

    public int MaximumNodesToExpand = 100000;

    // Use this for initialization
    public virtual void Start () {

        canvasRef = GameObject.Find("Canvas");
        unitsObjects = new List<GameObject>();
       
        PlayersUnits = new List<Unit>();

        PlayerUnitsDict = new Dictionary<string, GameObject>();
        PlayerUnitsInfoDict = new Dictionary<string, Unit>();
        PlayersUnitsBarsDict = new Dictionary<string, GameObject>();

        int unitI = 0;
        foreach (TypeUnit u in units)
        {
            GameObject instance;
            instance = creatUnit2d(u,unitI);
            Unit uni = null;
            switch (u)
            {
                case TypeUnit.Assassin:
                    uni = new Assassin();
                    break;
                case TypeUnit.Mage:
                    uni = new Mage();
                    break;
                case TypeUnit.Protector:
                    uni = new Protector();
                    break;
                case TypeUnit.Warrior:
                    uni = new Warrior();
                    break;
                default:
                    print("BUG creating unit");
                    break;
            }

            PlayersUnits.Add(uni);
            uni.id = instance.name;
            uni.x = (int)startPositions[unitI].x;
            uni.y = (int)startPositions[unitI].y;
            PlayerUnitsDict[uni.id] = instance;
            PlayerUnitsInfoDict[uni.id] = uni;
            GameObject barinstance = Instantiate(Resources.Load("Prefabs/HPBar"),canvasRef.transform) as GameObject;
            barinstance.GetComponent<UnitBarController>().theunit = uni;
            barinstance.GetComponent<UnitBarController>().PlayersUnits = PlayersUnits;
            if (this.whichplayer == 1) { 
                barinstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(200, -65 - (100 * unitI));
            }
            else
            {
                barinstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(1000, -65 - (100 * unitI));
            }
            barinstance.GetComponent<Image>().color = this.agentColor;
            PlayersUnitsBarsDict[uni.id] = barinstance;
            GameManager.instance.board[(int)startPositions[unitI].x, (int)startPositions[unitI].y] = uni;
            GameManager.instance.tileboard[(int)startPositions[unitI].x,
             (int)startPositions[unitI].y].inTile = uni;
            GameManager.instance.tileboard[(int)startPositions[unitI].x,
            (int)startPositions[unitI].y].Walkable = false;

            unitI++;
        }

    }



    private GameObject creatUnit2d(TypeUnit u, int unitI)
    {
        GameObject instance = Instantiate(Resources.Load("Prefabs/Unit")) as GameObject;
        //
        switch (u)
        {
            case TypeUnit.Assassin:
                instance.GetComponent<UnitController>().skin = "assassin";
                break;
            case TypeUnit.Mage:
                instance.GetComponent<UnitController>().skin = "mage";
                break;
            case TypeUnit.Protector:
                instance.GetComponent<UnitController>().skin = "protector";
                break;
            case TypeUnit.Warrior:
                instance.GetComponent<UnitController>().skin = "warrior";
                break;
            default:
                print("BUG creating unit");
                break;
        }
        instance.GetComponent<UnitController>().enabled = true;



        instance.transform.position = GameManager.instance.tileboard[(int)startPositions[unitI].x, (int)startPositions[unitI].y].transform.position+ new Vector3(0,0,-2);
        instance.gameObject.GetComponent<Renderer>().material.color = agentColor;
        instance.name = id + "_" + unitI + "_" + u.ToString();
        unitsObjects.Add(instance);


        return instance;
    }
	
	public virtual void TurnUpdate () {
		
	}

    public bool IsMyUnit(Unit unit)
    {
        return this.PlayerUnitsDict.ContainsKey(unit.id);
    }

   

    public void MarkNeighbours(Tile tile, bool mark) 
    {
        List<Tile> neighbours = tile.GetNeighbours();
        if(neighbours != null) 
        {
            foreach(Tile t in neighbours)
            {
                if(mark)
                    t.transform.GetComponent<Renderer>().material.color = Color.cyan;
                else
                    t.transform.GetComponent<Renderer>().material.color = Color.white;
            }
        }

    }

    public void Move(Tile source, Tile destination)
    {
        GameManager.instance.tileboard[(int)destination.gridPosition.x, (int)destination.gridPosition.y].inTile = source.inTile;
        UpdatePlayerUI(source.inTile,"Move [" + source.inTile.id+"] from " + source.gridPosition.ToString() + " to " + source.gridPosition.ToString() + "");

        MarkNeighbours(source, false);

        float board_x = GameManager.instance.tileboard[(int)destination.gridPosition.x, (int)destination.gridPosition.y].transform.position.x;
        float board_y = GameManager.instance.tileboard[(int)destination.gridPosition.x, (int)destination.gridPosition.y].transform.position.y;

        //UpdateWorld
        PlayerUnitsDict[source.inTile.id].transform.position = new Vector3(board_x, board_y, PlayerUnitsDict[source.inTile.id].transform.position.z); // manter yys.. se nao ficam mal nos tiles
        GameManager.instance.tileboard[(int)source.gridPosition.x, (int)source.gridPosition.y].inTile = null;
        destination.transform.GetComponent<Renderer>().material.color = Color.white;

        //Update board
        GameManager.instance.board[(int)destination.gridPosition.x, (int)destination.gridPosition.y] = destination.inTile;
        GameManager.instance.board[(int)source.gridPosition.x, (int)source.gridPosition.y] = null;

        //Update unit and tile position
        destination.inTile.x = (int)destination.gridPosition.x;
        destination.inTile.y = (int)destination.gridPosition.y;
        destination.Walkable = false;

        //Clear last tile position
        source.transform.GetComponent<Renderer>().material.color = Color.white;
        source.inTile = null;
        source.Walkable = true;
    }

    public void UpdatePlayerUI(Unit toupdt, string text)
    { 
        PlayersUnitsBarsDict[toupdt.id].GetComponent<UnitBarController>().movetext.text = GameManager.instance.totalTurns +" - "+ text;
        print(PlayersUnitsBarsDict[toupdt.id].GetComponent<UnitBarController>().movetext.text);
    }

    public void Attack(Tile source, Tile toAttack)
    {
        PlayerController advers = GameManager.instance.playerOneTurn ? GameManager.instance.playerTwo : GameManager.instance.playerOne;
        Unit currentUnit = source.inTile;
        Unit attacked = toAttack.inTile;
        

        //Calculate bonuses
        Tuple<float, float> currentUnitBonus = currentUnit.GetBonus(GameManager.instance.board, this.PlayersUnits);
        Tuple<float, float> attackedUnitBonus = attacked.GetBonus(GameManager.instance.board, advers.PlayersUnits);
        //Update UI with attack information
        UpdatePlayerUI(currentUnit,"Attack from [" + source.inTile.id + "] to " + toAttack.inTile.id + " for " + (currentUnitBonus.Item2 + currentUnit.attack));

        attacked.hp += Math.Min(0, (attackedUnitBonus.Item1)) - (currentUnitBonus.Item2 + currentUnit.attack);
        // Check if Unit is still alive
        if (attacked.hp <= 0)
        {
            //Free resources related to the killed united
            Destroy(advers.PlayerUnitsDict[toAttack.inTile.id]);
            Destroy(advers.PlayersUnitsBarsDict[toAttack.inTile.id]);
            advers.PlayersUnitsBarsDict.Remove(toAttack.inTile.id);
            advers.PlayerUnitsDict.Remove(toAttack.inTile.id);
            advers.PlayersUnits.Remove(toAttack.inTile);
            advers.PlayerUnitsInfoDict.Remove(toAttack.inTile.id);
            //Free its board position
            toAttack.inTile = null;
            toAttack.Walkable = true;
        }

    }

    public void MoveAnimation(){
        if (!OnMovement)
        {
            //Setup the animation
            moveDestination = GameManager.instance.BoardToWorldCoord((int)destination.gridPosition.x, (int)destination.gridPosition.y);
            moveStart = GameManager.instance.BoardToWorldCoord((int)source.gridPosition.x, (int)source.gridPosition.y);
            //Animation controller settings
            if(moveDestination.y - moveStart.y == 0f){ 
                PlayerUnitsDict[source.inTile.id].GetComponent<Animator>().SetFloat("Horizontal", moveDestination.x - moveStart.x);
            }
            PlayerUnitsDict[source.inTile.id].GetComponent<Animator>().SetFloat("Vertical", moveDestination.y - moveStart.y);
            theMovement = new Vector3(moveDestination.x, moveDestination.y, 0);
            //Mark the target destination with another color for reference
            Tile target = GameManager.instance.tileboard[(int)destination.gridPosition.x, (int)destination.gridPosition.y];
            target.GetComponent<Renderer>().material.color = Color.green;
            moveAmount = (theMovement - PlayerUnitsDict[source.inTile.id].transform.position).normalized * moveSpeed * Time.deltaTime;
            moveAmount.x = Math.Min(1.0f,moveAmount.x);
            moveAmount.y = Math.Min(1.0f,moveAmount.y);
            OnMovement = true;
        }
        else
        {
            if (Vector2.Distance(PlayerUnitsDict[source.inTile.id].transform.position, theMovement) > 0.1f)
            {
                PlayerUnitsDict[source.inTile.id].transform.position += new Vector3(moveAmount.x,moveAmount.y, 0);
                //Snape 
                if (Vector2.Distance(PlayerUnitsDict[source.inTile.id].transform.position, moveDestination) <= 0.1f)
                {
                    OnMovement = false;
                    PlayerUnitsDict[source.inTile.id].GetComponent<Animator>().SetFloat("Horizontal", 0);
                    PlayerUnitsDict[source.inTile.id].GetComponent<Animator>().SetFloat("Vertical", 0);
                    // make the move on the board
                    Move(source, destination); 
                    destination = null;
                    source = null;
                    updateboard = false;
                    GameManager.instance.NextTurn();
                }
            }
        }
    }

    public void AttackAnimation()
    {
        //Nothing for now
        updateboard = false;
        GameManager.instance.NextTurn();

    }
}

