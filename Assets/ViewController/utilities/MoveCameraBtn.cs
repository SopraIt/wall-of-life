using UnityEngine;

public class MoveCameraBtn : MonoBehaviour {

    public string direction;
    private GameObject stands;
    private GameObject BtnLeft;
    private GameObject BtnRight;
    private Renderer MaxRight;
    private Renderer MaxLeft;
    Vector3 velocity = new Vector3(0.1f, 0.0f, 0.0f);

	void Awake(){
        stands = GameObject.Find("Market");
        MaxLeft = GameObject.Find("MaxLeft").GetComponent<Renderer>();
        MaxRight = GameObject.Find("MaxRight").GetComponent<Renderer>();

        BtnLeft = GameObject.Find("ButtonLeft");
        BtnRight = GameObject.Find("ButtonRight");
    }


    private void OnTriggerStay2D(Collider2D collider){
        //TODO check stand position based on camera
        IsInView();
        if (collider.gameObject.name != "Hand")
            return;

        if (direction == "right")
        {
			MoveRight();
        }
        else if (direction == "left") {
			MoveLeft();
        }
    }

	void MoveRight(){
        if (MaxRight.isVisible)
        {
            SoundManager.SoundWrong();
            BtnRight.SetActive(false);
        }
        else
        {
            stands.transform.position = stands.transform.position - velocity;
        }

        if (!BtnLeft.activeInHierarchy) {
            BtnLeft.SetActive(true);
        }

    }

    void MoveLeft(){
        if (MaxLeft.isVisible)
        {
            SoundManager.SoundWrong();
            BtnLeft.SetActive(false);
        }
        else {
            stands.transform.position = stands.transform.position + velocity;
        }
        if (!BtnRight.activeInHierarchy)
        {
            BtnRight.SetActive(true);
        }
    }

    private bool IsInView()
    {
        Vector3 pointOnScreen = Camera.main.WorldToScreenPoint(stands.GetComponentInChildren<Renderer>().bounds.center);

        //Is in front
        if (pointOnScreen.z < 0)
        {
            Debug.Log("Behind: " + stands.name);
            return false;
        }

        //Is in FOV
        if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) ||
                (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
        {
            return false;
        }
        
        return true;
    }
}