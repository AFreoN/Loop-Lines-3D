using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance { get; private set; }
    public static Camera mainCam { get; private set; }

    public float LerpSpeed = .2f;
    public float LevitationHeight = .5f;
    public float MoveSpeed = .2f;

    Vector3 startPos;
    float Y= 0;
    bool Up;

    [Header("For controlling rotation")]
    [SerializeField] LayerMask hitLayer;
    [SerializeField] float rotSpeed = 10;
    [SerializeField] Transform directionalLight = null;
    Vector3 origin = Vector3.zero;
    Vector2 sPos = Vector2.zero, ePos = Vector2.zero;

    const string shapeTag = "Player";
    public static bool haveInput { get; private set; }

    private void Awake()
    {
        instance = this;
        mainCam = GetComponent<Camera>();
    }

    void Start()
    {
        startPos = transform.position;

        Up = Random.Range(0, 2) == 0 ? true : false;
        haveInput = false;
    }

    void Update()
    {
        if(GameManager.gameState == GAMESTATE.Game)
        {
            if (Input.touchSupported && Input.touchCount > 0)
            {
                GetTouchInput();
            }
            else
                GetMouseInput();
        }
        if(Up)
        {
            Y += Time.deltaTime * MoveSpeed;
            if(Y >= LevitationHeight)
            {
                Up = false;
            }
        }
        else if(Up == false)
        {
            Y -= Time.deltaTime * MoveSpeed;
            if(Y <= -LevitationHeight)
            {
                Up = true;
            }
        }
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, startPos.y + Y, transform.position.z), LerpSpeed);
    }

    void GetMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(isTouchingShape(Input.mousePosition))
            {
                haveInput = true;
                sPos = Input.mousePosition;
            }
        }
        else if (Input.GetMouseButton(0) && haveInput)
        {
            ePos = Input.mousePosition;

            float dif = (ePos - sPos).x / Screen.width * rotSpeed;
            transform.RotateAround(origin, Vector3.up, dif);
            directionalLight.RotateAround(origin, Vector3.up, dif);

            sPos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && haveInput)
        {
            haveInput = false;
        }
    }

    void GetTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            if (isTouchingShape(Input.mousePosition))
            {
                haveInput = true;
                sPos = touch.position;
            }
        }
        else if (touch.phase == TouchPhase.Moved && haveInput || touch.phase == TouchPhase.Stationary && haveInput)
        {
            ePos = touch.position;

            float dif = (ePos - sPos).x / Screen.width * rotSpeed;
            transform.RotateAround(origin, Vector3.up, dif);
            directionalLight.RotateAround(origin, Vector3.up, dif);

            sPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended && haveInput || touch.phase == TouchPhase.Canceled && haveInput)
        {
            haveInput = false;
        }
    }

    public bool isTouchingShape(Vector3 pos)
    {
        bool result = false;
        Ray ray = mainCam.ScreenPointToRay(pos);
        if(Physics.Raycast(ray, out RaycastHit hit,Mathf.Infinity, hitLayer))
        {
            if(hit.transform.CompareTag(shapeTag))
            {
                result = true;
            }
        }
        return result;
    }
}
