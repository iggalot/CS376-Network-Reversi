using System;

namespace Reversi.Models
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
        public int ByPlayer { get; set; }

        public GameMoveModel(int playerId, int move)
        {
            MoveIndex = move;
            ByPlayer = playerId;
        }
    }
}
