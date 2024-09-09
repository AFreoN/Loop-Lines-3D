using UnityEngine;

public class ShapeEndAnimator : MonoBehaviour
{
    public float waitTime = 2f;

    float t = 0;

    Animator anim;

    void Start()
    {
        t = waitTime;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        t -= Time.deltaTime;
        if(t < 0 )
        {
            t = waitTime;
            anim.SetTrigger("finish");
            GameManager.instance.SpawnPartyPapers();
        }
    }
}
