using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeniorMVC.Models;
namespace SeniorMVC.Controllers
{
    public class AdminDashController : Controller
    {
        // GET: Controller
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdminDashboard()
        {
            ViewBag.Title = "Admin";
            //ViewData["Username"] = u;
            return View();
        }

        public ActionResult ViewAllGameFrames()
        {
            var prox = new NashClient.NsashServicesClient();
            var twopgames = prox.ChooseGame(2);
            List<GameFrame> twopgamesmodel = new List<GameFrame>();
            for (int i = 0; i < twopgames.Count(); i++)
            {
                GameFrame temp = new GameFrame();
                temp.Title = twopgames[i].Title;
                temp.MinPlayers = twopgames[i].MinPlayers;
                temp.MaxPlayers = twopgames[i].MaxPlayers;
                temp.MinStrategies = twopgames[i].MinStrategies;
                temp.MaxStrategies = twopgames[i].MaxStrategies;
                twopgamesmodel.Add(temp);
            }
            ViewData["GameFrames"]= twopgamesmodel;
            return View(twopgamesmodel.ToList());
        }
    }
}