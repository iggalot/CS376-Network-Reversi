using System.Drawing;

namespace Reversi.Models
{
    public class Square
    {
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
        public Square()
        {
            Piece = new GamePiece();
            Shape = Regionshapes.UNDEFINED;
        }
    }
}
