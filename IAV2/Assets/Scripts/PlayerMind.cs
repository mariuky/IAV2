using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.DataStructures;
using UnityEngine;

namespace Assets.Scripts.SampleMind
{
    public class PlayerMind : AbstractPathMind
    {
        public override void Repath()
        {

        }

        public override Locomotion.MoveDirection GetNextMove(BoardInfo boardInfo, CellInfo currentPos, CellInfo[] goals)
        {
            var val = Random.Range(0, 4);
            if (val == 0) return Locomotion.MoveDirection.Up;
            if (val == 1) return Locomotion.MoveDirection.Down;
            if (val == 2) return Locomotion.MoveDirection.Left;
            return Locomotion.MoveDirection.Right;
        }
    }
}