using Reversi.Models;
using System.Media;
using System.Threading;

namespace ReversiClient
{
    public static class ReversiSounds
    {
        public static void PlaySounds(GameSounds sound)
        {
            string soundString = "";

            // chimes.wav
            // Windows Default.wav -- click to place
            // Windows User Account Control.wav -- click to place
            // tada.wav -- successful move
            switch (sound)
            {
                case GameSounds.SOUND_CLICK_SUCCESSFUL:
                    soundString = @"c:\Windows\Media\Windows User Account Control.wav";
                    break;
                case GameSounds.SOUND_FLIPTILE:
                    soundString = @"c:\Windows\Media\chimes.wav";
                    break;
                case GameSounds.SOUND_TURN_COMPLETE:
                    soundString = @"c:\Windows\Media\tada.wav";
                    break;
                default:
                    return;
            }
            using (var soundPlayer = new SoundPlayer(soundString))
            {
                Thread.Sleep(500);
                soundPlayer.Play(); // can also use soundPlayer.PlaySync()
            }
        }

    }
}
