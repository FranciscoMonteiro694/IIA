using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeepCopyExtensions;

public class MinMaxAlgorithm : MoveMaker
{
    public EvaluationFunction evaluator;
    private UtilityFunction utilityfunc;
    public int depth = 0;
    public int alpa1 = 0, alpa11=0;
    private PlayerController MaxPlayer;
    private PlayerController MinPlayer;

    public MinMaxAlgorithm(PlayerController MaxPlayer, EvaluationFunction eval, UtilityFunction utilf, PlayerController MinPlayer)
    {
        this.MaxPlayer = MaxPlayer;
        this.MinPlayer = MinPlayer;
        this.evaluator = eval;
        this.utilityfunc = utilf;
    }

    public override State MakeMove()
    {
        // The move is decided by the selected state
        return GenerateNewState();
    }

    private State GenerateNewState()
    {
        float alfa=-9999999, beta=+9999999;
        // Creates initial state
        alpa1 = 0;
        State newState = new State(this.MaxPlayer, this.MinPlayer);
        // Call the MinMax implementation
        State bestMove = MinMax(newState, alfa, beta); //Aqui comeca a chamar como se fosse o Max
        // returning the actual state. You should modify this
        return bestMove;
    }

    public State MinMax(State currentState, float alfa, float beta) //1 se Max e 2 se Min
    {

        this.MaxPlayer.ExpandedNodes = 0;
        State aux1 = null;
        float aux = -99999;
        float poi = 0;
        List<State> available_states = GeneratePossibleStates(currentState);
        for (int i = 0; i < available_states.Count; i++)
        {
            this.MaxPlayer.ExpandedNodes = 0;
            poi = SMin(new State(available_states[i]), alfa, beta);
            Debug.Log("Valor de poi: "+ poi);
            if (aux < poi)
            {
                aux1 = available_states[i];
                aux = poi;
            }
            

        }
        return aux1;
    }

    public float SMax(State state, float alfa, float beta)
    {

        if (isFinal(state))
        {
            return utilityfunc.evaluate(state);
        }
        if (this.MaxPlayer.ExpandedNodes >= this.MaxPlayer.MaximumNodesToExpand)
        {
            return evaluator.evaluate(state);
        }

        List<State> available_states = new List<State>();
        available_states = GeneratePossibleStates(state);
        int i;
        float best = SMin(new State(available_states[0]), alfa, beta);
        for (i = 1; i < available_states.Count; i++)
        {
            best = Math.Max(best, SMin(new State(available_states[i]), alfa, beta));
            if (best >= beta) {
                alpa1 += 1;
                return best;
            }
            alfa = Math.Max(alfa, best);

        }
        return best;
    }

    public float SMin(State state, float alfa, float beta)
    {

        if (isFinal(state))
        {
            return utilityfunc.evaluate(state);
        }
        if (this.MaxPlayer.ExpandedNodes >= this.MaxPlayer.MaximumNodesToExpand)
        {
            return evaluator.evaluate(state);
        }

        List<State> available_states = new List<State>();
        available_states = GeneratePossibleStates(state);
        int i;
        float best = SMax(new State(available_states[0]), alfa, beta);
        for (i = 1; i < available_states.Count; i++)
        {
            best = Math.Max(best, SMax(new State(available_states[i]), alfa, beta));
            if (best <= alfa)
            {
                alpa1 += 1;
                return best;
            }
            beta = Math.Max(beta, best);
        }
        return best;
    }
    private Boolean isFinal(State estado)
    {
        if (estado.PlayersUnits.Count == 0 || estado.AdversaryUnits.Count == 0)
        {
            return true;
        }
        else { return false; }
    }



    private List<State> GeneratePossibleStates(State state)
    {
        List<State> states = new List<State>();
        //Generate the possible states available to expand
        foreach (Unit currentUnit in state.PlayersUnits)
        {
            // Movement States
            List<Tile> neighbours = currentUnit.GetFreeNeighbours(state);
            foreach (Tile t in neighbours)
            {
                State newState = new State(state, currentUnit, true);
                newState = MoveUnit(newState, t);
                states.Add(newState);
            }
            // Attack states
            List<Unit> attackOptions = currentUnit.GetAttackable(state, state.AdversaryUnits);
            foreach (Unit t in attackOptions)
            {
                State newState = new State(state, currentUnit, false);
                newState = AttackUnit(newState, t);
                states.Add(newState);
            }

        }

        // YOU SHOULD NOT REMOVE THIS
        // Counts the number of expanded nodes;
        this.MaxPlayer.ExpandedNodes += states.Count;
        //

        return states;
    }

    private State MoveUnit(State state, Tile destination)
    {
        Unit currentUnit = state.unitToPermormAction;
        //First: Update Board
        state.board[(int)destination.gridPosition.x, (int)destination.gridPosition.y] = currentUnit;
        state.board[currentUnit.x, currentUnit.y] = null;
        //Second: Update Players Unit Position
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

        state.board[attacked.x, attacked.y] = attacked;
        int index = state.AdversaryUnits.IndexOf(attacked);
        state.AdversaryUnits[index] = attacked;



        if (attacked.hp <= 0)
        {
            //Board update by killing the unit!
            state.board[attacked.x, attacked.y] = null;
            index = state.AdversaryUnits.IndexOf(attacked);
            state.AdversaryUnits.RemoveAt(index);

        }
        state.isMove = false;
        state.isAttack = true;

        return state;

    }
}
