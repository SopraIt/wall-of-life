using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Windows.Kinect;
﻿
public class Draggable : MonoBehaviour {

	public bool correctAnswer = false;
	private bool suggestionMoving = false;
	private bool resetHalo = false;
    private bool moveOn = false;
    private bool dragging = false;
    public bool dragIn = false;
	public bool draggable = false;
    //private bool colliding = false;
    private int startingOrder;
    private string startingLayer;
    private GameObject Hand;
    public GameObject otherGameObject;
    private float handOpenCounter = 0.0f;

    public Vector2 originalPos;
	private Vector3 ArrivalPoint;

    private SpriteRenderer ThisRenderer;
    private Collider2D ThisCollider;
    private Rigidbody2D ThisBody;

    private void Awake()
    {
        ThisRenderer = gameObject.GetComponent<SpriteRenderer>();

        startingOrder = ThisRenderer.sortingOrder;
        startingLayer = ThisRenderer.sortingLayerName;
        ThisCollider = gameObject.GetComponent<Collider2D> ();
        ThisBody = GetComponent<Rigidbody2D>();
    }
    // Use this for initialization
    void Start()
    {
        originalPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);
        Hand = GameObject.Find("Hand");
    }

	bool isDraggable(){
		return gameObject.Equals(HandController.instance.CurrentGameObject);
	}

	void KinectDraggingCheck() {

        if (!KinectManager.instance || !KinectManager.instance.IsAvailable)
        {
            return;
        }
        else if (isDraggable() && !HandController.instance.isDragging)
        {
            if (KinectManager.instance.HandOpen)
            {
                handOpenCounter++;
            }
            else if (KinectManager.instance.HandClosed && handOpenCounter > 0)
            {
                handOpenCounter = 0;
                StartDragging();
            }
        }
        else if (KinectManager.instance.HandOpen && dragging)
        {
            handOpenCounter++;
            if (handOpenCounter > 2.5)
            {
                handOpenCounter = 0;

                StopDraggingObject();
            }
        }
        else {
        }
	}

    void OnMouseDown()
    {	
		if (isDraggable()) {
			Debug.Log("Dragging...");
			StartDragging();
		}
    }

    void OnMouseUp()
    {
		if (dragging) {
			Debug.Log("Stopped dragging...");
			StopDraggingObject();
		}
    }

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        //Debug.Log("Draggable collided with" + otherCollider.gameObject.name);
        otherGameObject = otherCollider.gameObject;

        if (otherCollider.tag == "DragIn") {
            dragIn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        otherGameObject = null;

        if (otherCollider.tag == "DragIn")
        {
            dragIn = false;
        }
        handOpenCounter = 0;
    }

    void StartDragging() {
        dragging = true;
        originalPos = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y);

        HandController.instance.isDragging = true;
        Hand.SendMessage("ChangeHandSprite", "DRAGGING");

        ThisRenderer.sortingOrder = 20;
        ThisRenderer.sortingLayerName = "UI";
        Colors.ChangeHaloColor(gameObject, Colors.HaloBlue);
    }

    void DragObject()
    {
        //Vector3 inputPosition = kinectPosition || Input.mousePosition;
        Vector2 destination;

        if (KinectManager.instance.IsAvailable)
        {
            destination = KinectManager.instance.HandPosition;
        }
        else {
            Vector3 inputRawPosition = Input.mousePosition;
            destination = Camera.main.ScreenToWorldPoint(inputRawPosition);
        }
        ThisBody.MovePosition(destination);
    }

    void StopDraggingObject()
    {
        ThisRenderer.sortingOrder = startingOrder;
        ThisRenderer.sortingLayerName = startingLayer;
        dragging = false;
        Hand.SendMessage("ChangeHandSprite", "OPEN");
        HandController.instance.isDragging = false;
        if (dragIn)
        {
            GameController.instance.Recipient = otherGameObject;
            GameController.instance.CheckAnswer(gameObject);
            dragIn = false;
            //colliding = false;
            otherGameObject = null;
            GameController.instance.Recipient = null;
        }
        else {
            ResetPosition();
            GameController.instance.CheckAnswer(gameObject);
        }
    }

	public void AnswerFeedback(bool correct){
		if (correct) {
			Destroy (gameObject);
		} else {
			ResetPosition ();
		}
	}

	IEnumerator ResetWithDelay(float timeToWait){
		yield return new WaitForSeconds(timeToWait);
		ThisCollider.enabled = true;
		ResetPosition ();
        Hand.SendMessage("HandInteract", true);

    }

    void ResetPosition() {
        //Add delay if Kinect tends to trigger StopDragging even when the hand is still closed
        gameObject.transform.position = originalPos;
        ThisRenderer.sortingOrder = startingOrder;
        ThisRenderer.sortingLayerName = startingLayer;
    }

	public void SuggestionMoveTo(GameObject ElegibleContainer, bool ResetHalo, bool MoveOn){
		ThisCollider.enabled = false;
		suggestionMoving = true;
        moveOn = MoveOn;
		Colors.ChangeHaloColor (gameObject, Colors.HaloBlue);
		Colors.Highlight(ElegibleContainer);
		ArrivalPoint = ElegibleContainer.transform.position;
		resetHalo = ResetHalo;
    }

	public void SuggestionMovement(){
		float step = 5.0f * Time.deltaTime;

        ThisRenderer.sortingOrder = 10;
        ThisRenderer.sortingLayerName = "UI";

        transform.localPosition = Vector2.MoveTowards(transform.position, ArrivalPoint, step);

		if (transform.position == ArrivalPoint) {
			suggestionMoving = false;
			StartCoroutine(ResetWithDelay (2.0f));

			Colors.Deselect(GameController.ElegibleContainer);
			Colors.Highlight(GameController.ElegibleContainer, Colors.HaloGreen, 2.0f);

			if (resetHalo) {
				Colors.Deselect(gameObject);
				resetHalo = false;
			} else {
				Colors.ChangeHaloColor(gameObject, Colors.HaloGreen);
			}
            //Hand.SendMessage("HandInteract", true);
            if (moveOn)
            {
                Destroy(gameObject);
            }
        }	
        
	}

	// Update is called once per frame
	void FixedUpdate () {
        KinectDraggingCheck();

        if (dragging) {
            DragObject();
        }

		if (suggestionMoving) {
			SuggestionMovement ();
		} 

    }
}
