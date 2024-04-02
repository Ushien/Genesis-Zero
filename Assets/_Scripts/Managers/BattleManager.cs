using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleManager : MonoBehaviour
{
    
    public static BattleManager Instance;

    //Game states
    public enum BattleState {START, TURN, END, WON, LOST}
    public enum TurnState {PLAYERTURN, ENEMYTURN}
    public enum PlayerTurnState {START, ACTION_CHOICE, APPLY_ACTIONS, END}
    public enum EnemyTurnState {START, ACTION_CHOICE, APPLY_ACTIONS, END}
    public enum PlayerActionChoiceState {CHARACTER_SELECTION, SWITCH_CHARACTER, SPELL_SELECTION, ENEMY_SELECTION, VALIDATED_ACTION, OTHER_STATE}
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

    public void LaunchBattle(List<Tuple<Vector2, ScriptableUnit, int>> composition){
        
        GridManager.Instance.GenerateGrids();
        UnitManager.Instance.SpawnEnemies(composition);

        StartBattle();
        ChangeState(Trigger.LEFT);
    }

    public void ChangeState(Trigger trigger){
        switch(playerActionChoiceState){
            case PlayerActionChoiceState.CHARACTER_SELECTION:
                Debug.Log("Bonjour");
                break;
        }
    }

    private void StartBattle(){
        battleState = BattleState.START;

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
