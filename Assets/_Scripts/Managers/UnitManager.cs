using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ScriptableUnit> _units;
    public BaseUnit EmptyUnit;

    void Awake(){
        Instance = this;

        _units = Resources.LoadAll<ScriptableUnit>("Units/Enemies").ToList();
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

    public void SpawnEnemies(ScriptableTeam units_to_spawn){
        SpawnEnemy(1,1);
        SpawnEnemy(1, 3);
    }

    public void spawnAllies(){

    }

    private ScriptableUnit GetRandomUnit(){
        return _units.OrderBy(o=> Random.value).First();
    }
}
