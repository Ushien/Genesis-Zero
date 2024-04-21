using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int _enemy_width, _enemy_height;
    [SerializeField] private int _ally_width, _ally_height;

    [SerializeField] private float gap_between_tiles = 0.05f;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _enemy_tiles;
    private Dictionary<Vector2, Tile> _ally_tiles;
    public enum Selection_mode {
        Single_selection,
        Horizontal_selection,
        Vertical_selection,
        All,
        Special
    }
    public enum Team_restriction{
        Choice,
        Allies_only,
        Enemies_only,
        Both
    }
    private Selection_mode selection_mode = Selection_mode.Single_selection;
    private Tile main_selection;    
    private List<Tile> selected_tiles;

    void Awake(){
        Instance = this;
    }

    public void GenerateGrids() {
        GameObject emptyGrid = new GameObject("Grid");
        GenerateAlliesGrid(emptyGrid);
        GenerateEnemiesGrid(emptyGrid);
    }

    public void GenerateAlliesGrid(GameObject parentGrid){
    _ally_tiles = new Dictionary<Vector2, Tile>();

        GameObject emptyGrid = new GameObject("Allies");
        emptyGrid.transform.parent = parentGrid.transform;
        GenerateGrid(Team.Ally, emptyGrid);
    }

    public void GenerateEnemiesGrid(GameObject parentGrid){
        _enemy_tiles = new Dictionary<Vector2, Tile>();

        GameObject emptyGrid = new GameObject("Enemies");
        emptyGrid.transform.parent = parentGrid.transform;
        GenerateGrid(Team.Enemy, emptyGrid);
    }

    public void GenerateGrid(Team team, GameObject parentGrid){
        int offset = 0;
        
        int width = _ally_width;
        int height = _ally_height;
        
        if(team == Team.Enemy){
            offset = 4;
            width = _enemy_width;
            height = _enemy_height;
        }
        
        for (int x = 0; x < width; x++) {
            for (int y = 0; y <height; y++) {

                var spawnedTile = Instantiate(_tilePrefab, new Vector3(gap_between_tiles*x + x + offset, gap_between_tiles*y + y), Quaternion.identity);

                spawnedTile.transform.parent = parentGrid.transform;
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.x_position = x;
                spawnedTile.y_position = y;

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset, team);

                if(team == Team.Enemy){
                    _enemy_tiles[new Vector2(x, y)] = spawnedTile;
                }
                else{
                    _ally_tiles[new Vector2(x, y)] = spawnedTile;
                }

            }
        }
        _cam.transform.position = new Vector3(3f, 1.5f, -10);

    }

    public Tile GetTileAtPosition(Team team, Vector2 pos){

        Dictionary<Vector2, Tile> tiles = _enemy_tiles;
        if(team == Team.Ally){
            tiles = _ally_tiles;
        }

        if (tiles.TryGetValue(pos, out var tile)){
            return tile;
        }

        return null;
    }

    public Tile GetBorderTile(Team team, Directions direction, int coordinate){

        int width = _enemy_width;
        int height = _enemy_height;
        if(team == Team.Ally){
            width = _ally_width;
            height = _ally_height;
        }

        switch (direction){
            case Directions.UP:
                return ReturnTilesList(team, coordinate, height-1)[coordinate];
            case Directions.DOWN:
                return ReturnTilesList(team, coordinate, 0)[coordinate];
            case Directions.RIGHT:
                return ReturnTilesList(team, width-1, coordinate)[0];
            case Directions.LEFT:
                return ReturnTilesList(team, 0, coordinate)[0];
            default:
                return null;
        }
    }

    private List<Tile> ReturnTilesList(Team team = Team.Both, int width = -1, int height = -1, bool occupiedByUnit = false){

        List<Tile> tiles_list = new List<Tile>();
        int compteur = 0;
        List<Team> teams = new List<Team>();

        if(team == Team.Ally){
            teams.Add(Team.Ally);
        }
        else if(team == Team.Enemy){
            teams.Add(Team.Enemy);
        }
        else if(team == Team.Both){
            teams.Add(Team.Ally);
            teams.Add(Team.Enemy);
        }

        foreach(Team individual_team in teams)
        {
            int grid_width;
            int grid_height;
            if(individual_team == Team.Ally)
            {
                grid_width = _ally_width;
                grid_height = _ally_height;
            }
            else
            {
                grid_width = _enemy_width;
                grid_height = _enemy_height;
            }

            for (int i = 0; i < grid_width; i++)
                {
                    for (int j = 0; j < grid_height; j++)
                    {
                        if((width == -1 || width == i)&&(height == -1 || height == j)){
                            if(occupiedByUnit){
                                //TODO
                            }
                            tiles_list.Add(GetTileAtPosition(individual_team, new Vector2(i, j)));
                            compteur++;
                        }
                    }
                }
        }
        
        return tiles_list;
    }

    private List<BaseUnit> UnitsFromTiles(List<Tile> tiles){
        List<BaseUnit> units_list = new List<BaseUnit>();
        foreach (Tile tile in tiles){
            if(tile.OccupiedUnit != null){
                units_list.Add(tile.OccupiedUnit);
            }
        }
        return units_list;
    }

    void Update(){

        foreach (Tile tile in ReturnTilesList()){
            if(tile.main_selection){
                SetMainSelection(tile);
            }
            tile._highlight.SetActive(false);
        }

        if(main_selection != null){

            selected_tiles = new List<Tile>();
            switch(selection_mode){

                case Selection_mode.Single_selection:

                    selected_tiles = ReturnTilesList(main_selection.team, width : main_selection.x_position, height : main_selection.y_position);
                    break;

                case Selection_mode.Horizontal_selection:

                    selected_tiles = ReturnTilesList(main_selection.team, height : main_selection.y_position);
                    break;

                case Selection_mode.Vertical_selection:

                    selected_tiles = ReturnTilesList(main_selection.team, width : main_selection.x_position);
                    break;

                case Selection_mode.All:

                    selected_tiles = ReturnTilesList();
                    break;

                case Selection_mode.Special:

                    //TODO
                    break;
                    
            }

            foreach (Tile tile in selected_tiles){
                tile._highlight.SetActive(true);
            }
        }

        if (Input.GetMouseButtonDown(0)){

            var units_list = UnitsFromTiles(selected_tiles);
            if(units_list != null){
                foreach( var x in units_list) {
                    DumpToConsole(x);
                }
            }
            
        }

        if (Input.GetMouseButtonDown(1)){
            
            CycleTroughSelectionModes();
            
        }

    }

    public Tile GetRandomTile(Team team) {
        return ReturnTilesList(team).OrderBy(t => Random.value).First();
    }

    private void SetMainSelection(Tile tile){
        main_selection = tile;
    }

    public Tile GetMainSelection(){
        return main_selection;
    }

    public static void DumpToConsole(object obj)
    {
        var output = JsonUtility.ToJson(obj, true);
        Debug.Log(output);
    }

    public void SetSelectionMode(Selection_mode mode){
        selection_mode = mode;
    }

    public void CycleTroughSelectionModes()
    {
        if (selection_mode == Selection_mode.Single_selection){
            selection_mode = Selection_mode.Vertical_selection;
        }
        else{
            selection_mode = Selection_mode.Single_selection;
        }
    }
}