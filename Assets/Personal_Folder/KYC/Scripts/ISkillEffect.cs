public interface ISkillEffect
{
    void Apply(PlayerStats stats);   // 스킬을 찍을 때 적용
    void Remove(PlayerStats stats);  // 초기화할 때 제거
}
