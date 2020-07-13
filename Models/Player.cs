using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Models
{
    public class Player
    {
        /// <summary>
        /// The ID of our player
        /// </summary>
        public Players ID { get; set; }

        /// <summary>
        /// The name of the player
        /// </summary>
        public string Name { get; set; }

        public Player(Players id, string name)
        {
            ID = id;
            Name = name;
        }
    }
}
