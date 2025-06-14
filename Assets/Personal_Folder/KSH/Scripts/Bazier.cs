using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Bazier : MonoBehaviour
{
    public float endPosRnd = 3;
    public Vector2 time;
    public float m_distanceFromStart = 6.0f; // 시작 지점을 기준으로 얼마나 꺾일지.
    public float m_distanceFromEnd = 3.0f; // 도착 지점을 기준으로 얼마나 꺾일지.
    public LayerMask LayerMask;


    Vector3[] points = new Vector3[4];
    float timerMax = 0;
    float timerCurrent = 0;
    Vector3 before;
    float beforeVelocity;

    void Start()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit , 99, LayerMask);
        Debug.Log(hit.point);
        Transform start = transform;
        Vector3  end = hit.point;


        // 끝에 도착할 시간을 랜덤으로 줌.
        timerMax = Random.Range(time.x, time.y);


        Vector3 o = end;
        o += Random.Range(-endPosRnd, endPosRnd) * Vector3.one;
        o += Random.Range(-endPosRnd, endPosRnd) * Vector3.one;
        o.y = 0;




        // 시작 지점.
        points[0] = start.position;

        // 시작 지점을 기준으로 랜덤 포인트 지정.
        points[1] = start.position +
            (m_distanceFromStart * Random.Range(-1.0f, 1.0f) * start.right) + // X (좌, 우 전체)
            (m_distanceFromStart * Random.Range(0.1f, 1.0f) * start.up) + // Y (아래쪽 조금, 위쪽 전체)
            (m_distanceFromStart * Random.Range(-1.0f, -0.8f) * start.forward); // Z (뒤 쪽만)

        // 도착 지점을 기준으로 랜덤 포인트 지정.
        points[2] = o +
            (m_distanceFromEnd * Random.Range(-1.0f, 1.0f) * Vector3.right) + // X (좌, 우 전체)
            (m_distanceFromEnd * Random.Range(0.1f, 1.0f) * Vector3.up) + // Y (위, 아래 전체)
            (m_distanceFromEnd * Random.Range(0.8f, 1.0f) * Vector3.forward); // Z (앞 쪽만)

        // 도착 지점.
        points[3] = o;

        transform.position = start.position;
        before = transform.position;
    }

    void Update()
    { 
        if (timerCurrent < timerMax)
        {
            // 경과 시간 계산.
            timerCurrent += Time.deltaTime;

            // 베지어 곡선으로 X,Y,Z 좌표 얻기.
            transform.position = new Vector3(
                CubicBezierCurve(points[0].x, points[1].x, points[2].x, points[3].x),
                CubicBezierCurve(points[0].y, points[1].y, points[2].y, points[3].y),
                CubicBezierCurve(points[0].z, points[1].z, points[2].z, points[3].z));

            beforeVelocity = 5 +  Vector3.Distance(transform.position, before) / Time.deltaTime;
        }
        else
        {
            //도착
            transform.position += transform.forward * beforeVelocity* Time.deltaTime;
        }

        transform.forward = transform.position - before;



        before = transform.position;
    }

    private float CubicBezierCurve(float a, float b, float c, float d)
    {
        float t = timerCurrent / timerMax; // (현재 경과 시간 / 최대 시간)

        float ab = Mathf.Lerp(a, b, t);
        float bc = Mathf.Lerp(b, c, t);
        float cd = Mathf.Lerp(c, d, t);

        float abbc = Mathf.Lerp(ab, bc, t);
        float bccd = Mathf.Lerp(bc, cd, t);

        return Mathf.Lerp(abbc, bccd, t);
    }
}