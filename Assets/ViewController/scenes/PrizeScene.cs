using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Model;

public class PrizeScene : MonoBehaviour {
    public GameObject buttonPrefab;
    public GameObject panelToAttachButtonsTo;
    public Camera cam;

    private int prizePerRow = 3;
    private Vector3 normalScale = new Vector3(1, 1, 0);

    // Use this for initialization
    void Start () {
        float xPosition = 0;
        float yPosition = 0.65f;
        float zPosition = 1;
        float index = 0.25f;// (1 / (userPerRow + 1));

        //List<Vector3> positions = getPosition();

        List<Prize> prizes = DataManager.GetPrizes();
        Prize consolationPrize = prizes.Find(x => x.id.Equals("7"));
        PlayerPrefs.SetString("consolationPrizeType", consolationPrize.prizeType);
        PlayerPrefs.SetString("consolationPrizeSource", consolationPrize.source);

        prizes.RemoveRange(6, 1);

        int i = 0;

        foreach (Prize prize in prizes)
        {
            /* Position start */
            GameObject button = Instantiate(buttonPrefab);

            if ((xPosition + index) >= 1)
            {
                xPosition = 0;
            }
            xPosition += index;
            
            if (i >= prizePerRow)
            {
                yPosition = 0.22f;
            }

            Vector3 buttonViewPos = new Vector3(xPosition, yPosition, zPosition);
            Vector3 buttonPosition = cam.ViewportToWorldPoint(buttonViewPos);            
            /* Position end */

            /* Image start */
            Sprite sprite = Resources.Load<Sprite>(prize.prizeImg);
            button.transform.Find("Image").GetComponent<Image>().sprite = sprite;
            button.transform.position = buttonPosition;
            button.transform.SetParent(panelToAttachButtonsTo.transform);//Setting button parent
            button.transform.localScale = normalScale;
            button.name = prize.prizeType;
            button.GetComponentInChildren<Text>().text = prize.description;
            button.GetComponent<Button>().onClick.AddListener(delegate { OnClick(prize.prizeType, prize.source); });//Setting what button does when clicked
            /* Image end */
            i++;
        }
    }

    void OnClick(string prizeType, string prizeSource)
    {
		SoundManager.SoundCorrect();

        /* set user data */
        PlayerPrefs.SetString("prizeType", prizeType);
        PlayerPrefs.SetString("prizeSource", prizeSource);
        PlayerPrefs.Save();
        Debug.Log("prizeType: " + prizeType);
        Debug.Log("prizeSource: " + prizeSource);

        SceneManager.LoadScene("Scenes/Briefing", LoadSceneMode.Single);
    }
}
