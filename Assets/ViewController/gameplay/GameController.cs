using System.Collections.Generic;
using UnityEngine;
using Model;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class GameController : MonoBehaviour
{
    public Camera cam;

    static GameObject answerPrefab;
    static GameObject wrongCrossPrefab;
    static GameObject rightCheckPrefab;
    static GameObject timerPrefab;
    static GameObject timerObj;

    static GameObject backdropObj;
    static Image backdropImg;

    GameObject IconToApply;
    GameObject Hand;

    private static string questionType;
    private IEnumerator coroutine;
    private bool CR_running = false;
    public bool PastLevelOne = false;
    public bool SkipTutorial = false;

    public static GameObject ElegibleContainer;
    public GameObject Recipient;
    public int stepIncrease;

    const float timer = 15.0f;

    public string getQuestionType()
    {
        return questionType;
    }

    public void setQuestionType(string value)
    {
        questionType = value;
    }

    public static GameController instance;

    private void Awake()
    {
        instance = this;

        PastLevelOne = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel() > 1 ? true : false;
        SkipTutorial = PlayerPrefs.GetString("skipTutorial") == "true";

        backdropObj = GameObject.Find("Backdrop");
        if (backdropObj)
            backdropImg = GameObject.Find("Backdrop").GetComponent<Image>();

        answerPrefab = Resources.Load("prefab/Answer") as GameObject;
        wrongCrossPrefab = Resources.Load("prefab/RedCross") as GameObject;
        rightCheckPrefab = Resources.Load("prefab/GreenCheck") as GameObject;

        timerPrefab = Resources.Load("prefab/Timer") as GameObject;

        Hand = GameObject.Find("Hand");
    }

    public static void ToggleObject(GameObject gameObject, bool boolean)
    {
        Collider2D collider2D = gameObject.GetComponent<Collider2D>();
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Image image = gameObject.GetComponentInChildren<Image>();
        if (collider2D)
            collider2D.enabled = boolean;
        if (spriteRenderer)
            spriteRenderer.enabled = boolean;
        if (image)
            image.enabled = boolean;
    }

    //Activates or disactivates objects based on tag
    public static void ActivateObjects(string objectType, bool enable)
    {
        GameObject[] objectsToModify = GameObject.FindGameObjectsWithTag(objectType);
        foreach (GameObject modifyThis in objectsToModify)
        {
            if (modifyThis.GetComponent<Collider2D>())
                modifyThis.GetComponent<Collider2D>().enabled = enable;
        }
    }

    //Marks target GameObject as DragIn objects and disables previous.
    public static void MarkContainerAsElegible(GameObject gameobject)
    {
        GameObject[] objectsToModify = GameObject.FindGameObjectsWithTag("DragIn");
        foreach (GameObject modifyThis in objectsToModify)
        {
            modifyThis.GetComponent<Collider2D>().enabled = false;
        }
        gameobject.GetComponent<Collider2D>().enabled = true;
        ElegibleContainer = gameobject;
    }

    public void AnswerCreation(Steps step, int stepIncrease)
    {
        AnswerCreation(step, new List<string>(), new List<int>(), stepIncrease);
    }

    public void AnswerCreation(Steps step, List<string> parameters1, List<int> parameters2, int stepIncrease)
    {
        ToggleBackdrop(true);

        int itemCount = 0;

        GameObject TargetParent;
        string stepTypeQuestion = step.type;
        Hashtable ht = new Hashtable();
        if (stepTypeQuestion == "param")
        {
            ht = CreateExclude(parameters1);
            int prodInsertPriceCount = 0;
            foreach (string product in parameters1)
            {
                int price = parameters2 != null && parameters2.Count >= 1 ? parameters2[prodInsertPriceCount] : 0;
                GetExclude(ht, product).Add(price);
                ht[product] = GetExclude(ht, product);
                prodInsertPriceCount++;
            }
        }
        List<Question> questions = step.questions;
        this.stepIncrease = stepIncrease;
        ShuffleMe(questions);
        foreach (Question item in questions)
        {
            itemCount++;
            int countNewLine = 1;

            GameObject answerButton = Instantiate(answerPrefab);
            answerButton.name = "question_" + item.id;
            answerButton.GetComponentsInChildren<Text>()[1].text = itemCount.ToString();
            if (stepTypeQuestion == "text")
            {
                answerButton.GetComponentInChildren<Text>().text = item.text;
            }
            else if (stepTypeQuestion == "param")
            {
                string questionTot = "";
                int prodCount = 0;
                //Debrief's question on product prices
                foreach (string product in parameters1)
                {
                    int price = parameters2 != null && parameters2.Count >= 1 ? parameters2[prodCount] : 0;
                    if (item.id != 1)
                    {
                        int rndNumber = GiveMeANumber(1, 10, GetExclude(ht, product));
                        if(price == rndNumber) Debug.LogWarning("not working");
                        price = rndNumber;
                        GetExclude(ht, product).Add(price);
                        ht[product] = GetExclude(ht, product);
                    }
                    questionTot += PastLevelOne ? "• " + string.Format(item.text, product.ToLower(), price) : string.Format(item.text, product.ToLower(), price);
                    if (countNewLine < parameters1.Count)
                    {
                        questionTot = questionTot + "\r\n";
                        countNewLine++;
                    }
                    prodCount++;
                }
                answerButton.GetComponentInChildren<Text>().text = questionTot;
                answerButton.GetComponentInChildren<Text>().resizeTextForBestFit = PastLevelOne;
                answerButton.GetComponentInChildren<Text>().alignment = PastLevelOne ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;

            }


            string TargetParentString = "Panel" + itemCount;
            TargetParent = GameObject.Find(TargetParentString);
            RectTransform TargetParentRect = TargetParent.GetComponent<RectTransform>();

            RectTransform anwserTextRect = answerButton.GetComponent<RectTransform>();
            anwserTextRect.transform.SetParent(TargetParentRect.transform);
            anwserTextRect.position = TargetParentRect.position;
            anwserTextRect.sizeDelta = TargetParentRect.rect.size;
            anwserTextRect.localScale = new Vector3(1, 1, 0);
        }
        PlayerPrefs.SetString("CorrectQuestion", "question_1");
        RewardsController.instance.SetFinalScore();
        coroutine = TimerForAnswer("Selection");
        StartCoroutine(coroutine);
    }

    private Hashtable CreateExclude(List<string> parameters1)
    {
        Hashtable ht = new Hashtable();

        foreach (string product in parameters1) {
            var exclude = new HashSet<int>();
            ht.Add(product, exclude);
        }
        return ht;
    }

    private HashSet<int> GetExclude(Hashtable ht, string parameter)
    {
        return (HashSet<int>)ht[parameter];
    }

    private int GiveMeANumber(int rngMin, int rngMax, List<int> randomNumbers)
    {
        var rand = new System.Random();
        int index = rand.Next(rngMin, rngMax);
        if (randomNumbers.Contains(index))
        {
            GiveMeANumber(1, 10, randomNumbers);
        }
        return index;
    }

    private int GiveMeANumber(int rngMin, int rngMax, HashSet<int> exclude)
    {

        var range = Enumerable.Range(rngMin, rngMax).Where(i => !exclude.Contains(i));

        var rand = new System.Random();
        int index = rand.Next(rngMin, rngMax - exclude.Count);
        return range.ElementAt(index);
    }

    public static void ShuffleMe<T>(IList<T> list)
    {
        int n = list.Count;

        for (int i = list.Count - 1; i > 1; i--)
        {
            int rnd = Random.Range(0, n);

            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }
    }

    public void AnswerCreation(List<string> categories, string correctAnswers, int stepIncrease)
    {

        this.stepIncrease = stepIncrease;
        int itemCount = 0;
        ToggleBackdrop(true);
        List<string> items = GetListOfItems(categories, correctAnswers);
        GameObject TargetParent;

        foreach (string item in items)
        {
            List<Item> itemsSplitted = DataManager.GetItemsByNameAndCategories(categories, item);
            GameObject answerButton = Instantiate(answerPrefab);
            string textButton = "";
            int countNewLine = 1;
            itemCount++;

            foreach (Item itemSplit in itemsSplitted)
            {
                answerButton.name = itemSplit.name;
                string text = itemSplit.text;
                text = PastLevelOne ? "• " + itemSplit.shorttext : itemSplit.text;
                textButton += text;
                if (countNewLine < itemsSplitted.Count)
                {
                    textButton = textButton + "\r\n";
                    countNewLine++;
                }
            }
            answerButton.GetComponentsInChildren<Text>()[1].text = itemCount.ToString();
            answerButton.GetComponentInChildren<Text>().text = textButton;
            answerButton.GetComponentInChildren<Text>().resizeTextForBestFit = PastLevelOne;
            answerButton.GetComponentInChildren<Text>().alignment = PastLevelOne ? TextAnchor.MiddleLeft : TextAnchor.MiddleCenter;

            string TargetParentString = "Panel" + itemCount;
            TargetParent = GameObject.Find(TargetParentString);
            RectTransform TargetParentRect = TargetParent.GetComponent<RectTransform>();

            RectTransform anwserTextRect = answerButton.GetComponent<RectTransform>();
            anwserTextRect.transform.SetParent(TargetParentRect.transform);
            anwserTextRect.position = TargetParentRect.position;
            anwserTextRect.sizeDelta = TargetParentRect.rect.size;
            anwserTextRect.localScale = new Vector3(1, 1, 0);
        }
        RewardsController.instance.SetFinalScore();
        coroutine = TimerForAnswer("Selection");
        StartCoroutine(coroutine);
    }

    public void AnswerCreation(Classes category, int stepIncrease)
    {
        List<Item> items = DataManager.GetItemsForClass(category.name);
        List<Item> listOfItems = new List<Item>();
        this.stepIncrease = stepIncrease;
        int itemCount = 0;

        ToggleBackdrop(true);

        GameObject TargetParent;

        foreach (Item item in items)
        {
            listOfItems.Add(item);
            itemCount++;

            GameObject answerButton = Instantiate(answerPrefab);
            answerButton.name = item.name;
            answerButton.GetComponentInChildren<Text>().text = item.text;
            answerButton.GetComponentsInChildren<Text>()[1].text = itemCount.ToString();

            string TargetParentString = "Panel" + itemCount;
            TargetParent = GameObject.Find(TargetParentString);
            RectTransform TargetParentRect = TargetParent.GetComponent<RectTransform>();

            RectTransform anwserTextRect = answerButton.GetComponent<RectTransform>();
            anwserTextRect.transform.SetParent(TargetParentRect.transform);
            anwserTextRect.position = TargetParentRect.position;
            anwserTextRect.sizeDelta = TargetParentRect.rect.size;
            anwserTextRect.localScale = new Vector3(1, 1, 0);
        }
        RewardsController.instance.SetFinalScore();
        coroutine = TimerForAnswer("Selection");
        StartCoroutine(coroutine);
    }

    public void AnswerCreation(string objTag, bool enable, int stepIncrease)
    {
        this.stepIncrease = stepIncrease;
        ActivateObjects(objTag, enable);
        if (RewardsController.instance != null)
        {
            RewardsController.instance.SetFinalScore();
        }
        if (SceneManager.GetActiveScene().name != "MarketList")
        {
            coroutine = TimerForAnswer("Draggable");
            StartCoroutine(coroutine);
        }
    }

    public void AnswerCreation(string waveAnswer)
    {
        coroutine = TimerForAnswer(waveAnswer);
        RewardsController.instance.SetFinalScore();
        StartCoroutine(coroutine);
        StartCoroutine(CheckWave());
    }

    IEnumerator CheckWave() {
        while (!KinectManager.instance.waveComplete) {
            yield return null;
        }
        StopTimer(coroutine);
        KinectManager.instance.checkWave = false;
        KinectManager.instance.waveComplete = false;
        KinectManager.instance.wavesComplete = 0;
        StartCoroutine(CorrectAnswer(GameObject.Find("Hand")));

    }

    private List<string> GetListOfItems(List<string> categories, string correctAnswers)
    {
        List<string> items = new List<string>();
        foreach (string categoryName in categories)
        {
            List<Item> itemsForCategory = DataManager.GetItemsForClass(categoryName);
            int i = 0;
            foreach (Item item in itemsForCategory)
            {
                if (!correctAnswers.Contains(item.name) && i < 5)
                {
                    if (items.Count < 5)
                    {
                        items.Insert(i, item.name);
                        i++;
                    }
                    else
                    {
                        var output = items[i];
                        output += "|" + item.name;
                        items[i] = output;
                        i++;
                    }
                }
            }
        }

        int rnd = Random.Range(0, 6);
        items.Insert(rnd, correctAnswers);
        return items;
    }


    IEnumerator TimerForAnswer(string objTag)
    {
        if (SkipTutorial || objTag == "Wave")
        {
            timerObj = GameObject.Instantiate(timerPrefab);
            timerObj.transform.SetParent(GameObject.Find("Canvas").transform, false);

            Image timerImg = timerObj.GetComponent<Image>();
            CR_running = true;

            float counter = timer;
            //        yield return new WaitForSeconds(timer);
            while (counter > 0)
            {
                counter -= Time.deltaTime;
                timerImg.fillAmount = (counter / timer);

                yield return null; //Don't freeze Unity
            }
            while (HandController.instance.isSelecting)
            {
                yield return null;
            }
            Destroy(timerObj);
            CR_running = false;
            
            NoAnswer();
        }
    }

    void StopTimer(IEnumerator timerCoroutine)
    {
        StopCoroutine(timerCoroutine);
        Destroy(timerObj);
    }

    public void DestroyAnswers()
    {
        GameObject Panel1 = GameObject.Find("Panel1");
        if (!Panel1)
            return;
        if (Panel1.transform.childCount > 0)
        {
            for (int index = 1; index <= 6; index++)
            {
                Destroy(GameObject.Find("Panel" + index).transform.GetChild(0).gameObject);
            }
        }
    }

    private GameObject CorrectObject()
    {
        if ("Draggable".Equals(questionType))
        {
            return GameObject.Find(PlayerPrefs.GetString("CorrectObj"));
        }
        else
        {
            return GameObject.Find(questionType);
        }
    }

    public void CheckAnswer(GameObject gameObject)
    {
        Hand.SendMessage("HandInteract", false);

        if (CR_running) StopTimer(coroutine);
        GameObject gameObjectCorrect;
        CR_running = false;

        gameObjectCorrect = CorrectObject();

        Debug.Log("questionType: " + questionType);
        Debug.Log("gameObjectCorrect: " + gameObjectCorrect);

        //Wrong Object
        if (!GameObject.ReferenceEquals(gameObjectCorrect, gameObject))
        {
            StartCoroutine(WrongAnswer(gameObject, gameObjectCorrect));
        }
        //Right Object
        else
        {
            //Draggable Behaviour
            if (gameObject.tag == "Draggable")
            {
                if (Recipient == ElegibleContainer)
                {
                    StartCoroutine(CorrectAnswer(gameObject));
                }
                else
                {
                    StartCoroutine(WrongAnswer(gameObject, gameObjectCorrect));
                }
            }
            //END OF Draggable Behaviour
            else
            {
                StartCoroutine(CorrectAnswer(gameObject));
            }
        }
    }

    void NoAnswer()
    {
        Hand.SendMessage("HandInteract", false);
        SoundManager.SoundWrong();
        GameObject gameObjectCorrect = CorrectObject();
        StartCoroutine(WrongAnswer(null, gameObjectCorrect));
    }

    IEnumerator WrongAnswer(GameObject gameObject, GameObject gameObjectCorrect)
    {
        yield return StartCoroutine(Feedback(gameObject, false));

        int count = PlayerPrefs.GetInt("errorsCount");

        if (count == 0)
        {
            yield return new WaitForSeconds(0.5f);
            if (SkipTutorial)
            {
                yield return new WaitForSeconds(SoundManager.PlayInstructionsForStep());
            }

            StartCoroutine(ToggleSuggest(gameObjectCorrect, true, false));
        }
        else if (count == 1)
        {
            DisableWrongAnswers(gameObject, gameObjectCorrect);

            StartCoroutine(ToggleSuggest(gameObjectCorrect, true, false));
        }
        else
        {
            StartCoroutine(ToggleSuggest(gameObjectCorrect, false, true));
            StartCoroutine(MoveOn());
        }
        count++;
        PlayerPrefs.SetInt("errorsCount", count);

        int totalErrors;
        System.Int32.TryParse(PlayerPrefs.GetString("totalErrors"), out totalErrors);
        totalErrors = totalErrors + 1;
        PlayerPrefs.SetString("totalErrors", totalErrors.ToString());

        PlayerPrefs.Save();
        Debug.Log("PLAYER ERROR COUNT: " + PlayerPrefs.GetInt("errorsCount"));
    }

    private void DisableWrongAnswers(GameObject gameObject, GameObject gameObjectCorrect)
    {
        string tag;

        switch (questionType)
        {
            case "Draggable":
                tag = "Draggable";
                break;
            default:
                tag = "Selection";
                break;
        }

        GameObject[] allObjects = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> listOfGameObjects = new List<GameObject>(allObjects);
        List<GameObject> cyclingList = new List<GameObject>(allObjects);
        //listOfGameObjects.Remove(gameObject);
        //remove game object not enabled

        string SceneName = SceneManager.GetActiveScene().name;
        //TODO: In marketlist, other elegible GO are normally disabled for gameplay reason.
        if (SceneName != "MarketList") { 
            foreach (GameObject obj in cyclingList)
            {
                BoxCollider2D[] myColliders = obj.GetComponents<BoxCollider2D>();
                foreach (BoxCollider2D bc in myColliders) enabled = bc.enabled;
                if (!enabled)
                {
                    listOfGameObjects.Remove(obj);
                }
            }
        }

        List<GameObject> listOfGameObjectsWrong = new List<GameObject>();
        int destroyCount = 0;
        int objToDestroy = (listOfGameObjects.Count / 2);
        foreach (GameObject obj in listOfGameObjects)
        {
            bool enabled = true;

        //TODO: See upper comment
        if (SceneName != "MarketList")
        {
            BoxCollider2D[] myColliders = obj.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D bc in myColliders) enabled = bc.enabled;
        }

            if (!obj.Equals(gameObjectCorrect) && destroyCount < objToDestroy && enabled)
            {
                listOfGameObjectsWrong.Add(obj);
                destroyCount++;
            }
        }
        //listOfGameObjectsWrong.Add(gameObject);
        foreach (GameObject gameObjectWrong in listOfGameObjectsWrong)
        {

            if (gameObjectWrong.GetComponent<MarketChoice>() != null)
            {
                gameObjectWrong.GetComponent<MarketChoice>().isActive = false;
            }

            DisableAnswer(gameObjectWrong);
        }
    }

    private void DisableAnswer(GameObject gameObjectWrong)
    {
        if (gameObjectWrong.GetComponent<Collider2D>())
            gameObjectWrong.GetComponent<Collider2D>().enabled = false;
        Colors.Disable(gameObjectWrong);
    }

    IEnumerator CorrectAnswer(GameObject gameobject)
    {
        CalculatePoints();

        yield return StartCoroutine(Feedback(gameobject, true));

        int count = PlayerPrefs.GetInt("errorsCount");

        //TODO custom message depending on the scene ( move into specific scene manager?)
        //OR
        //Pass as an argument not only the error count but also the current scene and step
        //The sound manager (or intermediate method) would manage the answer or possible answers
        yield return new WaitForSeconds(SoundManager.Congratulate(count));
        CloseAnswer();
    }

    IEnumerator MoveOn()
    {
        yield return new WaitForSeconds(SoundManager.SoundMoveOn());
        CloseAnswer();
    }

    private void CloseAnswer()
    {
        ResetErrors();
        ToggleBackdrop(false);
        DestroyAnswers();
        Hand.SendMessage("HandInteract", true);
        CheckStepIncrease();
    }

    private void CheckStepIncrease()
    {
        //TODO Better management of questiontype to remove backdrop or textual elements
        //if ("CorrectTextAnswer".Equals(questionType)) {
        //    Destroy(Backdrop);
        //}
        int step;
        //TODO Delegate to single Manager of the scene how to proceed
        switch (SceneManager.GetActiveScene().name)
        {
            case "Briefing":
                step = PlayerPrefs.GetInt("BriefingStep") + stepIncrease;
                PlayerPrefs.SetInt("BriefingStep", step);
                PlayerPrefs.Save();
                Briefing.instance.CheckStep();
                break;
            case "City":
                step = PlayerPrefs.GetInt("CityStep") + stepIncrease;
                PlayerPrefs.SetInt("CityStep", step);
                PlayerPrefs.Save();
                City.instance.CheckStep();
                break;
            case "MarketStand":
                step = PlayerPrefs.GetInt("MarketStandStep") + stepIncrease;
                PlayerPrefs.SetInt("MarketStandStep", step);
                MarketStand.Instance.CheckStep();
                break;

            case "MarketList":
                step = PlayerPrefs.GetInt("MarketListStep") + stepIncrease;
                PlayerPrefs.SetInt("MarketListStep", step);
                PlayerPrefs.Save();
                MarketList.instance.CheckStep();
                break;
            default:
                SceneManager.LoadScene("PrizeReward", LoadSceneMode.Single);
                Debug.Log("Default case");
                break;
        }
    }

    private void CalculatePoints()
    {
        int errorsCount = PlayerPrefs.GetInt("errorsCount");
        if (errorsCount == 0)
        {
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 1);
			ScoreScript.instance.ScoreUpdate();
        }
        PlayerPrefs.Save();
        Debug.Log("PLAYER score: " + PlayerPrefs.GetInt("score"));
    }

    private void ResetErrors()
    {
        int errorsCount = 0;


        PlayerPrefs.SetInt("errorsCount", errorsCount);

        PlayerPrefs.Save();
        Debug.Log("PLAYER errors: " + PlayerPrefs.GetInt("errorsCount"));
    }

    //TODO Custom message depending on scene (to move into specific scene manager?)

    IEnumerator Feedback(GameObject gameobject, bool correct)
    {
        if (gameobject != null)
        {
            ApplyIcon(gameobject, correct);


            if (correct)
            {
                PlayerPrefs.SetString("correct", "1");
                SoundManager.SoundCorrect();
            }
            else
            {
                SoundManager.SoundWrong();
            }

            if (gameobject.tag == "Draggable")
            {
                gameobject.GetComponent<Draggable>().AnswerFeedback(correct);
            }
        }
            yield return new WaitForSeconds(1.0f);
            RemoveIcon();
        
    }

    private void ApplyIcon(GameObject Answer, bool Correct)
    {

        if (Correct)
        {
            IconToApply = Instantiate(rightCheckPrefab);
        }
        else
        {
            IconToApply = Instantiate(wrongCrossPrefab);
        }
        IconToApply.transform.SetParent(GameObject.Find("Canvas").transform, false);

        IconToApply.transform.localScale = new Vector3(1, 1, 0);
        IconToApply.transform.position = Answer.transform.position;
    }

    public void UpgradePlayerLevel()
    {
        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();
        if (playerLevel < 4)
        {
            playerLevel = playerLevel + 1;
            DataManager.SaveUpgradePlayerLevel(PlayerPrefs.GetString("id"), playerLevel.ToString());
        }
    }

    private void RemoveIcon()
    {
        Destroy(IconToApply);
    }

    private void MarketListSuggestions(GameObject gameObjectCorrect, bool reset)
    {
        int errorCount = PlayerPrefs.GetInt("errorsCount");
        if (!SkipTutorial || (SkipTutorial && errorCount >= 1))
        {
            Colors.Suggest(gameObjectCorrect);
            MarketList.instance.SuggestionMoveTo(gameObjectCorrect, reset);
        }
    }

    IEnumerator ToggleSuggest(GameObject gameObjectCorrect, bool reset, bool moveOn)
    {
        //Market List and City Suggestions behaves very differently from others...
        string SceneName = SceneManager.GetActiveScene().name;
        if (SceneName == "MarketList")
        {
            MarketListSuggestions(gameObjectCorrect, reset);
            yield break;
        }
        //else if (SceneName == "City") {
        //    CitySuggestions(gameObjectCorrect, reset);
        //    yield break;
        //}

        Colors.Suggest(gameObjectCorrect);
        if (gameObjectCorrect.tag == "Draggable")
        {
            if (!moveOn)
            {
                yield return new WaitForSeconds(SoundManager.SoundSuggest());
            }
            gameObjectCorrect.GetComponent<Draggable>().SuggestionMoveTo(ElegibleContainer, reset, moveOn);
        }
        else
        {
            if (!moveOn)
            {
                yield return new WaitForSeconds(SoundManager.SoundSuggest());
            }
            Hand.SendMessage("HandInteract", true);
            if (reset)
            {
                Colors.Reset(gameObjectCorrect);
            }
        }
    }

    public void ToggleBackdrop(bool activate)
    {
        if (backdropImg)
        {
            backdropImg.enabled = activate;
        }
    }
}


