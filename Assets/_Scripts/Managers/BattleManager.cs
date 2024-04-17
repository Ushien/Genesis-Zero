using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.Tilemaps;

public class BattleManager : MonoBehaviour
{
    
    public static BattleManager Instance;

    //Game states
    public enum BattleState {OUT, START, TURN, END, WON, LOST}
    public enum TurnState {OUT, PLAYERTURN, ENEMYTURN}
    public enum PlayerTurnState {OUT, START, ACTION_CHOICE, APPLY_ACTIONS, END}
    public enum EnemyTurnState {OUT, START, ACTION_CHOICE, APPLY_ACTIONS, END}
    public enum PlayerActionChoiceState {OUT, CHARACTER_SELECTION, SWITCH_CHARACTER, SPELL_SELECTION, TARGET_SELECTION, VALIDATED_ACTION, OTHER_STATE}

    public enum Machine{BATTLESTATE, TURNSTATE, PLAYERTURNSTATE, ENEMYTURNSTATE, PLAYERACTIONCHOICESTATE}
    public enum Trigger {VALIDATE, CANCEL, LEFT, RIGHT, UP, DOWN, FORWARD}

    public BattleState battleState;
    public TurnState turnState;
    public PlayerTurnState playerTurnState;
    public EnemyTurnState enemyTurnState;
    public PlayerActionChoiceState playerActionChoiceState;

    public int nTurn = 0;

    void Awake(){
        Instance = this;
    }

    public void LaunchBattle(List<Tuple<Vector2, ScriptableUnit, int>> ally_composition, List<Tuple<Vector2, ScriptableUnit, int>> enemy_composition){
        
        GridManager.Instance.GenerateGrids();
        UnitManager.Instance.spawnAllies(ally_composition);
        UnitManager.Instance.SpawnEnemies(enemy_composition);

        StartBattle();
    }

    public void ChangeState(Machine machine, Trigger trigger){

        switch (machine)
        {
            case Machine.PLAYERACTIONCHOICESTATE:
                ChangePlayerActionChoiceState(trigger);
                break;
            default:
                break;
        }

        
    }

    private void ChangePlayerActionChoiceState(Trigger trigger){
        switch(playerActionChoiceState){

            case PlayerActionChoiceState.CHARACTER_SELECTION:

                switch (trigger){
                    case Trigger.VALIDATE:
                        playerActionChoiceState = PlayerActionChoiceState.SPELL_SELECTION;
                        break;
                    case Trigger.LEFT:
                        Debug.Log("Je bouge à gauche dans la sélection");
                        break;
                    case (Trigger.UP):
                        Debug.Log("Je bouge en haut dans la sélection");
                        break;
                    case (Trigger.DOWN):
                        Debug.Log("Je bouge en bas dans la sélection");
                        break;
                    case (Trigger.RIGHT):
                        Debug.Log("Je bouge à droite dans la sélection");
                        break;
                    default:
                        break;
                }
                break;
            
            default:
                break;

        }
    }

    public void DebugSetState(){
        battleState = BattleState.TURN;
        turnState = TurnState.PLAYERTURN;
        playerTurnState = PlayerTurnState.ACTION_CHOICE;
        playerActionChoiceState = PlayerActionChoiceState.CHARACTER_SELECTION;
    }

    public string GetCurrentStatesSummary(){
        string currentStates = "";
        currentStates = currentStates + "BattleState: " + battleState + "\n";
        currentStates = currentStates + "TurnState: " + turnState + "\n";
        currentStates = currentStates + "PlayerTurnState: " + playerTurnState + "\n";
        currentStates = currentStates + "EnemyTurnState: " + enemyTurnState + "\n";
        currentStates = currentStates + "PlayerActionChoiceState: " + playerActionChoiceState + "\n";
        return currentStates;

    }
    public BattleState GetBattleState(){
        return battleState;
    }

    public TurnState GetTurnState(){
        return turnState;
    }

    public PlayerTurnState GetPlayerTurnState(){
        return playerTurnState;
    }

    public EnemyTurnState GetEnemyTurnState(){
        return enemyTurnState;
    }

    public PlayerActionChoiceState GetPlayerActionChoiceState(){
        return playerActionChoiceState;
    }
    private void StartBattle(){
        battleState = BattleState.START;

        //Setup stuff

        //Start the first turn
        NextTurn();

    }

    private void StartTurn(){
        turnState = TurnState.PLAYERTURN;

        // Do things

        
    }

    [ContextMenu("Tour suivant")]
    public void NextTurn(){
        nTurn ++;
        StartTurn();
    }
}
