using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class Tile : MonoBehaviour {

    public Vector2 gridPosition = Vector2.zero;
    private bool walkable;
    private bool hasWall;

    public bool HasWall
    {
        get
        {
            return hasWall;
        }
        set
        {
            this.hasWall = value;
        }
    }

    public bool Walkable
    {
        get
        {
            return walkable;
        }
        set
        {
            this.walkable = value;
        }
    }

    public Unit inTile { get; set; }
    public Vector3 worldPosition;
    private bool unitSelected = false;
    public Tile()
    {
    }

    void OnMouseEnter() {
        if(walkable && IsNeighbour() && !hasWall)
        {
            transform.GetComponent<Renderer>().material.color = Color.gray;
		}
    }
	
	void OnMouseExit() {
        if(!unitSelected && walkable && !hasWall)
        {
            transform.GetComponent<Renderer>().material.color = Color.white;
        }

	}
	
	
	void OnMouseDown() {
        PlayerController player = GameManager.instance.GetCurrentPlayer();
        if (player is HumanPlayer)
        {
            ((HumanPlayer) player).UpdatePlay(this);
        }
    }

    public void SetSelected(bool selected)
    {
        if (selected)
        {
            transform.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            transform.GetComponent<Renderer>().material.color = Color.white;
        }
        unitSelected = selected;

    }

    public bool hasUnit()
    {

        return inTile != null;
    }

    public bool hasUnitSelected()
    {
        return true;
    }

    public List<Tile> GetNeighbours()
    {
        if(inTile == null)
        {
            return null;
        }
        return inTile.GetFreeNeighbours();
    }

    public bool IsNeighbour()
    {
        return this.IsNeighbour(this);
    }

    public bool IsNeighbour(Tile tile)
    {
        PlayerController tmp = GameManager.instance.GetCurrentPlayer();
        if (tmp is HumanPlayer player)
        {
            Tile source = player.GetSource();
            if (source == null)
            {
                return false;
            }
            return source.inTile.GetFreeNeighbours().Contains(tile);
        }
        else
        {
            return false;
        }
    }






}
