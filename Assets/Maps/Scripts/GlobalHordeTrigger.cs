using System;
using UnityEngine;

public class GlobalHordeTrigger : MonoBehaviour
{
    /// <summary>
    /// 전역 접근 가능한 Action.
    /// </summary>
    public static Action<Vector3> OnHordeTriggered;

    /// <summary>
    /// 트리거를 발동하려는 로직에서, HordeTrigger.Trigger(pos)로 트리거
    /// </summary>
    /// <param name="playerPosition"></param>
    public void Trigger(Vector3 playerPosition)
    {
        OnHordeTriggered?.Invoke(playerPosition);
    }
}
