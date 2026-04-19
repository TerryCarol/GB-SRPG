public interface IUnitState
{
    void Enter(Unit unit);
    void Execute(Unit unit);
    void Exit(Unit unit);
    void HandleInput(Unit unit, Tile targetTile);   // State 기반 입력관리부 연동
}
