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

    public void SpawnEnemy() {
        var enemyCount = 1;

        for (int i = 0; i < enemyCount; i++){
            var randomPrefab = GetRandomUnit<BaseEnemy>(Team.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            
            randomSpawnTile.SetUnit(spawnedEnemy);
        }

    }

    private T GetRandomUnit<T>(Team team) where T : BaseUnit {
        return (T)_units.Where(u => u.Team == team).OrderBy(o=> Random.value).First().UnitPrefab;
    }
}
