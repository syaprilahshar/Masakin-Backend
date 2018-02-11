using Masakin.Models;
using Newtonsoft.Json;
using Rework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace Masakin.Controllers
{
    public class BookingController : ApiController
    {
        //MasakinEntities db = new MasakinEntities();

        //static HttpClient client = new HttpClient();

        //public class LoginTokenResult
        //{
        //    public override string ToString()
        //    {
        //        return AccessToken;
        //    }

        //    [JsonProperty(PropertyName = "access_token")]
        //    public string AccessToken { get; set; }

        //    [JsonProperty(PropertyName = "error")]
        //    public string Error { get; set; }

        //    [JsonProperty(PropertyName = "error_description")]
        //    public string ErrorDescription { get; set; }

        //    [JsonProperty(PropertyName = ".expires")]
        //    public DateTime Expires { get; set; }

        //}

        //public class AirportTransferTransaction
        //{
        //    [JsonProperty(PropertyName = "NumberOfBookings")]
        //    public int NumberOfBookings { get; set; }

        //    [JsonProperty(PropertyName = "AirportTransferBookings")]
        //    public List<AirportTransferBookings> AirportTransferBookings { get; set; }
        //}

        //public class WifiRentalTransaction
        //{
        //    [JsonProperty(PropertyName = "NumberOfBookings")]
        //    public int NumberOfBookings { get; set; }

        //    [JsonProperty(PropertyName = "WiFiRentals")]
        //    public List<WiFiRentals> WiFiRentals { get; set; }
        //}

        //static void GetBaseAddress() {
        //    client.BaseAddress = new Uri("https://changi-recommends-api.azurewebsites.net/");
        //}

        //static AirportTransferTransaction GetAirportTransferTransaction(string token, string email, int page, int record)
        //{
        //    AirportTransferTransaction att = null;
        //    if (client.BaseAddress == null) { GetBaseAddress(); }
        //    client.Timeout.Add(new TimeSpan(0, 0, 5));
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        //    HttpResponseMessage response = client.GetAsync("api/AirportTransferTransactions?customerEmail=test@example.com&page="+page+"&recordsPerPage="+record).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        //att = await response.Content.ReadAsAsync<AirportTransferTransaction>();
        //        string resultJSON = response.Content.ReadAsStringAsync().Result;
        //        att = JsonConvert.DeserializeObject<AirportTransferTransaction>(resultJSON);
        //    }
        //    return att;
        //}

        //static AirportTransferTransactionDetail GetAirportTransferTransactionDetail(string token, string email, string bookingid)
        //{
        //    AirportTransferTransactionDetail attd = null;
        //    if (client.BaseAddress == null) { GetBaseAddress(); }
        //    client.Timeout.Add(new TimeSpan(0, 0, 5));
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        //    HttpResponseMessage response = client.GetAsync("api/AirportTransferTransactionDetail?customerEmail=test@example.com&bookingId="+bookingid).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        //att = await response.Content.ReadAsAsync<AirportTransferTransaction>();
        //        string resultJSON = response.Content.ReadAsStringAsync().Result;
        //        attd = JsonConvert.DeserializeObject<AirportTransferTransactionDetail>(resultJSON);
        //    }
        //    return attd;
        //}

        //static WifiRentalTransaction GetWifiRentals(string token, string email, int page, int record)
        //{
        //    WifiRentalTransaction att = null;
        //    if (client.BaseAddress == null) { GetBaseAddress(); }
        //    client.Timeout.Add(new TimeSpan(0, 0, 5));
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        //    HttpResponseMessage response = client.GetAsync("api/WiFiRentals?customerEmail=test@example.com&page="+page+"&recordsPerPage="+record).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        //att = await response.Content.ReadAsAsync<AirportTransferTransaction>();
        //        string resultJSON = response.Content.ReadAsStringAsync().Result;
        //        att = JsonConvert.DeserializeObject<WifiRentalTransaction>(resultJSON);
        //    }
        //    return att;
        //}

        //static WifiRentalDetail GetWifiRentalDetail(string token, string email, string bookingid)
        //{
        //    WifiRentalDetail attd = null;
        //    if (client.BaseAddress == null) { GetBaseAddress(); }
        //    client.Timeout.Add(new TimeSpan(0, 0, 5));
        //    client.DefaultRequestHeaders.Accept.Clear();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
        //    HttpResponseMessage response = client.GetAsync("api/WiFiRentalDetail?customerEmail=test@example.com&bookingId=" + bookingid).Result;
        //    if (response.IsSuccessStatusCode)
        //    {
        //        //att = await response.Content.ReadAsAsync<AirportTransferTransaction>();
        //        string resultJSON = response.Content.ReadAsStringAsync().Result;
        //        attd = JsonConvert.DeserializeObject<WifiRentalDetail>(resultJSON);
        //    }
        //    return attd;
        //}

        //static LoginTokenResult GetToken(string username, string password)
        //{
        //    if (client.BaseAddress == null) { GetBaseAddress(); }
        //    client.Timeout.Add(new TimeSpan(0, 0, 5));
        //    HttpResponseMessage response =
        //      client.PostAsync("Token",
        //        new StringContent(string.Format("grant_type=password&username={0}&password={1}",
        //          HttpUtility.UrlEncode(username),
        //          HttpUtility.UrlEncode(password)), Encoding.UTF8,
        //          "application/x-www-form-urlencoded")).Result;

        //    string resultJSON = response.Content.ReadAsStringAsync().Result;
        //    LoginTokenResult result = JsonConvert.DeserializeObject<LoginTokenResult>(resultJSON);

        //    return result;
        //}

        //[Route("airporttransfer/{usernameId}/{pageId}/{countId}")]
        //[HttpGet]
        //public OutputModel AirportTransferTransactions(int usernameId, int pageId, int countId)
        //{
        //    OutputModel output = new OutputModel();
        //    MasakinEntities db = new MasakinEntities();
        //    var accesstoken = "";
        //    try
        //    {
        //        var accsstkn = db.tblM_Token.Where(p => p.TokenName.ToLower() == "changilogin").FirstOrDefault();
        //        if (accsstkn != null)
        //        {
        //            if (accsstkn.ExpiredDate > DateTime.UtcNow)
        //            {
        //                accesstoken = accsstkn.AccessToken;
        //            }
        //            else
        //            {
        //                var token = GetToken("api@ctssea.com", "Changi123!");
        //                if (token.AccessToken != null)
        //                {
        //                    accsstkn.AccessToken = token.AccessToken;
        //                    accsstkn.ExpiredDate = token.Expires;
        //                    db.SaveChanges();
        //                    accesstoken = token.AccessToken;
        //                }
        //                else
        //                {
        //                    output.status = "failed";
        //                    output.message = token.ErrorDescription;
        //                    output.data = null;
        //                    return output;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var tokens = GetToken("api@ctssea.com", "Changi123!");
        //            if (tokens.AccessToken != null)
        //            {
        //                tblM_Token tkn = new tblM_Token();
        //                tkn.AccessToken = tokens.AccessToken;
        //                tkn.TokenName = "ChangiLogin";
        //                tkn.ExpiredDate = tokens.Expires;
        //                tkn.Username = "api@ctssea.com";
        //                db.tblM_Token.Add(tkn);
        //                db.SaveChanges();
        //                accesstoken = tokens.AccessToken;
        //            }
        //            else
        //            {
        //                output.status = "failed";
        //                output.message = tokens.ErrorDescription;
        //                output.data = null;
        //                return output;
        //            }
        //        }

        //        var user = db.tblM_User.Where(p => p.UserID == usernameId).FirstOrDefault();
        //        if (user == null)
        //        {
        //            output.status = "failed";
        //            output.message = "User id is not found.";
        //            output.data = null;
        //            return output;
        //        }

        //        var att = GetAirportTransferTransaction(accesstoken, user.Email, pageId, countId);

        //        if (att != null)
        //        {
        //            output.status = "success";
        //            output.message = "Success retrieve airport transfer";
        //            output.data = att;
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "Error get airport transfer list. Please try again.";
        //            output.data = null;
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

        //[Route("airporttransferdetail/{usernameId}/{bookingId}")]
        //[HttpGet]
        //public OutputModel AirportTransferTransactionDetail(int usernameId, string bookingId)
        //{
        //    OutputModel output = new OutputModel();
        //    MasakinEntities db = new MasakinEntities();
        //    var accesstoken = "";
        //    try
        //    {
        //        var accsstkn = db.tblM_Token.Where(p => p.TokenName.ToLower() == "changilogin").FirstOrDefault();
        //        if (accsstkn != null)
        //        {
        //            if (accsstkn.ExpiredDate > DateTime.UtcNow)
        //            {
        //                accesstoken = accsstkn.AccessToken;
        //            }
        //            else
        //            {
        //                var token = GetToken("api@ctssea.com", "Changi123!");
        //                if (token.AccessToken != null)
        //                {
        //                    accsstkn.AccessToken = token.AccessToken;
        //                    accsstkn.ExpiredDate = token.Expires;
        //                    db.SaveChanges();
        //                    accesstoken = token.AccessToken;
        //                }
        //                else
        //                {
        //                    output.status = "failed";
        //                    output.message = token.ErrorDescription;
        //                    output.data = null;
        //                    return output;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var tokens = GetToken("api@ctssea.com", "Changi123!");
        //            if (tokens.AccessToken != null)
        //            {
        //                tblM_Token tkn = new tblM_Token();
        //                tkn.AccessToken = tokens.AccessToken;
        //                tkn.TokenName = "ChangiLogin";
        //                tkn.ExpiredDate = tokens.Expires;
        //                tkn.Username = "api@ctssea.com";
        //                db.tblM_Token.Add(tkn);
        //                db.SaveChanges();
        //                accesstoken = tokens.AccessToken;
        //            }
        //            else
        //            {
        //                output.status = "failed";
        //                output.message = tokens.ErrorDescription;
        //                output.data = null;
        //                return output;
        //            }
        //        }

        //        var user = db.tblM_User.Where(p => p.UserID == usernameId).FirstOrDefault();
        //        if (user == null)
        //        {
        //            output.status = "failed";
        //            output.message = "User id is not found.";
        //            output.data = null;
        //            return output;
        //        }

        //        var att = GetAirportTransferTransactionDetail(accesstoken, user.Email, bookingId);

        //        if (att != null)
        //        {
        //            output.status = "success";
        //            output.message = "Success retrieve airport transfer detail";
        //            output.data = att;
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "Error get airport transfer detail. Please try again.";
        //            output.data = null;
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

        //[Route("wifirental/{usernameId}/{pageId}/{countId}")]
        //[HttpGet]
        //public OutputModel WifiRentalTransactions(int usernameId, int pageId, int countId)
        //{
        //    OutputModel output = new OutputModel();
        //    MasakinEntities db = new MasakinEntities();
        //    var accesstoken = "";
        //    try
        //    {
        //        var accsstkn = db.tblM_Token.Where(p => p.TokenName.ToLower() == "changilogin").FirstOrDefault();
        //        if (accsstkn != null)
        //        {
        //            if (accsstkn.ExpiredDate > DateTime.UtcNow)
        //            {
        //                accesstoken = accsstkn.AccessToken;
        //            }
        //            else
        //            {
        //                var token = GetToken("api@ctssea.com", "Changi123!");
        //                if (token.AccessToken != null)
        //                {
        //                    accsstkn.AccessToken = token.AccessToken;
        //                    accsstkn.ExpiredDate = token.Expires;
        //                    db.SaveChanges();
        //                    accesstoken = token.AccessToken;
        //                }
        //                else
        //                {
        //                    output.status = "failed";
        //                    output.message = token.ErrorDescription;
        //                    output.data = null;
        //                    return output;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var tokens = GetToken("api@ctssea.com", "Changi123!");
        //            if (tokens.AccessToken != null)
        //            {
        //                tblM_Token tkn = new tblM_Token();
        //                tkn.AccessToken = tokens.AccessToken;
        //                tkn.TokenName = "ChangiLogin";
        //                tkn.ExpiredDate = tokens.Expires;
        //                tkn.Username = "api@ctssea.com";
        //                db.tblM_Token.Add(tkn);
        //                db.SaveChanges();
        //                accesstoken = tokens.AccessToken;
        //            }
        //            else
        //            {
        //                output.status = "failed";
        //                output.message = tokens.ErrorDescription;
        //                output.data = null;
        //                return output;
        //            }
        //        }

        //        var user = db.tblM_User.Where(p => p.UserID == usernameId).FirstOrDefault();
        //        if (user == null)
        //        {
        //            output.status = "failed";
        //            output.message = "User id is not found.";
        //            output.data = null;
        //            return output;
        //        }

        //        var att = GetWifiRentals(accesstoken, user.Email, pageId, countId);

        //        if (att != null)
        //        {
        //            output.status = "success";
        //            output.message = "Success retrieve wifi rental";
        //            output.data = att;
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "Error get wifi rental list. Please try again.";
        //            output.data = null;
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

        //[Route("wifirentaldetail/{usernameId}/{bookingId}")]
        //[HttpGet]
        //public OutputModel WifiRentalTransactionDetail(int usernameId, string bookingId)
        //{
        //    OutputModel output = new OutputModel();
        //    MasakinEntities db = new MasakinEntities();
        //    var accesstoken = "";
        //    try
        //    {
        //        var accsstkn = db.tblM_Token.Where(p => p.TokenName.ToLower() == "changilogin").FirstOrDefault();
        //        if (accsstkn != null)
        //        {
        //            if (accsstkn.ExpiredDate > DateTime.UtcNow)
        //            {
        //                accesstoken = accsstkn.AccessToken;
        //            }
        //            else
        //            {
        //                var token = GetToken("api@ctssea.com", "Changi123!");
        //                if (token.AccessToken != null)
        //                {
        //                    accsstkn.AccessToken = token.AccessToken;
        //                    accsstkn.ExpiredDate = token.Expires;
        //                    db.SaveChanges();
        //                    accesstoken = token.AccessToken;
        //                }
        //                else
        //                {
        //                    output.status = "failed";
        //                    output.message = token.ErrorDescription;
        //                    output.data = null;
        //                    return output;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var tokens = GetToken("api@ctssea.com", "Changi123!");
        //            if (tokens.AccessToken != null)
        //            {
        //                tblM_Token tkn = new tblM_Token();
        //                tkn.AccessToken = tokens.AccessToken;
        //                tkn.TokenName = "ChangiLogin";
        //                tkn.ExpiredDate = tokens.Expires;
        //                tkn.Username = "api@ctssea.com";
        //                db.tblM_Token.Add(tkn);
        //                db.SaveChanges();
        //                accesstoken = tokens.AccessToken;
        //            }
        //            else
        //            {
        //                output.status = "failed";
        //                output.message = tokens.ErrorDescription;
        //                output.data = null;
        //                return output;
        //            }
        //        }

        //        var user = db.tblM_User.Where(p => p.UserID == usernameId).FirstOrDefault();
        //        if (user == null)
        //        {
        //            output.status = "failed";
        //            output.message = "User id is not found.";
        //            output.data = null;
        //            return output;
        //        }

        //        var att = GetWifiRentalDetail(accesstoken, user.Email, bookingId);

        //        if (att != null)
        //        {
        //            output.status = "success";
        //            output.message = "Success retrieve wifi rental detail";
        //            output.data = att;
        //        }
        //        else
        //        {
        //            output.status = "failed";
        //            output.message = "Error get wifi rental detail. Please try again.";
        //            output.data = null;
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

        

        public class FromMich {
            public long user_id { get; set; }
            public string username { get; set; }
            public decimal balance { get; set; }
            public string ref_id { get; set; }
        }

        

        private static string ByteToString(byte[] buff)
        {
            string sbinary = "";

            for (int i = 0; i < buff.Length; i++)
            {
                //sbinary += buff[i].ToString("X2"); // hex format

                string s = buff[i].ToString("X2");
                int length = s.Length;
                if (length >= 2)
                {
                    sbinary += s.Substring(length - 2, length);
                }
                else
                {
                    sbinary += "0";
                    sbinary += s;
                }
            }
            return (sbinary);
        }
    }
}
