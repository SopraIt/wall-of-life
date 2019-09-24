using System;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Video;

public class RewardsBehaviourScript : MonoBehaviour
{

    private VideoPlayer videoPlayer;
    private AudioSource audioPlayer;
    private String publicDir;

    private void Awake()
    {
        publicDir = "file://" + Application.streamingAssetsPath + "/Prizes/";
    }

    void Start()
    {
        videoPlayer = gameObject.GetComponent(typeof(VideoPlayer)) as VideoPlayer;
        audioPlayer = gameObject.GetComponent(typeof(AudioSource)) as AudioSource;

        string prizeType;
        string prizeSource;

        if (RewardsController.instance.CheckWinCondition())
        {
            prizeType = PlayerPrefs.GetString("prizeType");
            prizeSource = PlayerPrefs.GetString("prizeSource");
        } else {
            prizeType = PlayerPrefs.GetString("consolationPrizeType");
            prizeSource = PlayerPrefs.GetString("consolationPrizeSource");
        }
        Debug.Log("Type:" + prizeType + " Prize: " + prizeSource);

        //TODO manage empty source
        switch (prizeType)
        {
            case "olvideo":
            case "video":
                CreateVideoInstance(prizeSource);
                break;
            case "audio":
                StartCoroutine(CreateAudioInstance(prizeSource));
                break;
            case "application":
                System.Diagnostics.Process.Start(prizeSource);
                PauseMenu.instance.Pause();
                break;
            case "browser":
                Application.OpenURL(prizeSource);
                PauseMenu.instance.Pause();
                break;
            case "ytvideo":
                HandleYoutubePlayer(prizeSource);
                break;
            default:
                Debug.Log("source non set");
                break;
        }

        //StartCoroutine(PointsDown());
    }

    private void HandleYoutubePlayer( string prizeSource ) {
        Application.OpenURL(prizeSource);
        PauseMenu.instance.Pause();
    }

    //private void OnVideoFinished(VideoPlayer vPlayer)
    //{
    //    PauseMenu.instance.Pause();
    //}

    IEnumerator PointsDown()
    {   
        while (PlayerPrefs.GetInt("score") > 0)
        {
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") - 1);
            yield return null;
        }
    }

    void CreateVideoInstance(string prizeSource)
    {
        videoPlayer.playOnAwake = false;

        videoPlayer.targetCamera = Camera.main;
        videoPlayer.frame = 0;
        videoPlayer.isLooping = true;

        Regex regex = new Regex(@"http.*");
        System.Text.RegularExpressions.Match matchUrl = regex.Match(prizeSource);
        Debug.Log("is url loaded? " + matchUrl.Success);

        regex = new Regex(@".*(\.avi$|.*\.mp4$|.*\.mov|.*\.wmv|.*\.mpg|.*\.mpeg|.*\.m4v|.*\.webm)$");
        System.Text.RegularExpressions.Match matchFile = regex.Match(prizeSource);
        Debug.Log("is local loaded? " + matchFile.Success);

        if (matchUrl.Success)
        {
            // URL paths.
            videoPlayer.url = prizeSource;
        }
        else if (matchFile.Success)
        {
            // local paths.
            videoPlayer.url = publicDir + prizeSource;
        }
        else
        {
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.clip = Resources.Load("video/" + prizeSource) as VideoClip;
            videoPlayer.source = VideoSource.VideoClip;
        }

         videoPlayer.Play();
    }

    IEnumerator CreateAudioInstance(string prizeSource)
    {
        Regex regex = new Regex(@".*\.mp3$|.*\.ogg$|.*\.wav$");
        System.Text.RegularExpressions.Match matchFile = regex.Match(prizeSource);
        Debug.Log("is local loaded? " + matchFile.Success);

        if (matchFile.Success)
        {
            prizeSource = publicDir + prizeSource;
            
            Debug.Log("source: " + prizeSource);
           
            WWW loader = new WWW(prizeSource);

            yield return loader;

            if (!string.IsNullOrEmpty(loader.error))
            {
                Debug.LogWarning("Error! Cannot open file; " + loader.error);
            }


            AudioClip audioClip = loader.GetAudioClip(false, true);
            audioPlayer.clip = audioClip;
        }
        else
        {
            audioPlayer.clip = Resources.Load("audio/" + prizeSource) as AudioClip;
        }
        audioPlayer.Play();
    }
}
