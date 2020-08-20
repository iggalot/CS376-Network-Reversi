using ClientServerLibrary;
using GameObjects.Models;
using Reversi.Models;

namespace Reversi.ViewModels
{
    public class ReversiPlayerVM : BaseViewModel
    {
        private PlayerModel _reversiPlayerModel;

        public ReversiPlayerVM(PlayerModel model)
        {
            Model = model;

        }

        public string Name
        {
            get => Model.Name;
            set
            {
                if ((value == null) || (value == Model.Name))
                    return;

                Model.Name = value;

                OnPropertyChanged("Name");
            }
        }

        public Players IdType
        {
            get => Model.IdType;
            set
            {
                if (value == Model.IdType)
                    return;

                Model.IdType = value;

                OnPropertyChanged("IdType");
            }
        }

        public int PlayerId
        {
            get => Model.PlayerId;
            set
            {
                if (value == Model.PlayerId)
                    return;

                Model.PlayerId = value;

                OnPropertyChanged("PlayerId");
            }
        }

        public PlayerModel Model
        {
            get => _reversiPlayerModel;
            set
            {
                if (value == null)
                    return;

                _reversiPlayerModel = value;

                OnPropertyChanged("Model");
            }
        }

        public ReversiPlayerVM(ClientModel clientModel)
        {
            ReversiClientModel reversiClientModel = (ReversiClientModel) clientModel;

            Model = reversiClientModel.ClientPlayer;
        }
    }
}
