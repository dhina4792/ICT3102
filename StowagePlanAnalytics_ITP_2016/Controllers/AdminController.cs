using System.Web.Mvc;

namespace StowagePlanAnalytics_ITP_2016.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}