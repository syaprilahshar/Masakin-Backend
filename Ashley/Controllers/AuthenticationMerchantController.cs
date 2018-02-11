using Masakin.Models;
using Sinch.ServerSdk;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.IO;

namespace Masakin.Controllers
{
    public class AuthenticationMerchantController : ApiController
    {
        //Common cm = new Common();

        //[HttpPost]
        //public OutputModel CheckUser(string token, [FromBody] tblM_User usr)
        //{
        //    OutputModel output = new OutputModel();
        //    DB_MasterEntities db = new DB_MasterEntities();

        //    try
        //    {
        //        var data = db.tblM_User.Where(p =>
        //        p.Phone == usr.Phone
        //        && p.isVerified == 1).Select(p => p).FirstOrDefault();

        //        //if (data == null)
        //        //{
        //        //    data = db.tblM_User.Where(p =>
        //        //    p.Email == usr.Username
        //        //    && p.isVerified == 1).Select(p => p).FirstOrDefault();
        //        //}

        //        if (data != null)
        //        {
        //            if (data.isBanned == 0)
        //            {
        //                output.status = "failed";
        //                output.message = "User has been banned by admin";
        //                output.data = null;
        //                return output;
        //            }
        //            if (usr.Password == data.Password)
        //            {
        //                if (!db.tblM_User_DeviceID.Any(u => u.DeviceID.Contains(token) && u.UserID == data.UserID))
        //                {
        //                    var dids = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).Count();
        //                    if (dids != 0)
        //                    {
        //                        var didsz = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).SingleOrDefault();
        //                        didsz = didsz + ", " + token;
        //                        tblM_User_DeviceID update = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Single<tblM_User_DeviceID>();
        //                        update.DeviceID = didsz;
        //                        update.DateModified = DateTime.Now;
        //                        db.SaveChanges();
        //                    }
        //                    else
        //                    {
        //                        tblM_User_DeviceID add = new tblM_User_DeviceID();
        //                        add.UserID = data.UserID;
        //                        add.DeviceID = token;
        //                        add.DateModified = DateTime.Today;
        //                        db.tblM_User_DeviceID.Add(add);
        //                        db.SaveChanges();
        //                    }
        //                }
        //                output.status = "success";
        //                output.message = String.Empty;
        //                output.data = data;
        //            }
        //            else
        //            {
        //                output.status = "failed";
        //                output.message = "Password has changed";
        //                output.data = null;
        //                return output;
        //            }
        //        }
        //        else
        //        {
        //            //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
        //            output.status = "failed";
        //            output.message = "User not found";
        //            output.data = null;
        //            return output;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel Login(string deviceid, [FromBody] tblM_User usr)
        //{
        //    OutputModel output = new OutputModel();

        //    try
        //    {
        //        DB_MasterEntities db = new DB_MasterEntities();
        //        var data = db.tblM_User.Where(p =>
        //        p.Phone == usr.Phone
        //        && p.Password == usr.Password
        //        ).Select(p => p).FirstOrDefault();

        //        if (data == null)
        //        {
        //            data = db.tblM_User.Where(p =>
        //            p.Phone == usr.Phone
        //            && p.isVerified == 1
        //            && p.isActive == 1).Select(p => p).FirstOrDefault();
        //            if (data == null)
        //            {
        //                output.status = "error";
        //                output.message = "Account not found.";
        //                output.data = null;
        //                return output;
        //            }
        //            else
        //            {
        //                data = db.tblM_User.Where(p =>
        //                p.Phone == usr.Phone
        //                && p.Password == usr.Password
        //                && p.isVerified == 1
        //                && p.isActive == 1).Select(p => p).FirstOrDefault();
        //                if (data == null)
        //                {
        //                    output.status = "error";
        //                    output.message = "Wrong password.";
        //                    output.data = null;
        //                    return output;
        //                }
        //            }
        //        }

