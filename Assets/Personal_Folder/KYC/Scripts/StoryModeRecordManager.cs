using UnityEngine;

public class StoryModeRecordManager : MonoBehaviour
{
    void Start()
    {
        RecordManager.Instance.StartStoryTimer();
    }

    private void Update()
    {

    }
}
