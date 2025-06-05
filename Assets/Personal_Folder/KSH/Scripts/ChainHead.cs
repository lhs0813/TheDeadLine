using UnityEngine;

public class ChainHead : MonoBehaviour
{
    public GameObject bodypart;
    public float distance;
    public int num;


    GameObject before;

    void Start()
    {
        before = gameObject;

        for (int i = 0; i < num; i++)
        {
            var v = Instantiate(bodypart, transform.position, transform.rotation);
            var chain = v.GetComponent<ChainBody>();
            if (before)
            {
                chain.follow = before;
                before = v;
            }
            chain.distance = distance;
        }
    }
}
