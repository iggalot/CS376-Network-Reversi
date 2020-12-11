namespace GameObjects.Models
{
    /// <summary>
    /// Enum for the players of the game
    /// </summary>
    public enum Players
    {
        Undefined = -1,
        Player1 = 0,
        Player2 = 1
    }

    /// <summary>
    /// Enum for the shape of the gamepieces
    /// </summary>
    public enum Pieceshapes
    {
        Undefined = -1,
        Ellipse = 0,
        Square = 1
    }

    /// <summary>
    /// Enum for the shape of the gameboard regions
    /// </summary>
    public enum Regionshapes
    {
        Undefined = -1,
        Square = 0
    }

    /// <summary>
    /// Direction vectors for movement direction on a board from current location
    /// </summary>
    public enum DirectionVectors
    {
        NW = 1,
        N = 2,
        NE = 3,
        W = 4,
        E = 5,
        SW = 6,
        S = 7,
        SE = 8
    }

    /// <summary>
    /// Enum for the different sounds that the client will play.
    /// </summary>
    public enum GameSounds
    {
        SoundClickSuccessful = 0,
        SoundFliptile = 1,
        SoundTurnComplete = 2,
        SoundMoveAccepted = 3,
        SoundMoveRejected = 4
    }
}
