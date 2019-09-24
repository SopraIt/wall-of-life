using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    GameObject TutorImgPrefab;
    GameObject TutorImg;

    static AudioSource AudioSource;
    static GameObject SceneController;

    void Awake()
    {
        TutorImgPrefab = Resources.Load("prefab/TutorImg") as GameObject;
    }

    static public float Play(string audiofile)
    {
        if (!AudioSource)
        {
            SceneController = GameObject.Find("SceneManager");
            AudioSource = SceneController.AddComponent<AudioSource>();
        }
        AudioClip sound = Resources.Load(audiofile) as AudioClip;
        AudioSource.PlayOneShot(sound);
        return sound.length;
    }

    static public float SoundCorrect()
    {
        return Play("audio/correct");
    }

    static public float SoundWrong()
    {
        return Play("audio/wrong");
    }

    static public float SoundSuggest()
    {
        string audiofile = "voices/tutor/Suggest";
        return Play(audiofile);
    }

    static public float SoundMoveOn()
    {
        string audiofile = "voices/tutor/MoveOn";
        return Play(audiofile);
    }

    static public float ShoppingListStart()
    {
        string audiofile = "voices/tutor/ShoppingList";
        return Play(audiofile);
    }


    static public float Congratulate(int errors){
		string audiofile;
        switch (errors) {
		case 0:
			int index = UnityEngine.Random.Range(0, 4);
			audiofile = "voices/tutor/VeryGood" + index;
			break;
		default:
			audiofile = "voices/tutor/Good";
			break;
		}
		return Play(audiofile);
	}

    public static float PlayInstruction()
    {

        bool isPastDayOne = GameController.instance.SkipTutorial;
        if (!isPastDayOne)
        {
            return PlayInstructionsForStep();
        }
        else {
            return 0;
        }
    }

    public static float PlayInstructionsForStep()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Briefing":
                return PlayInstructionsForBriefing();
            case "City":
                return PlayInstructionsForCity();
            case "MarketList":
                return PlayInstructionsForMarketList();
            case "MarketStand":
                return PlayInstructionsForMarketStand();
            default:
                return 0;
        }
    }

    static float PlayInstructionsForBriefing()
    {
        int step = PlayerPrefs.GetInt("BriefingStep");
        string pathToVoice = "voices/tutor/brief/";
        string audioToPlay;

        switch (step)
        {
            case 1:
                audioToPlay = "03_taskConfirmation";
                break;
            case 2:
                audioToPlay = "04_takeTheMoney";
                break;
            case 4:
                audioToPlay = "07_putBackTheMoney";
                break;
            case 5:
                audioToPlay = "08_giveTheItems";
                break;
            default:
                audioToPlay = "00_empty";
                break;
        }
        return Play(pathToVoice + audioToPlay);
    }

    static float PlayInstructionsForCity()
    {
        int step = PlayerPrefs.GetInt("CityStep");
        string pathToVoice = "voices/tutor/city/";
        string audioToPlay;

        switch (step)
        {
            case 0:
                audioToPlay = "00_followMe";
                break;
            default:
                audioToPlay = "00_followMe";
                break;
        }
        return Play(pathToVoice + audioToPlay);
    }


    static float PlayInstructionsForMarketList()
    {
        return MarketList.instance.StartTutorial();

    }
    static float PlayInstructionsForMarketStand()
    {
        int step = PlayerPrefs.GetInt("MarketStandStep");
        string pathToVoice = "voices/tutor/marketStand/";

        string audioToPlay;
        switch (step)
        {
            case 0:
                audioToPlay = "00_askYourItem";
                break;
            case 1:
                audioToPlay = "01_takeYourItem";
                break;
            case 2:
                audioToPlay = "02_askHowMuch";
                break;
            case 3:
                audioToPlay = "03_payTheMerchant";
                break;
            case 4:
                audioToPlay = "04_askTheChange";
                break;
            case 5:
                audioToPlay = "05_takeTheChange";
                break;
			case 6:
				audioToPlay = "06_sayGoodbye";
                //TODO: workaround, to be fixed
				MarketStand.Instance.WaveTutorial();
                break;
            default:
                audioToPlay = "00_askYourItem";
                break;
        }
        return Play(pathToVoice + audioToPlay);
    }

    public static float PlayCorrectScore(int playerLevel)
    {
        string pathToVoice = "voices/tutor/stars/";
        string audioToPlay;
        switch (playerLevel)
        {
            case 1:
                audioToPlay = pathToVoice + "score_level1";
                break;
            case 2:
                audioToPlay = pathToVoice + "score_level2";
                break;
            case 3:
                audioToPlay = pathToVoice + "score_level3";
                break;
            case 4:
                audioToPlay = pathToVoice + "score_level4";
                break;
            default:
                audioToPlay = pathToVoice + "score_level1";
                break;
        }
        return Play(audioToPlay);
    }

    public static float PlayPriceObj(int price, string currentCategory)
    {
        string pathToVoice = "voices/merchants/prices/";
        List<string> merchantsFemale = new List<string>(){"kitchen", "school", "fruit"};
        string gender = merchantsFemale.Contains(currentCategory) ? "female" : "male";
        string audioToPlay;
        switch (gender)
        {
            case "female":
                audioToPlay = pathToVoice + "female_" + price;
                break;
            case "male":
                audioToPlay = pathToVoice + "male_" + price;
                break;
            default:
                audioToPlay = pathToVoice + "female_1";
                break;
        }
        return Play(audioToPlay);
    }

    public static float PlayItemAudio(string audioToPlay)
    {
        string pathToItem = "voices/tutor/item/";
        return Play(pathToItem + audioToPlay);
    }

    public void ShowTutorDialogue(string dialogue)
    {
        if (!TutorImg)
        {
            TutorImg = Instantiate(TutorImgPrefab);
            TutorImg.transform.SetParent(GameObject.Find("TutorCanvas").GetComponent<Transform>());
        }
    }
}
