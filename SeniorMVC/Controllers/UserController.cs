using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SeniorMVC.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserDashboard()
        {
            var prox = new NashClient.NsashServicesClient();
            List < string > p1strats= new List<string>();
            p1strats.Add("s1");
            p1strats.Add("st1");
            List<string> p2strats = new List<string>();
            p2strats.Add("s2");
            p2strats.Add("st2");
            int p1order = prox.JoinGame("Split&Steal", "Sherlock",p1strats.ToArray());
            int p2order = prox.JoinGame("Split&Steal", "John",p2strats.ToArray());
            var player1 = new NashClient.PF();
            var player2 = new NashClient.PF();
            player1 = prox.CheckGameStatus("Split&Steal", "Sherlock");
            player2 = prox.CheckGameStatus("Split&Steal", "John");
            List<NashClient.NE_Profile> maxpayoff = new List<NashClient.NE_Profile>();
            List<NashClient.NE_Profile> maxpayoff2 = new List<NashClient.NE_Profile>();
            Dictionary<NashClient.NE_Profile, bool> nepref = new Dictionary<NashClient.NE_Profile, bool>();
            prox.PreferencesGetter("o3 > o1 > o2 = o4", "a", maxpayoff.ToArray());
            nepref = prox.PreferencesGetter("o2 > o1 > o3 = o4", "Majoodi", maxpayoff2.ToArray());
            //List<NashClient.NE_Profile>
            return View();
        }
    }
}