using UnityEngine;

public class ShowSphere : MonoBehaviour
{
    [HideInInspector]
    public Transform targetPoint;

    [Range(0,1)]
    public float LerpSpeed = .2f;

    private void Update()
    {
        if(targetPoint != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint.position, LerpSpeed);
        }
    }
}
