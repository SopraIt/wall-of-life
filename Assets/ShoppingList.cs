using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingList : MonoBehaviour {

    GameObject Hand;
    GameObject Tutor;
    SpriteRenderer TutorRenderer;
    GameObject tutorPrefab;
    string TutorLayerNameDefault;
    int TutorLayerOrderDefault;
    bool TutorMissing = false;

    // Use this for initialization
    void Awake () {
        Hand = GameObject.Find("Hand");

        Tutor = GameObject.Find("Tutor");

    }

    public void ShoppingListBtn()
    {
        StartCoroutine(ItemsToBuy());
    }

    void SetTutor() {

        if (!Tutor)
        {
            tutorPrefab = Resources.Load("prefab/Tutor") as GameObject;
            Tutor = Instantiate(tutorPrefab);
            TutorMissing = true;
        }

        if (TutorMissing)
        {
            Tutor.SendMessage("FadeIn");
        }

        TutorRenderer = Tutor.GetComponent<SpriteRenderer>();
        TutorLayerNameDefault = TutorRenderer.sortingLayerName;
        TutorLayerOrderDefault = TutorRenderer.sortingOrder;
    }

    private IEnumerator ItemsToBuy()
    {
        GameController.instance.ToggleBackdrop(true);
        Hand.SendMessage("HandInteract", false);

        SetTutor();
        
        TutorRenderer.sortingLayerName = "UI";
        TutorRenderer.sortingOrder = 20;
        int stepLevel = 1;

        if (PlayerPrefs.GetInt("step") != 0)
        {
            stepLevel = PlayerPrefs.GetInt("step");
        }

        yield return new WaitForSeconds(SoundManager.ShoppingListStart());

        int playerLevel = SetUpPlayerPrefs.SetUpPlayerPrefs.GetPlayerLevel();

        for (int i = stepLevel; i <= playerLevel; i++)
        {
			if (i == stepLevel && stepLevel != playerLevel){
				yield return new WaitForSeconds(SoundManager.PlayItemAudio("00_first"));
			}
            string currentChoosenItem = PlayerPrefs.GetString("CorrectProduct" + i);
            yield return new WaitForSeconds(SoundManager.PlayItemAudio(currentChoosenItem));
            yield return new WaitForSeconds(0.25f);
			if (stepLevel != playerLevel){
				yield return new WaitForSeconds(SoundManager.PlayItemAudio("00_then"));
			}
			yield return new WaitForSeconds(0.25f);
        }

        GameController.instance.ToggleBackdrop(false);
        TutorRenderer.sortingLayerName = TutorLayerNameDefault;
        TutorRenderer.sortingOrder = TutorLayerOrderDefault;

        if (TutorMissing)
        {
            Tutor.SendMessage("FadeOut");
        }

        Hand.SendMessage("HandInteract", true);

    }
}