        //        if (data != null)
        //        {
        //            if (data.isActive == 1 && data.isVerified == 1)
        //            {
        //                if (!db.tblM_User_DeviceID.Any(u => u.DeviceID.Contains(deviceid) && u.UserID == data.UserID))
        //                {
        //                    var dids = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).Count();
        //                    if (dids != 0)
        //                    {
        //                        var didsz = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).SingleOrDefault();
        //                        didsz = didsz + ", " + deviceid;
        //                        tblM_User_DeviceID update = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Single<tblM_User_DeviceID>();
        //                        update.DeviceID = didsz;
        //                        update.DateModified = DateTime.Now;
        //                        db.SaveChanges();
        //                    }
        //                    else
        //                    {
        //                        tblM_User_DeviceID add = new tblM_User_DeviceID();
        //                        add.UserID = data.UserID;
        //                        add.DeviceID = deviceid;
        //                        add.DateModified = DateTime.Today;
        //                        db.tblM_User_DeviceID.Add(add);
        //                        db.SaveChanges();
        //                    }
        //                }

        //                //data.Balance = db.tblM_Balance.Where(p =>
        //                //p.UserID == data.UserID).FirstOrDefault();

        //                //if (data.isAgent == 1)
        //                //{
        //                //    data.Agent = db.tblM_User_Agent.Where(p => p.UserID == data.UserID).FirstOrDefault();
        //                //}

        //                output.status = "success";
        //                output.message = String.Empty;
        //                output.data = data;
        //                //cm.activity(Convert.ToInt32(data.UserID), "Log In");
        //            }
        //            //else if (data.isVerified == 0 && data.isActive == 0)
        //            //{
        //            //    var cd = db.tblT_User_Token.Where(p =>
        //            //        p.UserID == data.UserID
        //            //        ).Select(p => p).Single();

        //            //    string bodyEmail = "Welcome to CR. Your verification code is " + cd.VerificationCode + ".";
        //            //    string zx = sendEmail(data.Email, bodyEmail);
        //            //    if (zx == "yes")
        //            //    {
        //            //        output.status = "verify";
        //            //        output.message = "Please verify your account first.";
        //            //        output.data = cd;
        //            //    }
        //            //    else
        //            //    {
        //            //        output.status = "failed";
        //            //        output.message = "Failed to send the verification email";
        //            //        output.data = String.Empty;
        //            //    }
        //            //}
        //            else if (data.isVerified == 1 && data.isActive == 0)
        //            {
        //                output.status = "failed";
        //                output.message = "User has been banned by admin";
        //                output.data = null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel Logout([FromBody] tblM_User usr)
        //{
        //    OutputModel output = new OutputModel();

        //    try
        //    {
        //        cm.activity(Convert.ToInt32(usr.UserID), "Log Out");
        //        output.status = "success";
        //        output.message = String.Empty;
        //        output.data = null;
        //    }
        //    catch (Exception ex)
        //    {
        //        cm.activity(Convert.ToInt32(usr.UserID), "Log Out failed");
        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel RegisterWithPhone([FromBody] tblM_User usr)
        //{
        //    OutputModel output = new OutputModel();
        //    string uid = "";
        //    try
        //    {
        //        //string uname = checkUsername(usr.Username);
        //        //string em = checkEmail(usr.Email);
        //        string ph = checkPhone(usr.Phone);
        //        //if (uname == "yes")
        //        //{
        //        //    output.status = "failed";
        //        //    output.message = "Username already exist";
        //        //    output.data = String.Empty;
        //        //    return output;
        //        //}
        //        //if (em == "yes")
        //        //{
        //        //    output.status = "failed";
        //        //    output.message = "Email already exist";
        //        //    output.data = String.Empty;
        //        //    return output;
        //        //}
        //        if (ph == "yes")
        //        {
        //            output.status = "failed";
        //            output.message = "Phone number already exist";
        //            output.data = String.Empty;
        //            return output;
        //        }
        //        DB_MasterEntities db = new DB_MasterEntities();
        //        usr.isVerified = 0;
        //        usr.isActive = 0;
        //        usr.SMSCount = 1;
        //        db.tblM_User.Add(usr);
        //        db.SaveChanges();

        //        var data = db.tblM_User.Where(p => p.Phone == usr.Phone).Select(p => p).Single();

        //        uid = data.UserID.ToString();

        //        Random r = new Random();
        //        var x = r.Next(0, 1000000);
        //        string s = x.ToString("000000");

        //        DateTime exp = DateTime.Today.AddMinutes(10);
        //        tblT_User_Token temp = new tblT_User_Token();
        //        temp.UserID = data.UserID;
        //        temp.Token = s;
        //        temp.DateExpired = exp;
        //        db.tblT_User_Token.Add(temp);

