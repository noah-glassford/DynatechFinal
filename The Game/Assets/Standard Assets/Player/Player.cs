using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Player : MonoBehaviour
{
    [Header("General Data")]
    public gunManager gm;
    public GameObject UI;
    public GameObject EnemyDetect;
    public GameObject gunHolder;
    public GameObject headJoint;

    [Header("Movement Data")]
    public float moveSpeed = 4.5f;
    public float sprintSpeed = 6.5f;

    public float health = 6;

    private float _rawMovementSpeed;

    private float movementMulti = 10f;

    public float airMovementMulti = 0.3f;
    private float airDrag = 2f;

    public float actionSpeed = 1f;

    public float groundDrag = 6f;

    public float jumpForce = 10f;

    float horizontalMovement;
    float verticalMovement;

    bool isGrounded;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    RaycastHit slopeHit;

    Rigidbody rb;

    PhotonView PV;

    [Header("Look Data")]
    public float sensX;
    public float sensY;

    Camera cam;

    float mouseX;
    float mouseY;

    float lookMulti = 0.1f;

    float xRotation;
    float yRotation;

    [Header("Model Data")]
    public GameObject model;

    private void Awake()
    {
        //PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        cam = GetComponentInChildren<Camera>();

        if (!PV.IsMine)
        {
            Destroy(cam.gameObject);
            Destroy(rb);
            Destroy(gm);
            Destroy(UI);
            Destroy(EnemyDetect);
            Destroy(this.GetComponent<PlayerGameData>());
            Destroy(this.GetComponent<PlayerInteractions>());
            gunHolder.transform.parent = headJoint.transform;
        }

        else if(PV.IsMine)
        {
            Destroy(model);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!PV.IsMine)
        { 
            return;
        }
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);

        GetInput();
        ControlDrag();

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);    
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine)
        {
            return;
        }
        MovePlayer();

        //Please never do this!
        //gm.currentGun.GetComponent<Animator>().speed = actionSpeed;
    }

    void GetInput() {
        if(Time.timeScale == 0)
        {
            return;
        }
        //Movement
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;

        //Look
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * lookMulti;
        xRotation -= mouseY * sensY * lookMulti;

        xRotation = Mathf.Clamp(xRotation, -89f, 89f);
    }
    void ControlDrag() {
        if (isGrounded) rb.drag = groundDrag;
        else rb.drag = airDrag;
    }

    bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.05f)) {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    void MovePlayer() {
        //Check Player Inputs // Sprinting
        if (Input.GetKey(KeyCode.LeftShift)) _rawMovementSpeed = sprintSpeed;
        else _rawMovementSpeed = moveSpeed;

        //Player Raw Inputs Movement
        if (isGrounded && !OnSlope())
            rb.AddForce((moveDirection.normalized * _rawMovementSpeed * movementMulti), ForceMode.Acceleration);
        else if (isGrounded && OnSlope())
            rb.AddForce((slopeMoveDirection.normalized * _rawMovementSpeed * movementMulti), ForceMode.Acceleration);
        else
            rb.AddForce((moveDirection.normalized * _rawMovementSpeed * movementMulti * airMovementMulti), ForceMode.Acceleration);

        //Gravity Manipulation
        if(!isGrounded)
            GetComponent<Rigidbody>().AddForce(Physics.gravity * 3f, ForceMode.Acceleration);
    }
}
