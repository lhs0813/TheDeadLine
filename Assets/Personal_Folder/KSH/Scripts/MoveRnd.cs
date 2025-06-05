using DunGen;
using UnityEngine;

public class MoveRnd : MonoBehaviour
{
    public GameObject target;
    public float searchRadius;
    public float Speed = 5;
    public float RandomMoveRadius = 1;
    public float RandomMoveSpeedScale = 3;

    Vector3 startPosition;
    Vector3 oldPos;
    Vector3 randomTimeOffset;


    void Start()
    {
        startPosition = transform.position;
        oldPos = startPosition;
        randomTimeOffset = Random.insideUnitSphere * 10;
    }


    void Update()
    {
        if (target == null)
            target = Info.GetCloseEnemy(gameObject, searchRadius);



        Vector3 randomOffset = GetRadiusRandomVector() * RandomMoveRadius;
        if (RandomMoveRadius > 0)
        {
            if (target != null)
            {
                var fade = Vector3.Distance(transform.position, target.transform.position) / Vector3.Distance(startPosition, target.transform.position);
                //randomOffset *= fade;
            }
        }



        var frameMoveOffset = Vector3.zero;
        if (target == null)
        {
            frameMoveOffset = (transform.forward + randomOffset) * Speed * Time.deltaTime;
        }
        else
        {
            var forwardVec = (target.transform.position - transform.position).normalized;
            var currentForwardVector = (forwardVec + randomOffset) * Speed * Time.deltaTime;
            frameMoveOffset = currentForwardVector;
        }


        transform.position = oldPos + frameMoveOffset;
        oldPos = transform.position;
    }


    Vector3 GetRadiusRandomVector()
    {
        var x = Time.time * RandomMoveSpeedScale + randomTimeOffset.x;
        var vecX = Mathf.Sin(x / 7 + Mathf.Cos(x / 2)) * Mathf.Cos(x / 5 + Mathf.Sin(x));

        x = Time.time * RandomMoveSpeedScale + randomTimeOffset.y;
        var vecY = Mathf.Cos(x / 8 + Mathf.Sin(x / 2)) * Mathf.Sin(Mathf.Sin(x / 1.2f) + x * 1.2f);

        x = Time.time * RandomMoveSpeedScale + randomTimeOffset.z;
        var vecZ = Mathf.Cos(x * 0.7f + Mathf.Cos(x * 0.5f)) * Mathf.Cos(Mathf.Sin(x * 0.8f) + x * 0.3f);


        return new Vector3(vecX, vecY, vecZ);
    }
}
