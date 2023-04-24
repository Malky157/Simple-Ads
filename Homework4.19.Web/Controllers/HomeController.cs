using Homework4._19.Data;
using Homework4._19.Web.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Homework4._19.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _connection = $@"data Source=.;Initial Catalog=SimpleAas;Integrated Security=true;TrustServerCertificate=True";
        public IActionResult Index()
        {
            var ur = new UserRepository(_connection);
            var avm = new AdViewModel();

            avm.Ads = ur.GetAllAds();
            if (User.Identity.IsAuthenticated)
            {
                avm.UserId = ur.GetUserId(User.Identity.Name);
            }
            return View(avm);
        }
        [Authorize]
        public IActionResult NewAd()
        {
            var ur = new UserRepository(_connection);
            var avm = new AdViewModel()
            {
                UserId = ur.GetUserId(User.Identity.Name)
            };
            return View(avm);
        }
        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var ur = new UserRepository(_connection);
            int usersId = ur.GetUserId(User.Identity.Name);
            ad.UserId = usersId;
            ur.AddAd(ad);
            return RedirectToAction("Index");
        }

        public IActionResult Login()
        {
            return Redirect("/account/login");
        }

    }
}