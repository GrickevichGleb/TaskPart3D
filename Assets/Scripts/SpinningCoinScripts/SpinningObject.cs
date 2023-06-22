using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class SpinningObject : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 20f;
    [SerializeField] private new Renderer renderer;
    [SerializeField] private GameObject targetGameObject;
    
    private Camera _mainCamera;

    private bool _isHolding = false;
    private bool _isReleased = false;

    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        _mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RotateObject();
        UpdateTouchInput();
        ProcessTouchInput();
        
        //Android back button support
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene(0); //Load scene selector menu
            }
        }
    }


    private void RotateObject()
    {
        gameObject.
            transform.Rotate(0f, rotationSpeed*Time.deltaTime, 0f);
    }


    private void UpdateTouchInput()
    {
        
        if (Touchscreen.current.primaryTouch.press.isPressed)
        {
            _isHolding = true;
            _isReleased = false;
        }
        else
        {
            if (_isHolding)
            {
                _isReleased = true;
            }

            _isHolding = false;
        }
    }


    private void ProcessTouchInput()
    {
        if (_isReleased)
        {
            Vector2 screenTouchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            RaycastHit hit;
            if (Physics.Raycast(_mainCamera.ScreenPointToRay(screenTouchPosition), out hit))
            {
                if (hit.collider.gameObject == targetGameObject)
                {
                    Debug.Log(renderer.material.color);
                    SetObjColor(GetRandomColor());
                }
            }

            _isReleased = false;
        }
    }


    private void SetObjColor(Color color)
    {
        renderer.material.SetColor("_Color", color);   
    }
    
    
    private Color GetRandomColor()
    {
        Color color = new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
            );

        return color;
    }
}
