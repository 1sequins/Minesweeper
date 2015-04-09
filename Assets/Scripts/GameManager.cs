﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
   
    //----------------------------------------
    // Variable Declarations
    
    // handles
    public GameObject GridPrefab;
    public UIManager UI;
    private GridScript _grid;

    // private variables
    private Transform _gridtf;

    //-----------------------------------------
    // Function Definitions

    // getters & setters
    public GameSettings Settings { get; set; }


    // unity functions
    void Awake()
    {
        Settings = new GameSettings();
        Settings = GameSettings.Intermediate;
    }

    void Start ()
	{
        StartNewGame();
    }

    // member functions
    public void StartNewGame()
    {
        // delete current grid in the scene & instantiate new grid
        Destroy(GameObject.Find("Grid(Clone)"));
        _gridtf = ((GameObject)Instantiate(GridPrefab, new Vector3(0, 0, 0), Quaternion.identity)).transform;
        _grid = _gridtf.GetComponent<GridScript>();
        if(_grid == null) Debug.Log("_grid IS NULL!!");

        // build new scene with new settings
        UI.ReadSettings();              // IMPORTANT! updates the Settings property
        _grid.GenerateMap(Settings);    // grid manager "_grid" generates the map with given settings
        PlayerInput.InitialClickIssued = false;

        // update handles
        GetComponent<PlayerInput>().Grid = _grid;

        // close menu
        UI.GetComponentInChildren<Canvas>().enabled = false;
        PlayerInput.IsGamePaused = false;
    }
}


// SERIALIZE FIELD ???? DOESNT WORK???
[System.Serializable]
public class TileOptions
{
    [SerializeField]
    public int a;
    [SerializeField]
    public float b = 0.5f;
    [SerializeField]
    public bool c = false;
};
// ????????????????????????????????????

[System.Serializable]
public class GameSettings
{
    // static constant settings
    public static readonly GameSettings Beginner = new GameSettings(9, 9, 10);
    public static readonly GameSettings Intermediate = new GameSettings(16, 16, 40);
    public static readonly GameSettings Expert = new GameSettings(30, 16, 99);
    
    // fields
    [SerializeField]
    private int _height;
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _mines;

    public int Height
    {
        get { return _height; }
    }

    public int Width
    {
        get { return _width; }
    }

    public int Mines
    {
        get { return _mines; }
    }


    public GameSettings(int w, int h, int m)
    {
        _width = w;
        _height = h;
        _mines = m;
    }

    public GameSettings()
    {
    }

    // member functions
    public void Set(int w, int h, int m)
    {
        _width = w;
        _height = h;
        _mines = m;
    }
}