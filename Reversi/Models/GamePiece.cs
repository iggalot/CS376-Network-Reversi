﻿using System;

namespace Reversi.Models
{
    /// <summary>
    /// Class that governs the game pieces
    /// </summary>
    [Serializable]
    public class GamePiece
    {
        /// <summary>
        /// Owner of the piece
        /// </summary>
        public Player Owner { get; set; }

        /// <summary>
        /// The shape of the gamepiece
        /// </summary>
        public Pieceshapes Shape { get; set; }

        #region Default Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public GamePiece()
        {
            Owner = new Player();
            Shape = Pieceshapes.UNDEFINED;
        }

        public GamePiece(Pieceshapes shape, Player player)
        {
            Owner = player;
            Shape = shape;
        }

        /// <summary>
        /// Constructor for a piece of a known shape
        /// </summary>
        /// <param name="shape"></param>
        public GamePiece(Pieceshapes shape)
        {
            Shape = shape;
        }
        #endregion
    }
}
