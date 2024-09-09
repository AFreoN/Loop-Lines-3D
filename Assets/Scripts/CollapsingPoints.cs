using UnityEngine;

public class CollapsingPoints : MonoBehaviour
{
    public CollapsingPaths[] _CollapsingPaths;

    #region Old Clear Path Code
    /*
    public void ClearPath()
    {
        foreach(CollapsingPaths cp in _CollapsingPaths)
        {
            int n = 0;

            if(PathsController.instance.CheckCollapsePoints(cp.Point1,cp.Point3) == 0)
            {
                PathsController.instance.RemoveUnwantedPaths(cp.Point1, cp.Point2, cp.Point3);
            }
            else
            {
                n += PathsController.instance.CheckCollapsePoints(cp.Point1, cp.Point2);
                n += PathsController.instance.CheckCollapsePoints(cp.Point1, cp.Point3);
                n += PathsController.instance.CheckCollapsePoints(cp.Point2, cp.Point3);

                if (n == 1)
                {
                    PathsController.instance.RemoveUnwantedPaths(cp.Point1, cp.Point2, cp.Point3);
                    n = 0;
                }
                else if(PathsController.instance.CheckCollapsePoints(cp.Point1, cp.Point3) == 0)
                {
                    PathsController.instance.RemoveUnwantedPaths(cp.Point1, cp.Point2, cp.Point3);
                    n = 0;
                }
            }
        }
    }
    */
    #endregion

    public void ClearPath(Transform t1, Transform t2)
    {
        foreach(CollapsingPaths cp in _CollapsingPaths)
        {
            bool isThere = false;

            if(t1 == cp.Point1)
            {
                if(t2 == cp.Point2)
                    isThere = true;
            }
            else if(t1 == cp.Point2)
            {
                if (t2 == cp.Point1)
                    isThere = true;
            }

            if(isThere)
            {
                PathsController.instance.RemoveUnwantedPaths(cp.Point1, cp.Point2, cp.removeablePaths);
                break;
            }
        }

        foreach(CollapsingPaths cp in _CollapsingPaths)
            PathsController.instance.ClearLongPaths(cp);
    }
}

[System.Serializable]
public class CollapsingPaths
{
    public Transform Point1;
    public Transform Point2;

    public Path[] removeablePaths;
}
