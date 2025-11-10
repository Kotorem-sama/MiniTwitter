using Microsoft.AspNetCore.Mvc;

namespace MiniTwitter.Controllers
{
    public class AccountController : Controller
    {
        // GET: AccountController
        public ActionResult Login()
        {
            return View();
        }

    }
}
