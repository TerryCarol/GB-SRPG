public interface IUnitState
{
    void Enter(Unit unit);
    void Execute(Unit unit);
    void Exit(Unit unit);
}
