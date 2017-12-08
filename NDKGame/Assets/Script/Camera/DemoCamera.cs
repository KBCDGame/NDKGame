using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DemoCamera : MonoBehaviour
{
    //プレイヤー。
    private GameObject Player = null;
    //プレイヤーの位置。
    private Vector3 PlayerPos = Vector3.zero;
    //入力量保持用。
    private float InputX, InputY = 0.0f;
    //ホイールの入力量保持用。
    private float Scroll = 0.0f;
    //カメラが前後に移動したかどうかのフラグ。
    private bool IsMoveBeforeOrAfterFlag = false;

    ////カメラ。
    //private Camera GemaCamera = null;
    ////ズームに使う値保持用。
    //private float Scroll, View = 0.0f;
    [Header("回転速度。")]
    public float RotateSpeed = 200.0f;
    [Header("縦回転の上限。")]
    [Space(10), Tooltip("今は設定しても効果無し。")]
    public float MaxRotateY = 0.0f;
    [Header("縦回転の下限。")]
    [Space(10), Tooltip("今は設定しても効果無し。")]
    public float MinRotateY = 0.0f;
    [Header("プレイヤーからカメラまでの最大距離。")]
    public float PlayerToCameraMaxDistance = 80.0f;
    [Header("プレイヤーからカメラまでの最小距離。")]
    public float PlayerToCameraMinDistance = 3.0f;
    [Header("ゲームスタート時にプレイヤーからどれだけカメラをずらすか。")]
    public Vector3 StartOffsetPos = Vector3.zero;
    [Header("コントローラー使用時のカメラの前後移動のスピード。")]
    public float ControllerCameraMoveBeforeAndAfterSpeed = 2.0f;
    [Header("マウス使用時のカメラの前後移動のスピード。")]
    public float MouseCameraMoveBeforeAndAfterSpeed = 1000.0f;
    ////ズームの最大と最小。
    //public float ZoomMax,ZoomMin = 0.0f;
    ////ズームの時のスピード。
    //public float ZoomSpeed = 1.0f;


    // Use this for initialization
    void Start()
    {
        //最大と最小が反転して設定されていた場合。
        if (PlayerToCameraMinDistance > PlayerToCameraMaxDistance)
        {
            float work = PlayerToCameraMinDistance;
            PlayerToCameraMinDistance = PlayerToCameraMaxDistance;
            PlayerToCameraMaxDistance = work;
            Debug.Log("プレイヤーとカメラ間の最小距離とプレイヤーとカメラ間の最大距離が逆でした。");

        }
        //プレイヤー取得。
        Player = GameObject.FindWithTag("Player");
        ////カメラ取得。
        //GemaCamera = GetComponent<Camera>();

        if (Player != null)
        {
            //プレイヤーの位置取得。
            PlayerPos = Player.transform.position;
            //カメラの位置をプレイヤーからずらした位置に設定。
            Vector3 newPos = PlayerPos + StartOffsetPos;
            transform.position = newPos;
        }
        Camera Demo = GetComponent<Camera>();
        Demo.depth = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーを検索。
        if (FindPlayer() == false)
        {
            return;
        }

        //Input関係の処理。
        InputRotate();
    }

    void FixedUpdate()
    {
        //プレイヤーを検索。
        if (FindPlayer() == false)
        {
            return;
        }

        //プレイヤーの移動量分、自分(カメラ)も移動する
        transform.position += Player.transform.position - PlayerPos;
        PlayerPos = Player.transform.position;

        //PlayerPosの位置のY軸を中心に、回転（公転）する
        transform.RotateAround(PlayerPos, Vector3.up, InputX);

        //カメラの前後移動。
        CameraMoveBeforeAndAfter();

        //カメラの前後移動がされていないならカメラの回転をする。
        if (IsMoveBeforeOrAfterFlag != true)
        {
            //カメラの垂直移動（角度制限なし）
            transform.RotateAround(PlayerPos, transform.right, InputY);
        }
    }

    //Inputの回転をまとまたもの。
    private void InputRotate()
    {
        float DeltaSpeed = Time.deltaTime * RotateSpeed;
        // マウスの右クリックを押している間
        if (Input.GetMouseButton(1))
        {
            //マウスの移動量
            InputX = Input.GetAxis("Mouse X") * DeltaSpeed;
            InputY = Input.GetAxis("Mouse Y") * DeltaSpeed;
        }
        //マウスの右が押されていない時は右スティックの倒しを優先する。
        else
        {
            //右スティックの移動量
            InputX = Input.GetAxis("Horizontal2") * DeltaSpeed;
            InputY = Input.GetAxis("Vertical2") * DeltaSpeed;
        }

        //ホイールの入力。
        Scroll = Input.GetAxis("Mouse ScrollWheel");
    }

    ////カメラのズーム処理。
    //private void CameraZoom()
    //{
    //    float view = GemaCamera.fieldOfView - Scroll * ZoomSpeed;
    //    //Clampを使ってマイナスや極端に大きな値にならなよう調整。
    //    GemaCamera.fieldOfView = Mathf.Clamp(value: view, min:ZoomMin, max: ZoomMax);
    //}

    //ホイールとパッドを使ったカメラの前後移動。
    private void CameraMoveBeforeAndAfter()
    {
        //初期化。
        float value = 0.0f;
        float speed = 0.0f;

        //移動フラグ初期化。
        IsMoveBeforeOrAfterFlag = false;

        //コントローラーが使われているならコントローラー用の前後移動のスピードを設定。
        if (Input.GetKey(KeyCode.Joystick1Button4))
        {
            value = InputY;
            speed = ControllerCameraMoveBeforeAndAfterSpeed;
            IsMoveBeforeOrAfterFlag = true;
        }

        //マウスが使われているならマウス用の前後移動のスピードを設定。
        if (Scroll != 0.0f)
        {
            value = Scroll;
            speed = MouseCameraMoveBeforeAndAfterSpeed;
            IsMoveBeforeOrAfterFlag = true;
        }

        //どれくらい移動するか。
        Vector3 MovePos = transform.forward * -value * Time.deltaTime * speed;

        //カメラとプレイヤーの距離計算。
        float distance = (transform.position.z + MovePos.z) - PlayerPos.z;

        //スティックが前後どちらかに倒されていて、プレイヤーとカメラの距離が指定範囲内なら移動。
        if ((value > 0.0f && Mathf.Abs(distance) < PlayerToCameraMaxDistance) || (value < 0.0f && Mathf.Abs(distance) > PlayerToCameraMinDistance))
        {
            transform.position += MovePos;
        }
    }

    //プレイヤーがnullの間はプレイヤーを探す。
    private bool FindPlayer()
    {
        if (Player == null)
        {
            //プレイヤー取得。
            Player = GameObject.FindWithTag("Player");
            if (Player != null)
            {
                //プレイヤーの位置取得。
                PlayerPos = Player.transform.position;
                //カメラの位置をプレイヤーからずらした位置に設定。
                Vector3 newPos = PlayerPos + StartOffsetPos;
                transform.position = newPos;
            }
            return false;
        }
        else
        {
            return true;
        }
    }
}
