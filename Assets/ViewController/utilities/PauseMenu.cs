using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

    public static bool gameIsPaused = false;
    public bool menuIsLocked = false;
    public GameObject bgMenuUI;
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;
    public GameObject advancedOptionsMenuUI;
    public static PauseMenu instance = null;
    private List<string> options;

    private void Awake()
    {
        instance = this;

        options = new List<string>
        {
            "TremblingThreshold",
            "AcceptableMovementArea",
            "RapidCounterThreshold"
        };
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape) && !menuIsLocked)
        {
            if(gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
	}

    public void Resume()
    {
        bgMenuUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false);
        advancedOptionsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Pause()
    {
        bgMenuUI.SetActive(true);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Login", LoadSceneMode.Single);
    }

    public void OptionsMenu(bool activate) {
        pauseMenuUI.SetActive(!activate);
        optionsMenuUI.SetActive(activate);
        if (activate)
        {
            InitOptions();
        }
    }

    public void AdvancedOptions(bool activate)
    {
        optionsMenuUI.SetActive(!activate);
        advancedOptionsMenuUI.SetActive(activate);
        if (activate) {
            InitAdvancedOptions();
        }
    }

    public void UpdateSensitivity(string axis) {
        if (axis == "x")
        {
            int sliderValueX = (int)GameObject.Find("SliderX").GetComponent<Slider>().value;
            KinectManager.instance.sensitivityX = sliderValueX;
            PlayerPrefs.SetInt("sensitivityX", sliderValueX);
        }
        else if (axis == "y")
        {
            int sliderValueY = (int)GameObject.Find("SliderY").GetComponent<Slider>().value;
            KinectManager.instance.sensitivityY = sliderValueY;
            PlayerPrefs.SetInt("sensitivityY", sliderValueY);
        }

        PlayerPrefs.Save();
    }

    public void ResetSensitivity()
    {
        int defaultX = KinectManager.instance.defaultSensitivityX;

        KinectManager.instance.sensitivityX = defaultX;
        PlayerPrefs.SetInt("sensitivityX", defaultX);
        GameObject.Find("SliderX").GetComponent<Slider>().value = defaultX;

        int defaultY = KinectManager.instance.defaultSensitivityY;
        KinectManager.instance.sensitivityY = defaultY;
        PlayerPrefs.SetInt("sensitivityY", defaultY);
        GameObject.Find("SliderY").GetComponent<Slider>().value = defaultY;


        PlayerPrefs.Save();

    }

    private void InitOptions()
    {
        GameObject.Find("SliderY").GetComponent<Slider>().value = PlayerPrefs.GetInt("sensitivityY");
        GameObject.Find("SliderX").GetComponent<Slider>().value = PlayerPrefs.GetInt("sensitivityX");
    }

    private void InitAdvancedOptions()
    {
        foreach (var option in options) {
            float playerOption = PlayerPrefs.GetFloat(option);
            if (playerOption != 0)
            {
                GameObject.Find(option).GetComponent<Slider>().value = PlayerPrefs.GetFloat(option);
            }
        }
    }

    public void UpdateOption(string property)
    {
        float optionValue = GameObject.Find(property).GetComponent<Slider>().value;
        PlayerPrefs.SetFloat(property, optionValue);
        typeof(KinectManager).GetField(property).SetValue(KinectManager.instance, optionValue);
        PlayerPrefs.Save();
    }

    public void ResetOptions()
    {
        foreach (var option in options) {
            float defaultValue = (float)typeof(KinectManager).GetField("default" + option).GetValue(KinectManager.instance);
            PlayerPrefs.SetFloat(option, defaultValue);
            typeof(KinectManager).GetField(option).SetValue(KinectManager.instance, defaultValue);
            GameObject.Find(option).GetComponent<Slider>().value = defaultValue;
        }
        PlayerPrefs.Save();
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
