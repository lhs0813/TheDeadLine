using UnityEngine;

public class CheckRenderDisabled : MonoBehaviour
{
     GameObject Guns;

    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();

        var gunSpawner = GetComponentInChildren<GunSpawner>();
        if(gunSpawner)
            Guns = gunSpawner.transform.parent.gameObject;


        if(meshRenderer&& Guns)
        Invoke(nameof( Check),3f);
    }

    void Check()
    {
        Invoke(nameof(Check), 3f);

        Guns.active = meshRenderer.enabled;
    }
}
