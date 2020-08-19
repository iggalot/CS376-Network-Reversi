using System;

namespace GameObjects.Models
{
    /// <summary>
    /// The class defining the gameboard characteristics.
    /// </summary>
    [Serializable]
    public class BoardModel
    {

        #region Public Properties
        /// <summary>
        /// the collection of squares that make up the gameboard
        /// </summary>
        public SquareModel[] Squares { get; set; }

        /// <summary>
        /// Rows in the board
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Number of columns on the board
        /// </summary>
        public int Cols { get; set; }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a piece at the specified index
        /// </summary>
        /// <param name="index">The index on the gameboard</param>
        /// <param name="piece">The piece to add</param>
        public void AddPiece(int index, GamePieceModel piece) {
            Squares[index].Piece = piece;
        }

        /// <summary>
        /// Removes a piece at the specified index
        /// </summary>
        /// <param name="index">The index on the gameboard</param>
        public void RemovePiece(int index) {
            Squares[index].Piece = null;
        }

        /// <summary>
        /// Moves a piece from one square to another
        /// </summary>
        /// <param name="from">Moving from index...</param>
        /// <param name="to">Moving to index...</param>
        public void MovePiece(int from, int to) {
            Squares[to].Piece = Squares[from].Piece;
            Squares[from].Piece = null;
        }

        /// <summary>
        /// Draws the gameboard and its pieces
        /// </summary>
        public string DrawGameboardString() {
            string str = String.Empty;
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var index = i * Cols + j;

                    // If there's no piece...
                    if (Squares[index].Piece == null)
                    {
                        str += " - ";
                        continue;
                    }

                    // Otherwise determine the owner of the piece
                    switch(Squares[index].Piece.Owner.IdType)
                    {
                        case Players.Player1:
                            {
                                str += " 1 ";
                                break;
                            }
                        case Players.Player2:
                            {
                                str += " 2 ";
                                break;
                            }

                        case Players.Undefined:
                            str += " Undefined ";
                            break;

                        default:
                            str += " ERROR in Enum ";
                            break;
                    }
                }
                str += "\n";
            }
            str += "\n";
            //MessageBox.Show(str);
            return str;
        }


        /// <summary>
        /// Unpackets a gameboard packet string and reinserts the new line characters for displaying
        /// /// Packet:  NumCols ### Board Info 
        /// </summary>
        /// <param name="gameboardPacketString">The gameboard string to unpacket</param>
        /// <returns></returns>
        public static string UnpackGameboardPacketString(string gameboardPacketString)
        {
            string[] separateStrings = { "~~~" }; // The packet data separators
            string[] words = gameboardPacketString.Split(separateStrings, StringSplitOptions.None);

           
            int cols = int.Parse(words[0]);
            string gameboardString = words[1];

            string newstr = "";

            for(int i=0; i< gameboardString.Length;i++)
            {
                string temp = "";

                if(i % cols == 0)
                {
                    temp += "\n";
                }
                temp += " " + gameboardString[i] + " ";

                newstr += temp;
            }
            newstr += "\n";

           // MessageBox.Show(newstr);

            return newstr;
        }


        /// <summary>
        /// Writes the string to be used by a packet for this gameboard
        /// Packet:  NumCols ### Board Info 
        /// </summary>
        public string CreateGameboardPacketString()
        {
            string str = "";

            // Add the number of columns to the board data information
            str += Cols.ToString();
            str += "~~~"; // Information divider

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var index = i * Cols + j;

                    // If there's no piece...
                    if (Squares[index].Piece == null)
                    {
                        str += "-";
                        continue;
                    }

                    // Otherwise determine the owner of the piece
                    switch (Squares[index].Piece.Owner.IdType)
                    {
                        case Players.Player1:
                            {
                                str += "1";
                                break;
                            }
                        case Players.Player2:
                            {
                                str += "2";
                                break;
                            }

                        default:
                            str += "?";
                            break;
                    }
                }
            }
            str += "~~~"; // Information divider needed at the end of the string

            //MessageBox.Show(str);
            return str;
        }


        /// <summary>
        /// Returns an index on the gameboard based on a row or column offset
        /// </summary>
        /// <param name="index">The initial index</param>
        /// <param name="rowOffset">The row offset</param>
        /// <param name="colOffset">The column offset</param>
        /// <returns></returns>
        public int GetIndexByOffsets(int index, int rowOffset, int colOffset)
        {
            // determine current row and column from index
            int row = index / Cols;
            int col = index % Cols;

            int newRow = row + rowOffset;
            int newCol = col + colOffset;

            // If we are out of bounds on the rows
            if ((newRow < 0) || (newRow >= Rows))
                return -1;

            // If we are out of bounds on the columns
            if ((newCol < 0) || (newCol >= Cols))
                return -1;

            // Otherwise, return the value;
            return index + rowOffset * Cols + colOffset;
        }

        /// <summary>
        /// Returns a new index location based on a direction on the gameboard.
        /// If an invalid direction is determined, a position of -1 is return;
        /// </summary>
        /// <param name="index">The source location</param>
        /// <param name="dv">A direction vector from source location <see cref="DirectionVectors"/></param>
        /// <returns></returns>
        public int GetIndexByOffsets(int index, DirectionVectors dv)
        {
            int newIndex;
            switch (dv)
            { 
                case DirectionVectors.NW:
                    {
                        newIndex = GetIndexByOffsets(index, -1, -1);
                        break;
                    }
                case DirectionVectors.N:
                    {
                        newIndex = GetIndexByOffsets(index, -1, 0);
                        break;
                    }
                case DirectionVectors.NE:
                    {
                        newIndex = GetIndexByOffsets(index, -1, 1);
                        break;
                    }
                case DirectionVectors.W:
                    {
                        newIndex = GetIndexByOffsets(index, 0, -1);
                        break;
                    }
                case DirectionVectors.E:
                    {
                        newIndex = GetIndexByOffsets(index, 0, 1);
                        break;
                    }
                case DirectionVectors.SW:
                    {
                        newIndex = GetIndexByOffsets(index, 1, -1);
                        break;
                    }
                case DirectionVectors.S:
                    {
                        newIndex = GetIndexByOffsets(index, 1, 0);
                        break;
                    }
                case DirectionVectors.SE:
                    {
                        newIndex = GetIndexByOffsets(index, 1, 1);
                        break;
                    }
                default:
                    newIndex = -1;
                    break;
            }

//            if (newIndex == -1)
//                MessageBox.Show(index + " is out of bounds in direction: " + dv.ToString());

            return newIndex;
        }
         #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="cols">Number of columns on the gameboard</param>
        /// <param name="rows">Number of rows on the gameboard</param>
        public BoardModel(int cols, int rows)
        {
            // Save our board parameters
            Cols = cols;
            Rows = rows;

            var numSquares = Rows * Cols;

            // Create our collection of game squares
            Squares = new SquareModel[numSquares];

            // Create a game square object for each index
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    var index = i * Cols + j;
                    var square = new SquareModel(index)
                    {
                        // no piece in the board originally
                        Piece = null
                    };

                    Squares[index] = square;
                }                
            }
        }
        #endregion

        public string DisplayGameboardStats()
        {
            string str = string.Empty;
            str += "Gameboard (" + Rows*Cols + " squares): " + Rows.ToString() + " rows by " + Cols.ToString() + Cols.ToString() + "\n";
            return str;
        }
    }
}
