using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splash : MonoBehaviour {

    private GameObject Canvas;
    private CanvasGroup CanvasGroup;
    private Renderer PanelRenderer;
    bool FadeOver = false;
    bool CountDownStarted = false;

    // Use this for initialization
    void Start() {
        Canvas = GameObject.Find("CanvasGroup");
        if (Canvas != null) {
            CanvasGroup = Canvas.GetComponent<CanvasGroup>();
        }
    }

    // Update is called once per frame
    void Update() {
        if (!FadeOver)
        {
            IncreaseOpacity();
        }
        else if (!CountDownStarted) {
            CountDownStarted = true;
            StartCoroutine("CountDown");
        }

    }

    void IncreaseOpacity() {
        if (CanvasGroup.alpha >= 1)
        {
            FadeOver = true;
        }
        CanvasGroup.alpha += Time.deltaTime / 2;
    }

    void DecreaseOpacity() {
        if (CanvasGroup.alpha >= 1)
        {
            FadeOver = true;
        }
        CanvasGroup.alpha += Time.deltaTime / 2;
    }

    IEnumerator CountDown() {
        yield return new WaitForSecondsRealtime(3);
        while (CanvasGroup.alpha > 0) {
            CanvasGroup.alpha -= Time.deltaTime / 1;
            yield return null;
        }
        if (KinectManager.instance.IsAvailable)
        {
            Initiate.Fade("Setup", new Color32(0, 0, 0, 255), 0.5f, 0.5f);
        }
        else {
            Initiate.Fade("Login", new Color32(0, 0, 0, 255), 0.5f, 0.5f);
        }
    }
}
