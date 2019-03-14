using UnityEngine;
using System.Collections;
using System;
using DigitalRuby.Threading;

public class AIPlayer : PlayerController
{

    State currentmove;
    private bool computing;

    /////////////////////////////////
    ////// You should Implement these
    public enum TypeStrategy
    {
        MinMax,
        RandomStrategy,
    };

    public enum EvaluatationFunc
    {
        Eval
    };

    public enum UtilityFunc
    {
        Util
    };
    public TypeStrategy strategy;
    public EvaluatationFunc evalfunc;
    public UtilityFunc utilfunc;

    private void InitAI()
    {
        MoveMaker myStrategy = null;
        EvaluationFunction eval = null;
        UtilityFunction ufunc = null;

        ////////////////
        // your code here to initialize the MinMax algorithm
        // 1. Evaluation function
        // 2. Utility function
        // 3. Strategy (MinMax and MinMax Alpha Beta)
        ///////////////
        switch (evalfunc)
        {
            case EvaluatationFunc.Eval:
                eval = new EvaluationFunction();
                break;
            default:
                Debug.Log("Not an option");
                break;
        }

        switch (utilfunc)
        {
            case UtilityFunc.Util:
                ufunc = new UtilityFunction();
                break;
            default:
                Debug.Log("Not an option");
                break;
        }

        switch (strategy)
        {
            case TypeStrategy.RandomStrategy:
                myStrategy = new RandomSolution(this, GameManager.instance.GetAdversary(this));
                break;
            case TypeStrategy.MinMax:
                myStrategy = new MinMaxAlgorithm(this, eval, ufunc, GameManager.instance.GetAdversary(this));
                break;
            default:
                Debug.Log("Not an option");
                break;
        }

        moveMaker = myStrategy;
    }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        InitAI();
    }

    public void ComputeTheMove(object result)
    {
        currentmove = (State)result;
        computing = false;
        base.updateboard = true;
    }

    public override void TurnUpdate()
    {
        if (this.PlayersUnits.Count != 0 && !base.updateboard && !base.OnMovement && !computing)
        {
            computing = true;
            currentmove = null;
            EZThread.ExecuteInBackground(moveMaker.MakeMove, ComputeTheMove);
            Debug.Log("[AI] thinking.. computing:" + computing + " updateboard:" + base.updateboard);
        }

        if (!computing && base.updateboard)
        {
            Debug.Log("[AI] performing move! computing:" + computing + " updateboard:" + base.updateboard);
            UpdateBoard(currentmove);
        }
    }

    private void UpdateBoard(State state)
    {
        if (state.unitAttacked != null)
        {
            Attack(state.unitToPermormAction.GetAssociatedTile(), state.unitAttacked.GetAssociatedTile());
            base.AttackAnimation();
        }
        else
        {
            if (!base.OnMovement)
            {
                Unit unitToMove = state.unitToPermormAction;
                source = this.PlayerUnitsInfoDict[unitToMove.id].GetAssociatedTile();
                destination = unitToMove.GetAssociatedTile();
            }
            base.MoveAnimation();
        }
    }
}
