using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeniorMVC.NashClient;
using SeniorMVC.Models;
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
            NashClient.NsashServicesClient nash= new NashClient.NsashServicesClient();
            var NashObj = new NashClient.NsashServicesClient();
           // UserMVC user = new UserMVC();
            UserModel user = new UserModel();
            user = nash.SignIn(username, password);
            if (user != null)// this means that there is a user with those creditentials
            {
               // if(user.AdminFlag=='N')
                return View(username);
            }
            else return View();
        }

        public ActionResult Register()//string username , string password)
        {
            NashClient.NsashServicesClient nash = new NashClient.NsashServicesClient();
            NashClient.UserModel user = new NashClient.UserModel();
            //user.Username = username;
            user.Username = "asdf";
            //user.Password = password;
            user.Username = Request["Username"].ToString();
            user.Password = Request["Password"].ToString();
            user.AdminFlag = 'N';
            try
            {
                string response = nash.Register(user);
            }
            catch (Exception ex)
            {
                return View(ex.ToString());
            }
           // if (response == "done!")
                return View();
        }
    }
}