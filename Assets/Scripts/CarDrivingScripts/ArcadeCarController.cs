using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SearchService;

public class ArcadeCarController : MonoBehaviour
{
    private int _steeringInputValue;
    private int _torqueInputValue;

    private void Start()
    {
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToLandscapeLeft = true;

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
    }


    private void Update()
    {
        //Android back button support
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene(0); //Load scene selector menu
            }
        }
    }

    public void SteeringInput(int value)
    {
        _steeringInputValue = value;
    }

    public void TorqueInput(int value)
    {
        _torqueInputValue = value;
    }
    
    public Vector2 GetInputValues()
    {
        return new Vector2(_steeringInputValue, _torqueInputValue);
    }
  
}
    


    

