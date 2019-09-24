using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartGame : MonoBehaviour {
    // Used for Debugging
    void Start () {
        Button button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        SetUpPlayerPrefs.SetUpPlayerPrefs.ResetPlayer();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
