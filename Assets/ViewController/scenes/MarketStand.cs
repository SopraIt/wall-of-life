using UnityEngine;
using System.Collections.Generic;
using Model;
using System.Collections;
using UnityEngine.SceneManagement;

public class MarketStand : MonoBehaviour
{
    GameObject moneyBillPrefab;
    GameObject backpack;
    GameObject backpackPrefab;
    GameObject paperbag;
    GameObject paperbagPrefab;
    GameObject wallet;
    GameObject walletPrefab;
    GameObject coins;
	GameObject coinsPrefab;
	GameObject theArm;
	GameObject waveCanvas;
    GameObject objMerchant;

    List<GameObject> listOfGameObj;


    Sprite sprite;
    public static MarketStand Instance;
    int subStep = 1;
    int stepLevel = 1;
    private List<Steps> questions;
    string currentCategory;

    void Awake()
    {
        Instance = this;
        questions = DataManager.GetAnswersOfCurrentScene(SceneManager.GetActiveScene().name);
        moneyBillPrefab = Resources.Load("prefab/MoneyBill") as GameObject;
        backpackPrefab = Resources.Load("prefab/backpack") as GameObject;
        paperbagPrefab = Resources.Load("prefab/Paperbag") as GameObject;
        walletPrefab = Resources.Load("prefab/stand_wallet") as GameObject;
		coinsPrefab = Resources.Load("prefab/coins") as GameObject;

		waveCanvas = GameObject.Find("WaveCanvas");

        gameObject.AddComponent<GameController>();

    }

    void Start()
    {
        if (PlayerPrefs.GetInt("step") != 0)
        {
            stepLevel = PlayerPrefs.GetInt("step");
        }
       currentCategory = PlayerPrefs.GetString("category" + stepLevel);
       PrepareScene(currentCategory);

        CheckStep();
    }

    public void ResetStep() {
        PlayerPrefs.SetInt("MarketStandStep", 0);
        PlayerPrefs.SetInt("step", 0);
        List<string> categories = DataManager.GetClasses();
        int chooseRandomCat = Random.Range(0, categories.Count);

        PlayerPrefs.SetString("category1", categories[chooseRandomCat]);

        SceneManager.LoadScene("MarketStand");
    }

    public void CheckStep()
    {
        int step = PlayerPrefs.GetInt("MarketStandStep");
        switch (step)
        {
            case 0:
                StartCoroutine(AskTheItem());
                break;
            case 1:
                StartCoroutine(TakeTheItem());
                break;
            case 2:
                StartCoroutine(AskHowMuch());
                break;
            case 3:
                StartCoroutine(PayTheMerchant());
                break;
            case 4:
                StartCoroutine(AskTheChange());
                break;
            case 5:
                StartCoroutine(TakeTheChange());
                break;
            case 6:
                StartCoroutine(Goodbye());
                break;
            case 7:
                ExitFromMarket();
                break;
            default:
                Debug.Log("should not happen");
                break;
        }
    }

    void PrepareScene(string currentCategory)
    {
        //TODO: Remove? (See: HandController)
        HandController.instance.HandInteract(false);

        GameObject merchantPrefab = Resources.Load("prefab/MerchantItem") as GameObject;
        GameObject gameObjPrefab = Resources.Load("prefab/Item") as GameObject;
        GameObject standTopPrefab = Resources.Load("prefab/StandTop") as GameObject;

        Classes clas = DataManager.GetClasses(currentCategory);

        GameObject standTop = Instantiate(standTopPrefab);
        standTop.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("stands\\stand_" + clas.name + "_empty");
        Vector3 objViewPosStanTop = GameObject.Find("stand_bottom").transform.position;
        standTop.transform.position = objViewPosStanTop;


        objMerchant = Instantiate(merchantPrefab);
        CreatePrefabMerchant(objMerchant, clas.name);
        Vector3 objViewPosMerchant = GameObject.Find("MerchantSlot").transform.position;
        objMerchant.transform.position = objViewPosMerchant;
        
        List<Item> items = DataManager.GetItemsForClass(clas.name);
        List<Item> listOfItems = new List<Item>();

        listOfGameObj = new List<GameObject>();

        foreach (Item item in items)
        {
            listOfItems.Add(item);
            GameObject gameObject = Instantiate(gameObjPrefab);
            listOfGameObj.Add(CreatePrefab(gameObject, item));

        }
        SelectCasualObjectPosition(listOfGameObj);
        GameController.instance.stepIncrease = 1;
    }


    private GameObject CreatePrefab(GameObject gameObject, Item item)
    {
        sprite = Resources.Load<Sprite>(item.itemImage);

        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        gameObject.name = item.name;

        return gameObject;
    }

    private GameObject CreatePrefabMerchant(GameObject gameObject, string className)
    {
        string merchantSprite = "characters\\merchant_" + className;
        sprite = Resources.Load<Sprite>(merchantSprite);

        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        gameObject.name = "merchant";

        return gameObject;
    }

    private List<GameObject> SelectCasualObjectPosition(List<GameObject> listOfGameObject)
    {
        GameController.ShuffleMe(listOfGameObj);

        List<GameObject> listOfGameObjAndPosition = new List<GameObject>();

        int sortingOrder = 0;
        int itemCount = 1;
        
        foreach (GameObject obj in listOfGameObject)
        {
            string TargetParentString = "ItemSlot" + itemCount;
            GameObject TargetParent = GameObject.Find(TargetParentString);
            RectTransform TargetParentRect = TargetParent.GetComponent<RectTransform>();
            Transform ItemTransform = obj.transform;
            ItemTransform.position = TargetParentRect.position;
            itemCount++;

            obj.GetComponent<SpriteRenderer>().sortingOrder = sortingOrder;
            sortingOrder++;

            listOfGameObjAndPosition.Add(obj);
        }

        return listOfGameObjAndPosition;
    }

