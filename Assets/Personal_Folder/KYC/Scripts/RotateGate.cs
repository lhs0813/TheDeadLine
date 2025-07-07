using UnityEngine;
using System.Collections;

public class RotateGate : MonoBehaviour
{
    public GameObject Gate1;
    public GameObject Gate2;

    public float moveDistance = 1f;   // 이동 거리
    public float moveDuration = 3f;   // 이동 시간(초)

    void Start()
    {
        
    }

    public void Activate()
    {
        StartCoroutine(MoveOverTime());
        RecordManager.Instance.StopStoryTimer();
        float clearTime = RecordManager.Instance.LoadStoryTime();
    }

    private IEnumerator MoveOverTime()
    {
        Vector3 start1 = Gate1.transform.position;
        Vector3 start2 = Gate2.transform.position;
        Vector3 end1 = start1 + Vector3.forward * moveDistance;
        Vector3 end2 = start2 + Vector3.back * moveDistance;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            Gate1.transform.position = Vector3.Lerp(start1, end1, t);
            Gate2.transform.position = Vector3.Lerp(start2, end2, t);
            yield return null;
        }
        // 정확히 마지막 위치 보정
        Gate1.transform.position = end1;
        Gate2.transform.position = end2;
    }
}
