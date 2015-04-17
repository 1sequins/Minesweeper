﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{


    // handles
    public GameManager GM;

    // private variables
    private bool _isCustom;
    private GameSettings _UI_GameSettings;

    [SerializeField]
    private UIElements _elements;


    //-------------------------------------------------------------
    // Function Definitions

    // getters & setters
    public UIElements Elements
    {
        get { return _elements; }
    }

    // unity functions
    void Awake()
    {

    }

	void Start ()
    {
        // initial update
	    _UI_GameSettings = GM.Settings;
	    WriteSettingsToInputText(_UI_GameSettings);  
	}

    void Update()
    {
        //TimeScaleText.text = Time.timeScale.ToString();
    #if UNITY_WEBGL

        if (!Screen.fullScreen && (Screen.width != 960 && Screen.width != 800 && Screen.width != 1280))
        {
            Screen.SetResolution(960, 600, false);
        }

    #endif
    }

    //-----------------------------------------------------------
    // Game Options Function Definitions
    public void OptionSliderUpdate(float val)
    {
        int sliderValue = (int) val;    // slider has whole numbers --> safe to cast int

        switch (sliderValue)
        {
            case 0:     // beginner settings
                _elements.SliderDifficulty.text = "Beginner";
                _UI_GameSettings = GameSettings.Beginner;
                break;

            case 1:     // intermediate settings
                _elements.SliderDifficulty.text = "Intermediate";
                _UI_GameSettings = GameSettings.Intermediate;
                break;

            case 2:     // expert settings
                _elements.SliderDifficulty.text = "Expert";
                _UI_GameSettings = GameSettings.Expert;
                break;
            case 3:
                _elements.SliderDifficulty.text = "Custom";
                break;
        }

        WriteSettingsToInputText(_UI_GameSettings);

        // Set Custom Settings according to slider value. Read funct description
        SetCustomSettings(sliderValue == 3);

        //Debug.Log("UIMANAGER::SLIDERUPDATE val=" + sliderValue);
    }

    // Sets the interactability of input fields: if(3) true, else false;
    void SetCustomSettings(bool isCustom)
    {
        // set interactability
        _elements.WidthInput.interactable = isCustom;
        _elements.HeightInput.interactable = isCustom;
        _elements.MinesInput.interactable = isCustom;
        _isCustom = isCustom;

        //Debug.Log("MinesInput: " + _minesInput.IsInteractable());
    }

    public GameSettings ReadSettings()
    {
        if (_isCustom)
        {
            int w = Int32.Parse(_elements.WidthInput.text);
            int h = Int32.Parse(_elements.HeightInput.text);
            int m = Int32.Parse(_elements.MinesInput.text);
            _UI_GameSettings = new GameSettings(w, h, m);
        }

        return _UI_GameSettings;
    }

    void WriteSettingsToInputText(GameSettings settings)
    {
        if (settings == null)
        {
            Debug.Log("Settings NULL");
            return;
        }
        _elements.WidthInput.text = settings.Width.ToString();
        _elements.HeightInput.text = settings.Height.ToString();
        _elements.MinesInput.text = settings.Mines.ToString();
    } // called from ReadSettings()

    //-----------------------------------------------------------
    // "Other Settings" Function Definitions
    public void BackgroundSliderUpdate(float val)
    {
        GameObject.Find("Main Camera").GetComponent<Skybox>().material = Elements.Skyboxes[(int) val];
    }

    //-------------------------------------
    // Scores area

    public void UpdateTimeText(int time)
    {
        _elements.TimerText.text = "Timer: ";
        if (time < 10)
        {
            _elements.TimerText.text += "00" + time;
        }
        else if (time < 100)
        {
            _elements.TimerText.text += "0" + time;
        }
        else if (time < 1000)
        {
            _elements.TimerText.text += time.ToString();
        }
    }

    public void UpdateFlagText(int flagCount)
    {
        _elements.FlagText.text = "Flags: ";

        // handle the sign of the counter
        string flagCountText = "";
        if (flagCount < 0)
        {
            flagCountText += "-";
        }

        // set the counter value according to digit number of flag count
        flagCount = Mathf.Abs(flagCount);   // ignore sign
        if (Mathf.Abs(flagCount) < 10)
        {
            flagCountText += "00" + flagCount;
        }
        else if (Mathf.Abs(flagCount) < 100)
        {
            flagCountText += "0" + flagCount;
        }

        // add the constructed flag count text to the UI Text
        _elements.FlagText.text += flagCountText;

    }
}

[Serializable]
public class UIElements
{
    // UI elements accessed outside of UIManager
    [SerializeField]
    private Text _timerText;
    [SerializeField]
    private Text _flagText;
    [SerializeField]
    private Text _gameStateText;  // won/lost

    // UI elements accessed by only UIManager
    [SerializeField]
    private InputField _heightInput;
    [SerializeField]
    private InputField _widthInput;
    [SerializeField]
    private InputField _minesInput;
    [SerializeField]
    private Text _sliderDifficulty;

    [SerializeField]
    private Material[] _skyboxes;

    // Debug UI variables
    [SerializeField]
    private Text _timeScaleText;


    // getters & setters
    public Text TimerText
    {
        get { return _timerText; }
        set { _timerText = value; }
    }

    public Text FlagText
    {
        get { return _flagText; }
        set { _flagText = value; }
    }

    public Text TimeScaleText
    {
        get { return _timeScaleText; }
        set { _timeScaleText = value; }
    }

    public Text GameStateText
    {
        get { return _gameStateText; }
        set { _gameStateText = value; }
    }

    public InputField HeightInput
    {
        get { return _heightInput; }
        set { _heightInput = value; }
    }

    public InputField MinesInput
    {
        get { return _minesInput; }
        set { _minesInput = value; }
    }

    public InputField WidthInput
    {
        get { return _widthInput; }
        set { _widthInput = value; }
    }

    public Text SliderDifficulty
    {
        get { return _sliderDifficulty; }
        set { _sliderDifficulty = value; }
    }

    public Material[] Skyboxes
    {
        get { return _skyboxes; }
    }
}