        //        sendSMS(usr.Phone, usr.ActivationCode);

        //        db.SaveChanges();

        //        output.status = "success";
        //        output.message = "User Successfully Inserted";
        //        output.data = data;
        //        cm.activity(Convert.ToInt32(data.UserID), "Register success");
        //    }
        //    catch (Exception ex)
        //    {
        //        //cm.activity(Convert.ToInt32(uid), "Register failed");

        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = String.Empty;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel Register(string deviceid, [FromBody] tblM_User usr)
        //{
        //    OutputModel output = new OutputModel();
        //    string uid = "";
        //    try
        //    {
        //        string uname = checkUsername(usr.Username);
        //        string em = checkEmail(usr.Email);
        //        //string ph = checkPhone(usr.CountryCode, usr.Phone);
        //        if (uname == "yes")
        //        {
        //            output.status = "failed";
        //            output.message = "Username already exist";
        //            output.data = String.Empty;
        //            return output;
        //        }
        //        if (em == "yes")
        //        {
        //            output.status = "failed";
        //            output.message = "Email already exist";
        //            output.data = String.Empty;
        //            return output;
        //        }
        //        //if (ph == "yes")
        //        //{
        //        //    output.status = "failed";
        //        //    output.message = "Phone number already exist";
        //        //    output.data = String.Empty;
        //        //    return output;
        //        //}
        //        MasakinEntities db = new MasakinEntities();
        //        usr.DateRegistered = DateTime.Now;
        //        usr.isNewsletter = usr.isNewsletter;
        //        usr.isActived = 0;
        //        usr.isVerified = 0;
        //        usr.isAgent = 0;
        //        usr.isAdmin = 0;
        //        if (usr.isNewsletter == null) { usr.isNewsletter = 0; }
        //        db.tblM_User.Add(usr);
        //        db.SaveChanges();

        //        var data = db.tblM_User.Where(p =>
        //        p.Username == usr.Username
        //        && p.Password == usr.Password
        //        && p.Email == usr.Email
        //        //&& p.CountryCode == usr.CountryCode
        //        //&& p.Phone == usr.Phone
        //        ).Select(p => p).Single();

        //        uid = data.UserID.ToString();

        //        Random r = new Random();
        //        var x = r.Next(0, 1000000);
        //        string s = x.ToString("000000");

        //        tblM_Balance bal = new tblM_Balance();
        //        bal.UserID = data.UserID;
        //        bal.Balance = 2;
        //        bal.DateExpired = DateTime.Now.AddYears(1);
        //        db.tblM_Balance.Add(bal);

        //        tblT_User_Verification temp = new tblT_User_Verification();
        //        temp.UserID = data.UserID;
        //        temp.VerificationCode = s;
        //        temp.DateRegistered = DateTime.Now;
        //        db.tblT_User_Verification.Add(temp);

        //        if (deviceid != null)
        //        {
        //            tblM_User_DeviceID add = new tblM_User_DeviceID();
        //            add.UserID = data.UserID;
        //            add.DeviceID = deviceid;
        //            add.DateModified = DateTime.Today;
        //            db.tblM_User_DeviceID.Add(add);
        //            db.SaveChanges();
        //        }

        //        db.SaveChanges();

        //        //sendSMS(usr.CountryCode, usr.Phone, s).GetAwaiter().GetResult();

        //        string bodyEmail = "Welcome to CR. Your verification code is " + s + ".";
        //        string zx = sendEmail(data.Email, bodyEmail);
        //        if (zx == "yes")
        //        {
        //            output.status = "success";
        //            output.message = "User Successfully Inserted";
        //            output.data = data;
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "Failed to send the email";
        //            output.data = String.Empty;
        //        }

        //        var cd = db.tblT_User_Verification.Where(p =>
        //        p.UserID == data.UserID
        //        ).Select(p => p).Single();

        //        output.status = "success";
        //        output.message = "User Successfully Inserted";
        //        output.data = cd;
        //        cm.activity(Convert.ToInt32(data.UserID), "Register success");
        //    }
        //    catch (Exception ex)
        //    {
        //        cm.activity(Convert.ToInt32(uid), "Register failed");

        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = String.Empty;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel ForgotPassword([FromBody] tblM_User usr)
        //{
        //    OutputModel output = new OutputModel();
        //    string uid = "";
        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();