    IEnumerator AskTheItem()
    {
        //"And now, point the item you want to buy"
        yield return new WaitForSeconds(SoundManager.PlayInstruction());

        HandController.instance.HandInteract(true);

        GameController.instance.setQuestionType(PlayerPrefs.GetString("CorrectProduct" + stepLevel));
    }

    IEnumerator TakeTheItem()
    {
        //Reset the Market Items
        foreach (GameObject objectToDisable in listOfGameObj) {
            if (objectToDisable.GetComponent<Collider2D>())
                objectToDisable.GetComponent<Collider2D>().enabled = false;
            Colors.Disable(objectToDisable);
            Colors.Deselect(objectToDisable);
        }

        //"Take the item!"
        yield return new WaitForSeconds(SoundManager.PlayInstruction());

        backpack = Instantiate(backpackPrefab);
        backpack.GetComponent<Interactions>().EnterFromRight();
        GameController.MarkContainerAsElegible(backpack);

        paperbag = Instantiate(paperbagPrefab);
        paperbag.name = "paperbag";

        GameController.instance.setQuestionType("Draggable");
        PlayerPrefs.SetString("CorrectObj", "paperbag");
        PlayerPrefs.Save();

        GameController.instance.AnswerCreation("Draggable", true, 1);
    }

    IEnumerator AskHowMuch() {

        if (paperbag)
            paperbag.SendMessage("FadeOut");
		backpack.GetComponent<Interactions>().ExitFromRight(() => Destroy(backpack));
        yield return new WaitForSeconds(SoundManager.PlayInstruction());
        GameController.ActivateObjects("Draggable", false);

        GameController.instance.setQuestionType("question_1");
        GameController.instance.AnswerCreation(questions[0], 1);

    }
    public void SpawnMoneys() {
        GameObject moneyBill = Instantiate(moneyBillPrefab);
        moneyBill.name = "moneybill";
		moneyBill.transform.position = wallet.transform.position;
    }
    IEnumerator PayTheMerchant() {
        subStep = PlayerPrefs.GetInt("step") > 1 ? PlayerPrefs.GetInt("step") : 1;
        int price = PlayerPrefs.GetInt("PriceObj" + subStep);

        yield return new WaitForSeconds(1f);
        yield return new WaitForSeconds(SoundManager.PlayPriceObj(price, currentCategory));
        yield return new WaitForSeconds(1f);

        yield return new WaitForSeconds(SoundManager.PlayInstruction());

        wallet = Instantiate(walletPrefab);
        wallet.GetComponent<Interactions>().EnterFromRight(SpawnMoneys);

        GameController.MarkContainerAsElegible(objMerchant);

        GameController.instance.setQuestionType("Draggable");
        PlayerPrefs.SetString("CorrectObj", "moneybill");
        PlayerPrefs.Save();
        GameController.instance.AnswerCreation("Draggable", true, 1);
    }

    IEnumerator AskTheChange()
    {
        yield return new WaitForSeconds(SoundManager.PlayInstruction());
        GameController.ActivateObjects("Draggable", false);
        GameController.ActivateObjects("DragIn", false);

        GameController.instance.setQuestionType("question_1");
        GameController.instance.AnswerCreation(questions[1], 1);
    }

    IEnumerator TakeTheChange()
    {
        //"Take the change"
        yield return new WaitForSeconds(SoundManager.PlayInstruction());

        coins = Instantiate(coinsPrefab);
        coins.name = "coins";

        GameController.MarkContainerAsElegible(wallet);
            
        GameController.instance.setQuestionType("Draggable");
        PlayerPrefs.SetString("CorrectObj", "coins");
        PlayerPrefs.Save();
        GameController.instance.AnswerCreation("Draggable", true, 1);
    }

    IEnumerator Goodbye()
    {
        wallet.GetComponent<Interactions>().ExitFromRight(() => Destroy(wallet));
        if (KinectManager.instance && KinectManager.instance.IsAvailable)
        {
            //"Wave goodbye to the merchant!"
            yield return new WaitForSeconds(SoundManager.PlayInstruction());
            yield return new WaitForSeconds(1.5f);

            KinectManager.instance.checkWave = true;
            GameController.instance.AnswerCreation("Wave");

        }
        else {
            PlayerPrefs.SetInt("MarketStandStep", 7);
            CheckStep();
        }
        //WaveTutorial() called by PlayInstruction
        //I know that's at least semanthically if not logically unorthodox, we should move PlayInstruction in GameControllerMaybe.
    }

	public void WaveTutorial(){
		HandController.instance.HandInteract(false);
		System.Action Callback = ()=> HandController.instance.HandInteract(true);
		waveCanvas.transform.Find("WaveBackdrop").gameObject.SetActive(true);

		if (!theArm) {
			theArm = GameObject.Find("theArm");
		}
		theArm.GetComponent<Interactions>().WavingStart(Callback);

    }

    private int GetNewPrice(int prevPrice)
    {
        int newPrice;

        newPrice = Random.Range(1, 10);
        while (newPrice == prevPrice)
        {
            newPrice = Random.Range(1, 10);
        }

        return newPrice;

    }

    private void ExitFromMarket() {
        //Check if other items are to be bought
        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();
        if (stepLevel < playerLevel)
        {
            stepLevel++;
            PlayerPrefs.SetInt("step", stepLevel);
            PlayerPrefs.SetInt("MarketStandStep", 0);
            SceneManager.LoadScene("MarketList", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Briefing", LoadSceneMode.Single);
        }
    }

}