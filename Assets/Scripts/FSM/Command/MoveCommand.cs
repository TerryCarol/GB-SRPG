using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Command
{
    public class MoveCommand : ICommand
    {
        private Unit unit;
        private Tile targetTile;

        public MoveCommand(Unit unit, Tile targetTile)
        {
            this.unit = unit;
            this.targetTile = targetTile;
        }

        public void Execute()
        {
            var controller = unit.GetComponent<UnitController>();
            controller.MoveTo(targetTile);
        }
    }
}
