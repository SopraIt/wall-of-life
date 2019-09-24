using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine.SceneManagement;
using System.Text;

public class Briefing : MonoBehaviour
{
    public static Briefing instance;
    GameObject backpack;
    GameObject backpackPrefab;

    GameObject drawerWallet;
    GameObject wallet;
    GameObject walletPrefab;
    GameObject paperbag;
    GameObject paperbagPrefab;

    GameObject Caretaker;
    GameObject ShoppingList;

    List<Item> items;
    string chosenItemText;
    string chosenItemName;
    int chosenItemPrice;

    string pathToVoice = "voices/tutor/brief/";
    string pathToItem = "voices/tutor/item/";

    private List<Steps> questions;

    void Awake()
    {
        instance = this;
        backpackPrefab = Resources.Load("prefab/backpack") as GameObject;
        walletPrefab = Resources.Load("prefab/brief_money") as GameObject;
        paperbagPrefab = Resources.Load("prefab/Paperbag") as GameObject;

        Caretaker = GameObject.Find("Caretaker");
        ShoppingList = GameObject.Find("ShoppingList");

        questions = DataManager.GetAnswersOfCurrentScene(SceneManager.GetActiveScene().name);
        gameObject.AddComponent<GameController>();
        chosenItemText = PlayerPrefs.GetString("item1");
        CheckStep();
    }

