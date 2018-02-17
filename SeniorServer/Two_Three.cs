using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeniorServer
{
    class Two_Three
    {
        public Dictionary<string, string> Player_GameFrame_Queue = new Dictionary<string, string>();//queue of (Player.username,Game.title)
        public Dictionary<string, List<string>> Strategies = new Dictionary<string, List<string>>();//Dictionary (player.usrname , list<userstrategies>)
                                                                                                //    public Dictionary<int, int> StrategiesPointer = new Dictionary<int, int>();//index , GID
        public List<PF> playerInfo = new List<PF>();
        public List<NE_Profile> neprofiles = new List<NE_Profile>();
        public GameFrame N_Players_Game = new GameFrame();
    }
}
