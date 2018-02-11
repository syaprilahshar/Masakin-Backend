using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Masakin.Models
{
    public class SessionLib
    {
        // private constructor
        private SessionLib()
        {
            ID = string.Empty;
            Username = string.Empty;
            Fullname = string.Empty;
            Email = string.Empty;
            HP = string.Empty;
            Country = string.Empty;
            Role = string.Empty;
            Password = string.Empty;
        }
        public static SessionLib Current
        {
            get
            {
                SessionLib session =
                  (SessionLib)HttpContext.Current.Session["__MySession__"];
                if (session == null)
                {
                    session = new SessionLib();
                    HttpContext.Current.Session["__MySession__"] = session;
                }
                return session;
            }
        }
        public string ID { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string HP { get; set; }
        public string Country { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
    }
}