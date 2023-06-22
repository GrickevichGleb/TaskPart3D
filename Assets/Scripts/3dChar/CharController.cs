using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharController : MonoBehaviour
{
    [SerializeField] private Rigidbody characterRb = null;
    [SerializeField] private Animator charAnimator = null;
    [Space] 
    
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform leftToes = null;
    [SerializeField] private Transform rightToes = null;
    [SerializeField] private GameObject footstepMarkPref = null;
    [Space]
    
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpForce = 600f;
    [Space] 
    
    [SerializeField] private CinemachineFreeLook freeLookCamera;
    [SerializeField] private Joystick joystickLeft = null;
    [SerializeField] private Joystick joystickRight = null;
    [SerializeField] private float horizontalViewSens = 5f;
    [SerializeField] private float verticalViewSens = 1f;


    private Camera _camera;
    
    private float _leftInputX;
    private float _leftInputY;

    private float _rightInputX;
    private float _rightInputY;

    private bool _isAttacking;
    private bool _isJumping;
    private int _stepN = 0;// 1 = left step | 2 = right step (to prevent doubling)
    
    private float _currentSmoothVel;
    
    // Start is called before the first frame update
    void Start()
    {
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToLandscapeLeft = true;

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        ReadInputValues();
        
        //Android back button support
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SceneManager.LoadScene(0); //Load scene selector menu
            }
        }
    }


    private void FixedUpdate()
    {
        MoveCharacter();
        AnimationUpdate();
    }

    private void LateUpdate()
    {
        CameraUpdate();
    }


    public void JumpButton()
    {
        if (IsGrounded())
        {
            _isJumping = true;
        }
    }

    public void AttackButton()
    {
        _isAttacking = true;
    }

    // Animation events processing
    private void StepAnimEvent()
    {
        RaycastHit hit;
        if(Physics.Raycast(leftToes.position, Vector3.down, out hit, 0.05f))
        {
            if (hit.collider.CompareTag("Ground") && charAnimator.GetBool("moving"))
            {
                if (_stepN != 1)
                {
                    Debug.Log("Add footstep mark for left step");
                    Instantiate(footstepMarkPref, hit.point, Quaternion.identity);
                    _stepN = 1; 
                }
            }
        }
        
        if(Physics.Raycast(rightToes.position, Vector3.down, out hit, 0.05f))
        {
            if (hit.collider.CompareTag("Ground") && charAnimator.GetBool("moving"))
            {
                if (_stepN != 2)
                {
                    Debug.Log("Add footstep mark for right step");
                    Instantiate(footstepMarkPref, hit.point, Quaternion.identity);
                    _stepN = 2;
                }
                
            }
        }
        
    }


    private void ReadInputValues()
    {
        
        _leftInputX = joystickLeft.Horizontal;
        _leftInputY = joystickLeft.Vertical;

        _rightInputX = joystickRight.Horizontal;
        _rightInputY = joystickRight.Vertical;
        
    }


    private void MoveCharacter()
    {
        Vector3 inputMoveVec = new Vector3(_leftInputX, 0f, _leftInputY);
        Vector3 moveVec = transform.TransformDirection(inputMoveVec).normalized;

        if (moveVec.magnitude >= 0.1f)
        {

            float targetAngle = Mathf.Atan2(inputMoveVec.x, inputMoveVec.z) * Mathf.Rad2Deg;

            if (_leftInputY >= 0.1f && Mathf.Abs(_leftInputX) <= 0.4f)
            {
                targetAngle += _camera.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentSmoothVel, 0.1f);
                characterRb.MoveRotation(Quaternion.Euler(0f, angle, 0f));
            }
            
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * moveSpeed;
            characterRb.velocity = new Vector3(moveDirection.x, characterRb.velocity.y, moveDirection.z);
            //new Vector3(moveVec.x, characterRb.velocity.y, moveVec.z);
        }
        
        if (_isJumping)
        {
            characterRb.AddForce(Vector3.up*jumpForce, ForceMode.Impulse);
        }
    }


    private void AnimationUpdate()
    {
        float velxVal;
        float velyVal;
        bool isMovingVal;

        if (Mathf.Abs(joystickLeft.Vertical) >= 0.1f
            || Mathf.Abs(joystickLeft.Horizontal) >= 0.1f)
        {
            isMovingVal = true;
        }
        else
        {
            isMovingVal = false;
        }

        velyVal = joystickLeft.Vertical;
        velxVal = joystickLeft.Horizontal;
        
        charAnimator.SetBool("moving", isMovingVal);
        charAnimator.SetFloat("velx", velxVal);
        charAnimator.SetFloat("vely", velyVal);

        if (_isJumping)
        {
            charAnimator.SetTrigger("jump");
            _isJumping = false;
        }

        if (_isAttacking)
        {
            charAnimator.SetTrigger("attack");
            _isAttacking = false;
        }
    }


    private bool IsGrounded()
    {
        CapsuleCollider collider = gameObject.GetComponent<CapsuleCollider>();
        if (collider == null) return false;

        Vector3 colliderBottom = new Vector3(
            collider.bounds.center.x,
            collider.bounds.min.y,
            collider.bounds.center.z);

        bool isGrounded = Physics.CheckCapsule(
            collider.bounds.center,
            colliderBottom,
            0.1f,
            groundLayer,
            QueryTriggerInteraction.Ignore);

        return isGrounded;
    }

    private void CameraUpdate()
    {
        freeLookCamera.m_XAxis.Value = _rightInputX * horizontalViewSens;
        freeLookCamera.m_YAxis.Value = _rightInputY * verticalViewSens + 0.5f;
    }
}
