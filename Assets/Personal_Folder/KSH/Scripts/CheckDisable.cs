using UnityEngine;

public class CheckRenderDisabled : MonoBehaviour
{
     GameObject Guns;

    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        Guns = GetComponentInChildren<GunSpawner>().transform.parent.gameObject;


        Invoke(nameof( Check),3f);
    }

    void Check()
    {
        Invoke(nameof(Check), 3f);

        Guns.active = meshRenderer.enabled;
    }
}
