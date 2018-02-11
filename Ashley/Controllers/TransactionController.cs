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
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Masakin.Models.Catering;
using System.Text;

namespace Masakin.Controllers
{
    public class TransactionController : ApiController
    {
        Common cm = new Common();
        DB_MasterEntities db = new DB_MasterEntities();
        DB_MasakinEntities dbMasakin = new DB_MasakinEntities();

        [Route("transaction/post")]
        [HttpPost]
        public OutputModel PostTransaction([FromBody] tblT_Transaction utr)
        {
            OutputModel output = new OutputModel();
            string traid = "";
            int total = 0;
            int notavailable = 0;
            try
            {
                var catID = utr.CateringID;
                var catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catID).FirstOrDefault();
                if (catering.Quantity < utr.Quantity)
                {
                    notavailable += 1;
                }

                if (notavailable > 0)
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Stok yang tersedia tidak mencukupi. Silahkan cek kembali jumlah pesanan Anda.";
                }
                else
                {
                    //create transaction BBNK/PYM/20161205/0000001
                    tblT_Transaction rta = new tblT_Transaction();
                    rta.UserID = utr.UserID;
                    rta.MerchantID = utr.MerchantID;
                    rta.CateringID = utr.CateringID;
                    rta.Status = StatusModel.Pending;
                    rta.TransactionID = "MSKN/PYMTestingz/" + DateTime.Today.ToString("yyyyMMdd") + "/" + (dbMasakin.tblT_Transaction.Count() + 1).ToString("0000000");
                    traid = rta.TransactionID;
                    rta.Quantity = utr.Quantity;
                    rta.SubTotal = utr.SubTotal;
                    //rta.ShippingCost = utr.ShippingCost;
                    rta.ShippingCost = 0;
                    rta.Total = utr.Total;
                    rta.isPaid = 0;
                    rta.DateFirst = catering.DateFirst;
                    rta.DateCreated = DateTime.Now;

                    rta.Deliveree = utr.Deliveree;
                    rta.Phone = utr.Phone;
                    rta.Address = utr.Address;
                    rta.City = utr.City;
                    rta.HomeNumber = utr.HomeNumber;
                    rta.Notes = utr.Notes;
                    rta.Voucher = utr.Voucher;
                    rta.VoucherAmount = 0;
                    dbMasakin.tblT_Transaction.Add(rta);
                    catering.Quantity -= rta.Quantity;
                    catering.Sold += rta.Quantity;
                    dbMasakin.SaveChanges();

                    tblT_Transaction_Detail td = new tblT_Transaction_Detail();
                    td.TransactionID = traid;
                    td.CateringID = rta.CateringID;
                    td.MenuID = catering.Day1MenuID;
                    td.Date = catering.DateFirst;
                    td.Status = StatusModel.Pending;
                    td.isActive = 1;
                    td.DateCreated = DateTime.Now;
                    dbMasakin.tblT_Transaction_Detail.Add(td);
                    dbMasakin.SaveChanges();

                    td = new tblT_Transaction_Detail();
                    td.TransactionID = traid;
                    td.CateringID = rta.CateringID;
                    td.MenuID = catering.Day2MenuID;
                    td.Date = catering.DateFirst.Value.AddDays(1);
                    td.Status = StatusModel.Pending;
                    td.isActive = 1;
                    td.DateCreated = DateTime.Now;
                    dbMasakin.tblT_Transaction_Detail.Add(td);
                    dbMasakin.SaveChanges();

                    td = new tblT_Transaction_Detail();
                    td.TransactionID = traid;
                    td.CateringID = rta.CateringID;
                    td.MenuID = catering.Day3MenuID;
                    td.Date = catering.DateFirst.Value.AddDays(2);
                    td.Status = StatusModel.Pending;
                    td.isActive = 1;
                    td.DateCreated = DateTime.Now;
                    dbMasakin.tblT_Transaction_Detail.Add(td);
                    dbMasakin.SaveChanges();

                    td = new tblT_Transaction_Detail();
                    td.TransactionID = traid;
                    td.CateringID = rta.CateringID;
                    td.MenuID = catering.Day4MenuID;
                    td.Date = catering.DateFirst.Value.AddDays(3);
                    td.Status = StatusModel.Pending;
                    td.isActive = 1;
                    td.DateCreated = DateTime.Now;
                    dbMasakin.tblT_Transaction_Detail.Add(td);
                    dbMasakin.SaveChanges();

                    td = new tblT_Transaction_Detail();
                    td.TransactionID = traid;
                    td.CateringID = rta.CateringID;
                    td.MenuID = catering.Day5MenuID;
                    td.Date = catering.DateFirst.Value.AddDays(4);
                    td.Status = StatusModel.Pending;
                    td.isActive = 1;
                    td.DateCreated = DateTime.Now;
                    dbMasakin.tblT_Transaction_Detail.Add(td);
                    dbMasakin.SaveChanges();

                    string dates = String.Format("{0:dd-MM-yyyy}", catering.DateFirst);
                    var merchantId = utr.MerchantID;

                    
                    var pay = cm.PostPayment(rta.Total.ToString(), rta.TransactionID, "Pesanan " + utr.Quantity + " buah katering yang dimulai pada tanggal " + dates + ".");
                    if (pay.order.url != null)
                    {
                        tblT_Payment pays = new tblT_Payment();
                        pays.Ref = pay.order.refs;
                        pays.Trace = pay.trace;
                        pays.TransactionID = rta.TransactionID;
                        pays.URL = pay.order.url;
                        pays.Status = StatusModel.Pending;
                        pays.StatusMessage = "";
                        dbMasakin.tblT_Payment.Add(pays);
                        dbMasakin.SaveChanges();

                        var did = db.tblM_User_DeviceID.Where(p => p.UserID == merchantId).FirstOrDefault();
                        var str = did.DeviceID;
                        string[] dids = str.Split(',');
                        for (int a = 0; a < dids.Count(); a++)
                        {
                            cm.AndroidPushNotification(dids[a], "Pesanan Katering!", "Selamat! Anda mendapatkan pesanan sebanyak " + utr.Quantity + " buah untuk katering yang dimulai pada tanggal " + dates + ".");
                        }
                        //var mer = db.tblM_User.Where(p => p.UserID == merchantId).FirstOrDefault();
                        //cm.SMSNotification(mer.Phone, "Selamat! Anda mendapatkan pesanan sebanyak " + utr.Quantity + " buah untuk katering yang dimulai pada tanggal " + dates + ".");

                        output.status = "success";
                        //output.data = rta;
                        //output.data = pay;
                        output.data = pays;
                        output.message = null;
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    output.status = "error";
            //    output.data = null;
            //    output.message = ex.ToString();
            //}
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
            return output;
        }

        [Route("transaction/get/user/{userId}")]
        [HttpGet]
        public OutputModel GetAllUserTransaction(int userId)
        {
            OutputModel output = new OutputModel();
            try
            {
                var tra = dbMasakin.tblT_Transaction.Where(p => p.UserID == userId).ToList();
                
                output.status = "success";
                output.data = tra;
                //output.message = null;
                output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/current/user/{userId}")]
        [HttpGet]
        public OutputModel GetUserTransaction(int userId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                var tra = dbMasakin.tblT_Transaction.Where(p => p.UserID == userId && p.DateFirst >= dt).ToList();

                output.status = "success";
                output.data = tra;
                //output.message = null;
                output.message = "Transaksi berhasil di difetching dengan user id: " + userId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/merchant/{merchantId}")]
        [HttpGet]
        public OutputModel GetMerchantTransaction(int merchantId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);
                var tra = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst >= dt1 && p.Status == StatusModel.Paid).ToList();

                for (int i = 0; i < tra.Count; i++) {
                    var catid = tra[i].CateringID;
                    tra[i].Catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catid).FirstOrDefault();
                }

                output.status = "success";
                output.data = tra;
                //output.message = null;
                output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/user/notpaid/{userId}")]
        [HttpGet]
        public OutputModel GetUserNotPaidTransaction(int userId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);

                var list = dbMasakin.tblT_Transaction.Where(p => p.UserID == userId && p.Status == StatusModel.Pending && p.isPaid == 0).ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var catid = list[i].CateringID;
                        list[i].Catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catid).FirstOrDefault();

                        List<tblM_Menu> menu = new List<tblM_Menu>();
                        var menuid = list[i].Catering.Day1MenuID;
                        var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day1);

