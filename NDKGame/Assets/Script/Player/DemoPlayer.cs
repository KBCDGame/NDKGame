using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoPlayer : MonoBehaviour {

    //入力された値保持用。
    private float InputHorizontal = 0.0f;
    private float InputVertical = 0.0f;

    [Header("移動速度。")]
    public float MoveSpeed = 0.0f;
    [Header("ジャンプ速度。")]
    public float JumpSpeed = 0.0f;
    [Header("重力。")]
    public float Gravity = 0.0f;
    //カメラ。
    private GameObject Camera = null;
    //キャラクターコントローラー。
    private CharacterController CharaCon = null;
    //移動方向。
    private Vector3 MoveDirection = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        //キャラクターコントローラー取得。
        CharaCon = GetComponent<CharacterController>();
        //カメラ取得。
        Camera = GameObject.Find("DemoCamera");
    }

    //Inputの入力漏れを無くすためこっちに記述。
    void Update()
    {
        //入力値を格納。
        InputHorizontal = Input.GetAxis("Horizontal");
        InputVertical = Input.GetAxis("Vertical");

        //接地判定。
        if (CharaCon.isGrounded)
        {
            //接地中はジャンプ可。
            if (Input.GetButton("Jump"))
            {
                //ジャンプ。
                Jump();
            }
        }
    }

    //移動を安定させるためにFixedUpdateにした。
    // Update is called once per frame
    void FixedUpdate()
    {
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.transform.forward, new Vector3(1.0f, 0.0f, 1.0f)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * InputVertical + Camera.transform.right * InputHorizontal;

        // 移動方向にスピードを掛ける。
        MoveDirection.x = moveForward.x * MoveSpeed;
        MoveDirection.z = moveForward.z * MoveSpeed;

        // キャラクターの向きを進行方向に
        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveForward);
        }
        //重力計算。
        MoveDirection.y -= Gravity * Time.fixedDeltaTime;
        //計算した移動量をキャラコンに渡す。
        CharaCon.Move(MoveDirection * Time.fixedDeltaTime);
    }

    //ジャンプの処理。
    void Jump()
    {
        MoveDirection.y = JumpSpeed;
    }

    //if (Input.GetKey(KeyCode.Joystick1Button0))
    //{
    //    Debug.Log("Button A Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button1))
    //{
    //    Debug.Log("Button B Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button2))
    //{
    //    Debug.Log("Button X Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button3))
    //{
    //    Debug.Log("Button Y Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button4))
    //{
    //    Debug.Log("Button LB Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button5))
    //{
    //    Debug.Log("Button RB Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button6))
    //{
    //    Debug.Log("Button Back Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button7))
    //{
    //    Debug.Log("Button START Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button8))
    //{
    //    Debug.Log("L Stick Push Push");
    //}
    //if (Input.GetKey(KeyCode.Joystick1Button9))
    //{
    //    Debug.Log("R Stick Push");
    //}
    //float TrigerInput = Input.GetAxis("Triger");
    //if (TrigerInput < 0.0f)
    //{
    //    Debug.Log("L Triger");
    //}
    //else if (TrigerInput > 0.0f)
    //{
    //    Debug.Log("R Triger");
    //}
    //float HorizontalKeyInput = Input.GetAxis("HorizontalKey");
    //if (HorizontalKeyInput < 0.0f)
    //{
    //    Debug.Log("Left Key");
    //}
    //else if (HorizontalKeyInput > 0.0f)
    //{
    //    Debug.Log("Right Key");
    //}
    //float VerticalKeyInput = Input.GetAxis("VerticalKey");
    //if (VerticalKeyInput < 0.0f)
    //{
    //    Debug.Log("Up Key");
    //}
    //else if (VerticalKeyInput > 0.0f)
    //{
    //    Debug.Log("Down Key");
    //}	
}