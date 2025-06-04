using UnityEngine;

public class Item_Hovering : MonoBehaviour
{
    public float hoverAmplitude = 0.25f;   // 위아래 진폭
    public float hoverFrequency = 1f;      // 위아래 움직임 속도
    public float rotationSpeed = 60f;      // 초당 회전 속도 (degrees)

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition;
    }

    void Update()
    {
        float hoverOffset = hoverAmplitude * Mathf.Sin(Time.time * hoverFrequency * Mathf.PI * 2);

        // 부동소수점 연산 줄이기 위해 임시 변수 사용
        Vector3 newPosition = startPosition;
        newPosition.y += hoverOffset;

        transform.localPosition = newPosition;
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