                        menuid = list[i].Catering.Day2MenuID;
                        var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day2);

                        menuid = list[i].Catering.Day3MenuID;
                        var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day3);

                        menuid = list[i].Catering.Day4MenuID;
                        var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day4);

                        menuid = list[i].Catering.Day5MenuID;
                        var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day5);

                        list[i].Menu = menu;

                        var traiId = list[i].TransactionID;
                        var pay = dbMasakin.tblT_Payment.Where(p => p.TransactionID == traiId).FirstOrDefault();
                        if (pay != null) {
                            list[i].URL = pay.URL;
                        }
                    }
                    output.status = "success";
                    output.data = list;
                    output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                }
                else
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                }

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/user/past/{userId}")]
        [HttpGet]
        public OutputModel GetUserPastTransaction(int userId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);

                var list = dbMasakin.tblT_Transaction.Where(p => p.UserID == userId && p.DateFirst < dt && p.isPaid == 1).ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var catid = list[i].CateringID;
                        list[i].Catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catid).FirstOrDefault();

                        List<tblM_Menu> menu = new List<tblM_Menu>();
                        var menuid = list[i].Catering.Day1MenuID;
                        var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day1);

                        menuid = list[i].Catering.Day2MenuID;
                        var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day2);

                        menuid = list[i].Catering.Day3MenuID;
                        var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day3);

                        menuid = list[i].Catering.Day4MenuID;
                        var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day4);

                        menuid = list[i].Catering.Day5MenuID;
                        var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day5);

                        list[i].Menu = menu;
                    }
                    output.status = "success";
                    output.data = list;
                    output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                }
                else
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                }

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/user/current/{userId}")]
        [HttpGet]
        public OutputModel GetUserCurrentTransaction(int userId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);

                var list = dbMasakin.tblT_Transaction.Where(p => p.UserID == userId && p.DateFirst == dt && p.isPaid == 1).ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var catid = list[i].CateringID;
                        list[i].Catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catid).FirstOrDefault();

                        List<tblM_Menu> menu = new List<tblM_Menu>();
                        var menuid = list[i].Catering.Day1MenuID;
                        var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day1);

                        menuid = list[i].Catering.Day2MenuID;
                        var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day2);

                        menuid = list[i].Catering.Day3MenuID;
                        var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day3);

                        menuid = list[i].Catering.Day4MenuID;
                        var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day4);

                        menuid = list[i].Catering.Day5MenuID;
                        var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day5);

                        list[i].Menu = menu;
                    }
                    output.status = "success";
                    output.data = list;
                    output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                }
                else
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                }

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/user/next/{userId}")]
        [HttpGet]
        public OutputModel GetUserNextTransaction(int userId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);

                var list = dbMasakin.tblT_Transaction.Where(p => p.UserID == userId && p.DateFirst > dt && p.isPaid == 1).ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var catid = list[i].CateringID;
                        list[i].Catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catid).FirstOrDefault();

                        List<tblM_Menu> menu = new List<tblM_Menu>();
                        var menuid = list[i].Catering.Day1MenuID;
                        var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day1);

                        menuid = list[i].Catering.Day2MenuID;
                        var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day2);

                        menuid = list[i].Catering.Day3MenuID;
                        var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day3);

                        menuid = list[i].Catering.Day4MenuID;
                        var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day4);

                        menuid = list[i].Catering.Day5MenuID;
                        var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                        menu.Add(day5);

                        list[i].Menu = menu;
                    }
                    output.status = "success";
                    output.data = list;
                    output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                }
                else
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Transaksi berhasil di difetching dengan user id: " + userId;
                }

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/merchant/past/{merchantId}")]
        [HttpGet]
        public OutputModel GetMerchantPastTransaction(int merchantId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);

                var catering = dbMasakin.tblT_Catering.Where(p => p.MerchantID == merchantId && p.DateFirst < dt && p.isActive == 1).FirstOrDefault();
                catering.Quantity = 0;
                var list = dbMasakin.tblT_Transaction.Where(p => p.CateringID == catering.CateringID && p.Status == StatusModel.Paid).ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        catering.Quantity += list[i].Quantity;
                    }
                    List<tblM_Menu> menu = new List<tblM_Menu>();
                    var menuid = catering.Day1MenuID;
                    var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day1);

                    menuid = catering.Day2MenuID;
                    var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day2);

                    menuid = catering.Day3MenuID;
                    var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day3);

                    menuid = catering.Day4MenuID;
                    var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day4);

                    menuid = catering.Day5MenuID;
                    var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day5);

                    catering.Menu = menu;

                    output.status = "success";
                    output.data = catering;
                    output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;
                }
                else
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;
                }

                //var tra = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst < dt && p.Status == StatusModel.Paid).ToList();

                //for (int i = 0; i < tra.Count; i++)
                //{
                //    var catid = tra[i].CateringID;
                //    tra[i].Catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catid).FirstOrDefault();
                //}

                //output.status = "success";
                //output.data = tra;
                //output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/merchant/current/{merchantId}")]
        [HttpGet]
        public OutputModel GetMerchantCurrentTransaction(int merchantId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);


                var catering = dbMasakin.tblT_Catering.Where(p => p.MerchantID == merchantId && p.DateFirst == dt && p.isActive == 1).FirstOrDefault();
                catering.Quantity = 0;
                var list = dbMasakin.tblT_Transaction.Where(p => p.CateringID == catering.CateringID && p.Status == StatusModel.Paid).ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        catering.Quantity += list[i].Quantity;
                    }
                    List<tblM_Menu> menu = new List<tblM_Menu>();
                    var menuid = catering.Day1MenuID;
                    var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day1);

                    menuid = catering.Day2MenuID;
                    var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day2);

                    menuid = catering.Day3MenuID;
                    var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day3);

                    menuid = catering.Day4MenuID;
                    var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day4);

                    menuid = catering.Day5MenuID;
                    var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day5);

                    catering.Menu = menu;

                    output.status = "success";
                    output.data = catering;
                    output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;
                } else {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;
                }
                


                //var tra = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst == dt && p.Status == StatusModel.Paid).ToList();

                //for (int i = 0; i < tra.Count; i++)
                //{
                //    var catid = tra[i].CateringID;
                //    tra[i].Catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catid).FirstOrDefault();
                //}

                //output.status = "success";
                //output.data = tra;]
                //output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/merchant/next/{merchantId}")]
        [HttpGet]
        public OutputModel GetMerchantNextTransaction(int merchantId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);

                var catering = dbMasakin.tblT_Catering.Where(p => p.MerchantID == merchantId && p.DateFirst > dt && p.isActive == 1).FirstOrDefault();
                catering.Quantity = 0;
                var list = dbMasakin.tblT_Transaction.Where(p => p.CateringID == catering.CateringID && p.Status == StatusModel.Paid).ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        catering.Quantity += list[i].Quantity;
                    }
                    List<tblM_Menu> menu = new List<tblM_Menu>();
                    var menuid = catering.Day1MenuID;
                    var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day1);

                    menuid = catering.Day2MenuID;
                    var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day2);

                    menuid = catering.Day3MenuID;
                    var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day3);

                    menuid = catering.Day4MenuID;
                    var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day4);

                    menuid = catering.Day5MenuID;
                    var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuid).FirstOrDefault();
                    menu.Add(day5);

                    catering.Menu = menu;

                    output.status = "success";
                    output.data = catering;
                    output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;
                }
                else
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;
                }

                //var tra = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst > dt && p.Status == StatusModel.Paid).ToList();

                //for (int i = 0; i < tra.Count; i++)
                //{
                //    var catid = tra[i].CateringID;
                //    tra[i].Catering = dbMasakin.tblT_Catering.Where(p => p.CateringID == catid).FirstOrDefault();
                //}

                //output.status = "success";
                //output.data = tra;
                //output.message = "Transaksi berhasil di difetching dengan merchant id: " + merchantId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/merchant/buyers/{merchantId}/{cateringId}")]
        [HttpGet]
        public OutputModel GetMerchantListOrdersTransaction(int merchantId, int cateringId)
        {
            OutputModel output = new OutputModel();
            try
            {
                var catering = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.CateringID == cateringId && p.Status == StatusModel.Paid).ToList();
                if (catering.Count > 0)
                {
                    output.status = "success";
                    output.data = catering;
                    output.message = "Daftar pemesan berhasil didapatkan dengan catering id: " + cateringId;
                }
                else
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Daftar pemesan berhasil didapatkan dengan catering id: " + cateringId;
                }

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/merchant/detail/{merchantId}/{cateringId}")]
        [HttpGet]
        public OutputModel GetMerchantTransactionDetail(int merchantId, int cateringId)
        {
            OutputModel output = new OutputModel();
            try
            {
                var catering = dbMasakin.tblT_Catering.Where(p => p.MerchantID == merchantId && p.CateringID == cateringId).FirstOrDefault();
                List<tblM_Menu> menu = new List<tblM_Menu>();

                var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day1MenuID).FirstOrDefault();
                menu.Add(day1);

                var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day2MenuID).FirstOrDefault();
                menu.Add(day2);

                var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day3MenuID).FirstOrDefault();
                menu.Add(day3);

                var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day4MenuID).FirstOrDefault();
                menu.Add(day4);

                var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day5MenuID).FirstOrDefault();
                menu.Add(day5);

                catering.Menu = menu;

                output.status = "success";
                output.data = catering;
                output.message = "Daftar menu berhasil didapatkan dengan catering id: " + cateringId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/get/merchant/order/{merchantId}")]
        [HttpGet]
        public OutputModel GetMerchantTransactionOrder(int merchantId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            try
            {
                var tra = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst == dt && p.isPaid == 1).FirstOrDefault();
                if (tra != null)
                {
                    var catering = dbMasakin.tblT_Catering.Where(p => p.MerchantID == merchantId && p.CateringID == tra.CateringID).FirstOrDefault();
                    List<tblM_Menu> menu = new List<tblM_Menu>();

                    var day1 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day1MenuID).FirstOrDefault();
                    menu.Add(day1);

                    var day2 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day2MenuID).FirstOrDefault();
                    menu.Add(day2);

                    var day3 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day3MenuID).FirstOrDefault();
                    menu.Add(day3);

                    var day4 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day4MenuID).FirstOrDefault();
                    menu.Add(day4);

                    var day5 = dbMasakin.tblM_Menu.Where(p => p.MenuID == catering.Day5MenuID).FirstOrDefault();
                    menu.Add(day5);

                    catering.Menu = menu;

                    output.status = "success";
                    output.data = catering;
                    output.message = "Daftar menu berhasil didapatkan dengan merchant id: " + merchantId;
                }
                else {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Daftar menu berhasil didapatkan dengan merchant id: " + merchantId;
                }
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/detail/get/{transactionId}")]
        [HttpGet]
        public OutputModel GetTransactionDetail(string transactionId)
        {

            OutputModel output = new OutputModel();
            String traId = cm.Base64Decode(transactionId);
            try
            {
                var tra = dbMasakin.tblT_Transaction_Detail.Where(p => p.TransactionID == traId).ToList();
                for (int i = 0; i < tra.Count; i++) {
                    var menuId = tra[i].MenuID;
                    tra[i].Menu = dbMasakin.tblM_Menu.Where(p => p.MenuID == menuId).FirstOrDefault();
                }
                output.status = "success";
                output.data = tra;
                output.message = "Detail transaksi berhasil di difetching dengan transaction id: " + traId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/detail/process/{transactionId}")]
        [HttpGet]
        public OutputModel DetailProcess(int transactionDetailId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            try
            {
                var tra = dbMasakin.tblT_Transaction_Detail.Where(p => p.DetailID == transactionDetailId).FirstOrDefault();
                tra.Status = StatusModel.OnProcess;
                dbMasakin.SaveChanges();

                output.status = "success";
                output.data = tra;
                output.message = "Detail transaksi sementara di proses dengan detail id: " + transactionDetailId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/process/{merchantId}/{dayId}")]
        [HttpGet]
        public OutputModel ProcessTransaction(int merchantId, int dayId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            try
            {
                var trans = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst == dt).ToList();
                var currentDay = dt.AddDays(dayId);
                for (int i = 0; i < trans.Count; i++)
                {
                    var traId = trans[i].TransactionID;
                    var traDetail = dbMasakin.tblT_Transaction_Detail.Where(p => p.TransactionID == traId
                    && p.Date == currentDay).FirstOrDefault();
                    if (traDetail != null)
                    {
                        traDetail.Status = StatusModel.OnProcess;
                        dbMasakin.SaveChanges();
                    }

                    var userId = trans[i].UserID;
                    var did = db.tblM_User_DeviceID.Where(p => p.UserID == userId).FirstOrDefault();
                    var str = did.DeviceID;
                    string[] dids = str.Split(',');
                    for (int a = 0; a < dids.Count(); a++)
                    {
                        cm.AndroidPushNotification(dids[a], "Informasi Pesanan", "Pesanan Anda untuk hari ini dengan nomor transaksi " + traId + " lagi dipersiapkan. Terima kasih!");
                    }

                    var usr = db.tblM_User.Where(p => p.UserID == userId).FirstOrDefault();
                    cm.SMSNotification(usr.Phone, "Pesanan Anda untuk hari ini dengan nomor transaksi " + traId + " lagi dipersiapkan. Terima kasih!");
                }

                output.status = "success";
                output.data = null;
                output.message = "Anda sudah mengkonfirmasi akan memproses pesanan hari ini. Selamat bekerja!";

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/send/{merchantId}/{dayId}")]
        [HttpGet]
        public OutputModel SendTransaction(int merchantId, int dayId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            try
            {
                var trans = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst == dt).ToList();
                var currentDay = dt.AddDays(dayId);
                for (int i = 0; i < trans.Count; i++)
                {
                    var traId = trans[i].TransactionID;
                    var traDetail = dbMasakin.tblT_Transaction_Detail.Where(p => p.TransactionID == traId
                    && p.Date == currentDay).FirstOrDefault();
                    if (traDetail != null)
                    {
                        traDetail.Status = StatusModel.Send;
                        dbMasakin.SaveChanges();
                    }

                    var userId = trans[i].UserID;
                    var did = db.tblM_User_DeviceID.Where(p => p.UserID == userId).FirstOrDefault();
                    var str = did.DeviceID;
                    string[] dids = str.Split(',');
                    for (int a = 0; a < dids.Count(); a++)
                    {
                        cm.AndroidPushNotification(dids[a], "Pesanan Dikirimkan", "Pesanan Anda untuk hari ini dengan nomor transaksi " + traId + " sedang melakukan pengantaran ke lokasi Anda. Mohon bersabar menunggu dan dari pihak katering akan menghubungi Anda. Terima kasih!");
                    }

                    var usr = db.tblM_User.Where(p => p.UserID == userId).FirstOrDefault();
                    cm.SMSNotification(usr.Phone, "Pesanan Anda untuk hari ini dengan nomor transaksi " + traId + " sedang melakukan pengantaran ke lokasi Anda. Mohon bersabar menunggu dan dari pihak katering akan menghubungi Anda. Terima kasih!");
                }

                output.status = "success";
                output.data = null;
                output.message = "Anda sudah mengkonfirmasi akan mengirimkan pesanan hari ini ke pelanggan. Selamat berkendara!";

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/finish/{merchantId}/{dayId}")]
        [HttpGet]
        public OutputModel FinishTransaction(int merchantId, int dayId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            int total = 0;
            try
            {
                var trans = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst == dt).ToList();
                var currentDay = dt.AddDays(dayId);
                for (int i = 0; i < trans.Count; i++) {
                    var traId = trans[i].TransactionID;
                    var traDetail = dbMasakin.tblT_Transaction_Detail.Where(p => p.TransactionID == traId
                    && p.Date == currentDay).FirstOrDefault();
                    if (traDetail != null) {
                        traDetail.Status = StatusModel.Done;
                        dbMasakin.SaveChanges();
                    }

                    if (dayId == 4) {
                        trans[i].Status = StatusModel.Done;
                        dbMasakin.SaveChanges();
                        total += (int)trans[i].Total;
                    }

                    var userId = trans[i].UserID;
                    var did = db.tblM_User_DeviceID.Where(p => p.UserID == userId).FirstOrDefault();
                    var str = did.DeviceID;
                    string[] dids = str.Split(',');
                    for (int a = 0; a < dids.Count(); a++)
                    {
                        cm.AndroidPushNotification(dids[a], "Pesanan Selesai!", "Pesanan Anda untuk hari ini dengan nomor transaksi " + traId +" telah selesai diantar dan dengan ini katering Anda telah berakhir. Semoga Anda puas dengan layanan kami dan ditunggu orderan selanjutnya! Terima kasih!");
                    }

                    var usr = db.tblM_User.Where(p => p.UserID == userId).FirstOrDefault();
                    cm.SMSNotification(usr.Phone, "Pesanan Anda untuk hari ini dengan nomor transaksi " + traId + " telah selesai diantar dan dengan ini katering Anda telah berakhir. Semoga Anda puas dengan layanan kami dan ditunggu orderan selanjutnya! Terima kasih!");
                }

                if (dayId == 4)
                {
                    var merch = db.tblM_User.Where(p => p.UserID == merchantId).FirstOrDefault();
                    merch.Balance += total;
                    db.SaveChanges();
                }

                var merchantz = db.tblM_User.Where(p => p.UserID == merchantId).FirstOrDefault();

                output.status = "success";
                output.data = null;
                output.message = "Transaksi hari ini sudah selesai. Terima kasih atas kerjasamanya, " + merchantz.Name + "!";

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/status/today/{merchantId}")]
        [HttpGet]
        public OutputModel StatusDetailTransaction(int merchantId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            DateTime dtNow = DateTime.Today;

            try
            {
                var trans = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst == dt).ToList();
                for (int i = 0; i < trans.Count; i++)
                {
                    var traId = trans[i].TransactionID;
                    var traDetail = dbMasakin.tblT_Transaction_Detail.Where(p => p.TransactionID == traId
                    && p.Date == dtNow).FirstOrDefault();
                    if (traDetail != null)
                    {
                        output.status = "success";
                        output.data = traDetail.Status;
                        output.message = "Status detail transaksi hari ini untuk merchat id " + merchantId + " adalah : " + traDetail.Status;

                    }
                    else
                    {
                        output.status = "failed";
                        output.data = null;
                        output.message = "Gagal mendapatkan status hari ini untuk merchat id " + merchantId + ". hari ini " + dtNow.ToString() + " and mulai minggu ini " + dt.ToString();
                    }
                }

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/status/week/{merchantId}")]
        [HttpGet]
        public OutputModel StatusDetailWeekTransaction(int merchantId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            //DateTime dtNow = DateTime.Today;
            List<String> stat = new List<string>();
            try
            {
                var trans = dbMasakin.tblT_Transaction.Where(p => p.MerchantID == merchantId && p.DateFirst == dt).ToList();
                if (trans.Count > 0)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        var traId = trans[i].TransactionID;
                        var traDetail = dbMasakin.tblT_Transaction_Detail.Where(p => p.TransactionID == traId).ToList();
                        for (int j = 0; j < traDetail.Count; j++)
                        {
                            stat.Add(traDetail[j].Status);
                        }
                        output.status = "success";
                        output.data = stat;
                        output.message = "Status detail transaksi hari ini untuk merchat id " + merchantId + " berhasil didapatkan";
                    }
                }
                else
                {
                    output.status = "failed";
                    output.data = null;
                    output.message = "Gagal mendapatkan status hari ini untuk merchat id " + merchantId + ". Minggu ini " + dt.ToString();
                }
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/detail/send/{transactionId}")]
        [HttpGet]
        public OutputModel DetailSend(int transactionDetailId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            try
            {
                var tra = dbMasakin.tblT_Transaction_Detail.Where(p => p.DetailID == transactionDetailId).FirstOrDefault();
                tra.Status = StatusModel.Send;
                dbMasakin.SaveChanges();

                output.status = "success";
                output.data = tra;
                output.message = "Detail transaksi sementara dikirimkan dengan detail id: " + transactionDetailId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/detail/finish/{transactionId}")]
        [HttpGet]
        public OutputModel DetailFinish(int transactionDetailId)
        {
            OutputModel output = new OutputModel();
            DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            try
            {
                var tra = dbMasakin.tblT_Transaction_Detail.Where(p => p.DetailID == transactionDetailId).FirstOrDefault();
                tra.Status = StatusModel.Done;
                dbMasakin.SaveChanges();

                if (tra.Date == dt.AddDays(4)) {
                    var traid = tra.TransactionID;
                    var tr = dbMasakin.tblT_Transaction.Where(p => p.TransactionID == traid).First();
                    tr.Status = StatusModel.Done;
                    dbMasakin.SaveChanges();

                    var merid = tr.MerchantID;
                    var merchant = db.tblM_User.Where(p => p.UserID == merid).FirstOrDefault();
                    merchant.Balance += tr.Total;
                    db.SaveChanges();
                }

                output.status = "success";
                output.data = tra;
                output.message = "Detail transaksi selesai diproses dengan detail id: " + transactionDetailId;

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("transaction/return/success/{transactionId}")]
        [HttpGet]
        public HttpResponseMessage SuccessReturn(string transactionId)
        {
            var response = new HttpResponseMessage();
            var traid = cm.Base64Decode(transactionId);
            try
            {
                var tra = dbMasakin.tblT_Transaction.Where(p => p.TransactionID == traid).FirstOrDefault();
                if (tra != null)
                {
                    tra.Status = StatusModel.Paid;
                    tra.isPaid = 1;
                    dbMasakin.SaveChanges();

                    var pay = dbMasakin.tblT_Payment.Where(p => p.TransactionID == traid).FirstOrDefault();
                    pay.Status = StatusModel.Paid;
                    dbMasakin.SaveChanges();

                    var catId = tra.CateringID;
                    var cat = dbMasakin.tblT_Catering.Where(p => p.CateringID == catId).FirstOrDefault();
                    cat.Quantity -= tra.Quantity;
                    cat.Sold += tra.Quantity;
                    dbMasakin.SaveChanges();

                    string dates = String.Format("{0:dd-MM-yyyy}", tra.DateFirst);

                    var merId = tra.MerchantID;
                    var mer = db.tblM_User.Where(p => p.UserID == merId).FirstOrDefault();
                    cm.SMSNotification(mer.Phone, "Selamat! Anda mendapatkan pesanan sebanyak " + tra.Quantity + " buah untuk katering yang dimulai pada tanggal " + dates + ".");

                    var usrId = tra.MerchantID;
                    var user = db.tblM_User.Where(p => p.UserID == usrId).FirstOrDefault();
                    cm.SMSNotification(user.Phone, "Selamat! Pembayaran Anda berhasil untuk nomor transaksi " + traid + " dan katering Anda akan dimulai pada tanggal " + dates + ". Terima kasih!");

                    response.Content = new StringContent("<html><body><center><h1>Pembayaran berhasil untuk nomor transaksi " + traid +". Terima kasih!</h1></center></body></html>");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    response.EnsureSuccessStatusCode();

                }
                else
                {
                    response.Content = new StringContent("<html><body><center><h1>Nomor transaksi " + traid + " tidak ditemukan.</h1></center></body></html>");
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception ex)
            {
                response.Content = new StringContent("<html><body><center><h1>Pesan error: " + ex.ToString() + "</h1></center></body></html>");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                response.EnsureSuccessStatusCode();
            }
            return response;
        }

        [Route("transaction/return/failed/{transactionId}")]
        [HttpGet]
        public HttpResponseMessage FailedReturn(string transactionId)
        {
            var response = new HttpResponseMessage();
            var traid = cm.Base64Decode(transactionId);
            try
            {
                response.Content = new StringContent("<html><body><center><h1>Pembayaran gagal untuk nomor transaksi " + traid + ". Silahkan ulangi kembali</h1></center></body></html>");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                response.Content = new StringContent("<html><body><center><h1>Pesan error: " + ex.ToString() + "</h1></center></body></html>");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                response.EnsureSuccessStatusCode();
            }
            return response;
        }

        [Route("transaction/return/cancel/{transactionId}")]
        [HttpGet]
        public HttpResponseMessage CancelReturn(string transactionId)
        {
            var response = new HttpResponseMessage();
            var traid = cm.Base64Decode(transactionId);
            try
            {
                response.Content = new StringContent("<html><body><center><h1>Pembayaran dibatalkan untuk nomor transaksi " + traid + ". Silahkan ulangi kembali</h1></center></body></html>");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                response.Content = new StringContent("<html><body><center><h1>Pesan error: " + ex.ToString() + "</h1></center></body></html>");
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
                response.EnsureSuccessStatusCode();
            }
            return response;
        }


        [Route("test/androidpushnotification")]
        [HttpGet]
        public OutputModel TestFCM()
        {
            OutputModel output = new OutputModel();
            try
            {
                var did = db.tblM_User_DeviceID.ToList();
                for (int i = 0; i < did.Count; i++)
                {
                    var str = did[i].DeviceID;
                    string[] dids = str.Split(',');
                    for (int a = 0; a < dids.Count(); a++)
                    {
                        cm.AndroidPushNotification(dids[a], "Test", "Berhasil test Android push notification");
                    }
                }

                output.status = "success";
                output.data = null;
                output.message = "Berhasil test Android push notification";

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("test/smsnotification")]
        [HttpGet]
        public OutputModel TestSMSNotification()
        {
            OutputModel output = new OutputModel();
            try
            {
                var did = db.tblM_User.ToList();
                for (int i = 0; i < did.Count; i++)
                {
                    var ph = did[i].Phone;
                    cm.SMSNotification(ph, "Berhasil test sms notification");
                }

                output.status = "success";
                output.data = null;
                output.message = "Berhasil test sms notification";

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }

        [Route("test/smsotp")]
        [HttpGet]
        public OutputModel TestSMSOTP()
        {
            OutputModel output = new OutputModel();
            try
            {
                var did = db.tblM_User.ToList();
                for (int i = 0; i < did.Count; i++)
                {
                    var ph = did[i].Phone;
                    var key = did[i].UserID;
                    cm.SMSOTP(key.ToString(), ph, "");
                }

                output.status = "success";
                output.data = null;
                output.message = "Berhasil test sms notification";

            }
            catch (Exception ex)
            {
                output.status = "error";
                output.data = null;
                output.message = ex.ToString();
            }
            return output;
        }
    }
}