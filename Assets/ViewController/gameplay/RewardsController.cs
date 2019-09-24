using Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "winConditionController", menuName = "wall-of-life/List", order = 1)]
public class RewardsController : MonoBehaviour
{
    public int score;
    public int finalScore = 0;

    private int FirstLvStars;
    private int SecondLvStars;
    private int ThirdLvStars;

    public static RewardsController instance = null;

    private bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        Stars Stars = DataManager.GetStars();
        System.Int32.TryParse(Stars.First, out FirstLvStars);
        System.Int32.TryParse(Stars.Second, out SecondLvStars);
        System.Int32.TryParse(Stars.Third, out ThirdLvStars);
    }

    public void SetFinalScore()
    {
        finalScore = finalScore + 1;
        Debug.Log("Final score: " + finalScore);
    }

    public bool CheckWinCondition()
    {
        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();
        switch (playerLevel) {
            case 3:
                finalScore = ThirdLvStars;
                break;
            case 2:
                finalScore = SecondLvStars;
                break;
            case 1:
                finalScore = FirstLvStars;
                break;
            default:
                Debug.Log("Level Not Recognized");
                break;
        }

        score = PlayerPrefs.GetInt("score");

        return score >= finalScore;
    }

}
