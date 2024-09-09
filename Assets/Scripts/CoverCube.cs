using UnityEngine;

public class CoverCube : MonoBehaviour
{
    bool Cover = false;

    Vector3 direction;
    Vector3 SpawnPoint;
    Vector3 EndPoint;

    Vector3 HoldingPosition;

    const float extensionDistance = .03f;

    public void StartCovering(Vector3 Direction, Vector3 SpawnPos, Vector3 EndPos)
    {
        direction = Direction;

        direction = new Vector3(Mathf.Abs(direction.x), Mathf.Abs(direction.y), Mathf.Abs(direction.z));
        //direction = direction * 1.1f;
        direction.x = direction.x < .9f ? 0 : direction.x + 0.1f;
        direction.y = direction.y < .9f ? 0 : direction.y + 0.1f;
        direction.z = direction.z < .9f ? 0 : direction.z + 0.1f;

        if (direction.x < .9f)
        {
            direction.x = .12f;
        }
        if (direction.y < .9f)
        {
            direction.y = .12f;
        }
        if (direction.z < .9f)
        {
            direction.z = .12f;
        }

        #region newcodetest
        Vector3 normDirection = direction.normalized;
        direction = getDirectionMultiplied(direction);
        SpawnPoint = SpawnPos - normDirection * .15f;
        EndPoint = EndPos + normDirection * .15f;
        #endregion

        SpawnPoint = SpawnPos;
        EndPoint = EndPos;
        GetHoldingPos();
        Cover = true;
    }

    private void Update()
    {
        if(Cover)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, direction, .2f);
            transform.position = Vector3.Lerp(transform.position, HoldingPosition,.2f);

            if(Vector3.Distance(transform.localScale , direction) < .1f)
            {
                transform.localScale = direction;
                transform.position = HoldingPosition;
                Destroy(gameObject.GetComponent<CoverCube>());
            }
        }
    }

    void GetHoldingPos()
    {
        if(Mathf.Abs(SpawnPoint.x - EndPoint.x) < .5f)
        {
            HoldingPosition.x = SpawnPoint.x;
        }
        else
        {
            HoldingPosition.x = (EndPoint.x + SpawnPoint.x)/2;
        }

        if ( Mathf.Abs(SpawnPoint.y - EndPoint.y) <.5f)
        {
            HoldingPosition.y = SpawnPoint.y;
        }
        else
        {
            HoldingPosition.y = (EndPoint.y + SpawnPoint.y)/2;
        }

        if (Mathf.Abs(SpawnPoint.z - EndPoint.z) < .5f)
        {
            HoldingPosition.z = SpawnPoint.z;
        }
        else
        {
            HoldingPosition.z = (EndPoint.z + SpawnPoint.z) / 2;
        }
    }

    Vector3 getDirectionMultiplied(Vector3 d)
    {
        Vector3 result = Vector3.zero;

        float maxMag = float.MinValue;
        Vector3 dir = Vector3.right; //x = 0; y =1; z = 2;
        if (d.x > maxMag)
        {
            dir = Vector3.right;
            maxMag = d.x;
        }
        if (d.y > maxMag)
        {
            dir = Vector3.up;
            maxMag = d.y;
        }
        if(d.z > maxMag)
        {
            dir = Vector3.forward;
            maxMag = d.z;
        }

        result = d + dir * extensionDistance;
        return result;
    }
}
