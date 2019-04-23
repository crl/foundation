using foundation;

namespace gameSDK
{
    public class ActorMoveEventX:EventX
    {
        new public const string START = "move_start";
        public const string NEXT_STEP = "next_step";
        public const string ANIMATOR_MOVE = "animator_move";
        public const string REACHED = "reached";

        /// <summary>
        /// 
        /// </summary>
        public const string TERMINATE = "terminate";

        public const string TICK = "tick";
        public ActorMoveEventX(string type, object data = null) : base(type, data, false)
        {
            
        }

    }
}