
namespace G.Globals
{
    /// <summary>
    /// Class containing all Enums.
    /// Enums are int32 by default.
    /// We override enum "name" : byte for memory optimization
    /// </summary>
    public static class Enums
    {

        /// <summary>
        /// Tells GamePlayScreen to display which realm.
        /// </summary>
        public enum Realm : byte
        {
            WorldMap,

            Level
        };

        /// <summary>
        /// LevelPlayer's move state
        /// </summary>
        public enum MoveState : byte
        {
            Idle,

            Upgrade,

            Walk,

            Run,

            Fall,

            FireFall,

            Reaction,

            FireReaction,

            Punch,

            PunchUp,

            Kick,

            ComboKick,

            FirePunch1,

            FirePunch2,

            FirePunch3,

            SpecialCombo,

            FinalCombo1,

            FinalCombo2,
        };

        /// <summary>
        /// WorldPlayer's move direction on world map
        /// </summary>
        public enum Direction : byte
        {
            South,

            SouthWest,

            West,

            NorthWest,

            North,

            NorthEast,

            East,

            SouthEast,
        }

        /// <summary>
        /// Level Player Facing direction along the X axis.
        /// </summary>
        public enum FaceDirection
        {
            Left = -1,
            Right = 1,
        }
    }
}