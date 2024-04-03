using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] public GameObject _highlight;

    public int x_position;
    public int y_position;
    public BaseUnit OccupiedUnit;
    public bool free => OccupiedUnit == null;

    public bool main_selection = false;

    public void Init(bool isOffset) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter() {
        //main_selection = true;
    }

    public void Select(){
        main_selection = true;
    }

    void OnMouseDown() {
        // if(BattleManager.Instance.Battlestate != BattleState.HeroesTurn) return;
    }

    void OnMouseExit() {
        main_selection = false;
    }

    public void SetUnit(BaseUnit unit){
            if(unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
            unit.transform.position = transform.position;
            OccupiedUnit = unit;
            unit.OccupiedTile = this;
    }

    public Tile GetNextTile(KeyboardDirections direction){
        Tile next_tile = null;
        switch (direction){
            case(KeyboardDirections.UP):
                break;
            case(KeyboardDirections.DOWN):
                break;
            case(KeyboardDirections.LEFT):
                break;
            case(KeyboardDirections.RIGHT):
                next_tile = GridManager.Instance.GetTileAtPosition(new Vector2(x_position + 1, y_position));
                break;
        }
        return next_tile;
    }
}