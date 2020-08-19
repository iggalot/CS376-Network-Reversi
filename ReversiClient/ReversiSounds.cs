using GameObjects.Models;
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
                case GameSounds.SoundClickSuccessful:
                    soundString = @"c:\Windows\Media\Windows User Account Control.wav";
                    break;
                case GameSounds.SoundFliptile:
                    soundString = @"c:\Windows\Media\chimes.wav";
                    break;
                case GameSounds.SoundTurnComplete:
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