        //        var data = db.tblM_User.Where(p =>
        //            p.Email == usr.Email).Select(p => p).Single();

        //        uid = data.UserID.ToString();

        //        Random r = new Random();
        //        var x = r.Next(0, 1000000);
        //        string s = x.ToString("000000");

        //        var cek = db.tblM_User_ForgotPassword.Where(p => p.UserID == data.UserID).FirstOrDefault();

        //        if (cek == null)
        //        {
        //            tblM_User_ForgotPassword temp = new tblM_User_ForgotPassword();
        //            temp.UserID = data.UserID;
        //            temp.Code = s;
        //            temp.DateModified = DateTime.Now;
        //            temp.Count = 1;
        //            db.tblM_User_ForgotPassword.Add(temp);
        //        }
        //        else
        //        {
        //            cek.Count += 1;
        //            cek.Code = s;
        //            cek.DateModified = DateTime.Now;
        //        }

        //        db.SaveChanges();

        //        //sendSMS(usr.CountryCode, usr.Phone, s).GetAwaiter().GetResult();
        //        string bodyEmail = "Hello from CR. Your code for forgot password is " + s + ".";
        //        string zx = sendEmail(data.Email, bodyEmail);
        //        if (zx == "yes")
        //        {
        //            output.status = "success";
        //            output.message = "Success send the email";
        //            output.data = data;
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "Failed send the email";
        //            output.data = String.Empty;
        //        }

        //        var cd = db.tblM_User_ForgotPassword.Where(p =>
        //        p.UserID == data.UserID
        //        ).Select(p => p).Single();

        //        output.status = "success";
        //        output.message = "Success send the email";
        //        output.data = cd;
        //        //cm.activity(Convert.ToInt32(data.UserID), "Register success");
        //    }
        //    catch (Exception ex)
        //    {
        //        //cm.activity(Convert.ToInt32(uid), "Register failed");

        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = String.Empty;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel RecoveryPassword([FromBody] tblM_User usr)
        //{
        //    OutputModel output = new OutputModel();

        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();
        //        var hit = db.tblM_User.Where(p =>
        //            p.UserID == usr.UserID).Select(p => p).Count();

        //        if (hit == 1)
        //        {
        //            var data = db.tblM_User.Where(p =>
        //                p.UserID == usr.UserID).Select(p => p).FirstOrDefault();
        //            data.Password = usr.Password;
        //            db.SaveChanges();

        //            var xxx = db.tblM_User.Where(p =>
        //                p.UserID == usr.UserID).Select(p => p).FirstOrDefault();
        //            //cm.activity(Convert.ToInt32(usr.UserID), "Authentication success");

        //            output.status = "success";
        //            output.message = "Success change password";
        //            output.data = xxx;
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "Failed change password";
        //            output.data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        cm.activity(Convert.ToInt32(usr.UserID), "Authentication failed");

        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel UpdateProfile(string language, [FromBody] tblM_User usr)
        //{
        //    OutputModel output = new OutputModel();
        //    string uid = "";
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();

        //        var asdsa = db.tblM_User.Where(p => p.UserID == usr.UserID && p.Password == usr.Password).FirstOrDefault();
        //        if (asdsa != null)
        //        {
        //            asdsa.DateModified = DateTime.Now;
        //            asdsa.Fullname = usr.Fullname.Trim();
        //            asdsa.Salutation = usr.Salutation.Trim();
        //            asdsa.CountryCode = usr.CountryCode.Trim();
        //            asdsa.Gender = usr.Gender.Trim();
        //            asdsa.DoB = usr.DoB;
        //            asdsa.Country = usr.Country.Trim();
        //            asdsa.ImageURL = usr.ImageURL;
        //            asdsa.Thumbnail = usr.Thumbnail;
        //            db.SaveChanges();

