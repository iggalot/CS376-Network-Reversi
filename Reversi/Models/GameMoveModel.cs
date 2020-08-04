using System;

namespace Reversi.Models
{
    [Serializable]
    public class GameMoveModel
    {
        /// <summary>
        /// Index of the move that was made
        /// </summary>
        public int MoveIndex { get; set; } = -1;

        /// <summary>
        /// Player who made the move
        /// </summary>
        public int ByPlayer { get; set; } = -1;

        public GameMoveModel(int player_id, int move)
        {
            MoveIndex = move;
            ByPlayer = player_id;
        }
    }
}
