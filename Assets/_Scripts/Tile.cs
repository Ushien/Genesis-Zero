using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Représente une case de la grille de combat
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] public GameObject _highlight;
    [SerializeField] private Animator tileAnimator;
    //TODO Marqueur pour montrer la sélection principale

    public int x_position;
    public int y_position;
    public BaseUnit OccupiedUnit;
    public bool free => OccupiedUnit == null;
    public Team team;

    public bool main_selection = false;

    public void Init(bool isOffset, Team teamToSet) {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
        team = teamToSet;
    }

    void OnMouseEnter() {
        //main_selection = true;
    }

    public void Select(){
        main_selection = true;
        tileAnimator.Play("selection");
    }

    public void Unselect(){
        main_selection = false;
        tileAnimator.Play("deselection");
    }

    void OnMouseDown() {
        // if(BattleManager.Instance.Battlestate != BattleState.HeroesTurn) return;
    }

    void OnMouseExit() {
        //main_selection = false;
    }

    public BaseUnit GetUnit(){
        return OccupiedUnit;
    }

    public void SetUnit(BaseUnit unit = null){
        if (unit != null){
            if(unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
            unit.transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

            // LifeBar Setup
            unit.lifeBar = InterfaceManager.Instance.SetupLifebar(unit.name, unit.transform.position, unit.totalHealth, unit.armor, unit.Team);
            
            OccupiedUnit = unit;
            unit.OccupiedTile = this;
        }
        else{
            OccupiedUnit = null;
        }

    }

    /// <summary>
    /// Récupère la case suivante, dans une direction donnée.
    /// Prend en compte les grilles alliées et ennemies.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Tile GetNextTile(Directions direction){
        Tile next_tile = null;
        switch (direction){
            case Directions.UP:
                next_tile = GridManager.Instance.GetTileAtPosition(team, new Vector2(x_position, y_position + 1));
                // Passage de la grille des alliés à la grille des ennemis
                if(next_tile == null && team == Team.Ally){
                    next_tile = GridManager.Instance.GetBorderTile(Team.Enemy, Directions.DOWN, x_position);
                }
                break;
            case Directions.DOWN:
                next_tile = GridManager.Instance.GetTileAtPosition(team, new Vector2(x_position, y_position - 1));
                // Passage de la grille des ennemis à la grille des alliés
                if(next_tile == null && team == Team.Enemy){
                    next_tile = GridManager.Instance.GetBorderTile(Team.Ally, Directions.UP, x_position);
                }
                break;
            case Directions.LEFT:
                next_tile = GridManager.Instance.GetTileAtPosition(team, new Vector2(x_position - 1, y_position));
                break;
            case Directions.RIGHT:
                next_tile = GridManager.Instance.GetTileAtPosition(team, new Vector2(x_position + 1, y_position));
                break;
        }
        return next_tile;
    }
}