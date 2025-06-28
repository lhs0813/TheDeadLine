// SubwayMovement.cs
using UnityEngine;

/// <summary>
/// 지하철을 일정 방향으로 이동시키는 스크립트
/// </summary>
public class SubwayMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("이동 속도 (단위: 유닛/초)")]
    public float speed = 10f;
    [Tooltip("이동 방향 (로컬 스페이스 기준)")]
    public Vector3 direction = Vector3.forward;

    void Update()
    {
        // 속도가 거의 0 이상일 때만 이동
        if (Mathf.Abs(speed) > 0.01f)
        {
            transform.Translate(direction.normalized * speed * Time.deltaTime, Space.Self);
        }
    }

    /// <summary>
    /// 런타임에 속도를 변경할 수 있는 메서드
    /// </summary>
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}
