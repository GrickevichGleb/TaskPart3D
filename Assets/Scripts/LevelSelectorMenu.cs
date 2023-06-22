using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorMenu : MonoBehaviour
{
    [SerializeField] private string[] sceneNames = new string[3];


    private void Start()
    {
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;

        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;

    }

    public void SelectScene(int i)
    {
        SceneManager.LoadScene(sceneNames[i]);
    }
}
