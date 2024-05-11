using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    private List<BaseUnit> units;
    public BaseUnit EmptyUnit;
    public Passive EmptyPassive;

    GameObject all_units;
    GameObject all_allies;
    GameObject all_enemies;

    void Awake(){
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units/Enemies").ToList();
        units = new List<BaseUnit>();

        all_units = new GameObject("Units");
        all_allies = new GameObject("Allies");
        all_enemies = new GameObject("Enemies");

        all_allies.transform.parent = all_units.transform;
        all_enemies.transform.parent = all_units.transform;
    }

    public void SpawnUnit(Vector2 position, ScriptableUnit unit_to_spawn, int level, Team team){

        var new_unit = Instantiate(EmptyUnit);
        if(team == Team.Ally){
            new_unit.transform.parent = all_allies.transform;
        }
        else{
            new_unit.transform.parent = all_enemies.transform;
        }
        
        new_unit.Setup(unit_to_spawn, level, team);
        units.Add(new_unit);

        var SpawnTile = GridManager.Instance.GetTileAtPosition(team, position);
        SpawnTile.SetUnit(new_unit);
    }

    public void SpawnEnemies(List<Tuple<Vector2, ScriptableUnit, int>> units_to_spawn){
        foreach(Tuple<Vector2, ScriptableUnit, int> unit in units_to_spawn){
            SpawnUnit(unit.Item1, unit.Item2, unit.Item3, Team.Enemy);
        }
    }

    public void spawnAllies(List<Tuple<Vector2, ScriptableUnit, int>> units_to_spawn){
        foreach(Tuple<Vector2, ScriptableUnit, int> unit in units_to_spawn){
            SpawnUnit(unit.Item1, unit.Item2, unit.Item3, Team.Ally);
        }
    }

    public ScriptableUnit GetRandomScriptableUnit(){
        return _units.OrderBy(o=> UnityEngine.Random.value).First();
    }

    public BaseUnit GetRandomUnit(Team team = Team.Both){
        if(team == Team.Both){
            return units.OrderBy(o=> UnityEngine.Random.value).First();
        }
        else{
            return units.Where(unit => unit.Team == team).OrderBy(o=> UnityEngine.Random.value).First();
        }
    }

    public List<BaseUnit> GetUnits(Team team = Team.Both){
        if(team == Team.Both){
            return units.ToList();
        }
        else{
            return units.Where(unit => unit.Team == team).ToList();
        }
    }

    public List<BaseUnit> GetAdjacentUnits(BaseUnit unit){
        return GridManager.Instance.UnitsFromTiles(GridManager.Instance.GetAdjacentTiles(unit.GetTile()));
    }

    /// <summary>
    /// Return all the allies of a given Unit
    /// </summary>
    /// <param name="centralUnit"></param>
    /// <returns></returns>
    public List<BaseUnit> GetUnitsExcept(BaseUnit centralUnit){
        return GetUnits(centralUnit.GetTeam()).Where(unit => unit != centralUnit).ToList();
    }

    public int CountUnits(Team team){
        return GetUnits(team).Count;
    }

    public bool DidEveryCharacterGaveInstruction(){
        bool theyDid = true;

        foreach (BaseUnit unit in GetUnits(Team.Ally))
        {
            theyDid = theyDid && unit.HasGivenInstruction();
        }

        return theyDid;
    }

    public void MakeUnitsActive(){
        foreach (BaseUnit unit in GetUnits(Team.Ally))
        {
            unit.GiveInstruction(false);
        }
    }

    public void Kill(BaseUnit unit){
        units.Remove(unit);
        unit.OccupiedTile.SetUnit(null);
        unit.OccupiedTile = null; 
        unit.gameObject.SetActive(false);
    }

    public void ApplyEndTurnEffects(Team team){
        foreach (BaseUnit unit in GetUnits(team))
        {
            unit.ApplyEndturnEffects();
        }
    }
}

public enum Team {
    Ally = 0,
    Enemy = 1,
    Both = 2
}

