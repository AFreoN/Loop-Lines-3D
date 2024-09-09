using UnityEngine;

public class PointsConvertor : MonoBehaviour
{
    public Transform p1, p2;

    public float Angle;

    Vector2 s1, s2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        s1 = Camera.main.WorldToScreenPoint(p1.position);
        s2 = Camera.main.WorldToScreenPoint(p2.position);

        Angle = Vector2.Angle(s1 - s2, Vector2.up);
    }
}