    public void ResetBriefStep()
    {
		PlayerPrefs.SetInt("BriefingStep", 4);
        PlayerPrefs.SetInt("errorsCount", 0);
        PlayerPrefs.Save();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void CheckStep()
    {
        int step = PlayerPrefs.GetInt("BriefingStep");
        switch (step)
        {
            case 0:
                InitialSetup();
                break;
            case 1:
                StartCoroutine(TaskConfirmation());
                break;
            case 2:
                GetTheMoneys();
                break;
            case 3:
                StartCoroutine(GoToMarket());
                break;
            //DEBRIEF
            case 4:
                StartCoroutine(PutTheMoneysBack());
                break;
            case 5:
                StartCoroutine(GiveTheItem());
                break;
            case 6:
                StartCoroutine(WhereHaveYouBeen());
                break;
            case 7:
                StartCoroutine(WhatDidYouBuy());
                break;
            case 8:
                StartCoroutine(HowMuchDidYouPaid());
                break;
            case 9:
                StartCoroutine(GoToPrize());
                break;
            default:
                Debug.Log("Default case");
                break;
        }
    }

    void InitialSetup()
    {

        SelectRandomAnswerByLevel();
        StartCoroutine(TaskDescription());
    }

    void SelectRandomAnswerByLevel()
    {

        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();
        List<int> randomCatDiff = new List<int>();
        List<string> oldCategories = new List<string>();
        for (int i = 1; i <= playerLevel; i++)
        {
            
			string category = GetRandomCategories(oldCategories);
            oldCategories.Add(category);
            PlayerPrefs.SetString("category" + i, category);
            PlayerPrefs.SetString("CurrentCategory", category);
            randomCatDiff.Add(DataManager.GetClasses().FindIndex(x => x.Equals(category)));

            items = DataManager.GetItemsForClass(category);
            //Choosing a random item
            int chooseRandom = Random.Range(0, items.Count);
            chosenItemText = items[chooseRandom].text;
            chosenItemName = items[chooseRandom].name;
            chosenItemPrice = int.Parse(items[chooseRandom].Price);
            Debug.Log("Selected Item: " + chosenItemName.ToUpper());
            PlayerPrefs.SetString("item" + i, chosenItemText);
            PlayerPrefs.SetString("CorrectProduct" + i, chosenItemName);
            PlayerPrefs.SetString("CurrentProduct", chosenItemName);
            PlayerPrefs.SetInt("PriceObj" + i, chosenItemPrice);
        }

    }

    private string GetRandomCategories(List<string> oldCategories)
    {
        List<string> categories = DataManager.GetClasses();
        int chooseRandomCat = Random.Range(0, categories.Count);
        if (!oldCategories.Contains(categories[chooseRandomCat]))
        {
            return categories[chooseRandomCat];
        }
        return GetRandomCategories(oldCategories);
    }
    
    IEnumerator TaskDescription()
    {
        //"Hi, today we are going to the market!"
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "00_greetings_run"));
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "00_greetings"));

        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();
        
        //"Today you will buy... " + chosenItemText + "!"
        for (int i=1; i <= playerLevel; i++)
        {
			if (i == 1 && i != playerLevel) {
				//"...First..."
				//yield return new WaitForSeconds(SoundManager.Play(pathToItem + "00_first"));			
			}

            string currentChoosenItem = PlayerPrefs.GetString("CorrectProduct" + i);
            yield return new WaitForSeconds(SoundManager.Play(pathToItem + currentChoosenItem));
            yield return new WaitForSeconds(0.25f);

			if (i != playerLevel) {
				//"...then..."
				//yield return new WaitForSeconds (SoundManager.Play (pathToItem + "00_then" + (i + 1)));
			}
			yield return new WaitForSeconds(0.25f);
        }

        //"Don't worry, first time I'll help you out."
        //yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "00_firstTime"));

		//"Today I'm not gonna give you hints, you're gonna do it by yourself"
		if(GameController.instance.SkipTutorial){
			yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "00_noTutorial"));	
		}

        //"For each good answer, you're gonna gain a star.
        //With enough stars you can redeem one of those prizes"
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "01a_taskDescription"));
		yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "01b_taskDescription"));

        PlayerPrefs.SetInt("BriefingStep", 1);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Scenes/PrizeSelect", LoadSceneMode.Single);
    }  

    private void CreateAnswersList ()
    {
        List<string> categoryList = new List<string>();
        List<string> answerTypeList = new List<string>();
        string correctAnswers = "";
        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();

        for (int i = 1; i <= playerLevel; i++)
        {
            PlayerPrefs.SetString("correct", "0");

            categoryList.Add(PlayerPrefs.GetString("category" + i));
            Classes answerType = DataManager.GetClasses(PlayerPrefs.GetString("category" + i));
            answerTypeList.Add(answerType.name);
            GameController.instance.setQuestionType(PlayerPrefs.GetString("CorrectProduct" + i));
            correctAnswers += ((PlayerPrefs.GetString("CorrectProduct" + i)) + "|");
        }

        GameController.instance.AnswerCreation(answerTypeList, correctAnswers, 1);
    }

    IEnumerator TaskConfirmation()
    {

        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();

        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "00_greetings_run"));
        //"You'll have it if you can gain N stars"
		yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "02_starsGoal"));
        yield return new WaitForSeconds(SoundManager.PlayCorrectScore(playerLevel));

        //"First of all, do you remember what you have to buy?"
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "03_taskConfirmation"));

        CreateAnswersList();
    }

    void GetTheMoneys()
    {
        //ToDo Those two can be coupled in an unique method(?)
        StartCoroutine(MoneySearch());
    }

    IEnumerator MoneySearch()
    {
        //"Now take the money and put it in the backpack!"
        yield return new WaitForSeconds(SoundManager.PlayInstruction());
        GameController.instance.setQuestionType("Draggable");

        //Hardcoded - integrate inside AnswerCreation?
        PlayerPrefs.SetString("CorrectObj", "brief_money");
        PlayerPrefs.Save();

        BackpackEnter();
        GameController.MarkContainerAsElegible(backpack);
        GameController.instance.AnswerCreation("Draggable", true, 1);
    }

    void BackpackEnter(System.Action action) {
        if (!backpack)
        {
            backpack = Instantiate(backpackPrefab);
        }
        backpack.GetComponent<Interactions>().EnterFromRight(action);
    }

    void BackpackEnter()
    {
        if (!backpack)
        {
            backpack = Instantiate(backpackPrefab);
        }
        backpack.GetComponent<Interactions>().EnterFromRight();
    }

    IEnumerator GoToMarket()
    {
        HandController.instance.HandInteract(false);
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "05_shoppingList"));
        GameController.ToggleObject(ShoppingList, true);
        //TODO extend FadeIn for images (currently only for spirtes)

        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "06_goToMarket"));
        PlayerPrefs.SetInt("BriefingStep", 4);
        SceneManager.LoadScene("Scenes/MarketList", LoadSceneMode.Single);
    }

    IEnumerator PutTheMoneysBack()
    {
        //Disable other draggables
        drawerWallet = GameObject.Find("brief_money");
        drawerWallet.SetActive(false);
        GameObject[] Draggables = GameObject.FindGameObjectsWithTag("Draggable");
        foreach (GameObject draggable in Draggables) {
            draggable.tag = "Untagged";
        }
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "00_greetings_run"));
        //"Put the wallet with the money back at its place"
        yield return new WaitForSeconds(SoundManager.PlayInstruction());

        wallet = Instantiate(walletPrefab);
        wallet.name = "moneyBack";
        wallet.GetComponent<Collider2D>().enabled = true;
        wallet.GetComponent<Interactions>().EnterFromRight();
        GameObject drawer = GameObject.Find("background_brief_drawer");
        drawer.GetComponent<Collider2D>().enabled = true;
        GameController.MarkContainerAsElegible(drawer);
        
        GameController.instance.setQuestionType("Draggable");
        PlayerPrefs.SetString("CorrectObj", "moneyBack");
        PlayerPrefs.Save();
        GameController.instance.AnswerCreation("Draggable", true, 1);
    }

    public void SpawnPaperbag()
    {
        GameObject paperbag = Instantiate(paperbagPrefab);
        paperbag.name = "paperbag";
        paperbag.GetComponent<Interactions>().EnterFromRight();
        paperbag.GetComponent<Collider2D>().enabled = true;

    }

    IEnumerator GiveTheItem()
    {
        drawerWallet.SetActive(true);
        drawerWallet.tag = "Untagged";
        drawerWallet.GetComponent<Collider2D>().enabled = false;

        GameObject CaretakeContainer = GameObject.Find("CaretakerContainer");
        CaretakeContainer.GetComponent<Collider2D>().enabled = true;
        GameController.MarkContainerAsElegible(CaretakeContainer);
        Caretaker.GetComponent<Interactions>().enabled = true;
        //"Now you can give to the caretaker what you have bought today"
        yield return new WaitForSeconds(SoundManager.PlayInstruction());
        SpawnPaperbag();

        GameController.instance.setQuestionType("Draggable");
        PlayerPrefs.SetString("CorrectObj", "paperbag");
        PlayerPrefs.Save();
        GameController.instance.AnswerCreation("Draggable", true, 1);

    }

    IEnumerator WhereHaveYouBeen()
    {
        Caretaker.SendMessage("FadeOut");
        if (paperbag)
            paperbag.SendMessage("FadeOut");

        // "Now, let's see if you can tell me...
        // Where have you been?"
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "10_where"));

        GameController.ActivateObjects("Draggable", false);
        GameController.ActivateObjects("DragIn", false);

        GameController.instance.setQuestionType("question_1");
        GameController.instance.AnswerCreation(questions[0], 1);
    }
    IEnumerator WhatDidYouBuy()
    {
        // "What did you buy?"
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "11_what"));
        CreateAnswersList();
    }
    IEnumerator HowMuchDidYouPaid()
    {
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "12_howMuch"));
        // "How much do you spent?"

        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();
       
        List<string> lisString = new List<string>();
        List<int> lisInt = new List<int>();

        for (int i = 1; i <= playerLevel; i++)
        {
            string category = PlayerPrefs.GetString("category" + i);
            string product = PlayerPrefs.GetString("CorrectProduct" + i);
            int price = PlayerPrefs.GetInt("PriceObj" + i);


            Classes categoryElement = DataManager.GetClasses(category);
            List<Item> items = DataManager.GetItemsForClass(categoryElement.name);
            foreach (Item item in items)
            {
                if (item.name == product)
                {
                    product = GameController.instance.PastLevelOne ? item.shorttext : item.text;
                    break;
                }
            }

            lisString.Insert(0, product);
            lisInt.Insert(0, price);
        }



        GameController.instance.setQuestionType("question_1");
        GameController.instance.AnswerCreation(questions[1], lisString, lisInt, 1);

    }

    IEnumerator GoToPrize()
    {
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "13_youWorkedHard"));
        // "Congratulations! You worked really hard!"
        yield return new WaitForSeconds(SoundManager.Play("audio/applause"));

        //create new Match for current player/person
        string playerId = PlayerPrefs.GetString("id");
        DataManager.newGame(playerId);


        if (RewardsController.instance.CheckWinCondition())
        {
            GameController.instance.UpgradePlayerLevel();
            yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "14_heresThePrize"));
        }
        else
        {
            yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "15_consolationPrize"));
        }

        SceneManager.LoadScene("Scenes/PrizeReward", LoadSceneMode.Single);
    }


}