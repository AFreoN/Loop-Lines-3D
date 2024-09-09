using UnityEngine;

public class PopUpShape : MonoBehaviour
{
    [Range(.01f,.5f)]
    public float LerpSpeed = .1f;

    [Range(1.01f,2)]
    public float MaxScale = 1.3f;

    [HideInInspector]
    public Mesh currentMesh;

    Material mat;
    int a = 255;

    bool HaveMesh = false;

    void Start()
    {
        mat = gameObject.GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (HaveMesh)
        {

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * MaxScale, LerpSpeed);
            a = (int)Mathf.Lerp(a, 0, LerpSpeed);

            mat.SetColor("_BaseColor", new Color32(255, 255, 255, (byte)a));

            if (a == 0)
            {
                Destroy(gameObject);
            }
        }
        else if(currentMesh != null)
        {
            HaveMesh = true;
            gameObject.GetComponent<MeshFilter>().mesh = currentMesh;
        }
    }
}
