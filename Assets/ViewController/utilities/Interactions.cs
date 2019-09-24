using System.Collections;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interactions : MonoBehaviour
{
    //float secondsCount;
    //private static GameObject Hand;
	bool moving = false;
	Vector2 StartingPoint;
	Vector2 ArrivalPoint;
	float speed;

	bool rotating = false;
	Vector3 axis;
	int waves = 0;
	int maxWaves = 6;

    GameController gameController;
    Action Callback;

    // publically editable speed
    public float fadeDelay = 0.0f;
    public float fadeTime = 0.5f;
    public bool fadeInOnStart = false;
    public bool fadeOutOnStart = false;
    public bool fadeOutDestroy = true;
    private bool logInitialFadeSequence = false;
    // store colours
    private Color[] colors;
    
    // Use this for initialization
    IEnumerator Start()
    {
        yield return null; 
        //yield return new WaitForSeconds(fadeDelay);

        if (fadeInOnStart)
        {
            logInitialFadeSequence = true;
            FadeIn();
        }

        if (fadeOutOnStart)
        {
            FadeOut(fadeTime);
        }
    }

    //utils
    public static void SelectObject(GameObject gameObject)
    {
        if (SceneManager.GetActiveScene().name.Equals("Login") || SceneManager.GetActiveScene().name.Equals("PrizeSelect"))
        {
            gameObject.GetComponent<Button>().onClick.Invoke();
        }
        else
        {
            GameController.instance.CheckAnswer(gameObject);
        }
    }

    public static void SelectUIObject(GameObject gameObject)
    {
        gameObject.GetComponent<Button>().onClick.Invoke();
    }

    public static void AnimationEnter(GameObject gameobject) {
        gameobject.GetComponent<Animator>().SetTrigger("DragIn");
    }

    public void EnterFromRight(Action callback)
    {
        Callback = callback;
        EnterFromRight();
    }

    public void EnterFromRight(){

		//TODO: this is really sloppy code, consider using proper animations.
		Vector2 edgeOfScreen = Camera.main.ViewportToWorldPoint(new Vector2(1.0f, 0.05f));
		Renderer thisRend = gameObject.GetComponent<Renderer>();
		float thisWidth = thisRend.bounds.extents.x;
		float thisHeight = thisRend.bounds.extents.y;

        Vector2 SpawnPoint = new Vector2
        {
            x = edgeOfScreen.x + thisWidth,
            y = edgeOfScreen.y + thisHeight
        };

        ArrivalPoint = new Vector2
        {
            x = edgeOfScreen.x - (thisWidth * 1.3f),
            y = SpawnPoint.y
        };

        transform.localPosition = SpawnPoint;

		speed = 10.0f;

		moving = true;
	}

    public void ExitFromRight(Action callback)
    {
        Callback = callback;
        ExitFromRight();
    }

    public void ExitFromRight(){
		Vector2 edgeOfScreen = Camera.main.ViewportToWorldPoint(new Vector2(1.0f, 0.05f));
		Renderer thisRend = gameObject.GetComponent<Renderer>();
		float thisWidth = thisRend.bounds.extents.x;
		float thisHeight = thisRend.bounds.extents.y;

        ArrivalPoint = new Vector2
        {
            x = edgeOfScreen.x + thisWidth,
            y = edgeOfScreen.y + thisHeight
        };

        speed = 10.0f;

		moving = true;
	}

    //********************FadeInOut****************************//
    // check the alpha value of most opaque object
    float MaxAlpha()
    {
        float maxAlpha = 0.0f;
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        foreach (Renderer item in rendererObjects)
        {
            maxAlpha = Mathf.Max(maxAlpha, item.material.color.a);
        }
        return maxAlpha;
    }

    // fade sequence
    IEnumerator FadeSequence(float fadingOutTime)
    {
        // log fading direction, then precalculate fading speed as a multiplier
        bool fadingOut = (fadingOutTime < 0.0f);
        float fadingOutSpeed = 1.0f / fadingOutTime;

        // grab all child objects
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer>();
        if (colors == null)
        {
            //create a cache of colors if necessary
            colors = new Color[rendererObjects.Length];

            // store the original colours for all child objects
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                colors[i] = rendererObjects[i].material.color;
            }
        }

        // make all objects visible
        for (int i = 0; i < rendererObjects.Length; i++)
        {
            rendererObjects[i].enabled = true;
        }

        // get current max alpha
        float alphaValue = MaxAlpha();

        // This is a special case for objects that are set to fade in on start. 
        // it will treat them as alpha 0, despite them not being so. 
        if (logInitialFadeSequence && !fadingOut)
        {
            alphaValue = 0.0f;
            logInitialFadeSequence = false;
        }

        // iterate to change alpha value 
        while ((alphaValue >= 0.0f && fadingOut) || (alphaValue <= 1.0f && !fadingOut))
        {
            alphaValue += Time.deltaTime * fadingOutSpeed;

            for (int i = 0; i < rendererObjects.Length; i++)
            {
                Color newColor = (colors != null ? colors[i] : rendererObjects[i].material.color);
                newColor.a = Mathf.Min(newColor.a, alphaValue);
                newColor.a = Mathf.Clamp(newColor.a, 0.0f, 1.0f);
                rendererObjects[i].material.SetColor("_Color", newColor);
            }

            yield return null;
        }

        // turn objects off after fading out
        if (fadingOut)
        {
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                rendererObjects[i].enabled = false;
            }
            if (fadeOutDestroy)
                Destroy(gameObject);
        }
        
    }

    void FadeIn()
    {
        FadeIn(fadeTime);
    }

    void FadeOut()
    {
        FadeOut(fadeTime);
    }

    void FadeIn(float newFadeTime)
    {
        StopAllCoroutines();
        StartCoroutine("FadeSequence", newFadeTime);
    }

    void FadeOut(float newFadeTime)
    {
        StopAllCoroutines();
        StartCoroutine("FadeSequence", -newFadeTime);
    }

    //********************FadeInOut****************************//

    public void WavingStart(Action callback){
		Callback = callback;
		WavingStart();
	}

	public void WavingStart(){
		rotating = true;
		waves = 0;
		axis = new Vector3(0,0,1);
	}

	void Waving(){
		Transform thisTransf = gameObject.transform;
		Vector3 point = thisTransf.position;
		thisTransf.RotateAround(point, axis, Time.deltaTime * 100);
	}

    // Update is called once per frame
    void Update()
    {
		if (moving) {
			float step = speed * Time.deltaTime;

			transform.localPosition = Vector2.MoveTowards (transform.position, ArrivalPoint, step);

			if (transform.position.x == ArrivalPoint.x) {
                if (Callback != null) {
                    Callback();
                }
                moving = false;
			}
		}
		if (rotating) {
			Waving();
			if (transform.rotation.z >= 0.6f) {
				axis = new Vector3(0,0,-1);
				waves++;
			}else if (transform.rotation.z <= 0.0f){
				axis = new Vector3(0,0,1);
				waves++;
			}
			if (waves >= maxWaves) {
				Debug.Log (Callback);

				rotating = false;
				if (Callback != null) {
					Callback();
					}
				GameObject.Find("WaveBackdrop").SetActive(false);
				}
			}
		} 
}