using Command;

public class UnitMoveState : IUnitState
{
    private Tile targetTile;
    public Tile TargetTile 
    { 
        get => targetTile;
    }

    public void SetTargetTile(Tile targetTile)
    {
        this.targetTile = targetTile;
    }

    public void Enter(Unit unit)
    {
        //Debug.Log($"{unit.UnitName} entered Move State");
        ICommand moveCommand = new MoveCommand(unit, targetTile);
        CommandInvoker.Instance.SetCommand(moveCommand);
    }

    public void Execute(Unit unit)
    {
        // 실행 도중 이동 중인지를 감시할 수 있음
    }

    public void Exit(Unit unit)
    {
        CommandInvoker.Instance.ClearCommand();
        //unit.GetComponent<UnitStateController>().IsBusy = false;
    }
}
