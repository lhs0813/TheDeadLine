using UnityEngine;

public class RotateOnTrigger : MonoBehaviour
{
    public GameObject targetObject;        // 회전시킬 오브젝트
    public float openAngle = -20f;         // 열릴 때 X축 회전 각도
    private float closeAngle = 89.7f;          // 닫힐 때 X축 회전 각도
    public float rotationSpeed = 50f;      // 회전 속도

    private bool isPlayerInside = false;

    void Update()
    {
        if (targetObject == null)
            return;

        // 현재 X축 회전값 (보정 포함)
        float currentX = targetObject.transform.rotation.eulerAngles.x;
        if (currentX > 180f) currentX -= 360f;

        // 목표 회전값 결정
        float targetX = isPlayerInside ? openAngle : closeAngle;

        // 부드럽게 회전
        float newX = Mathf.MoveTowards(currentX, targetX, rotationSpeed * Time.deltaTime);
        Vector3 newRotation = new Vector3(
            newX,
            targetObject.transform.rotation.eulerAngles.y,
            targetObject.transform.rotation.eulerAngles.z
        );
        targetObject.transform.rotation = Quaternion.Euler(newRotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}