        //            if (asdsa.isAgent == 1)
        //            {
        //                if (language != null)
        //                {
        //                    var langc = db.tblM_User_Agent.Where(p => p.UserID == asdsa.UserID).Count();
        //                    if (langc > 0)
        //                    {
        //                        var lang = db.tblM_User_Agent.Where(p => p.UserID == asdsa.UserID).FirstOrDefault();
        //                        lang.Language = language;
        //                        lang.DateModified = DateTime.Now;
        //                        db.SaveChanges();
        //                    }
        //                    else
        //                    {
        //                        tblM_User_Agent ag = new tblM_User_Agent();
        //                        ag.Language = language;
        //                        ag.UserID = usr.UserID;
        //                        ag.DateModified = DateTime.Now;
        //                        db.tblM_User_Agent.Add(ag);
        //                        db.SaveChanges();
        //                    }
        //                }
        //            }

        //            var data = db.tblM_User.Where(p => p.UserID == usr.UserID).FirstOrDefault();

        //            data.Balance = db.tblM_Balance.Where(p =>
        //            p.UserID == data.UserID).FirstOrDefault();

        //            data.Agent = db.tblM_User_Agent.Where(p =>
        //            p.UserID == data.UserID).FirstOrDefault();

        //            output.status = "success";
        //            output.message = "User Profile Updated";
        //            output.data = data;
        //            cm.activity(Convert.ToInt32(usr.UserID), "Profile updated");
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "User not found or password has changed";
        //            output.data = null;
        //            cm.activity(Convert.ToInt32(usr.UserID), "Profile updated failed : user id not found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //cm.activity(Convert.ToInt32(uid), "Update profile error : " + ex.ToString());

        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = String.Empty;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel UpdateImage(int uid)
        //{
        //    OutputModel output = new OutputModel();
        //    Dictionary<string, object> dict = new Dictionary<string, object>();
        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();

        //        var asdsa = db.tblM_User.Where(p => p.UserID == uid).FirstOrDefault();
        //        if (asdsa != null)
        //        {
        //            var httpRequest = HttpContext.Current.Request;
        //            int a = httpRequest.Files.Count;
        //            foreach (string file in httpRequest.Files)
        //            {
        //                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

        //                var postedFile = httpRequest.Files[file];
        //                if (postedFile != null && postedFile.ContentLength > 0)
        //                {

        //                    int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

        //                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
        //                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
        //                    var extension = ext.ToLower();
        //                    if (!AllowedFileExtensions.Contains(extension))
        //                    {

        //                        var message = string.Format("Please Upload image of type .jpg, .gif, or .png.");

        //                        output.status = "failed";
        //                        output.message = message;
        //                        output.data = null;

        //                        return output;
        //                    }
        //                    else if (postedFile.ContentLength > MaxContentLength)
        //                    {

        //                        var message = string.Format("Please Upload a file upto 1 mb.");

        //                        output.status = "failed";
        //                        output.message = message;
        //                        output.data = null;

        //                        return output;
        //                    }
        //                    else
        //                    {
        //                        var filePath = HttpContext.Current.Server.MapPath("~/UserImage/" + postedFile.FileName + extension);
        //                        postedFile.SaveAs(filePath);

        //                        var update = db.tblM_User.Where(p => p.UserID == uid).FirstOrDefault();
        //                        update.ImageURL = "/UserImage/" + postedFile.FileName + extension;
        //                        db.SaveChanges();

        //                        output.status = "success";
        //                        output.message = "User Profile updated";
        //                        output.data = null;

        //                        return output;
        //                    }
        //                }

        //                //var message1 = string.Format("Image Updated Successfully.");
        //            }
        //            //var res = string.Format("Please Upload a image.");
        //            //dict.Add("error", res);
        //            //return Request.CreateResponse(HttpStatusCode.NotFound, dict);

        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "User Profile Failed";
        //            output.data = null;
        //            cm.activity(uid, "Profile updated failed : user id not found");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //cm.activity(Convert.ToInt32(uid), "Update profile error : " + ex.ToString());

        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = String.Empty;
        //    }

        //    return output;
        //}

        //[HttpGet]
        //public OutputModel GetAllAgents()
        //{
        //    OutputModel output = new OutputModel();

        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();
        //        var data = db.tblM_User.Where(p =>
        //        p.isAgent == 1
        //        && p.isVerified == 1
        //        && p.isActived == 1).Select(p => p).ToList();

