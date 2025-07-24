using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Représente une case de la grille de combat
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private Animator tileAnimator;
    //TODO Marqueur pour montrer la sélection principale

    public int x_position;
    public int y_position;
    public BaseUnit OccupiedUnit;
    public bool free => OccupiedUnit == null;
    public Team team;
    public bool selected = false;

    public void Init(bool isOffset, Team teamToSet) {
        Color newColor = isOffset ? _offsetColor : _baseColor;
        GetComponent<Renderer>().material.SetColor("_BaseColor", newColor);
        team = teamToSet;
    }

    void OnMouseEnter() {
        //main_selection = true;
    }

    public void Select(){
        selected = true;
        tileAnimator.Play("selection");
    }

    public void Unselect(){
        selected = false;
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
            unit.transform.parent.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            InterfaceManager.Instance.UpdateLifeBarPosition(unit);
            
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

        Vector2 directionsCoordinates = Tools.ConvertDirectionsToVector2(direction);
        int next_x = x_position + (int)directionsCoordinates.x;
        int next_y = y_position + (int)directionsCoordinates.y;
        Tile next_tile = GridManager.Instance.GetTileAtPosition(team, new Vector2(next_x, next_y));
 
        if (next_tile == null && team == Team.Ally && next_y > y_position)
        {
            return GridManager.Instance.GetBorderTile(Team.Enemy, Directions.DOWN, next_x);
        }
        else if (next_tile == null && team == Team.Enemy && next_y < 0)
        {
            return GridManager.Instance.GetBorderTile(Team.Ally, Directions.UP, next_x);
        }
        else
        {
            return next_tile;
        }
    }

    public Vector2 GetPosition(){
        return new Vector2(x_position, y_position);
    }
}