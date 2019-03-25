using System;
using System.Collections.Generic;
using System.Text;

namespace GoTime.Models
{
    public class BoardPosition
    {
        private BoardPosition() { }

        public PositionState PositionState { get; internal set; }

        public static BoardPosition New()
        {
            return new BoardPosition();
        }

        public void SetPositionStatue(PositionState positionState)
        {
            PositionState = positionState;
        }
    }
}
