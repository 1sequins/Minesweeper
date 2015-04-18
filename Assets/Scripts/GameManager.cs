﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
   
    //----------------------------------------
    // Variable Declarations

    // static variables
    public static bool IsGamePaused;
    public static bool IsGameOver;

    // handles
    public GameObject GridPrefab;
    public UIManager UI;
    private GridScript _grid;
    public ParticleSystem[] Explosions;

    // private variables
    private Transform _gridtf;
    private GameSettings _settings;

    // score variables
    private float _startTime;
    private float _endTime;
    private int _flagCount;

    private Score score;
    private List<List<Score>> _highScores; 


    //-----------------------------------------
    // Function Definitions

    // getters & setters
    public GameSettings Settings
    {
        get { return _settings; }
        set { _settings = value; }
    }

    // unity functions
    void Awake()
    {
        _settings = new GameSettings();
        _settings = GameSettings.Intermediate;

    }

    void Start ()
    {
        GetHighScores();
        StartNewGame(_settings);
    }

    private void Update()
    {
        UI.UpdateFlagText(_flagCount);
        if (PlayerInput.InitialClickIssued && !IsGamePaused && !IsGameOver)
        {
            UI.UpdateTimeText((int) (Time.time - _startTime));
        }
    }

    // member functions
    public void StartNewGame(GameSettings settings)
    {
        // delete current grid in the scene & instantiate new grid
        // using the settings that are read from UI Input fields
        Destroy(GameObject.Find("Grid(Clone)"));
        _gridtf = ((GameObject)Instantiate(GridPrefab, new Vector3(0, 0, 0), Quaternion.identity)).transform;
        _grid = _gridtf.GetComponent<GridScript>();
        if (_grid == null) Debug.Log("_grid IS NULL!!");

        _settings = settings;
        _grid.GenerateMap(_settings);    // grid manager "_grid" generates the map with given settings

        // update handles in companion scripts
        GetComponent<PlayerInput>().Grid = _grid;
        
        ResetGameState();
        UI.ResetHUD(_flagCount);
    }

    public void StartTimer()
    {
        _startTime = Time.time;
    }

    public void GameOver(bool win)
    {
        IsGameOver = true;
        UI.Elements.GameStateText.enabled = true;
        UI.Elements.GameStateText.text = "Game: " + (win ? " Won" : " Lost");
        _endTime = Time.time - _startTime;
        Debug.Log("GAME ENDED IN " + (_endTime - _startTime) + " SECONDS. GAME WON:" + win);
        
        // set time related data
        //Time.timeScale = 0f;
        IsGamePaused = true;
        if (win)
        {
            int timePassed = (int)(_endTime - _startTime);
            score = new Score(timePassed);

            // TODO: HIGHSCORES if score in top 10, ask user input, put on leaderboard
        }
        
    }

    public void UpdateFlagCounter(bool condition)
    {
        _flagCount += condition ? 1 : -1;
    }   // true: increment | false: decrement

    private void ResetGameState()
    {
        PlayerInput.InitialClickIssued = false;
        IsGamePaused = false;
        IsGameOver = false;
        _flagCount = _settings.Mines;
    }

    // high score functions
    void GetHighScores()
    {
        _highScores = new List<List<Score>>();
        // ReadDatabase()
        CreateDummyScores();
        
        LoadScoresToUI();
    }

    //TODO: read database function

    void CreateDummyScores()
    {
        int baseScore;

         // 3 databases: 1 for each difficulty
        _highScores.Add(new List<Score>()); // 0: Beginner
        _highScores.Add(new List<Score>()); // 1: Intermediate
        _highScores.Add(new List<Score>()); // 2: Expert
        
        for (int i = 0; i < 3; i++)
        {
            baseScore = (i + 1)*5;  // 5-6-7... beginner, 10-11-12... intermediate...
            for (int j = 0; j < 10; j++)
            {
                switch (i)
                {
                    case 0:
                        _highScores[i].Add(new Score(baseScore + j));
                        break;

                    case 1:
                        _highScores[i].Add(new Score(baseScore + j));
                        break;

                    case 2:
                        _highScores[i].Add(new Score(baseScore + j));
                        break;
                }
                
            }
        }
    }

    void LoadScoresToUI()
    {
        // get UI Text objects
        Text beginnerScores, intermediateScores, expertScores;

        beginnerScores = GameObject.Find("Beginner_Scores").GetComponent<Text>();
        intermediateScores = GameObject.Find("Intermediate_Scores").GetComponent<Text>();
        expertScores = GameObject.Find("Expert_Scores").GetComponent<Text>();

        if (beginnerScores == null || intermediateScores == null || expertScores == null)
        {
            Debug.Log("GAMEMANAGER:: LOADSCORESTOUI:: NULL HANDLES!");
            return;
        }

        // construct text to be displayed in UI elements
        String beginnerScoresText, intermediateScoresText, expertScoresText;
        beginnerScoresText = intermediateScoresText = expertScoresText = "";

        for (int j = 0; j < _highScores[0].Count; j++)
            beginnerScoresText += HighScoreFormat(j, _highScores[0][j]); 
        for (int j = 0; j < _highScores[1].Count; j++)
            intermediateScoresText += HighScoreFormat(j, _highScores[1][j]); 
        for (int j = 0; j < _highScores[2].Count; j++)
            expertScoresText += HighScoreFormat(j, _highScores[2][j]); 


        // update UI elements' text fields
        beginnerScores.text = beginnerScoresText;
        intermediateScores.text = intermediateScoresText;
        expertScores.text = expertScoresText;
    }

    string HighScoreFormat(int i, Score score)
    {
        return "\t" + (i + 1) + "\t\t" + score.Name + "\t\t" + score.TimePassed + "\n\n";
    }

    public void Detonate(Tile tile)
    {
        int index = Random.Range(0, Explosions.Length);
        Explosions[index].transform.position = tile.transform.position + new Vector3(0, 1, 0);
        Explosions[index].Play();
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

    public bool isValid()
    {
        if  // invalid conditions
            (
            (   _width <= 0 || _height <= 0 || _mines <= 0 ) ||
            (   _mines >= _width*_height                   ) ||
            (   false                                      )
            )

            return false;

        // if everything's ok - return true
        return true;
    }
}

[System.Serializable]
public class Score
{
    private int _timePassed;
    private string _name;

    public int TimePassed
    {
        get { return _timePassed; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public Score(int timePassed)
    {
        _timePassed = timePassed;
        _name = "Anon" + timePassed.ToString();
    }

    public Score(int timePassed, string name)
    {
        _timePassed = timePassed;
        _name = name;
    }
}