using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;

    private Dictionary<Vector2, Tile> _tiles;
    private enum Selection_mode {
        Single_selection,
        Horizontal_selection,
        Vertical_selection,
        All,
        Special
    }
    private Selection_mode selection_mode = Selection_mode.Single_selection;
    private Tile main_selection;    

    void Awake(){
        Instance = this;
    }

    public void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y <_height; y++) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.x_position = x;
                spawnedTile.y_position = y;

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);


                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
        _cam.transform.position = new Vector3((float)_width/2 - 0.5f, (float)_height/2 - 0.5f, -10);
    }

    public Tile GetTileAtPosition(Vector2 pos){
        if (_tiles.TryGetValue(pos, out var tile)) {
            return tile;
        }

        return null;
    }

    private List<Tile> ReturnTilesList(int width = -1, int height = -1, bool occupiedByUnit = false){
        List<Tile> tiles_list = new List<Tile>();
        int compteur = 0;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                if((width == -1 || width == i)&&(height == -1 || height == j)){
                    if(occupiedByUnit){
                        //TODO
                    }
                    tiles_list.Add(GetTileAtPosition(new Vector2(i, j)));
                    compteur++;
                }
            }
        }
        return tiles_list;
    }

    void Update(){

        main_selection = null;

        foreach (Tile tile in ReturnTilesList()){
            bool selected = tile.main_selection;
            if(selected){
                main_selection = tile;
            }
            tile._highlight.SetActive(false);
        }

        if(main_selection != null){

            List<Tile> selected_tiles = new List<Tile>();

            switch(selection_mode){

                case Selection_mode.Single_selection:

                    selected_tiles = ReturnTilesList(width : main_selection.x_position, height : main_selection.y_position);
                    break;

                case Selection_mode.Horizontal_selection:

                    selected_tiles = ReturnTilesList(height : main_selection.y_position);
                    break;

                case Selection_mode.Vertical_selection:

                    selected_tiles = ReturnTilesList(width : main_selection.x_position);
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

            DumpToConsole(main_selection);
            
        }

        if (Input.GetMouseButtonDown(1)){
            
            CycleTroughSelectionModes();
            
        }

    }

    public Tile GetEnemySpawnTile() {
        Debug.Log(_tiles);
        return GetTileAtPosition(new Vector2(1, 1));
        //return _tiles.Where(t=>t.Key.x < _width/2 && t.Value.free).OrderBy(t => Random.value).First().Value;
    }

    public static void DumpToConsole(object obj)
    {
        var output = JsonUtility.ToJson(obj, true);
        Debug.Log(output);
    }

    public void CycleTroughSelectionModes()
    {
        /*
        bool next_mode = false;

        foreach (var mode in System.Enum.GetNames(typeof(Selection_mode)))
        {
            if(mode == selection_mode){
                next_mode = true;
            }
            if(next_mode){
                selection_mode = mode;
                next_mode = false;
            }
        }
        if (next_mode){
            selection_mode = Selection_mode[0];
        }
*/
        //Debug.Log(selection_mode);

        if (selection_mode == Selection_mode.Single_selection){
            selection_mode = Selection_mode.All;
        }
        else{
            selection_mode = Selection_mode.Single_selection;
        }
    }
}