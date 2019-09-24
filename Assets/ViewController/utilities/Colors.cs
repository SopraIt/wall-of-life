using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Colors : MonoBehaviour {

    static GameObject HaloPrefab = Resources.Load("prefab/Halo") as GameObject;
    static GameObject HaloUIPrefab = Resources.Load("prefab/HaloUI") as GameObject;
    //static GameObject Halo;

    public static Color HaloYellow = new Color32(255, 248, 148, 200);
    public static Color HaloGreen = new Color32(58, 255, 77, 200);
    public static Color HaloRed = new Color32(255, 0, 0, 200);
    public static Color HaloBlue = new Color32(150, 190, 255, 200);
    public static Color Disabled = new Color32(100, 100, 100, 255);
    public static Color Unreachable = new Color32(184, 184, 184, 255);
    public static Color Normal = new Color32(255, 255, 255, 255);
    public static Color TextDisabled = new Color32(0, 0, 0, 20);
    public static Color Backdrop = new Color32(0, 0, 0, 100);

    public static Sprite ButtonRed = Resources.Load<Sprite>("_ui/UI_Button_Standard_Red");
    public static Sprite ButtonGreen = Resources.Load<Sprite>("_ui/UI_Button_Standard_Green");
    public static Sprite ButtonBlue = Resources.Load<Sprite>("_ui/UI_Button_Standard_Sky");



    public static SpriteRenderer Highlight(GameObject currentGameObject)
    {
        if (currentGameObject.tag == "UI_selection")
        {
            GameObject HaloUI = Instantiate(HaloUIPrefab);
            HaloUI.transform.SetParent(currentGameObject.transform, false);
            HaloUI.transform.SetSiblingIndex(0);
            return null;
        }

        GameObject Halo = Instantiate(HaloPrefab);
        Halo.transform.SetParent(currentGameObject.transform, false);
		Halo.transform.SetSiblingIndex(0);


        SpriteRenderer SpriteRenderer = currentGameObject.GetComponent<SpriteRenderer>();
        Image ImageRenderer = currentGameObject.GetComponent<Image>();
        SpriteRenderer HaloRenderer = Halo.GetComponent<SpriteRenderer>();
        HaloRenderer.material.SetColor("_Color", HaloYellow);

        if (SpriteRenderer)
        {
            HighlightObject(HaloRenderer, SpriteRenderer);
        }
        else if (ImageRenderer)
        {
            HighlightUI(HaloRenderer, currentGameObject);
        }
		return HaloRenderer;
    }

	public static void Highlight(GameObject currentGameObject, Color32 targetColor){
		Highlight(currentGameObject).material.SetColor ("_Color", targetColor);
	}
	public static void Highlight(GameObject currentGameObject, Color32 targetColor, float resetTime){
		Highlight(currentGameObject, targetColor);
		GameController.instance.StartCoroutine(DeselectInTime(currentGameObject, resetTime));
	}

    static void HighlightObject(SpriteRenderer HaloRenderer, SpriteRenderer ParentRenderer)
    {
        HaloRenderer.size = ParentRenderer.size * 2.0f;
        HaloRenderer.sortingLayerName = ParentRenderer.sortingLayerName;
        HaloRenderer.sortingOrder = ParentRenderer.sortingOrder - 1;
    }

    static void HighlightUI(SpriteRenderer HaloRenderer, GameObject ParentObject)
    {
        BoxCollider2D ParentRenderer = ParentObject.GetComponent<BoxCollider2D>();
        HaloRenderer.size = ParentRenderer.size * 2.0f;
        HaloRenderer.sortingLayerName = "UI";
    }

    public static void ChangeHaloColor(GameObject gameObject, Color32 targetColor) {
        Transform currentGameObjectTransform = gameObject.GetComponent<Transform>();
        int numberOfChildren = currentGameObjectTransform.childCount;
        if (numberOfChildren >= 1)
        {
            foreach (Transform child in currentGameObjectTransform)
            {
                if (child.tag == "Halo") {
                    SpriteRenderer HaloRenderer = child.GetComponent<SpriteRenderer>();
                    HaloRenderer.material.SetColor("_Color", targetColor);
                }

            }

        }
    }

	public static IEnumerator DeselectInTime(GameObject Object, float resetTime){
		yield return new WaitForSeconds(resetTime);
		Deselect(Object);
	}

    public static void Deselect(GameObject currentGameObject) {
        //TODO Can this be improved?
        Transform currentGameObjectTransform = currentGameObject.GetComponent<Transform>();
        int numberOfChildren = currentGameObjectTransform.childCount;
        if (numberOfChildren >= 1) {

            foreach (Transform child in currentGameObjectTransform)
            {
                if (child.tag == "Halo")
                {
                    Destroy(child.gameObject);
					//return;
                }
            }
        }
    }

    public static void Suggest(GameObject gameObject) {
        SpriteRenderer objectRenderer = gameObject.GetComponent<SpriteRenderer>();
        Image objectImage = gameObject.GetComponent<Image>();

        if (objectRenderer)
        {
			Debug.Log("Suggesting");
			Debug.Log(gameObject);
            Highlight(gameObject);
            ChangeHaloColor(gameObject, HaloGreen);
        }
        else if (objectImage)
        {
            objectImage.sprite = ButtonGreen;

        }
    }

    public static void Reset(GameObject gameObject)
    {
        SpriteRenderer objectRenderer = gameObject.GetComponent<SpriteRenderer>();
        Image objectImage = gameObject.GetComponent<Image>();

        if (objectRenderer)
        {
            Deselect(gameObject);
        }
        else if (objectImage)
        {
            objectImage.sprite = ButtonBlue;

        }
    }

    public static void Disable(GameObject gameObject) {
        SpriteRenderer objectRenderer = gameObject.GetComponent<SpriteRenderer>();
        Image objectImage = gameObject.GetComponent<Image>();

        if (objectRenderer)
        {
            objectRenderer.material.SetColor("_Color", Disabled);
        }
        else if (objectImage){
            Text buttonText = gameObject.GetComponentInChildren<Text>();

            objectImage.color = Disabled;
            buttonText.color = TextDisabled;
        }
    }
}
