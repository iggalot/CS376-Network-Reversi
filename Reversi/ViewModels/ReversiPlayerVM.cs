using GameObjects.Models;

namespace Reversi.ViewModels
{
    public class ReversiPlayerVM : BaseViewModel
    {
        private PlayerModel _reversiPlayerVM;

        public ReversiPlayerVM(PlayerModel model)
        {
            Model = model;

        }

        public string Name
        {
            get => "TestReversiName";


        }
        public PlayerModel Model
        {
            get => _reversiPlayerVM;
            set
            {
                if (value == null)
                    return;

                _reversiPlayerVM = value;

                OnPropertyChanged("Model");
            }
        }
    }
}
