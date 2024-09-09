using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public static Transform currentShape = null;
    CollapsingPoints collapsePoints = null;

    Animator anim;
    List<GameObject> allCoverCubes = new List<GameObject>();

    [SerializeField][Range(.01f,1)] float LerpSpeed = .3f;
    [Header("For Inputs")]
    public float MinInputDistance = .1f;

    //For Input
    Vector2 StartPos, EndPos;       //Input touch positions
    Vector2 inputDirection = Vector2.zero;
    bool haveInput = false;
    bool receiveInput = true;
    bool StartLerping = false;
    float MovedDis = 0;

    public Transform StartPoint, EndPoint;      //Transform(circle) used for showing the position of the touch in  the screen

    Transform[] allChilds;

    Transform currentPoint;         //Temp Variable for storing the current position point
    Transform NextPoint;            //Temp variable for storing the next position point

    [Header("Player position shower")]
    public Transform MainPointShower;       //Sphere used to show the current position of the player
    public Transform ShowSphere;            //Sphere used to show the next point player gonna reach

    ParticleSystem cubePS;          //Particle to show while the mainpoint is moving to another target
    Animator mainAnim;                     //Animations attached to the main point shower
    public ParticleSystem cubeEndPSPrefab;      //Particles used when the mainpointshower reaches the required destination

    public Transform CoverCubesPrefab;      //Premade cube used to cover the painted the path

    [Header("Input Lines Shower")]
    public LineRenderer line;   //Input Line Renderer
    RaycastHit hit1, hit2;      //Hit points of the touch locations
    [SerializeField] Transform fingerImg = null;

    [Header("Final popping Shape Particle")]
    public Transform shapePS;   //Particle system used at the end of the stage in the same shape as the levels shape

    public Transform ShapesHolder;

    private void Start()
    {
        collapsePoints = currentShape.GetComponent<CollapsingPoints>();
        cubePS = MainPointShower.GetChild(0).GetComponent<ParticleSystem>();
        mainAnim = MainPointShower.GetComponent<Animator>();

        anim = ShapesHolder.GetComponent<Animator>();

        StartPoint.position = Vector2.one * -200;
        EndPoint.position = Vector2.one * -200;

        Vector3 sawPos = Vector3.zero;
        allChilds = new Transform[currentShape.childCount];
        for (int i = 0; i < allChilds.Length; i++)
        {
            allChilds[i] = currentShape.GetChild(i);
            sawPos += allChilds[i].position;
        }
        currentPoint = allChilds[0];
        MainPointShower.position = currentPoint.position;

        sawPos /= allChilds.Length;
        Camera.main.transform.LookAt(sawPos);
        ShapesHolder.position = sawPos;

        ShowSphere.position = Vector3.zero;
        ShowSphere.gameObject.SetActive(false);

        line.positionCount = 2;
        fingerImg.gameObject.SetActive(false);
    }

    private void Update()
    {
        #region For Lerping MainPoint
        if (StartLerping)
        {
            MainPointShower.position = Vector3.Lerp(MainPointShower.position, currentPoint.position, LerpSpeed);
            if(Vector3.Distance(MainPointShower.position,currentPoint.position) < .06f)
            {
                MainPointShower.position = currentPoint.position;
                MainPointShower.GetChild(1).GetComponent<ParticleSystem>().Play();

                cubePS.Stop();
                mainAnim.SetBool("squash", false);
                Instantiate(cubeEndPSPrefab, MainPointShower.position, cubeEndPSPrefab.transform.rotation);
                StartLerping = false;
                receiveInput = true;

                AudioManager.instance.Play("bubble");

                if(PathsController.completed)
                {
                    Transform t = Instantiate(shapePS, transform.position, transform.rotation);
                    t.GetComponent<PopUpShape>().currentMesh = currentShape.GetComponent<MeshFilter>().mesh;
                    //ParticleSystem rend = t.GetComponent<ParticleSystem>();
                    GameManager.instance.SpawnPartyPapers();

                    currentShape.SetParent(ShapesHolder);
                    anim.SetTrigger("finish");
                    GameObject[] g = allCoverCubes.ToArray();
                    for(int i = 0;i < g.Length;i++)
                    {
                        Destroy(g[i]);
                    }
                    currentShape.GetComponent<Renderer>().sharedMaterial = GameManager.instance.shapesMaterial;
                    MainPointShower.gameObject.SetActive(false);

                    ShapesHolder.gameObject.AddComponent<ShapeEndAnimator>();

                    AudioManager.instance.Play("win");

                    DataManager.SaveLevel();
                }
            }
        }
        #endregion

        if (receiveInput && GameManager.gameState == GAMESTATE.Game && !CameraController.haveInput)
        {
            if (Input.touchSupported)
            {
                if (Input.touchCount > 0)
                {
                    GetTouchInput();
                }
            }
            else
            {
                GetMouseInput();
            }
        }
        else if(haveInput)
        {
            haveInput = false;
        }

        if(Physics.Raycast(Camera.main.ScreenPointToRay(StartPos), out hit1))
        {
            line.SetPosition(0, hit1.point);
        }
        if (Physics.Raycast(Camera.main.ScreenPointToRay(EndPos), out hit2))
        {
            line.SetPosition(1, hit2.point);
        }
    }

    void GetMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CameraController.instance.isTouchingShape(Input.mousePosition))
                return;

            InitInput(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            if(haveInput == false)
            {
                InitInput(Input.mousePosition);
            }
            EndPos = Input.mousePosition;
            EndPoint.position = EndPos;

            inputDirection = EndPos - StartPos;
            Vector2 s = new Vector2(StartPos.x / Screen.width, StartPos.y / Screen.height);
            Vector2 e = new Vector2(EndPos.x / Screen.width, EndPos.y / Screen.height);
            MovedDis = (s-e).magnitude;

            if(MovedDis >= MinInputDistance)
            {
                if(ShowSphere.gameObject.activeInHierarchy == false)
                {
                    ShowSphere.gameObject.SetActive(true);
                }
                CheckNextPoint3();
            }

            fingerImg.position = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0) && haveInput)
        {
            OnInputEnd();
        }
    }

    void GetTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            if (CameraController.instance.isTouchingShape(Input.mousePosition))
                return;

            InitInput(touch.position);
        }
        else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
        {
            if (haveInput == false)
            {
                InitInput(touch.position);
            }
            EndPos = touch.position;
            EndPoint.position = EndPos;

            inputDirection = EndPos - StartPos;
            Vector2 s = new Vector2(StartPos.x / Screen.width, StartPos.y / Screen.height);
            Vector2 e = new Vector2(EndPos.x / Screen.width, EndPos.y / Screen.height);
            MovedDis = (s - e).magnitude;

            if (MovedDis >= MinInputDistance)
            {
                if (ShowSphere.gameObject.activeInHierarchy == false)
                {
                    ShowSphere.gameObject.SetActive(true);
                }
                CheckNextPoint3();
            }

            fingerImg.position = touch.position;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            OnInputEnd();
        }
    }

    void InitInput(Vector2 position)
    {
        StartPos = position;
        EndPos = position;
        StartPoint.position = StartPos;
        EndPoint.position = EndPos;
        ShowSphere.gameObject.SetActive(true);
        haveInput = true;
        line.enabled = true;

        fingerImg.position = position;
        fingerImg.gameObject.SetActive(true);
    }

    void OnInputEnd()
    {
        if (MovedDis >= MinInputDistance)
        {
            OnRelease();
            GameManager.instance.MoveUsed();
        }
        else
        {
            StartPoint.position = Vector2.one * -1200;
            EndPoint.position = Vector2.one * -1200;

            ShowSphere.GetComponent<ShowSphere>().targetPoint = null;
            ShowSphere.gameObject.SetActive(false);

            haveInput = false;
            line.enabled = false;
        }
        fingerImg.gameObject.SetActive(false);
    }

    void OnRelease()
    {
        //CheckDirection();
        StartPoint.position = Vector2.one * -200;
        EndPoint.position = Vector2.one * -200;

        if (PathsController.instance.ChechPath(currentPoint, NextPoint))
        {
            Transform t = Instantiate(CoverCubesPrefab, currentPoint.position, Quaternion.identity);
            t.GetComponent<CoverCube>().StartCovering(NextPoint.position - currentPoint.position, currentPoint.position, NextPoint.position);

            allCoverCubes.Add(t.gameObject);
            
            if(collapsePoints != null)
                collapsePoints.ClearPath(currentPoint, NextPoint);
        }

        Vector3 dir = NextPoint.position - currentPoint.position;
        MainPointShower.forward = dir.normalized;
        mainAnim.SetBool("squash",true);
        MainPointShower.GetChild(1).GetComponent<ParticleSystem>().Stop();

        currentPoint = NextPoint;
        //MainPointShower.position = currentPoint.position;
        receiveInput = false;
        StartLerping = true;

        //ShowSphere.position = Vector3.zero;
        ShowSphere.GetComponent<ShowSphere>().targetPoint = null;
        ShowSphere.gameObject.SetActive(false);

        haveInput = false;
        line.enabled = false;

        cubePS.Play();
    }

    void CheckNextPoint3()
    {
        Transform[] possiblePoint = currentPoint.GetComponent<PossiblePoints>().AllPossiblePoints;

        if (ShowSphere.GetComponent<ShowSphere>().targetPoint == null)
        {
            NextPoint = possiblePoint[Random.Range(0, possiblePoint.Length)];
            ShowSphere.GetComponent<ShowSphere>().targetPoint = NextPoint;
        }

        float maxDot = float.MinValue;
        int index = 0;
        for(int i = 0; i < possiblePoint.Length; i++)
        {
            Vector2 v1 = Camera.main.WorldToScreenPoint(currentPoint.position);
            Vector2 v2 = Camera.main.WorldToScreenPoint(possiblePoint[i].position);
            Vector2 dir = v2 - v1;

            float dot = Vector2.Dot(inputDirection.normalized, dir.normalized);
            if (dot > maxDot)
            {
                maxDot = dot;
                index = i;
            }
        }

        NextPoint = possiblePoint[index];
        ShowSphere.GetComponent<ShowSphere>().targetPoint = NextPoint;
    }
}
