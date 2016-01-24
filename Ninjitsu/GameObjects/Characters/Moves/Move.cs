#region Using Statements
using Microsoft.Xna.Framework.Input;
using G.Globals;
#endregion

namespace G.GameObjects.Characters.Moves
{
    /// <summary>
    /// Describes a sequences of buttons which must be pressed to active the move.
    /// A real game might add a virtual PerformMove() method to this class.
    /// </summary>
    public class Move
    {
        public Enums.MoveState State;

        // The sequence of button presses required to activate this move.
        public Buttons[] Sequence;

        // Set this to true if the input used to activate this move may
        // be reused as a component of longer moves.
        public bool IsSubMove;

        public Move(Enums.MoveState state, params Buttons[] sequence)
        {
            State = state;
            Sequence = sequence;
        }
    }
}
