using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SeniorMVC.Models
{
    public class GameFrameMVC
    {
        public string Title { set; get; }
        public int MinPlayers { set; get; }
        public int MaxPlayers { set; get; }
        public int MinStrategies { set; get; }
        public int MaxStrategies { set; get; }
    }
}