using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerManager : MonoBehaviour
{
    public static TutorialTriggerManager Instance;
    public List<Transform> enemyPoints;

    [System.Serializable]
    public class TutorialStep
    {
        public string triggerID;
        public GameObject uiToShow;
        public virtual void Execute()
        {

        }
    }

    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    private Dictionary<string, GameObject> _triggerUIDictionary;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // ID - UI 매핑
        _triggerUIDictionary = new Dictionary<string, GameObject>();
        foreach (var step in tutorialSteps)
        {
            if (!_triggerUIDictionary.ContainsKey(step.triggerID))
            {
                _triggerUIDictionary.Add(step.triggerID, step.uiToShow);
            }
        }
    }

    public GameObject GetUIByID(string triggerID)
    {
        if (_triggerUIDictionary.TryGetValue(triggerID, out var uiObj))
            return uiObj;
        return null;
    }


}
