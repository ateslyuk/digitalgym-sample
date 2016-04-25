using Facebook;
using Model;
using System.Configuration;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;

namespace DigitalGym.Controllers
{
    [Authorize]
    public class FacebookTokensController : ApiController
    {
        private DigitalgymDBEntities db = new DigitalgymDBEntities();

        [ResponseType(typeof(FacebookToken))]
        [Route("Facebook/GrantPermission")]
        public IHttpActionResult GrantPermission() // gets access token and saves it to the DB
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var client = new FacebookClient();
            // getting access token (user should be logged in Facebook)
            dynamic result = client.Get("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["ApplicationId"], // ApplicationId and ApplicationSecret are stored in web.config file
                client_secret = ConfigurationManager.AppSettings["ApplicationSecret"],
                grant_type = "client_credentials"
            });
            FacebookToken token = new FacebookToken();
            token.Username = User.Identity.Name;
            token.AccessToken = result["access_token"];
                        
            db.FacebookToken.Add(token);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (FacebookTokenExists(token.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = token.Id }, token);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FacebookTokenExists(int id)
        {
            return db.FacebookToken.Count(e => e.Id == id) > 0;
        }
    }
}