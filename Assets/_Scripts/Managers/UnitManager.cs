using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

/// <summary>
/// Gestion des unités sur un plan global.
/// </summary>

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<BaseUnit> units;
    public BaseUnit EmptyUnit;
    public Passive EmptyPassive;

    GameObject all_units;
    GameObject all_allies;
    GameObject all_enemies;

    void Awake(){
        Instance = this;

        units = new List<BaseUnit>();

        all_units = new GameObject("Units");
        all_allies = new GameObject("Allies");
        all_enemies = new GameObject("Enemies");

        all_allies.transform.parent = all_units.transform;
        all_enemies.transform.parent = all_units.transform;
    }

    public void Reset(){
        units = new List<BaseUnit>();
    }

    /*
    public void SpawnUnit(Vector2 position, ScriptableUnit unit_to_spawn, int level, Team team){

        var new_unit = Instantiate(EmptyUnit);
        if(team == Team.Ally){
            new_unit.transform.parent = all_allies.transform;
        }
        else{
            new_unit.transform.parent = all_enemies.transform;
        }
        
        new_unit.Setup(unit_to_spawn, level, team, position);
        units.Add(new_unit);

        Tile spawnTile = GridManager.Instance.GetTileAtPosition(team, position);
        spawnTile.SetUnit(new_unit);
    }
    */

    public void SpawnUnit(BaseUnit unit_to_spawn, Team team){

        if(team == Team.Ally){
            unit_to_spawn.transform.parent = all_allies.transform;
            unit_to_spawn.SetTeam(Team.Ally);
        }
        else{
            unit_to_spawn.transform.parent = all_enemies.transform;
            unit_to_spawn.SetTeam(Team.Enemy);
        }
        
        units.Add(unit_to_spawn);

        Tile spawnTile = GridManager.Instance.GetTileAtPosition(team, unit_to_spawn.GetPosition());
        spawnTile.SetUnit(unit_to_spawn); 
    }

    /*
    public void SpawnEnemies(List<Tuple<Vector2, ScriptableUnit, int>> units_to_spawn){
        foreach(Tuple<Vector2, ScriptableUnit, int> unit in units_to_spawn){
            SpawnUnit(unit.Item1, unit.Item2, unit.Item3, Team.Enemy);
        }
    }

    public void SpawnAllies(List<Tuple<Vector2, ScriptableUnit, int>> units_to_spawn){
        foreach(Tuple<Vector2, ScriptableUnit, int> unit in units_to_spawn){
            SpawnUnit(unit.Item1, unit.Item2, unit.Item3, Team.Ally);
        }
    }
    */

    public void SpawnAllies(List<BaseUnit> units_to_spawn){
        foreach(BaseUnit unit in units_to_spawn){
            SpawnUnit(unit, Team.Ally);
        }  
    }

    public void SpawnEnemies(List<BaseUnit> units_to_spawn){
        foreach(BaseUnit unit in units_to_spawn){
            SpawnUnit(unit, Team.Enemy);
        }  
    }


    public BaseUnit CreateUnit(Vector2 position, ScriptableUnit unit_to_create, int level, Team team){
        var new_unit = Instantiate(EmptyUnit);
        new_unit.Setup(unit_to_create, level, team, position);
        return new_unit;
    }

    public List<BaseUnit> CreateUnits(List<Tuple<Vector2, ScriptableUnit, int>> units_to_create, Team team){
        List<BaseUnit> unitList = new List<BaseUnit>();
        foreach(Tuple<Vector2, ScriptableUnit, int> unit in units_to_create){
            unitList.Add(CreateUnit(unit.Item1, unit.Item2, unit.Item3, team));
        }
        return unitList;
    }

    public BaseUnit GetRandomUnit(Team team = Team.Both){
        if(team == Team.Both){
            return units.OrderBy(o=> UnityEngine.Random.value).First();
        }
        else{
            return units.Where(unit => unit.GetTeam() == team).OrderBy(o=> UnityEngine.Random.value).First();
        }
    }

    public List<BaseUnit> GetUnits(Team team = Team.Both){
        if(team == Team.Both){
            return units.ToList();
        }
        else{
            return units.Where(unit => unit.GetTeam() == team).ToList();
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
        //unit.gameObject.SetActive(false);
    }

    public void ApplyEndTurnEffects(){
        foreach (BaseUnit unit in GetUnits())
        {
            unit.ApplyEndturnEffects();
        }
    }

    /// <summary>
    /// Rend toutes les unités encore en vie prêtes pour leur combat suivant.
    /// </summary>
    public void EndBattle(){
        foreach (BaseUnit unit in GetUnits()){
            unit.EndBattle();
        }
        Reset();
    }

    public void StartBattle(){
        foreach (BaseUnit unit in GetUnits())
        {
            unit.StartBattle();
        }
    }

    public void RemoveUnits(Team team){
        if(team == Team.Ally || team == Team.Both){
            foreach (Transform unit in all_allies.transform)
            {
                Destroy(unit.gameObject);
            }
        }
        if(team == Team.Enemy || team == Team.Both){
            foreach (Transform unit in all_enemies.transform)
            {
                Destroy(unit.gameObject);
            }
        }
    }

    public void MakeUnitsVisible(Team team, bool visibility){
        if(team == Team.Ally || team == Team.Both){
            foreach (Transform unit in all_allies.transform)
            {
                unit.gameObject.SetActive(visibility);
            }
        }
        if(team == Team.Enemy || team == Team.Both){
            foreach (Transform unit in all_enemies.transform)
            {
                unit.gameObject.SetActive(visibility);
            }
        }
    }
}

public enum Team {
    Ally = 0,
    Enemy = 1,
    Both = 2
}

