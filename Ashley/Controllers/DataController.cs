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
    public class DataController : ApiController
    {
        Common cm = new Common();
        DB_MasterEntities db = new DB_MasterEntities();

        [Route("provinsi")]
        [HttpGet]
        public OutputModel GetProvinsi()
        {
            OutputModel output = new OutputModel();

            try
            {
                var prov = db.tblM_Provinsi.Where(p => p.isActive == 1).ToList();
                if (prov != null)
                {
                    output.status = "success";
                    output.message = "Success retrieve provinsi data";
                    output.data = prov;
                }
                else
                {
                    output.status = "failed";
                    output.message = "No provinsi available";
                    output.data = String.Empty;
                }
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }

            return output;
        }

        [Route("kota/{provinsiId}")]
        [HttpGet]
        public OutputModel GetKota(int provinsiId)
        {
            OutputModel output = new OutputModel();

            try
            {
                var kota = db.tblM_Kabupaten.Where(p => p.ProvinsiID == provinsiId && p.isActive == 1).ToList();
                if (kota != null)
                {
                    output.status = "success";
                    output.message = "Success retrieve kota data";
                    output.data = kota;
                }
                else
                {
                    output.status = "failed";
                    output.message = "No kota available";
                    output.data = String.Empty;
                }
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }

            return output;
        }



        [HttpPost]
        public OutputModel CheckUser(string token, [FromBody] tblM_User usr)
        {
            OutputModel output = new OutputModel();
            
            try
            {
                var data = db.tblM_User.Where(p =>
                p.Phone == usr.Phone
                && p.isVerified == 1).Select(p => p).FirstOrDefault();

                if (data != null)
                {
                    if (data.isBanned == 0)
                    {
                        output.status = "failed";
                        output.message = "User has been banned by admin";
                        output.data = null;
                        return output;
                    }
                    if (usr.Password == data.Password)
                    {
                        if (!db.tblM_User_DeviceID.Any(u => u.DeviceID.Contains(token) && u.UserID == data.UserID))
                        {
                            var dids = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).Count();
                            if (dids != 0)
                            {
                                var didsz = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).SingleOrDefault();
                                didsz = didsz + ", " + token;
                                tblM_User_DeviceID update = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Single<tblM_User_DeviceID>();
                                update.DeviceID = didsz;
                                update.DateModified = DateTime.Now;
                                db.SaveChanges();
                            }
                            else
                            {
                                tblM_User_DeviceID add = new tblM_User_DeviceID();
                                add.UserID = data.UserID;
                                add.DeviceID = token;
                                add.DateModified = DateTime.Now;
                                db.tblM_User_DeviceID.Add(add);
                                db.SaveChanges();
                            }
                        }
                        output.status = "success";
                        output.message = String.Empty;
                        output.data = data;
                    }
                    else
                    {
                        output.status = "failed";
                        output.message = "Password has changed";
                        output.data = null;
                        return output;
                    }
                }
                else
                {
                    //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
                    output.status = "failed";
                    output.message = "User not found";
                    output.data = null;
                    return output;
                }
            }
            catch (Exception ex)
            {
                //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }

            return output;
        }

        [HttpPost]
        public OutputModel Login(string deviceid, [FromBody] tblM_User usr)
        {
            OutputModel output = new OutputModel();

            try
            {
                var data = db.tblM_User.Where(p =>
                p.Phone == usr.Phone
                && p.Password == usr.Password
                ).Select(p => p).FirstOrDefault();

                if (data == null) {
                    data = db.tblM_User.Where(p =>
                    p.Phone == usr.Phone
                    && p.isVerified == 1
                    && p.isActive == 1).Select(p => p).FirstOrDefault();
                    if (data == null)
                    {
                        output.status = "error";
                        output.message = "Account not found.";
                        output.data = null;
                        return output;
                    }
                    else
                    {
                        data = db.tblM_User.Where(p =>
                        p.Phone == usr.Phone
                        && p.Password == usr.Password
                        && p.isVerified == 1
                        && p.isActive == 1).Select(p => p).FirstOrDefault();
                        if (data == null) {
                            output.status = "error";
                            output.message = "Wrong password.";
                            output.data = null;
                            return output;
                        }
                    }
                }

                if (data != null)
                {
                    if (data.isActive == 1 && data.isVerified == 1)
                    {
                        if (!db.tblM_User_DeviceID.Any(u => u.DeviceID.Contains(deviceid) && u.UserID == data.UserID))
                        {
                            var dids = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).Count();
                            if (dids != 0)
                            {
                                var didsz = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Select(p => p.DeviceID).SingleOrDefault();
                                didsz = didsz + ", " + deviceid;
                                tblM_User_DeviceID update = db.tblM_User_DeviceID.Where(p => p.UserID == data.UserID).Single<tblM_User_DeviceID>();
                                update.DeviceID = didsz;
                                update.DateModified = DateTime.Now;
                                db.SaveChanges();
                            }
                            else
                            {
                                tblM_User_DeviceID add = new tblM_User_DeviceID();
                                add.UserID = data.UserID;
                                add.DeviceID = deviceid;
                                add.DateModified = DateTime.Now;
                                db.tblM_User_DeviceID.Add(add);
                                db.SaveChanges();
                            }
                        }

                        output.status = "success";
                        output.message = String.Empty;
                        output.data = data;
                        //cm.activity(Convert.ToInt32(data.UserID), "Log In");
                    }
                    else if (data.isVerified == 0 && data.isActive == 0)
                    {
                        var cd = db.tblT_User_Token.Where(p =>
                            p.UserID == data.UserID
                            ).Select(p => p).Single();

                        Random r = new Random();
                        var x = r.Next(0, 1000000);
                        string s = x.ToString("000000");

                        cd.Token = s;
                        cd.DateExpired = DateTime.Now.AddMinutes(10);

                        db.SaveChanges();

                        //sendSMS(data.Phone, s);

                        output.status = "verify";
                        output.message = "Please verify your account first.";
                        output.data = cd;
                    }
                    else if (data.isVerified == 1 && data.isActive == 0)
                    {
                        output.status = "banned";
                        output.message = "User has been banned by admin";
                        output.data = null;
                    }
                }
            }
            catch (Exception ex)
            {
                //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }

            return output;
        }

        [HttpPost]
        public OutputModel Logout([FromBody] tblM_User usr)
        {
            OutputModel output = new OutputModel();

            try
            {
                cm.activity(Convert.ToInt32(usr.UserID), "Log Out");
                output.status = "success";
                output.message = String.Empty;
                output.data = null;
            }
            catch (Exception ex)
            {
                cm.activity(Convert.ToInt32(usr.UserID), "Log Out failed");
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }

            return output;
        }

        [HttpPost]
        public OutputModel RegisterWithPhone(string deviceid, [FromBody] tblM_User usr)
        {
            OutputModel output = new OutputModel();
            string uid = "";
            try
            {
                string ph = checkPhone(usr.Phone);
                if (ph == "yes")
                {
                    output.status = "failed";
                    output.message = "Phone number already exist";
                    output.data = String.Empty;
                    return output;
                }
                usr.isVerified = 0;
                usr.isActive = 0;
                usr.SMSCount = 1;
                usr.DateJoin = DateTime.Now;
                db.tblM_User.Add(usr);
                db.SaveChanges();

                var data = db.tblM_User.Where(p => p.Phone == usr.Phone).Select(p => p).Single();

                uid = data.UserID.ToString();

                if (deviceid != null)
                {
                    tblM_User_DeviceID add = new tblM_User_DeviceID();
                    add.UserID = data.UserID;
                    add.DeviceID = deviceid;
                    add.DateModified = DateTime.Now;
                    db.tblM_User_DeviceID.Add(add);
                    db.SaveChanges();
                }

                Random r = new Random();
                var x = r.Next(0, 1000000);
                string s = x.ToString("000000");

                DateTime exp = DateTime.Now.AddMinutes(10);
                tblT_User_Token temp = new tblT_User_Token();
                temp.UserID = data.UserID;
                temp.Token = s;
                temp.DateExpired = exp;
                db.tblT_User_Token.Add(temp);

                //sendSMS(usr.Phone, temp.Token);

                db.SaveChanges();

                output.status = "success";
                output.message = "User Successfully Inserted";
                output.data = temp;
                cm.activity(Convert.ToInt32(data.UserID), "Register success");
            }
            catch (Exception ex)
            {
                //cm.activity(Convert.ToInt32(uid), "Register failed");

                output.status = "error";
                output.message = ex.Message;
                output.data = String.Empty;
            }

            return output;
        }

        [HttpPost]
        public OutputModel RecoveryPassword(string ph)
        {
            OutputModel output = new OutputModel();
            
            try
            {
                tblM_User sds = new tblM_User();
                var data = db.tblM_User.Where(p => p.Phone == ph).Select(p => p).Single();

                if (data != null)
                {
                    if (data.SMSCount < 5)
                    {
                        sendSMSRecovery(data.Phone, data.Name, data.Password);

                        output.status = "success";
                        output.message = String.Empty;
                        output.data = null;
                        //output.data = data;
                        //cm.activity(Convert.ToInt32(data.UserID), "Recovery password");
                        data.SMSCount += 1;
                        db.SaveChanges();
                    }
                    else
                    {
                        output.status = "failed";
                        output.message = "Kuota fitur sms Anda sudah habis";
                        output.data = null;
                    }
                }
                else
                {
                    output.status = "error";
                    output.message = "Gagal mengambil password";
                    output.data = null;
                }
            }
            catch (Exception ex)
            {
                //cm.activity(Convert.ToInt32(usr.UserID), "Log In failed");
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }
            return output;
        }

        [HttpPost]
        public OutputModel UpdateProfile([FromBody] tblM_User usr)
        {
            OutputModel output = new OutputModel();
            try
            {
                var asdsa = db.tblM_User.Where(p => p.Phone == usr.Phone && p.Password == usr.Password).FirstOrDefault();
                if (asdsa != null)
                {
                    asdsa.Name = usr.Name.Trim();
                    asdsa.Gender = usr.Gender.Trim();
                    asdsa.DoB = usr.DoB;
                    asdsa.ImageURL = usr.ImageURL;
                    asdsa.ThumbnailURL = usr.ThumbnailURL;
                    db.SaveChanges();

                    var data = db.tblM_User.Where(p => p.UserID == usr.UserID).FirstOrDefault();

                    output.status = "success";
                    output.message = "User Profile Updated";
                    output.data = data;
                    //cm.activity(Convert.ToInt32(usr.UserID), "Profile updated");
                }
                else
                {
                    output.status = "failed";
                    output.message = "User not found or password has changed";
                    output.data = null;
                    //cm.activity(Convert.ToInt32(usr.UserID), "Profile updated failed : user id not found");
                }
            }
            catch (Exception ex)
            {
                //cm.activity(Convert.ToInt32(uid), "Update profile error : " + ex.ToString());

                output.status = "error";
                output.message = ex.Message;
                output.data = String.Empty;
            }

            return output;
        }

        [HttpGet]
        public OutputModel GetDetailUser(int uid)
        {
            OutputModel output = new OutputModel();
            
            try
            {
                var data = db.tblM_User.Where(p => p.UserID == uid
                && p.isVerified == 1
                && p.isActive == 1).Select(p => p).FirstOrDefault();

                output.status = "success";
                output.message = null;
                output.data = data;
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }

            return output;
        }

        [HttpPost]
        public OutputModel AuthCode([FromBody] tblT_User_Token tkn)
        {
            OutputModel output = new OutputModel();

            try
            {
                var data = db.tblT_User_Token.Where(p =>
                p.UserID == tkn.UserID
                && p.Token == tkn.Token).Select(p => p).FirstOrDefault();

                if (data != null)
                {
                    if (DateTime.Now <= data.DateExpired)
                    {
                        tblM_User update = db.tblM_User.Where(p => p.UserID.Equals(tkn.UserID)).Single<tblM_User>();
                        update.isActive = 1;
                        update.isVerified = 1;
                        update.DateVerified = DateTime.Now;
                        update.DateActivated = update.DateVerified;
                        db.SaveChanges();

                        //cm.activity(Convert.ToInt32(usr.UserID), "Authentication success");

                        output.status = "success";
                        output.message = "User has been verified.";
                        output.data = update;
                    }
                    else
                    {
                        output.status = "failed";
                        output.message = "Kode telah kadaluarsa. Silahkan ambil kode verifikasi dengan fitur kirim kode.";
                        output.data = data;
                    }
                }
                else {
                    output.status = "failed";
                    output.message = "Kode verifikasi salah.";
                    output.data = data;
                }
            }
            catch (Exception ex)
            {
                //cm.activity(Convert.ToInt32(tkn.UserID), "Authentication failed");

                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }

            return output;
        }

        [Route("authcode/{phoneId}")]
        [HttpGet]
        public OutputModel ResendAuthCode(string phoneId)
        {
            OutputModel output = new OutputModel();
            
            try
            {
                var usr = db.tblM_User.Where(p => p.Phone == phoneId).FirstOrDefault();
                if (usr != null)
                {
                    if (usr.SMSCount > 5)
                    {
                        output.status = "failed";
                        output.message = "Maaf, kami tidak bisa memproses permintaan Anda karena Anda sudah memakai fitur sms + " + usr.SMSCount + " + kali";
                        output.data = String.Empty;
                    }
                    else
                    {
                        usr.SMSCount += 1;

                        Random r = new Random();
                        var x = r.Next(0, 1000000);
                        string s = x.ToString("000000");

                        DateTime exp = DateTime.Now.AddMinutes(10);

                        var temp = db.tblT_User_Token.Where(p => p.UserID == usr.UserID).FirstOrDefault();
                        temp.Token = s;
                        temp.DateExpired = exp;
                        db.SaveChanges();
                        output.status = "success";
                        output.message = "Success resend auth code";
                        output.data = temp;
                    }
                }
                else
                {
                    output.status = "failed";
                    output.message = "User id is not found";
                    output.data = String.Empty;
                }
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }

            return output;
        }

        private string checkPhone(string ph)
        {
            string em = "null";
            var data = db.tblM_User.Where(p =>
            p.Phone.Equals(ph)).Select(p => p).Count();
            if (data != 0)
            {
                return em = "yes";
            }
            else
            {
                return em = "no";
            }
        }

        void sendSMS(string no, string code)
        {
            string body = "Kode aktivasi anda " + code + " berlaku selama 10 menit";
            string url = ConfigurationManager.AppSettings["SMSURL"] + "userkey=" + ConfigurationManager.AppSettings["SMSUserKey"] + "&passkey=" + ConfigurationManager.AppSettings["SMSPassKey"] + "&nohp=" + no + "&pesan=" + body;
            var client = new WebClient();
            var content = client.DownloadString(url);
        }

        void sendSMSRecovery(string no, string name, string pass)
        {
            string body = "Halo " + name + ", password anda : " + pass;
            string url = ConfigurationManager.AppSettings["SMSURL"] + "userkey=" + ConfigurationManager.AppSettings["SMSUserKey"] + "&passkey=" + ConfigurationManager.AppSettings["SMSPassKey"] + "&nohp=" + no + "&pesan=" + body;
            var client = new WebClient();
            var content = client.DownloadString(url);
        }
    }
}