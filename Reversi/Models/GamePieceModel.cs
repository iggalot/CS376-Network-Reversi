using System;

namespace Reversi.Models
{
    /// <summary>
    /// Class that governs the game pieces
    /// </summary>
    [Serializable]
    public class GamePieceModel
    {
        /// <summary>
        /// Owner of the piece
        /// </summary>
        public PlayerModel Owner { get; set; }

        /// <summary>
        /// The shape of the gamepiece
        /// </summary>
        public Pieceshapes Shape { get; set; }

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public GamePieceModel()
        {
            Owner = new PlayerModel();
            Shape = Pieceshapes.Undefined;
        }

        public GamePieceModel(Pieceshapes shape, PlayerModel player)
        {
            Owner = player;
            Shape = shape;
        }

        /// <summary>
        /// Constructor for a piece of a known shape
        /// </summary>
        /// <param name="shape"></param>
        public GamePieceModel(Pieceshapes shape)
        {
            Shape = shape;
        }
        #endregion
    }
}
