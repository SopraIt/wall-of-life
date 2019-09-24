using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine.SceneManagement;
using System.Text;
using System.Linq;

public class City : MonoBehaviour
{
    public static City instance;

    GameObject tutor;
 
    void Awake()
    {
        instance = this;
        tutor = GameObject.Find("Tutor");
        gameObject.AddComponent<GameController>();

        PlayerPrefs.SetInt("CityStep", 0);
        CheckStep();
    }


    public void CheckStep()
    {
        int step = PlayerPrefs.GetInt("CityStep");
        switch (step)
        {
            case 0:
                CheatScene();
                //PrepareScene();
                break;
            case 1:
                ProceedToMarket();
                break;
            default:
                Debug.Log("Default case");
                break;
        }
    }

    private void CheatScene() {
        PlayerPrefs.SetString("level", "1");
        PlayerPrefs.SetInt("score", 10);

        string pathToVoice = "voices/tutor/brief/";

        if (RewardsController.instance.CheckWinCondition())
        {
          SoundManager.Play(pathToVoice + "14_heresThePrize");
        }
        else
        {
           SoundManager.Play(pathToVoice + "15_consolationPrize");
        }

        SceneManager.LoadScene("Scenes/PrizeReward", LoadSceneMode.Single);
    }

    private void PrepareScene()
    {
        //ToDo Those can be coupled in an unique method(?)
        GameController.instance.setQuestionType("Draggable");
        PlayerPrefs.SetString("CorrectObj", "Kiddo");
        PlayerPrefs.Save();
        GameObject market = GameObject.Find("Market");
        GameController.MarkContainerAsElegible(market);
        GameController.instance.AnswerCreation("Draggable", true, 1);


        StartCoroutine(Tutorial());

    }
    IEnumerator Tutorial() {
        yield return new WaitForSeconds(SoundManager.PlayInstruction());

        float step = 15.0f * Time.deltaTime;

        GameObject[] wayPoints = GameObject.FindGameObjectsWithTag("WayPoint").OrderBy(go => go.name).ToArray();

        foreach (GameObject point in wayPoints)
        {
            Debug.Log("point " + point);

            Vector2 tutorPosition = tutor.transform.position;
            Vector2 arrivalPoint = point.transform.position;

            Debug.Log("tutorPosition " + tutorPosition);
            Debug.Log("arrivalPoint " + arrivalPoint);

            while (tutorPosition != arrivalPoint) {
                Debug.Log("while");

                tutorPosition = Vector2.MoveTowards(tutorPosition, arrivalPoint, step);
                tutor.transform.position = tutorPosition;
                yield return new WaitForFixedUpdate();
            }
            //yield return new WaitUntil(() => (tutorPosition == arrivalPoint));
        }

        HandController.instance.HandInteract(true);

    }

    void ProceedToMarket() {
        PlayerPrefs.SetInt("CityStep", 2);
        SceneManager.LoadScene("MarketList");
    }

}