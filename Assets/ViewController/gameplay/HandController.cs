using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HandController : MonoBehaviour {
   
    private Rigidbody2D myHand;

    //private Vector3 upperCorner = new Vector3(Screen.width, Screen.height, 0.0f);
    
    //private float maxHorizontal;
    //private float maxVertical;

    Vector2 targetPosition;
    public Vector2 velocity;

    public Sprite sprite_hand_open;
    public Sprite sprite_hand_close;
    public Sprite sprite_hand_dragging;
    public Sprite sprite_hand_pointing;
    public Sprite sprite_hand_disabled;
    public Sprite sprite_hand_awakening;
    private IEnumerator handInteractionCoroutine;
    private SpriteRenderer handSpriteRenderer;

    public static HandController instance;

	public bool isDragging = false;
    public bool isSelecting = false;
    public bool handIsEnabled = false;

    public GameObject CurrentGameObject;
    GameObject LoadingCirclePrefab;
    GameObject LoadingCircle;
    
    private Image LoadingBar;

    public bool selected = false;
    private string itemSelection;

    void Awake()
    {
        instance = this;

        LoadingCirclePrefab = Resources.Load("prefab/CircularProgressBar") as GameObject;
        velocity = new Vector2(0.5f, 0.5f);

        myHand = GetComponent<Rigidbody2D>();
        handSpriteRenderer = GetComponent<SpriteRenderer>();

        HandInteract(true);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!isSelecting && !isDragging) {
            CurrentGameObject = other.gameObject;
            if (CurrentGameObject.tag == "Selection")
            {
                itemSelection = CurrentGameObject.tag;
                StartSelectItem();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (CurrentGameObject) {
            Colors.Deselect(CurrentGameObject);
        }

        CurrentGameObject = other.gameObject;

        //Dragging Behaviour
        if (CurrentGameObject.tag == "DragIn")
        {
            Colors.Highlight(CurrentGameObject);
			return;
        }
        if (isDragging)
            return;
        //END OF Dragging Behaviour

        if (CurrentGameObject.tag == "Draggable")
        {
            //ChangeHandSprite("OPEN");
            Colors.Highlight(CurrentGameObject);
        }
        else if (CurrentGameObject.tag != "Untagged")
        {
            //ChangeHandSprite("POINTING");
            itemSelection = CurrentGameObject.tag;
            StartSelectItem();
        }
        else {
            //ChangeHandSprite("POINTING");
            CurrentGameObject = null;
        }
    }

    void StartSelectItem()
    {
		isSelecting = true;
        ChangeHandSprite("POINTING");
        Destroy(LoadingCircle);

        Colors.Highlight(CurrentGameObject);

        LoadingCircle = Instantiate(LoadingCirclePrefab);
        LoadingCircle.transform.SetParent(GameObject.Find("Canvas").transform, false);

        LoadingBar = LoadingCircle.GetComponent<Image>();
        StopCoroutine("SelectingItem");
        StartCoroutine("SelectingItem");
    }

    IEnumerator SelectingItem()
    {
        float waitTime = 1.5f;
        float counter = 0f;
        if (CurrentGameObject.tag.Equals("LoadButton") || CurrentGameObject.name.Equals("ShoppingList"))
        {
            waitTime = 1f;
        }
        while (counter < waitTime)
        {
            counter += Time.deltaTime;
            LoadingBar.fillAmount = (counter / waitTime);
            LoadingCircle.transform.position = this.transform.position;
            yield return null; //Don't freeze Unity
        }
        LoadingBar.fillAmount = 0f;
        if (this.itemSelection == "Selection")
            Interactions.SelectObject(CurrentGameObject);
        else if(this.itemSelection == "UI_selection")
            Interactions.SelectUIObject(CurrentGameObject);
        else if (this.itemSelection == "LoadButton")
            Interactions.SelectUIObject(CurrentGameObject);
    }

    void StopSelectingItem(GameObject gameObject)
    {
        StopCoroutine("SelectingItem");
        Destroy(LoadingCircle);
		isSelecting = false;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        GameObject otherGameObject = other.gameObject;
        Colors.Deselect(otherGameObject);

        if (isDragging)
            return;

		if (otherGameObject == CurrentGameObject) {
			StopSelectingItem (otherGameObject);
			//ChangeHandSprite ("POINTING");
            CurrentGameObject = null;
        }
    }

	public void HandInteract(bool activate){
        CurrentGameObject = null;
        if (handInteractionCoroutine != null) {
            StopCoroutine(handInteractionCoroutine);
        }
        handInteractionCoroutine = ActivateHand(activate);
        StartCoroutine(handInteractionCoroutine);
	}

    IEnumerator ActivateHand(bool activate) {
        Collider2D HandCollider = gameObject.GetComponent<Collider2D>();

        if (activate)
        {
            if (KinectAvailable()) {
                //ChangeHandSprite("AWAKENING");
                KinectManager.instance.AwakeCheckStart();
                while (!KinectManager.instance.HandAwaken)
                {
                    yield return null;
                }
            }

            HandCollider.enabled = true;
            handIsEnabled = true;
            ChangeHandSprite("POINTING");
        }
        else
        {
            HandCollider.enabled = false;
            handIsEnabled = false;
            ChangeHandSprite("DISABLED");
        }
    }

    void Start() {
        //if (myCam == null) {
        //    myCam = Camera.main;
        //}
        //handWidth = myHandRend.bounds.extents.x;
        //handHeight = myHandRend.bounds.extents.y;
        //SetHandBoundaries();
    }

    //public void SetHandBoundaries() {
    //    Vector3 targetWidth = myCam.ScreenToWorldPoint(upperCorner);
    //    //maxHorizontal = targetWidth.x - handWidth;
    //    //maxVertical = targetWidth.y - handHeight;
    //    //maxHorizontal = targetWidth.x;
    //    //maxVertical = targetWidth.y;
    //}

    void UpdateHandSprite() {
        if (KinectManager.instance.HandClosed)
        {
            ChangeHandSprite("CLOSE");
        }
        else if (KinectManager.instance.HandOpen)
        {
            ChangeHandSprite("OPEN");
        }
    }

    public void ChangeHandSprite(string hand_sprite_type){
        Sprite newSprite;

        switch (hand_sprite_type) {
            case "OPEN":
                newSprite = sprite_hand_open;
                break;
            case "CLOSE":
                newSprite = sprite_hand_close;
                break;
            case "POINTING":
                newSprite = sprite_hand_pointing;
                break;
            case "DRAGGING":
                newSprite = sprite_hand_dragging;
                break;
            case "DISABLED":
                newSprite = sprite_hand_disabled;
                break;
            case "AWAKENING":
                newSprite = sprite_hand_awakening;
                break;
            default:
                newSprite = sprite_hand_open;
                break;
        }

        handSpriteRenderer.sprite = newSprite;
    }

    bool KinectAvailable() {
        return KinectManager.instance && KinectManager.instance.IsAvailable;
    }

    void FixedUpdate()
    {
        //Move hand alternatively with Kinect or with mouse
        if (KinectAvailable())
        {
            targetPosition = KinectManager.instance.HandPosition;
            if (handIsEnabled && !isDragging && !isSelecting)
            {
                UpdateHandSprite();
            }
        }
        else
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                                 Input.mousePosition.y, 0.0f));
            //float maxWidth = Mathf.Clamp(mousePosition.x, -maxHorizontal, maxHorizontal);
            //float maxHeight = Mathf.Clamp(mousePosition.y, -maxVertical, maxVertical);
            targetPosition = new Vector2(mousePosition.x, mousePosition.y);
        }
        myHand.MovePosition(targetPosition);
    }
}
