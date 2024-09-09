using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class PathsController : MonoBehaviour
{
    public static PathsController instance;

    public Transform Shape;
    public List<Path> allPaths = new List<Path>();

    Transform[] allPoints;
    PossiblePoints[] pp;

    public static bool completed;
    public static Vector3 SawPos;

    private void Awake()
    {
        instance = this;
        completed = false;
    }

    private void Start()
    {
        allPaths = new List<Path>();
        allPaths.Clear();

        allPoints = new Transform[Shape.childCount];
        pp = new PossiblePoints[allPoints.Length];
        for (int i = 0; i < allPoints.Length; i++)
        {
            allPoints[i] = Shape.GetChild(i);
            pp[i] = allPoints[i].GetComponent<PossiblePoints>();
        }

        FindPaths();

        SawPos = Vector3.zero;
        for(int i = 0; i < allPoints.Length; i++)
        {
            SawPos += allPoints[i].position;
        }
        SawPos /= allPoints.Length;

        Camera.main.transform.LookAt(SawPos);
    }

    void FindPaths()
    {
        for (int i = 0; i < allPoints.Length; i++)
        {
            for (int j = 0; j < pp[i].AllPossiblePoints.Length; j++)
            {
                Path p = new Path
                {
                    point1 = allPoints[i],
                    point2 = pp[i].AllPossiblePoints[j],
                    Index = allPaths.Count
                    
                };
                allPaths.Add(p);
            }
        }

        ClearPaths(allPaths.ToArray());
    }

    void ClearPaths(Path[] p)
    {
        Path[] p2 = new Path[p.Length];
        p2 = p;

        for(int i = 0;i < p.Length; i++)
        {
            for(int j = i + 1; j < p2.Length; j++)
            {
                CheckEquality(p[i], p2[j]);
            }
        }
    }

    void CheckEquality(Path p1, Path p2)
    {

        if (p1.point1 == p2.point1 || p1.point1 == p2.point2)
        {
            if (p1.point2 == p2.point1 || p1.point2 == p2.point2)
            {
                Path p = Array.Find(allPaths.ToArray(), item => item.Index == p2.Index);
                if(allPaths.Contains(p))
                {
                    allPaths.Remove(p);
                }
            }
        }
    }


    public bool ChechPath(Transform t1, Transform t2)
    {
        if (allPaths.Count > 0)
        {
            bool ret = false;
            for (int i = 0; i < allPaths.Count; i++)
            {
                Path p = allPaths[i];
                if (t1 == p.point1 || t1 == p.point2)
                {
                    if (t2 == p.point1 || t2 == p.point2)
                    {
                        ret = true;
                        allPaths.Remove(p);

                        if(allPaths.Count == 0)
                        {
                            completed = true;
                            //Debug.Log("You Won The Game");
                            StartCoroutine(Stagefinished());
                        }

                        if(InGamePanel.instance != null)
                        {
                            InGamePanel.instance.currentPath = allPaths.Count;
                        }
                        return ret;
                    }
                    else
                    {
                        ret = false;
                    }
                }
                else
                {
                    ret = false;
                }
            }
            return ret;
        }
        else
        {
            //Debug.Log("You Won The Game");
            StartCoroutine(Stagefinished());
            return false;
        }
    }

    IEnumerator Stagefinished()
    {
        yield return new WaitForSeconds(1);
        GameManager.instance.StageFinished();
    }

    public int CheckCollapsePoints(Transform t1, Transform t2)
    {
        int c = 0;

        for (int i = 0; i < allPaths.Count; i++)
        {
            Path p = allPaths[i];
            if (t1 == p.point1 || t1 == p.point2)
            {
                if (t2 == p.point1 || t2 == p.point2)
                {
                    c++;
                }
            }
        }

        return c;
    }

    public void RemoveUnwantedPaths(Transform t1, Transform t2, Path[] removeablePaths)
    {
        ChechPath(t1, t2);

        foreach(Path p in removeablePaths)
        {
            ChechPath(p.point1, p.point2);
        }
    }

    public void ClearLongPaths(CollapsingPaths cp)
    {
        if (cp == null || cp.removeablePaths == null || cp.removeablePaths.Length == 0)
            return;

        int totalLength = cp.removeablePaths.Length;
        int count = 0;
        foreach (Path p in cp.removeablePaths)   //For each removeable path, we have to check is not available
        {
            if (isPathAvailable(p.point1, p.point2))
                count++;
        }

        if(count == 0)     //0 means all the removeable paths are not available which means we have remove the bigger path
        {
            ChechPath(cp.Point1, cp.Point2);
        }
    }

    bool isPathAvailable(Transform t1, Transform t2)
    {
        bool result = false;
        foreach(Path p in allPaths)
        {
            if(t1 == p.point1)
            {
                if (t2 == p.point2)
                {
                    result = true;
                    break;
                }
            }
            else if(t1 == p.point2)
            {
                if(t2 == p.point1)
                {
                    result = true;
                    break;
                }
            }
        }
        return result;
    }
}
[System.Serializable]
public class Path
{
    public Transform point1;
    public Transform point2;
    [HideInInspector]
    public int Index;
}
