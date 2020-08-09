using System;
using System.Drawing;

namespace Reversi.Models
{
    /// <summary>
    /// A class that defines the basic game regions for our game.
    /// </summary>
    [Serializable]
    public class SquareModel
    {
        public string SquareTextTest { get; set; } = "Square test";

        //TODO this should not be here...but it helps with the data binding when the board is created
        public static int Width { get; set; } = 30;
        public static int Height { get; set; } = 30;

        /// <summary>
        /// Index for the square location on the gameboard
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Shape of the region
        /// </summary>
        public Regionshapes Shape { get; set; }

        /// <summary>
        /// Color of the square
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Piece contained in the square
        /// </summary>
        public GamePieceModel Piece { get; set; }

        /// <summary>
        /// Determines if the square contains a game piece
        /// </summary>
        public bool HasPiece
        {
            get
            {
                if ((Piece == null) || (Piece.Shape == Pieceshapes.Undefined))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SquareModel(int index)
        {
            Index = index;
            Piece = new GamePieceModel();
            Shape = Regionshapes.Undefined;
        }
    }
}
