using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    public BaseUnit EmptyUnit;

    void Awake(){
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units/Enemies").ToList();
    }

    public void SpawnUnit(Vector2 position, ScriptableUnit unit_to_spawn, int level){

        var new_unit = Instantiate(EmptyUnit);
        new_unit.Setup(unit_to_spawn, level);

        var SpawnTile = GridManager.Instance.GetTileAtPosition(position);
        SpawnTile.SetUnit(new_unit);
    }

    public void SpawnEnemy(int x_position, int y_position) {
        
        var enemyCount = 1;

        for (int i = 0; i < enemyCount; i++){
            var scriptableUnit = GetRandomUnit();
            var new_unit = Instantiate(EmptyUnit);
            new_unit.Setup(scriptableUnit, 1);

            var SpawnTile = GridManager.Instance.GetTileAtPosition(new Vector2(x_position, y_position));
            SpawnTile.SetUnit(new_unit);
        }

    }

    public void SpawnEnemies(List<Tuple<Vector2, ScriptableUnit, int>> units_to_spawn){
        foreach(Tuple<Vector2, ScriptableUnit, int> unit in units_to_spawn){
            SpawnUnit(unit.Item1, unit.Item2, unit.Item3);
        }
    }

    public void spawnAllies(){

    }

    private ScriptableUnit GetRandomUnit(){
        return _units.OrderBy(o=> UnityEngine.Random.value).First();
    }
}

public enum Team {
    Ally = 0,
    Enemy = 1
}

