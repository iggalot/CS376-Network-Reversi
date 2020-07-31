using Reversi.Models;

namespace ReversiClient.ViewModels
{
    public class ReversiGameboardViewModel
    {
        public string GameboardViewModelText { get; set; } = "My gameboard viewmodel text";
        public Board Model { get; set; }
        public ReversiGameboardViewModel(Board gameboard)
        {
            Model = gameboard;
        }
    }
}
