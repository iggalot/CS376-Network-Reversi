using System;

namespace GameObjects.Models
{
    [Serializable]
    public class GameMoveModel
    {
        /// <summary>
        /// Index of the move that was made
        /// </summary>
        public int MoveIndex { get; set; }

        /// <summary>
        /// Player who made the move
        /// </summary>
        public Players ByPlayer { get; set; }

        public GameMoveModel(Players playerType, int move)
        {
            MoveIndex = move;
            ByPlayer = playerType;
        }
    }
}
