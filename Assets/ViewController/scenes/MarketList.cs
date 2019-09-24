using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MarketList : MonoBehaviour
{
    GameObject[] marketStands;

    public static MarketList instance;
    GameObject ShoppingList;
    const string pathToVoice = "voices/tutor/marketList/";
    const string pathToCategory = "voices/tutor/categories/";

    const int times = 2;
    int stepLevel = 1;
    bool panningCamera = false;
    Vector2 cameraPosition;
    Vector2 targetPosition;

    private void Awake()
    {
        PlayerPrefs.SetInt("MarketListStep", 0);
        ShoppingList = GameObject.Find("ShoppingList");
        instance = this;
        marketStands = GameObject.FindGameObjectsWithTag("Selection");
        cameraPosition = Camera.main.transform.position;
    }

    void Start()
    {
		instance = this;
		CheckStep();
    }

	public void ResetAll(){
		PlayerPrefs.SetInt("MarketListStep", 0);
		PlayerPrefs.SetString("category1", "fruit");
		PlayerPrefs.SetInt("errorsCount", 0);
		PlayerPrefs.SetInt("errorsCount", 0);

        PlayerPrefs.Save();
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}

    public void CheckStep()
    {
        int step = PlayerPrefs.GetInt("MarketListStep");
        switch (step)
        {
            case 0:
                PrepareScene();
                break;
            case 1:
                SceneManager.LoadScene("MarketStand", LoadSceneMode.Single);
                break;
            default:
                Debug.Log("Default case");
                break;
        }
    }

    private void PrepareScene() {

        GameController.ToggleObject(ShoppingList, true);
        if (PlayerPrefs.GetInt("step") != 0)
        {
            stepLevel = PlayerPrefs.GetInt("step");
        }
        string rightStand = PlayerPrefs.GetString("category" + stepLevel);
        
        GameController.instance.setQuestionType(rightStand);
        GameController.instance.AnswerCreation("Selection", false, 1);

        if (!GameController.instance.SkipTutorial)
        {
            StartCoroutine(Tutorial());
        }
        else {
            HandController.instance.HandInteract(true);
            foreach (GameObject obj in marketStands)
            {
                obj.GetComponent<MarketChoice>().Toggle(false);
            }
        }
    }

    public float StartTutorial() {
        StartCoroutine(Tutorial());
        return 0;
    }

    IEnumerator Tutorial()
    {
        HandController.instance.HandInteract(false);
        //"Let's find the stand!"
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "01_searchTheStand"));
		string FirstStand = PlayerPrefs.GetString ("category" + stepLevel);

		GameObject targetGo = GameObject.Find(FirstStand);

        List<GameObject> listOfGameObj = new List<GameObject>(marketStands);
        listOfGameObj.Remove(targetGo);
        
        foreach (GameObject obj in marketStands) {
            obj.GetComponent<MarketChoice>().enabled = false;
            obj.GetComponent<MarketChoice>().Toggle(true);
        }

        for (int i = 0; i < times; i++)
        {
            int r = Random.Range(0, listOfGameObj.Count);
            GameObject wrongStand = listOfGameObj[r];
            yield return StartCoroutine(MoveCameraTo(wrongStand));
            Colors.Highlight(wrongStand);
            //"That's not the correct one since it sells *clothes*"
            yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "02_notThisOne"));
            Debug.Log("wrongStand.name: " + wrongStand.name);
            yield return new WaitForSeconds(SoundManager.Play(pathToVoice + wrongStand.name));
            Colors.Deselect(wrongStand);
            listOfGameObj.Remove(wrongStand);
        }

        yield return StartCoroutine(MoveCameraTo(targetGo));

        //"Instead, we have to buy *fruit*!"
        Colors.Highlight(targetGo, Colors.HaloGreen);

        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "03_weNeedToBuy"));
        yield return new WaitForSeconds(SoundManager.Play(pathToVoice + targetGo.name));

        Colors.Deselect(targetGo);

        yield return StartCoroutine(ResetCamera());

		if( !GameController.instance.SkipTutorial ){
			yield return new WaitForSeconds(SoundManager.Play(pathToVoice + "00_tutorial"));
		}

        foreach (GameObject obj in marketStands)
        {        
            obj.GetComponent<MarketChoice>().enabled = true;
            obj.GetComponent<MarketChoice>().selectable = true;

        }
        HandController.instance.HandInteract(true);
    }

    public void SuggestionMoveTo(GameObject correctObj, bool reset){
		StartCoroutine(Suggest(correctObj, reset));
	}

	IEnumerator Suggest(GameObject correctObj, bool reset)
	{
		yield return StartCoroutine(MoveCameraTo(correctObj));
		//"That's the right stand!"
		yield return new WaitForSeconds(SoundManager.SoundSuggest());
		HandController.instance.HandInteract(true);

		if (reset)
			Colors.Deselect (correctObj);
		
		yield return StartCoroutine(ResetCamera());
	}

    IEnumerator MoveCameraTo(GameObject targetObj)
    {
        targetPosition = new Vector2(targetObj.transform.position.x, cameraPosition.y);
        panningCamera = true;
        yield return new WaitWhile(() => panningCamera);
    }
    IEnumerator ResetCamera()
    {
        targetPosition = new Vector2(0.0f, cameraPosition.y);
        panningCamera = true;
        yield return new WaitWhile(() => panningCamera);
    }

    void panCamera()
    {

        Vector2 cameraPosition = Camera.main.transform.position;
        Vector2 startingPosition = cameraPosition;

        float step = 20.0f * Time.deltaTime;

        cameraPosition = Vector2.MoveTowards(startingPosition, targetPosition, step);
        Camera.main.transform.position = cameraPosition;

        if (cameraPosition == targetPosition)
        {
            panningCamera = false;
        }
    }

    void FixedUpdate()
    {
        if (panningCamera)
        {
            panCamera();
        }
    }
}