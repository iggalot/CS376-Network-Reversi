namespace Reversi.Models
{
    public enum Players
    {
        UNDEFINED = -1,
        PLAYER1 = 0,
        PLAYER2 = 1
    }

    public enum Pieceshapes
    {
        UNDEFINED = -1,
        ELLIPSE = 0,
        SQUARE = 1
    }

    public enum Regionshapes
    {
        UNDEFINED = -1,
        SQUARE = 0
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
        SOUND_CLICK_SUCCESSFUL = 0,
        SOUND_FLIPTILE = 1,
        SOUND_TURN_COMPLETE = 2,
        SOUND_MOVE_ACCEPTED = 3,
        SOUND_MOVE_REJECTED = 4
    }
}
