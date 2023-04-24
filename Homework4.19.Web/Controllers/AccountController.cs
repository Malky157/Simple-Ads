using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Homework4._19.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Homework4._19.Web.Models;

namespace Homework4._19.Web.Controllers
{
    public class AccountController : Controller
    {
        private string _connection = $@"data Source=.;Initial Catalog=SimpleAas;Integrated Security=true;TrustServerCertificate=True";
        public IActionResult Login()
        {
            var avm = new AdViewModel();
            if (TempData["Error"] != null)
            {
                avm.Error = (string)TempData["Error"];
            }                      
            return View(avm);
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var ur = new UserRepository(_connection);
            var user = ur.Login(email, password);
            if (user == null)
            {
                TempData["error"] = "Invalid password";               
                return RedirectToAction("login");
            }
            //this code logs in the current user
            var claims = new List<Claim>
                {
                    new Claim("user", email) //this will store the users email into the login cookie
                };
            HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", "user", "role")))
                    .Wait();           
            return Redirect("/home/index");
        }
        public IActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult SignUp(User user)
        {
            var ur = new UserRepository(_connection);
            ur.AddUser(user);
            return RedirectToAction("login");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            var ur = new UserRepository(_connection);
            var avm = new AdViewModel()
            {
                Ads = ur.GetAdsForUser(User.Identity.Name)
            };            
            return View(avm);
        }
        [Authorize]
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            //if (id == 0)
            //{
            //    return RedirectToAction("Index");
            //}
            var ur = new UserRepository(_connection);
            ur.DeleteAd(id);
            return Redirect("/home/index");
        }
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/home/index");
        }
    }
}
