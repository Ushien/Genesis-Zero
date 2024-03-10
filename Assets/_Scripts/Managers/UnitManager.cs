using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<Unit> _units;

    void Awake(){
        Instance = this;

        _units = Resources.LoadAll<Unit>("Units").ToList();
    }

    public void SpawnEnemy(int x_position, int y_position) {
        var enemyCount = 1;

        for (int i = 0; i < enemyCount; i++){
            var randomPrefab = GetRandomUnit<BaseUnit>(Team.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            //var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            var SpawnTile = GridManager.Instance.GetTileAtPosition(new Vector2(x_position, y_position));
            
            SpawnTile.SetUnit(spawnedEnemy);
        }

    }

    private T GetRandomUnit<T>(Team team) where T : BaseUnit {
        return (T)_units.OrderBy(o=> Random.value).First().UnitPrefab;
    }
}