        //        for (int i = 0; i < data.Count; i++)
        //        {
        //            var user = data[i].UserID;
        //            data[i].Agent = db.tblM_User_Agent.Where(p => p.UserID == user).FirstOrDefault();
        //        }

        //        output.status = "success";
        //        output.message = null;
        //        output.data = data;
        //    }
        //    catch (Exception ex)
        //    {
        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[HttpGet]
        //public OutputModel GetDetailUser(int uid)
        //{
        //    OutputModel output = new OutputModel();

        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();
        //        var data = db.tblM_User.Where(p => p.UserID == uid
        //        && p.isVerified == 1
        //        && p.isActived == 1).Select(p => p).FirstOrDefault();

        //        data.Balance = db.tblM_Balance.Where(p =>
        //            p.UserID == uid).FirstOrDefault();

        //        if (data.isAgent == 1)
        //        {
        //            data.Agent = db.tblM_User_Agent.Where(p => p.UserID == data.UserID).FirstOrDefault();
        //        }

        //        output.status = "success";
        //        output.message = null;
        //        output.data = data;
        //    }
        //    catch (Exception ex)
        //    {
        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[HttpPost]
        //public OutputModel AuthCode([FromBody] tblT_User_Verification usr)
        //{
        //    OutputModel output = new OutputModel();

        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();
        //        var data = db.tblT_User_Verification.Where(p =>
        //        p.UserID == usr.UserID
        //        && p.VerificationCode == usr.VerificationCode).Select(p => p).Count();

        //        if (data == 1)
        //        {
        //            tblM_User update = db.tblM_User.Where(p => p.UserID == usr.UserID).Single<tblM_User>();
        //            update.isActived = 1;
        //            update.isVerified = 1;
        //            db.SaveChanges();

        //            cm.activity(Convert.ToInt32(usr.UserID), "Authentication success");

        //            var xxx = db.tblM_User.Where(p =>
        //            p.UserID == usr.UserID).Select(p => p).Single();

        //            xxx.Balance = db.tblM_Balance.Where(p =>
        //            p.UserID == xxx.UserID).FirstOrDefault();

        //            output.status = "success";
        //            output.message = "User has been verified.";
        //            output.data = xxx;
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "Verification code is not match.";
        //            output.data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        cm.activity(Convert.ToInt32(usr.UserID), "Authentication failed");

        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        ////public OutputModel ResendAuth([FromBody] tblM_User usr)
        ////{
        ////    OutputModel output = new OutputModel();
        ////    string uid = "";
        ////    try
        ////    {
        ////        MasakinEntities db = new MasakinEntities();
        ////        var data = db.tblM_User.Where(p =>
        ////        p.username.Equals(usr.username)
        ////        && p.password.Equals(usr.password)
        ////        && p.email.Equals(usr.email)
        ////        && p.country_code.Equals(usr.country_code)
        ////        && p.phone.Equals(usr.phone)).Select(p => p).Single();

        ////        uid = data.user_id.ToString();

        ////        Random r = new Random();
        ////        var x = r.Next(0, 1000000);
        ////        string s = x.ToString("000000");

        ////        tblT_User_Verification temp = new tblT_User_Verification();
        ////        temp.user_id = data.user_id;
        ////        temp.verification_code = s;
        ////        db.tblT_User_Verification.Add(temp);
        ////        db.SaveChanges();
        ////        sendSMS(usr.country_code, usr.phone, s).GetAwaiter().GetResult();
        ////        //string zx = sendEmail(data.email, s);
        ////        //if (zx == "yes")
        ////        //{
        ////        //    output.status = "success";
        ////        //    output.message = "Resend authentication code success";
        ////        //    output.data = data;
        ////        //}
        ////        //else
        ////        //{
        ////        //    output.status = "failed";
        ////        //    output.message = "Resend authentication code failed";
        ////        //    output.data = String.Empty;
        ////        //}

        ////        cm.activity(Convert.ToInt32(data.user_id), "Resend authentication code success");

        ////        output.status = "success";
        ////        output.message = "Resend authentication code success";
        ////        output.data = data;
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        cm.activity(Convert.ToInt32(uid), "Resend authentication code failed");

        ////        output.status = "error";
        ////        output.message = ex.Message;
        ////        output.data = String.Empty;
        ////    }

        ////    return output;
        ////}

