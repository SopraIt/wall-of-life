using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Speech : MonoBehaviour
{
    //public static Speech instance;
    //static GameObject DialogueBubblePrefab = Resources.Load("prefab/dialogue_bubble") as GameObject;
    static GameObject DialogueBubble;

    static GameObject SpeechCanvasPrefab = Resources.Load("prefab/SpeechCanvas") as GameObject;
    static GameObject SpeechCanvas;

    static GameObject TutorImgPrefab = Resources.Load("prefab/TutorImg") as GameObject;
    static GameObject TutorImg;

    public static void ShowDialogue(string dialogue)
    {
        if (!SpeechCanvas) {
            SpeechCanvas = Instantiate(SpeechCanvasPrefab);
        }
       SpeechCanvas.transform.GetComponentInChildren<Text>().text = dialogue;
    }

    public static void ShowTutorDialogue(string dialogue) {

        if (!TutorImg)
        {
            TutorImg = Instantiate(TutorImgPrefab);
            TutorImg.transform.SetParent(GameObject.Find("TutorCanvas").GetComponent<Transform>());
        }
        ShowDialogue(dialogue);

    }

    static public void RemoveBubble() {
        Destroy(SpeechCanvas);

        TutorImg = GameObject.Find("TutorImg");
        if (TutorImg) {
            Destroy(TutorImg);
        }
    }
}
