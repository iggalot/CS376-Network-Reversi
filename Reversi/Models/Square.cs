using System;
using System.Drawing;

namespace Reversi.Models
{
    /// <summary>
    /// A class that defines the basic game regions for our game.
    /// </summary>
    [Serializable]
    public class Square
    {
        public string SquareTextTest { get; set; } = "Square test";

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
        public GamePiece Piece { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Square(int index)
        {
            Index = index;
            Piece = new GamePiece();
            Shape = Regionshapes.UNDEFINED;
        }
    }
}
