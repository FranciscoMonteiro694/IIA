using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeepCopyExtensions;

public class RandomSolution : MoveMaker
{
    private PlayerController MaxPlayer;
    private PlayerController MinPlayer;
    private System.Random random;

    public RandomSolution(PlayerController MaxPlayer, PlayerController MinPlayer)
    {
        this.MaxPlayer = MaxPlayer;
        this.MinPlayer = MinPlayer;
        random = new System.Random();
    }

    public override State MakeMove()
    {
        return GenerateNewState(); 
    }

    private State GenerateNewState()
    {

        State newState = new State(this.MaxPlayer, this.MinPlayer);
        List<State> states = GeneratePossibleStates(newState);
        states.Sort();
        return states[this.random.Next(0, states.Count) ] ;
    }

    private List<State> GeneratePossibleStates(State state)
    {
        List<State> states = new List<State>();
        foreach(Unit currentUnit in state.PlayersUnits)
        {

            List<Tile> neighbours = currentUnit.GetFreeNeighbours(state);
            foreach (Tile t in neighbours)
            {
                State newState = new State(state, currentUnit, true); 
                newState = MoveUnit(newState, t);
                states.Add(newState);
            }
            List<Unit> attackOptions = currentUnit.GetAttackable(state, state.AdversaryUnits);
            foreach (Unit t in attackOptions)
            {
                State newState = new State(state, currentUnit, false);
                newState = AttackUnit(newState, t);
                states.Add(newState);
            }

        }
        return states;
    }

    private State MoveUnit(State state,  Tile destination)
    {
        Unit currentUnit = state.unitToPermormAction;
        state.Score = 0;
        //Update Board
        state.board[(int)destination.gridPosition.x, (int)destination.gridPosition.y] = currentUnit;
        state.board[(int)currentUnit.x, (int)currentUnit.y] = null;
        //Update Players Unit Position
        currentUnit.x = (int)destination.gridPosition.x;
        currentUnit.y = (int)destination.gridPosition.y;
        state.isMove = true;
        state.isAttack = false;
        return state;
    }

    private State AttackUnit(State state, Unit toAttack)
    {
        Unit currentUnit = state.unitToPermormAction;
        Unit attacked = toAttack.DeepCopyByExpressionTree();

        Tuple<float, float> currentUnitBonus = currentUnit.GetBonus(state.board, state.PlayersUnits);
        Tuple<float, float> attackedUnitBonus = attacked.GetBonus(state.board, state.AdversaryUnits);
        attacked.hp += Math.Min(0, (attackedUnitBonus.Item1)) - (currentUnitBonus.Item2 + currentUnit.attack);
        state.unitAttacked = attacked;
        if (attacked.hp <= 0)
        {
            state.board[attacked.x, attacked.y] = null;
            int index = state.AdversaryUnits.IndexOf(attacked);
            state.AdversaryUnits.RemoveAt(index);

        }
        state.isMove = false;
        state.isAttack = true;
        state.Score = 10000;

        return state;

    }
}