        //[HttpPut]
        //public OutputModel AuthRegister([FromBody] tblT_User_Verification usr)
        //{
        //    OutputModel output = new OutputModel();

        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();
        //        var data = db.tblT_User_Verification.Where(p =>
        //        p.UserID.Equals(usr.UserID)
        //        && p.VerificationCode.Equals(usr.VerificationCode)).Select(p => p).Any();

        //        if (data == true)
        //        {
        //            tblM_User update = db.tblM_User.Where(p => p.UserID.Equals(usr.UserID)).Single<tblM_User>();
        //            update.isActived = 1;
        //            update.isVerified = 1;
        //            db.SaveChanges();

        //            cm.activity(Convert.ToInt32(usr.UserID), "Authentication success");

        //            output.status = "success";
        //            output.message = "User has been verified.";
        //            output.data = data;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        cm.activity(Convert.ToInt32(usr.UserID), "Authentication failed");

        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[Route("token/{usernameId}/{tokenId}")]
        //[HttpGet]
        //public OutputModel UpdateToken(int usernameId, string tokenId)
        //{
        //    OutputModel output = new OutputModel();

        //    try
        //    {
        //        MasakinEntities db = new MasakinEntities();
        //        var data = db.tblM_User_DeviceID.Where(p =>
        //        p.UserID == usernameId).Select(p => p).FirstOrDefault();

        //        if (data != null)
        //        {
        //            if (!db.tblM_User_DeviceID.Any(u => u.DeviceID.Contains(tokenId) && u.UserID == data.UserID))
        //            {
        //                var dids = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).Count();
        //                if (dids != 0)
        //                {
        //                    var didsz = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).SingleOrDefault();
        //                    didsz = didsz + ", " + tokenId;
        //                    tblM_User_DeviceID update = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Single<tblM_User_DeviceID>();
        //                    update.DeviceID = didsz;
        //                    update.DateModified = DateTime.Now;
        //                    db.SaveChanges();
        //                }
        //                else
        //                {
        //                    tblM_User_DeviceID add = new tblM_User_DeviceID();
        //                    add.UserID = data.UserID;
        //                    add.DeviceID = tokenId;
        //                    add.DateModified = DateTime.Today;
        //                    db.tblM_User_DeviceID.Add(add);
        //                    db.SaveChanges();
        //                }
        //            }

        //            output.status = "success";
        //            output.message = string.Empty;
        //            output.data = null;
        //            //cm.activity(Convert.ToInt32(data.UserID), "Log In");
        //        }
        //        else
        //        {
        //            //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
        //            output.status = "failed";
        //            output.message = "user not found";
        //            output.data = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[Route("balance/{usernameId}")]
        //[HttpGet]
        //public OutputModel Topup(int usernameId)
        //{
        //    OutputModel output = new OutputModel();
        //    MasakinEntities db = new MasakinEntities();
        //    try
        //    {
        //        var bal = db.tblM_Balance.Where(p => p.UserID == usernameId).FirstOrDefault();
        //        output.status = "success";
        //        output.message = "Success retrieve balance";
        //        output.data = bal;
        //    }
        //    catch (Exception ex)
        //    {
        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        //[Route("authcode/{phoneId}")]
        //[HttpGet]
        //public OutputModel ResendAuthCode(string phoneId)
        //{
        //    OutputModel output = new OutputModel();
        //    DB_MasterEntities db = new DB_MasterEntities();
        //    try
        //    {
        //        var usr = db.tblM_User.Where(p => p.Phone == phoneId).FirstOrDefault();
        //        if (usr != null)
        //        {
        //            if (usr.SMSCount > 5)
        //            {
        //                output.status = "failed";
        //                output.message = "Maaf, kami tidak bisa memproses permintaan Anda karena Anda sudah memakai fitur sms + " + usr.SMSCount + " + kali";
        //                output.data = String.Empty;
        //            }
        //            else
        //            {
        //                usr.SMSCount += 1;

        //                Random r = new Random();
        //                var x = r.Next(0, 1000000);
        //                string s = x.ToString("000000");

        //                DateTime exp = DateTime.Today.AddMinutes(10);

