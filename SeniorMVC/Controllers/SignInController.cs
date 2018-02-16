using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SeniorMVC.NashClient;
using SeniorMVC.Models;
using System.Web.Security;
using System.Data.Entity.Validation;

namespace SeniorMVC.Controllers
{
    public class SignInController : Controller
    {
        // GET: SignIn
        public ActionResult Index()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult LogIn()
        //{
        //    NashClient.NsashServicesClient nash = new NashClient.NsashServicesClient();
        //    var NashObj = new NashClient.NsashServicesClient();
        //    // UserMVC user = new UserMVC();
        //    UserModel user = new UserModel();
        //    UserModel s = new UserModel();
        //    s.Username = Request["Username"].ToString();
        //    s.Password = Request["Password"].ToString();
        //    user = nash.SignIn(s);
        //    if (user != null)// this means that there is a user with those creditentials
        //    {
        //        // if(user.AdminFlag=='N')
        //        return View();
        //    }
        //    else return View();
        //}
        //[HttpPost]
        //public ActionResult Register()//string username , string password)
        //{
        //    NashClient.NsashServicesClient nash = new NashClient.NsashServicesClient();
        //    NashClient.UserModel user = new NashClient.UserModel();
        //    //user.Username = username;
        //    //user.Username = "asdf";
        //    //user.Password = password;
        //    user.Username = Request["Username"].ToString();
        //    user.Password = Request["Password"].ToString();
        //    user.AdminFlag = 'N';
        //    try
        //    {
        //        string response = nash.Register(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        return View(ex.ToString());
        //    }
        //    // if (response == "done!")
        //    return View();
        //}

        [HttpPost]
        public ActionResult LOGIN(Models.User userr)
        {
            //if (ModelState.IsValid)  
            //{  
            if (IsValid(userr.Username, userr.Password))
            {
                FormsAuthentication.SetAuthCookie(Request["Username"].ToString(), false);
                return RedirectToAction("Dasboard", "SignIn");
            }
            else
            {
                ModelState.AddModelError("", "Login details are wrong.");
            }
            return View(userr);
        }

        public ActionResult Dashboard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult REGISTER(Models.User user)
        {
            try
            {
                //     if (ModelState.IsValid)
                //   {
                using (var db = new SeniorMVC.Models.Final_Senior_DBEntities1())
                {
                    var crypto = new SimpleCrypto.PBKDF2();
                    var encrypPass = crypto.Compute(user.Password);
                    var newUser = db.Users.Create();
                    newUser.Username = user.Username;
                    newUser.Password = encrypPass;
                    newUser.PasswordSalt = crypto.Salt;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    return RedirectToAction("LOGIN", "SignIn");
                }
                // }
            
                //else
                //{
                //    ModelState.AddModelError("", "Data is not correct");
                //}
            
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            return View();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private bool IsValid(string username, string password)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool IsValid = false;

            using (var db = new SeniorMVC.Models.Final_Senior_DBEntities1())
            {
                var user = db.Users.FirstOrDefault(u => u.Username == username);
                if (user != null)
                {
                    if (user.Password == crypto.Compute(password, user.PasswordSalt))
                    {
                        IsValid = true;
                    }
                }
            }
            return IsValid;
        }
    }
}