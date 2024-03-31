using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleManager : MonoBehaviour
{
    
    public static BattleManager Instance;
    
    public enum BattleState {OUTSIDE, START, PLAYERTURN, ENEMYTURN, END, WON, LOST}
    public enum TurnState {OUTSIDE, START, MIDDLE, END}

    public int nTurn = 0;
    public BattleState battleState;
    public TurnState turnState;

    void Awake(){
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LaunchBattle(List<Tuple<Vector2, ScriptableUnit, int>> composition){
        
        GridManager.Instance.GenerateGrids();
        UnitManager.Instance.SpawnEnemies(composition);

        StartBattle();
    }

    private void StartBattle(){
        battleState = BattleState.START;

        //Start the first turn
        NextTurn();

    }

    private void StartTurn(){
        turnState = TurnState.START;
    }

    [ContextMenu("Tour suivant")]
    public void NextTurn(){
        nTurn ++;
        StartTurn();
    }
}