        //                var temp = db.tblT_User_Token.Where(p => p.UserID == usr.UserID).FirstOrDefault();
        //                temp.Token = s;
        //                temp.DateExpired = exp;
        //                db.SaveChanges();
        //                output.status = "success";
        //                output.message = "Success resend auth code";
        //                output.data = temp;
        //            }
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "User id is not found";
        //            output.data = String.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        output.status = "error";
        //        output.message = ex.Message;
        //        output.data = null;
        //    }

        //    return output;
        //}

        ////private string sendEmail(string email, string code)
        ////{
        ////    System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
        ////    mail.To.Add(email);
        ////    mail.From = new MailAddress("syaprilstudio@gmail.com", "CR Verification Code", System.Text.Encoding.UTF8);
        ////    mail.Subject = "CR Verification Code";
        ////    mail.SubjectEncoding = System.Text.Encoding.UTF8;
        ////    mail.Body = code;
        ////    mail.BodyEncoding = System.Text.Encoding.UTF8;
        ////    mail.IsBodyHtml = true;
        ////    mail.Priority = MailPriority.High;
        ////    SmtpClient client = new SmtpClient();
        ////    client.Credentials = new System.Net.NetworkCredential("syaprilstudio@gmail.com", "Arshavin23!");
        ////    client.Port = 587;
        ////    client.Host = "smtp.gmail.com";
        ////    client.EnableSsl = true;
        ////    try
        ////    {
        ////        client.Send(mail);
        ////        return "yes";
        ////        //Page.RegisterStartupScript("UserMsg", "<script>alert('Successfully Send...');if(alert){ window.location='SendMail.aspx';}</script>");
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Exception ex2 = ex;
        ////        string errorMessage = string.Empty;
        ////        while (ex2 != null)
        ////        {
        ////            errorMessage += ex2.ToString();
        ////            ex2 = ex2.InnerException;
        ////        }
        ////        return "no";
        ////        //Page.RegisterStartupScript("UserMsg", "<script>alert('Sending Failed...');if(alert){ window.location='SendMail.aspx';}</script>");
        ////    }
        ////}

        ////private string checkUsername(string uname)
        ////{
        ////    string em = "null";
        ////    MasakinEntities db = new MasakinEntities();
        ////    var data = db.tblM_User.Where(p =>
        ////    p.Username.Equals(uname)).Select(p => p).Count();
        ////    if (data != 0)
        ////    {
        ////        return em = "yes";
        ////    }
        ////    else
        ////    {
        ////        return em = "no";
        ////    }
        ////}

        ////private string checkEmail(string email)
        ////{
        ////    string em = "null";
        ////    MasakinEntities db = new MasakinEntities();
        ////    var data = db.tblM_User.Where(p =>
        ////    p.Email.Equals(email)).Select(p => p).Count();
        ////    if (data != 0)
        ////    {
        ////        return em = "yes";
        ////    }
        ////    else
        ////    {
        ////        return em = "no";
        ////    }
        ////}

        //private string checkPhone(string ph)
        //{
        //    string em = "null";
        //    DB_MasterEntities db = new DB_MasterEntities();
        //    var data = db.tblM_User.Where(p =>
        //    p.Phone.Equals(ph)).Select(p => p).Count();
        //    if (data != 0)
        //    {
        //        return em = "yes";
        //    }
        //    else
        //    {
        //        return em = "no";
        //    }
        //}

        //void sendSMS(string no, string code)
        //{
        //    string body = "Kode aktivasi anda " + code + " berlaku selama 10 menit";
        //    string url = ConfigurationManager.AppSettings["SMSURL"] + "userkey=" + ConfigurationManager.AppSettings["SMSUserKey"] + "&passkey=" + ConfigurationManager.AppSettings["SMSPassKey"] + "&nohp=" + no + "&pesan=" + body;
        //    var client = new WebClient();
        //    var content = client.DownloadString(url);
        //}

        //void sendSMSRecovery(string no, string name, string pass)
        //{
        //    string body = "Halo " + name + ", password anda : " + pass;
        //    string url = ConfigurationManager.AppSettings["SMSURL"] + "userkey=" + ConfigurationManager.AppSettings["SMSUserKey"] + "&passkey=" + ConfigurationManager.AppSettings["SMSPassKey"] + "&nohp=" + no + "&pesan=" + body;
        //    var client = new WebClient();
        //    var content = client.DownloadString(url);
        //}
    }
}