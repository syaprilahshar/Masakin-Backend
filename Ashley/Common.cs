using Masakin.Models;
using Masakin.Models.Catering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

namespace Masakin
{
    public class Common
    {
        DB_MasterEntities db = new DB_MasterEntities();
        static HttpClient client = new HttpClient();

        static void GetBaseAddress()
        {
            client.BaseAddress = new Uri("https://api.mainapi.net/");
        }

        public string phoneNumberFormat(string phone)
        {
            string sub = phone.Substring(0, 1);
            string subs = phone.Substring(1, phone.Length - 1);
            if (sub == "0")
            {
                return "+62" + subs;
            }
            else
            {
                return phone;
            }

        }

        public string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public Xsight SMSOTP(string key, string phone, string body)
        {
            Xsight xs = null;
            if (client.BaseAddress == null) { GetBaseAddress(); }
            client.Timeout.Add(new TimeSpan(0, 0, 5));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["bearerToken"].ToString());
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("phoneNum", phoneNumberFormat(phone)),
                new KeyValuePair<string, string>("digit", "4")
            };
            var content = new FormUrlEncodedContent(pairs);

            //var jsonString = "{\"phoneNum\":\"" + phone + "\",\"digit\":4}";
            //var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync("smsotp/1.0.1/otp/" + key, content).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                xs = JsonConvert.DeserializeObject<Xsight>(resultJSON);
            }
            return xs;
        }

        public Xsight AuthSMSOTP(string key, string otp)
        {
            Xsight xs = null;
            if (client.BaseAddress == null) { GetBaseAddress(); }
            client.Timeout.Add(new TimeSpan(0, 0, 5));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["bearerToken"].ToString());
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("otpstr", otp),
                new KeyValuePair<string, string>("digit", "4")
            };
            var content = new FormUrlEncodedContent(pairs);

            //var jsonString = "{\"otpstr\":\"" + otp + "\",\"digit\":4}";
            //var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("smsotp/1.0.1/otp/" + key + "/verifications", content).Result;
            if (response.IsSuccessStatusCode)
            {
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                xs = JsonConvert.DeserializeObject<Xsight>(resultJSON);
            }
            return xs;
        }

        public Tansaction PostPayment(string amount, string traId, string desc)
        {
            Tansaction xs = null;
            if (client.BaseAddress == null) { GetBaseAddress(); }
            client.Timeout.Add(new TimeSpan(0, 0, 5));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["bearerToken"].ToString());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var pairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("ivp_method", "create"),
                new KeyValuePair<string, string>("ivp_store", ConfigurationManager.AppSettings["finpayStoreId"].ToString()),
                new KeyValuePair<string, string>("ivp_authkey", ConfigurationManager.AppSettings["finpayAuthKey"].ToString()),
                new KeyValuePair<string, string>("ivp_amount", amount),
                new KeyValuePair<string, string>("ivp_currency", "idr"),
                new KeyValuePair<string, string>("ivp_test", "0"),
                new KeyValuePair<string, string>("ivp_cart", traId),
                new KeyValuePair<string, string>("ivp_desc", desc),
                new KeyValuePair<string, string>("return_auth", ConfigurationManager.AppSettings["finpayAuthURL"].ToString() + Base64Encode(traId)),
                new KeyValuePair<string, string>("return_decl", ConfigurationManager.AppSettings["finpayAuthURL"].ToString() + Base64Encode(traId)),
                new KeyValuePair<string, string>("return_can", ConfigurationManager.AppSettings["finpayAuthURL"].ToString() + Base64Encode(traId))
            };
            var content = new FormUrlEncodedContent(pairs);

            //var jsonString = "{\"ivp_method\":\"create\",\"ivp_store\":\"" + ConfigurationManager.AppSettings["finpayStoreId"].ToString() + "\",\"ivp_authkey\":\"" + ConfigurationManager.AppSettings["finpayAuthKey"].ToString() + "\",\"ivp_amount\":\"" + amount + "\",\"ivp_currency\":\"idr\",\"ivp_test\":\"1\",\"ivp_cart\":\"" + traId + "\",\"ivp_desc\":\"" + desc + "\",\"return_auth\":\"" + ConfigurationManager.AppSettings["finpayAuthURL"].ToString() + "\",\"return_decl\":\"" + ConfigurationManager.AppSettings["finpayDeclinedURL"].ToString() + "\",\"return_can\":\"" + ConfigurationManager.AppSettings["finpayCancelledURL"].ToString() + "\"}";
            //var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("finpay/2.0.0/transactions", content).Result;
            if (response.IsSuccessStatusCode)
            {
                //att = await response.Content.ReadAsAsync<AirportTransferTransaction>();
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                xs = JsonConvert.DeserializeObject<Tansaction>(resultJSON);
            }
            return xs;
        }

        public Tansaction PaymentCheck(string storeId, string authKey, string refs)
        {
            Tansaction xs = null;
            if (client.BaseAddress == null) { GetBaseAddress(); }
            client.Timeout.Add(new TimeSpan(0, 0, 5));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["bearerToken"].ToString());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var jsonString = "{\"ivp_method\":\"check\",\"ivp_store\":\"" + storeId + "\",\"ivp_authkey\":\"" + authKey + "\",\"order_ref\":\"" + refs + "\"}";
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("finpay/2.0.0/check-transactions", httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                //att = await response.Content.ReadAsAsync<AirportTransferTransaction>();
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                xs = JsonConvert.DeserializeObject<Tansaction>(resultJSON);
            }
            return xs;
        }

        public SMSNotification SMSNotification(string phone, string messages)
        {
            SMSNotification xs = null;
            if (client.BaseAddress == null) { GetBaseAddress(); }
            client.Timeout.Add(new TimeSpan(0, 0, 5));
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ConfigurationManager.AppSettings["bearerToken"].ToString());
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var jsonString = "{\"msisdn\":\"" + phoneNumberFormat(phone) + "\",\"content\":\"" + messages + "\"}";
            var httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("smsnotification/1.0.0/messages", httpContent).Result;
            if (response.IsSuccessStatusCode)
            {
                //att = await response.Content.ReadAsAsync<AirportTransferTransaction>();
                string resultJSON = response.Content.ReadAsStringAsync().Result;
                xs = JsonConvert.DeserializeObject<SMSNotification>(resultJSON);
            }
            return xs;
        }


        public string activity(int userid, string message)
        {
            try
            {
                //tblT_Log log = new tblT_Log();
                //log.UserID = userid;
                //log.Activity = message;
                //log.Date = DateTime.Today;
                //db.tblT_Log.Add(log);
                //db.SaveChanges();
                return "success";
            }
            catch (Exception ex)
            {
                return "failed";
            }
        }

        public FCMPushNotificationStatus SendPushNotification(string deviceId, string message)
        {
            FCMPushNotificationStatus result = new FCMPushNotificationStatus();

            try
            {
                result.Successful = false;
                result.Error = null;

                var value = message;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ConfigurationManager.AppSettings["FCMServerApiKey"].ToString()));
                tRequest.Headers.Add(string.Format("Sender: id={0}", ConfigurationManager.AppSettings["FCMSenderId"].ToString()));

                string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";

                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                result.Response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successful = false;
                result.Response = null;
                result.Error = ex;
            }

            return result;
        }

        public void AndroidPushNotification(string deviceId, string title, string message)
        {
            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = message,
                        title = title
                    }
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", ConfigurationManager.AppSettings["FCMServerApiKey"].ToString()));
                tRequest.Headers.Add(string.Format("Sender: id={0}", ConfigurationManager.AppSettings["FCMSenderId"].ToString()));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {

                string str = ex.Message;
            }
        }

        public class Tansaction
        {
            public string method { get; set; }
            public string trace { get; set; }
            public Order order { get; set; }
        }

        public class Order
        {
            [JsonProperty("ref")]
            public string refs { get; set; }
            public string url { get; set; }
            public string cartid { get; set; }
            public int test { get; set; }
            public string amount { get; set; }
            public string currency { get; set; }
            public string description { get; set; }
            public Status status { get; set; }
        }

        public class Status
        {
            public int code { get; set; }
            public string text { get; set; }
        }

        //SettingService.SettingsClient ss = new SettingService.SettingsClient();
        //public bool checkLogin(string ph, string passw)
        //{
        //    try
        //    {
        //        var usr = db.tblM_User.Where(p => p.Phone == ph && p.Password == passw && (p.isAgent == 1 || p.isAdmin == 1)).FirstOrDefault();
        //        if (usr != null)
        //        {
        //            SessionLib.Current.ID = usr.UserID.ToString();
        //            SessionLib.Current.Fullname = usr.Fullname.ToString();
        //            //SessionLib.Current.Email = xxx.Email.ToString();
        //            SessionLib.Current.HP = usr.Username.ToString();
        //            if (usr.isAdmin == 1)
        //            {
        //                SessionLib.Current.Role = Roles.Admin;
        //            }
        //            else
        //            {
        //                SessionLib.Current.Role = Roles.Agent;
        //            }
        //            SessionLib.Current.Password = passw;
        //            return true;
        //        }
        //        else
        //            return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public class Admin
        {
            public long Id { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Role { get; set; }
            public int isActive { get; set; }
        }
    }
}