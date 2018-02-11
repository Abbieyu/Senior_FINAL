using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeniorMVC.NashClient;
namespace SeniorMVC.Controllers
{
    public class SignInController : Controller
    {
        // GET: SignIn
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(string username , string password)
        {
            var NashObj = new NashClient.NsashServicesClient();
            UserModel user = new UserModel();
           user= NashObj.SignIn(username, password);
            if (user != null)
            {
                return View();
            }
            else return View();
        }
    }
}