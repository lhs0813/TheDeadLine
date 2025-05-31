using UnityEngine;

public interface IZombieState
{
    void Enter(ZombieBase zombie);
    void Update();
    void Exit();
}
