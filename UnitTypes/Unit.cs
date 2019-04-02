using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class Unit 
{
    public float hp;
    public int attack;
    public int defense;
    public string id;
    internal int x;
    internal int y;

    public int pontos;

    public int[,] neighbourhood;
    public int[,] attackrange;
    public float attackbonus;
    public float hpbonus;
    public int maxHp { get; set; }


    public List<Unit> GetAttackable(State state, List<Unit> adversary)
    {
        int gridSizeX = state.board.GetLength(0);
        int gridSizeY = state.board.GetLength(1);
        List<Unit> attackable = new List<Unit>();
        for (int i = 0; i < attackrange.GetLength(0); i++)
        {
            int checkX = this.x + attackrange[i, 0];
            int checkY = this.y + attackrange[i, 1];
            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                Unit neighbour = state.board[checkX, checkY];
                if (neighbour != null && adversary.Contains(neighbour) && !neighbour.IsDead())
                {
                    attackable.Add(neighbour);
                    }
            }
        }
        return attackable;
    }

    public List<Tile> GetFreeNeighbours()
    {
        Tile tile = GetAssociatedTile();
        return GameManager.instance.GetNeighbours(tile, neighbourhood);
    }
    public List<Tile> GetFreeNeighbours(State state)
    {
        int gridSizeX = state.board.GetLength(0);
        int gridSizeY = state.board.GetLength(1);
        List<Tile> neighbours = new List<Tile>();
        for (int i = 0; i < neighbourhood.GetLength(0); i++)
        {
            int checkX = this.x + neighbourhood[i, 0];
            int checkY = this.y + neighbourhood[i, 1];
            if ( checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                Unit neighbour = state.board[checkX, checkY];
                if (neighbour == null && !GameManager.instance.TileFromBoardPoint(checkX, checkY).HasWall)
                    neighbours.Add(GameManager.instance.TileFromBoardPoint(checkX, checkY));

            }
        }
        return neighbours;
    }

    public Tuple<float, float> GetBonus(Unit[,] board, List<Unit> PlayersUnits)
    {
        int[,] bonusneighbourhood = {
            {0, 1}, {1, 0}, {0, -1}, {-1, 0}, //{0,0},
            {1,1},  {-1,1}, {1,-1}, {-1,-1},
        };
        int gridSizeX = board.GetLength(0);
        int gridSizeY = board.GetLength(1);
        float currentHpBonus = 0.0f;
        float currentAttackBonus = 0.0f;
        for (int i = 0; i < bonusneighbourhood.GetLength(0); i++)
        {
            int checkX = this.x + bonusneighbourhood[i, 0];
            int checkY = this.y + bonusneighbourhood[i, 1];
            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                Unit neighbour = board[checkX, checkY];

                if (neighbour != null && !neighbour.IsDead() && PlayersUnits.Contains(neighbour))
                {
                    Protector prot = neighbour as Protector;
                    if(prot != null)
                    {
                        currentHpBonus += prot.hpbonus * this.hp;
                    }
                    Warrior warr = neighbour as Warrior;
                    if (warr != null)
                    {
                        currentAttackBonus += warr.attackbonus * this.attack;
                    }
                }
            }
        }
        return new Tuple<float, float>(currentHpBonus, currentAttackBonus);
    }

    public Tile GetAssociatedTile()
    {
        return GameManager.instance.TileFromBoardPoint(x, y);
    }


    public List<Unit> GetAttackable()
    {
        PlayerController adversary = GameManager.instance.playerOneTurn? GameManager.instance.playerTwo : GameManager.instance.playerOne;
        int gridSizeX = GameManager.instance.board.GetLength(0);
        int gridSizeY = GameManager.instance.board.GetLength(1);
        List<Unit> attackable = new List<Unit>();
        for (int i = 0; i < attackrange.GetLength(0); i++)
        {
            int checkX = this.x + attackrange[i, 0];
            int checkY = this.y + attackrange[i, 1];
            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
            {
                Unit neighbour = GameManager.instance.board[checkX, checkY];

                if (neighbour != null && adversary.PlayersUnits.Contains(neighbour) && !neighbour.IsDead())
                {
                    attackable.Add(neighbour);
                }
            }
        }
        return attackable;
    }

    public override string ToString()
    {
        return id + ", hp:" + hp + " x:"+x + " y:" + y + " atack: "+this.attack;
    }

    public override bool Equals(object obj)
    {
        Unit other = (Unit)obj;
        return this.id == other.id;
    }

    public bool IsDead()
    {
        return this.hp <= 0;
    }
}

