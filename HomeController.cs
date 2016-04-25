using Facebook;
using System.Configuration;
using System.Web.Mvc;

namespace DigitalGym.Controllers
{
    public class FacebookController : Controller
    {
        public ActionResult Index()
        {
            FacebookClient fb = new FacebookClient();

            if (Request.QueryString["code"] != null)
            {
                string accessCode = Request.QueryString["code"].ToString();

                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = ConfigurationManager.AppSettings["ApplicationId"],
                    client_secret = ConfigurationManager.AppSettings["ApplicationSecret"],
                    redirect_uri = "http://localhost:58506/",
                    code = accessCode
                });

                fb.AccessToken = result.access_token;

                result = fb.Post("me/feed", new { message = "My message." }); // posting to FB
            }
            else
            {
                var loginUrl = fb.GetLoginUrl(new
                {
                    client_id = ConfigurationManager.AppSettings["ApplicationId"],
                    redirect_uri = "http://localhost:58506/",
                    response_type = "code",
                    scope = "publish_actions" // allows to post on Facebook
                });
                Response.Redirect(loginUrl.AbsoluteUri);
            }

            return View();
        }
    }
}
