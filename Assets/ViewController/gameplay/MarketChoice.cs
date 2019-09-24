using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketChoice : MonoBehaviour {

    public Camera myCam;
    public bool selectable = false;
    public bool isActive;

    // Use this for initialization
    void Start () {
        if (myCam == null)
        {
            myCam = Camera.main;
        }
        this.isActive = true;
    }
	
    void SelectabileOn()
    {
        if (!selectable) {
            //MarketRenderer.material.SetColor("_Color", Color.green);
            Toggle(true);
            gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Color", Colors.Normal);
            selectable = true;
            gameObject.GetComponent<BoxCollider2D>().enabled = true ;
        }
    }

    void SelectabileOff()
    {
        if (selectable)
        {
            // MarketRenderer.material.SetColor("_Color", Color.white);
            Toggle(false);
            selectable = false;
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void Toggle(bool boolean) {
        Color32 color = boolean ? Colors.Normal : Colors.Unreachable;
        gameObject.GetComponent<SpriteRenderer>().material.SetColor("_Color", color);
    }


    void ActiveSelectable ()
    {
        Vector3 centerPos = myCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        float centerPosMax = centerPos.x + 3.5f;
        float centerPosMin = centerPos.x - 3.5f;
        float thisPos = this.transform.position.x;
        if (thisPos < centerPosMax && thisPos > centerPosMin)
        {
            SelectabileOn();
        }
        else
        {
            SelectabileOff();
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
       if (isActive) { 
            ActiveSelectable();
       }
    }
}
