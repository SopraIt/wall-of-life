using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour {

    public static ScoreScript instance;

    public int maxFontSize;
    private int startingFontSize;

    private Text score;
    
    // Use this for initialization
    void Awake () {
        instance = this;
        if ((score == null) && (GameObject.Find("ScoreText").GetComponent<Text>() != null))
        {
            score = GameObject.Find("ScoreText").GetComponent<Text>();
            startingFontSize = score.fontSize;
            score.text = "" + PlayerPrefs.GetInt("score");
        }
        else
        {
            Debug.LogWarning("Missing Text component. Please add one");
        }

    }

    public void ScoreUpdate() {
        StartCoroutine("UpdateScore");
    }

    IEnumerator UpdateScore() {
        while (score.fontSize < maxFontSize) {
            score.fontSize++;
            yield return null;
        }
        score.text = "" + PlayerPrefs.GetInt("score");
        while (score.fontSize > startingFontSize)
        {
            score.fontSize--;
            yield return null;
        }
    }
}
