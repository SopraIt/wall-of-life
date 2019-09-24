using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Model;
using System;
using System.Collections;
using System.IO;

public class Login : MonoBehaviour {
    public GameObject buttonPrefab;
    public Button buttonUp, buttonDown;
    public GameObject panelToAttachButtonsTo;
    public Camera cam;
    private int page = 0;
    private int currentPage = 1;
    private int userPerRow = 3;
    public Text pagination;

    private FileInfo[] allFiles;
    private String publicDir;

    private Vector3 normalScale = new Vector3(1, 1, 0);

    private void Awake()
    {
        publicDir = Application.streamingAssetsPath + "/Images/login-profile/";
    }

    IEnumerator LoadPlayerUI(GameObject button, FileInfo playerFile)
    {
        if (playerFile.Name.Contains("meta"))
        {
            yield break;
        }
        else
        {
            string wwwPlayerFilePath = "file://" + playerFile.FullName.ToString();
            WWW www = new WWW(wwwPlayerFilePath);
            yield return www;
            button.transform.Find("Image").GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
        }
    }


    void Start()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(publicDir);
        allFiles = directoryInfo.GetFiles("*.*");


        SetUpPlayerPrefs.SetUpPlayerPrefs.ResetPlayer();
        LoadPlayers(page, currentPage);
        pagination.text = page != 0 ? page.ToString() : "";
       
        if (page == 0)
        {
            buttonUp.image.enabled = false;
            buttonUp.interactable = false;
        }

        buttonUp.onClick.AddListener(() => TaskOnClick(false));
        buttonDown.onClick.AddListener(() => TaskOnClick(true));
    }

    void LoadPlayers(int page, int currentPage)
    {
        int indexStartOfPlayers = page * 6;
        int indexMaxOfPlayers = currentPage * 6;
        pagination.text = page != 0 ? page.ToString() : "";

        float xPosition = 0;
        float yPosition = 0.65f;
        float zPosition = 1;
        float index = 0.25f;// (1 / (userPerRow + 1));
        List<Person> people = DataManager.GetPeople();
           

        buttonUp.image.enabled = true;
        buttonUp.GetComponent<Collider2D>().enabled = true;
        buttonDown.image.enabled = true;
        buttonDown.GetComponent<Collider2D>().enabled = true;
        if (page == 0)
        {
            buttonUp.image.enabled = false;
            buttonUp.GetComponent<Collider2D>().enabled = false;
        }
        if (currentPage > people.Count)
        {
            buttonDown.image.enabled = true;
            buttonDown.GetComponent<Collider2D>().enabled = true;
        }

        
        for (int i = indexStartOfPlayers; i < indexMaxOfPlayers; i++)
        {
            Person person;
            try
            {
                person = people[i];
            } catch (ArgumentOutOfRangeException)
            {
                buttonDown.image.enabled = false;
                buttonDown.GetComponent<Collider2D>().enabled = false;
                return;
            }

    
         /* Position start */
            
            GameObject button = (GameObject)Instantiate(buttonPrefab);

            if ((xPosition + index) >= 1)
            {
                xPosition = 0;
            }
            xPosition += index;

            yPosition = 0.65f;
            if (i >= (userPerRow + indexStartOfPlayers))
            {
                yPosition = 0.3f;
            }

            Vector3 buttonViewPos = new Vector3(xPosition, yPosition, zPosition);

            Vector3 buttonPosition = cam.ViewportToWorldPoint(buttonViewPos);

            /* Position end */

            /* Image start */

            button.GetComponentInChildren<Text>().text = person.personName;
            foreach (FileInfo file in allFiles)
            {
                if (file.Name.Contains(person.profileImgUrl))
                {
                    StartCoroutine(LoadPlayerUI( button, file));
                }
            }

            button.transform.position = buttonPosition;
            button.transform.SetParent(panelToAttachButtonsTo.transform);//Setting button parent
            button.transform.localScale = normalScale;
            button.GetComponent<Button>().onClick.AddListener(delegate { OnClick(person); });
        }
    }
    void TaskOnClick(bool up)
    {
        GameObject[] GameObjects = (GameObject.FindGameObjectsWithTag("UI_selection")) ;
        for (int i = 0; i < GameObjects.Length; i++)
        {
            if (GameObjects[i].name.Equals("Button_UI_selection(Clone)")) {
                Destroy(GameObjects[i]);
            }
        }
        if (up)
        {
            currentPage++;
            page++;
            LoadPlayers(page, currentPage);
        }
        else
        {
            currentPage--;
            page--;
            LoadPlayers(page, currentPage);
        }
    }

    void LoadNextPlayers ()
    {
        LoadPlayers(page, currentPage);
    }

    void OnClick(Person person)
    {
        SoundManager.SoundCorrect();
        PlayerPrefs.SetFloat("playTime", Time.time);
        Debug.Log("PERSON ID : " + person.id);
        SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerFromPerson(person);
        KinectManager.instance.UpdateMainHand();
        Initiate.Fade("Briefing", new Color32(0, 0, 0, 255), 5.0f, 0.5f);
        //SceneManager.LoadScene("Scenes/Briefing", LoadSceneMode.Single);
    }

    public void ResetUsers() {
        DataManager.RollbackDB();
        SceneManager.LoadScene("Scenes/Login", LoadSceneMode.Single);
    }
    
}
