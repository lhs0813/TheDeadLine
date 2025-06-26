using System.Collections;
using UnityEngine;

public class Ragdoll_Manager : MonoBehaviour
{
    public static Ragdoll_Manager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ApplyDelayedImpulse(Rigidbody rb, Vector3 force, Vector3 point)
    {
        StartCoroutine(ApplyImpulseAfterDelay(rb, force, point));
    }

    private IEnumerator ApplyImpulseAfterDelay(Rigidbody rb, Vector3 force, Vector3 point)
    {
        yield return new WaitForSeconds(0.05f);

        if (rb != null && !rb.isKinematic)
        {
            rb.AddForceAtPosition(force, point, ForceMode.Impulse);
        }
    }
}
