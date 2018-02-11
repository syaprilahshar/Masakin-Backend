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
using Masakin.Models.Catering;

namespace Masakin.Controllers
{
    public class FoodController : ApiController
    {
        Common cm = new Common();
        DB_MasterEntities db_master = new DB_MasterEntities();
        DB_MasakinEntities db_masakin = new DB_MasakinEntities();

        [Route("menu/{merchantId}")]
        [HttpGet]
        public OutputModel GetAllMenu(int merchantId)
        {
            OutputModel output = new OutputModel();
            try
            {
                var data = db_masakin.tblM_Menu.Where(p => p.MerchantID == merchantId
                && p.isActive == 1).Select(p => p).ToList();

                if (data.Count == 0)
                {
                    output.status = "failed";
                    output.message = "Anda belum memiliki menu";
                    output.data = null;
                }
                else
                {
                    output.status = "success";
                    output.message = null;
                    output.data = data;
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

        [Route("menu/add")]
        [HttpPost]
        public OutputModel AddMenu([FromBody] tblM_Menu menu)
        {
            OutputModel output = new OutputModel();
            try
            {
                menu.isActive = 1;
                menu.DateCreated = DateTime.Now;
                db_masakin.tblM_Menu.Add(menu);
                db_masakin.SaveChanges();

                var data = db_masakin.tblM_Menu.Where(p => p.MerchantID == menu.MerchantID 
                            && p.Name == menu.Name
                            && p.Description == menu.Description
                            && p.Price == menu.Price
                            && p.isActive == 1).FirstOrDefault();

                if (data != null)
                {
                    output.status = "success";
                    output.message = null;
                    output.data = data;
                }
                else
                {
                    output.status = "failed";
                    output.message = "Menu not found";
                    output.data = null;
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

        [Route("menu/update")]
        [HttpPost]
        public OutputModel UpdateMenu([FromBody] tblM_Menu menu)
        {
            OutputModel output = new OutputModel();
            try
            {
                var data = db_masakin.tblM_Menu.Where(p => p.MenuID == menu.MenuID
                && p.MerchantID == menu.MerchantID).FirstOrDefault();
                
                if (data != null)
                {
                    data.Name = menu.Name;
                    data.Description = menu.Description;
                    data.Price = menu.Price;
                    data.ImageURL = menu.ImageURL;
                    data.ThumbnailURL = menu.ThumbnailURL;
                    db_masakin.SaveChanges();

                    output.status = "success";
                    output.message = null;
                    output.data = data;
                }
                else
                {
                    output.status = "failed";
                    output.message = "Menu not found";
                    output.data = null;
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

        [Route("menu/{merchantId}/{menuId}")]
        [HttpGet]
        public OutputModel GetMenuById(int merchantId, int menuId)
        {
            OutputModel output = new OutputModel();
            try
            {
                var data = db_masakin.tblM_Menu.Where(p => p.MerchantID == merchantId
                && p.MenuID == menuId
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

        [Route("menu/id/{menuId}")]
        [HttpGet]
        public OutputModel GetMenuByIdforUser(int menuId)
        {
            OutputModel output = new OutputModel();
            MenuShowUser menu = new MenuShowUser();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);
                DateTime dt2 = dt1.AddDays(7);
                DateTime dt3 = dt2.AddDays(7);

                var data = db_masakin.tblM_Menu.Where(p => p.MenuID == menuId
                && p.isActive == 1).Select(p => p).FirstOrDefault();

                menu.MenuID = data.MenuID;
                menu.MerchantID = data.MerchantID;
                menu.Name = data.Name;
                menu.Price = data.Price;
                menu.Description = data.Description;
                menu.ImageURL = data.ImageURL;
                menu.ThumbnailURL = data.ThumbnailURL;

                var user = db_master.tblM_User.Where(p => p.UserID == data.MerchantID && p.isActive == 1).FirstOrDefault();

                menu.MerchantName = user.Name;

                var catering = db_masakin.tblT_Catering.Where(p => p.MerchantID == menu.MerchantID
                && (p.Day1MenuID == menu.MenuID || p.Day2MenuID == menu.MenuID || p.Day3MenuID == menu.MenuID || p.Day4MenuID == menu.MenuID || p.Day5MenuID == menu.MenuID)
                && (p.DateFirst == dt1 || p.DateFirst == dt2 || p.DateFirst == dt3)
                && p.isActive == 1).ToList();

                for (int i = 0; i < catering.Count; i++)
                {
                    if (i == 0)
                    {
                        menu.DateFirst = catering[i].DateFirst.ToString();
                        menu.DateLast = catering[i].DateLast.ToString();
                    }
                    else
                    {
                        menu.DateFirst = menu.DateFirst + "," + catering[i].DateFirst.ToString();
                        menu.DateLast = menu.DateLast + "," + catering[i].DateLast.ToString();
                    }
                }


                var delivery = db_master.tblT_Merchant_Delivery_Location.Where(p => p.UserID == menu.MerchantID
                && p.isActive == 1).ToList();

                for (int i = 0; i < delivery.Count; i++)
                {
                    var kabupatenId = delivery[i].KabupatenID;
                    var location = db_master.tblM_Kabupaten.Where(p => p.KabupatenID == kabupatenId).FirstOrDefault();
                    if (location != null)
                    {
                        if (i == 0)
                        {
                            menu.DeliveryLocation = location.Nama;
                        }
                        else
                        {
                            menu.DeliveryLocation = menu.DeliveryLocation + "," + location.Nama;
                        }
                    }
                }
                
                output.status = "success";
                output.message = null;
                output.data = menu;
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }
            return output;
        }

        [Route("catering/{merchantId}")]
        [HttpGet]
        public OutputModel GetAllCatering(int merchantId)
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                var data = db_masakin.tblT_Catering.Where(p => p.MerchantID == merchantId
                && p.DateFirst >= dt
                && p.isActive == 1).Select(p => p).ToList();

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

        [Route("catering/{merchantId}/{cateringId}")]
        [HttpGet]
        public OutputModel GetCateringById(int merchantId, int cateringId)
        {
            OutputModel output = new OutputModel();
            try
            {
                var data = db_masakin.tblT_Catering.Where(p => p.MerchantID == merchantId
                && p.CateringID == cateringId
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

        [Route("catering/update")]
        [HttpPost]
        public OutputModel UpdateCatering([FromBody] tblT_Catering ctr)
        {
            OutputModel output = new OutputModel();
            try
            {
                var data = db_masakin.tblT_Catering.Where(p => p.MerchantID == ctr.MerchantID
                && p.LocalID == ctr.LocalID
                && p.CateringID == ctr.CateringID
                && p.isActive == 1).Select(p => p).FirstOrDefault();

                if (ctr.Day1MenuID != data.Day1MenuID && ctr.Day2MenuID != data.Day2MenuID && ctr.Day3MenuID != data.Day3MenuID && ctr.Day4MenuID != data.Day4MenuID && ctr.Day5MenuID != data.Day5MenuID)
                {
                    data.LocalID = data.CateringID;
                    data.Rate = 0;
                }

                data.Day1MenuID = ctr.Day1MenuID;
                data.Day2MenuID = ctr.Day2MenuID;
                data.Day3MenuID = ctr.Day3MenuID;
                data.Day4MenuID = ctr.Day4MenuID;
                data.Day5MenuID = ctr.Day5MenuID;
                data.Description = ctr.Description;
                data.Name = ctr.Name;
                data.Quantity = ctr.Quantity;
                data.Price = ctr.Price;
                data.isActive = ctr.isActive;
                data.DateModified = DateTime.Now;

                db_masakin.SaveChanges();

                if (data.isRepeat == 1)
                    GenerateCateringAfterGetAll((int)data.MerchantID);

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

        [Route("catering/submit")]
        [HttpPost]
        public OutputModel SubmitCatering([FromBody] tblT_Catering ctr)
        {
            OutputModel output = new OutputModel();
            try
            {
                var check = db_masakin.tblT_Catering.Where(p => p.MerchantID == ctr.MerchantID
                && p.DateFirst == ctr.DateFirst
                && p.isActive == 1).Select(p => p).FirstOrDefault();

                if (check == null)
                {
                    ctr.Rate = 0;
                    ctr.isActive = 1;
                    ctr.Sold = 0;
                    ctr.Temporary = 0;
                    db_masakin.tblT_Catering.Add(ctr);
                    db_masakin.SaveChanges();

                    var data = db_masakin.tblT_Catering.Where(p => p.MerchantID == ctr.MerchantID
                    && p.DateFirst == ctr.DateFirst
                    && p.isActive == 1).Select(p => p).FirstOrDefault();
                    data.DateCreated = DateTime.Now;
                    data.DateModified = DateTime.Now;
                    data.LocalID = data.CateringID;
                    db_masakin.SaveChanges();

                    if (ctr.isRepeat == 1)
                    {
                        if (ctr.DateFirst == DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(7))
                        {
                            DateTime dt2 = data.DateFirst.Value.AddDays(7);
                            DateTime dt2L = data.DateLast.Value.AddDays(7);
                            var week2 = db_masakin.tblT_Catering.Where(P => P.MerchantID == data.MerchantID
                                        && P.DateFirst == dt2
                                        && P.isActive == 1).FirstOrDefault();
                            if (week2 == null)
                            {
                                var ctr2 = data;
                                ctr2.DateCreated = DateTime.Now;
                                ctr2.DateFirst = dt2;
                                ctr2.DateLast = dt2L;
                                db_masakin.tblT_Catering.Add(ctr2);
                                db_masakin.SaveChanges();
                            }

                            DateTime dt3 = data.DateFirst.Value.AddDays(7);
                            DateTime dt3L = data.DateLast.Value.AddDays(7);
                            var week3 = db_masakin.tblT_Catering.Where(P => P.MerchantID == data.MerchantID
                                        && P.DateFirst == dt3
                                        && P.isActive == 1).FirstOrDefault();
                            if (week3 == null)
                            {
                                var ctr3 = data;
                                ctr3.DateCreated = DateTime.Now;
                                ctr3.DateFirst = dt3;
                                ctr3.DateLast = dt3L;
                                db_masakin.tblT_Catering.Add(ctr3);
                                db_masakin.SaveChanges();
                            }
                        }
                        else if (ctr.DateFirst == DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(14))
                        {
                            DateTime dt3 = data.DateFirst.Value.AddDays(7);
                            DateTime dt3L = data.DateLast.Value.AddDays(7);
                            var week3 = db_masakin.tblT_Catering.Where(P => P.MerchantID == data.MerchantID
                                        && P.DateFirst == dt3
                                        && P.isActive == 1).FirstOrDefault();
                            if (week3 == null)
                            {
                                var ctr3 = data;
                                ctr3.DateCreated = DateTime.Now;
                                ctr3.DateFirst = dt3;
                                ctr3.DateLast = dt3L;
                                db_masakin.tblT_Catering.Add(ctr3);
                                db_masakin.SaveChanges();
                            }
                        }
                    }

                    output.status = "success";
                    output.message = null;
                    output.data = null;
                }
                else
                {
                    output.status = "failed";
                    output.message = "Katering untuk periode tersebut sudah tersedia.";
                    output.data = null;
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

        [Route("catering/generate")]
        [HttpGet]
        public OutputModel GenerateCatering()
        {
            OutputModel output = new OutputModel();
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);
                DateTime dt2 = dt1.AddDays(7);
                DateTime dt3 = dt2.AddDays(7);
                var list = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt
                && p.isActive == 1).Select(p => p).ToList();

                for (int i = 0; i < list.Count; i++) {
                    if (list[i].isRepeat == 1) {
                        var merId = list[i].MerchantID;
                        var f1 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt1
                                                && p.MerchantID == merId
                                                && p.isActive == 1).Select(p => p).FirstOrDefault();
                        if (f1 == null)
                        {
                            f1 = list[i];
                            f1.Sold = 0;
                            f1.Temporary = 0;
                            f1.DateCreated = DateTime.Now;
                            f1.DateModified = DateTime.Now;
                            f1.DateFirst = dt1;
                            f1.DateLast = dt1.AddDays(4);
                            db_masakin.tblT_Catering.Add(f1);
                            db_masakin.SaveChanges();
                        }

                        var f2 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt2
                                                && p.MerchantID == merId
                                                && p.isActive == 1).Select(p => p).FirstOrDefault();
                        if (f2 == null)
                        {
                            f2 = list[i];
                            f2.Sold = 0;
                            f2.Temporary = 0;
                            f2.DateCreated = DateTime.Now;
                            f2.DateModified = DateTime.Now;
                            f2.DateFirst = dt2;
                            f2.DateLast = dt2.AddDays(4);
                            db_masakin.tblT_Catering.Add(f2);
                            db_masakin.SaveChanges();
                        }

                        var f3 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt3
                                                && p.MerchantID == merId
                                                && p.isActive == 1).Select(p => p).FirstOrDefault();
                        if (f3 == null)
                        {
                            f3 = list[i];
                            f3.Sold = 0;
                            f3.Temporary = 0;
                            f3.DateCreated = DateTime.Now;
                            f3.DateModified = DateTime.Now;
                            f3.DateFirst = dt3;
                            f3.DateLast = dt3.AddDays(4);
                            db_masakin.tblT_Catering.Add(f3);
                            db_masakin.SaveChanges();
                        }
                    }
                }

                output.status = "success";
                output.message = null;
                output.data = null;
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }
            return output;
        }

        [Route("catering/generate/{MerchantId}")]
        [HttpGet]
        public OutputModel GenerateCateringByMerchant(int MerchantId)
        {
            OutputModel output = new OutputModel();
            GenerateCatering catering = new GenerateCatering();
            catering.MerchantID = MerchantId;

            try
            {
                GenerateCateringAfterGetAll(MerchantId);
                
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(7);
                DateTime dt2 = dt.AddDays(7);
                DateTime dt3 = dt2.AddDays(7);
                var week1 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt
                && p.isActive == 1
                && p.MerchantID == MerchantId).Select(p => p).FirstOrDefault();

                catering.Week1DateFirst = dt;
                catering.Week1DateLast = dt.AddDays(4);
                catering.Week2DateFirst = dt2;
                catering.Week2DateLast = dt2.AddDays(4);
                catering.Week3DateFirst = dt3;
                catering.Week3DateLast = dt3.AddDays(4);

                if (week1 == null)
                {
                    catering.Week1 = false;
                    catering.Week2 = false;
                    catering.Week3 = false;                    
                }
                else {
                    catering.Week1 = true;
                    catering.CateringWeek1 = week1;
                    List<tblM_Menu> menuWeek1 = new List<tblM_Menu>();
                    var menu1 = db_masakin.tblM_Menu.Where(p => p.MenuID == week1.Day1MenuID).Select(p => p).FirstOrDefault();
                    menuWeek1.Add(menu1);

                    var menu2 = db_masakin.tblM_Menu.Where(p => p.MenuID == week1.Day2MenuID).Select(p => p).FirstOrDefault();
                    menuWeek1.Add(menu2);

                    var menu3 = db_masakin.tblM_Menu.Where(p => p.MenuID == week1.Day3MenuID).Select(p => p).FirstOrDefault();
                    menuWeek1.Add(menu3);

                    var menu4 = db_masakin.tblM_Menu.Where(p => p.MenuID == week1.Day4MenuID).Select(p => p).FirstOrDefault();
                    menuWeek1.Add(menu4);

                    var menu5 = db_masakin.tblM_Menu.Where(p => p.MenuID == week1.Day5MenuID).Select(p => p).FirstOrDefault();
                    menuWeek1.Add(menu5);

                    catering.CateringWeek1.Menu = menuWeek1;
                }

                var week2 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt2
                && p.isActive == 1
                && p.MerchantID == MerchantId).Select(p => p).FirstOrDefault();

                if (week2 == null)
                {
                    catering.Week2 = false;
                    catering.Week3 = false;
                }
                else
                {
                    catering.Week2 = true;
                    catering.CateringWeek2 = week2;
                    List<tblM_Menu> menuWeek2 = new List<tblM_Menu>();
                    var menu1 = db_masakin.tblM_Menu.Where(p => p.MenuID == week2.Day1MenuID).Select(p => p).FirstOrDefault();
                    menuWeek2.Add(menu1);

                    var menu2 = db_masakin.tblM_Menu.Where(p => p.MenuID == week2.Day2MenuID).Select(p => p).FirstOrDefault();
                    menuWeek2.Add(menu2);

                    var menu3 = db_masakin.tblM_Menu.Where(p => p.MenuID == week2.Day3MenuID).Select(p => p).FirstOrDefault();
                    menuWeek2.Add(menu3);

                    var menu4 = db_masakin.tblM_Menu.Where(p => p.MenuID == week2.Day4MenuID).Select(p => p).FirstOrDefault();
                    menuWeek2.Add(menu4);

                    var menu5 = db_masakin.tblM_Menu.Where(p => p.MenuID == week2.Day5MenuID).Select(p => p).FirstOrDefault();
                    menuWeek2.Add(menu5);

                    catering.CateringWeek2.Menu = menuWeek2;
                }

                var week3 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt3
                && p.isActive == 1
                && p.MerchantID == MerchantId).Select(p => p).FirstOrDefault();

                if (week3 == null)
                {
                    catering.Week3 = false;
                }
                else
                {
                    catering.Week3 = true;
                    catering.CateringWeek3 = week3;
                    List<tblM_Menu> menuWeek3 = new List<tblM_Menu>();
                    var menu1 = db_masakin.tblM_Menu.Where(p => p.MenuID == week3.Day1MenuID).Select(p => p).FirstOrDefault();
                    menuWeek3.Add(menu1);

                    var menu2 = db_masakin.tblM_Menu.Where(p => p.MenuID == week3.Day2MenuID).Select(p => p).FirstOrDefault();
                    menuWeek3.Add(menu2);

                    var menu3 = db_masakin.tblM_Menu.Where(p => p.MenuID == week3.Day3MenuID).Select(p => p).FirstOrDefault();
                    menuWeek3.Add(menu3);

                    var menu4 = db_masakin.tblM_Menu.Where(p => p.MenuID == week3.Day4MenuID).Select(p => p).FirstOrDefault();
                    menuWeek3.Add(menu4);

                    var menu5 = db_masakin.tblM_Menu.Where(p => p.MenuID == week3.Day5MenuID).Select(p => p).FirstOrDefault();
                    menuWeek3.Add(menu5);

                    catering.CateringWeek3.Menu = menuWeek3;
                }

                output.status = "success";
                output.message = null;
                output.data = catering;
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }
            return output;
        }

        [Route("catering/week/{weekId}")]
        [HttpGet]
        public OutputModel AllCateringByWeek(int weekId)
        {
            OutputModel output = new OutputModel();
            
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(7);
                DateTime dt2 = dt.AddDays(7);
                DateTime dt3 = dt2.AddDays(7);
                DateTime fixDate;

                if (weekId == 1)
                    fixDate = dt;
                else if (weekId == 2)
                    fixDate = dt2;
                else
                    fixDate = dt3;

                var cat = db_masakin.tblT_Catering.Where(p => p.DateFirst == fixDate
                && p.isActive == 1).Select(p => p).ToList();

                for (int i = 0; i < cat.Count; i++) {
                    var MerchId = cat[i].MerchantID;
                    cat[i].Merchant = db_master.tblM_User.Where(p => p.UserID == MerchId).FirstOrDefault();

                    List<tblM_Menu> menu = new List<tblM_Menu>();
                    var day1 = cat[i].Day1MenuID;
                    var menu1 = db_masakin.tblM_Menu.Where(p => p.MenuID == day1).Select(p => p).FirstOrDefault();
                    menu.Add(menu1);

                    var day2 = cat[i].Day2MenuID;
                    var menu2 = db_masakin.tblM_Menu.Where(p => p.MenuID == day2).Select(p => p).FirstOrDefault();
                    menu.Add(menu2);

                    var day3 = cat[i].Day3MenuID;
                    var menu3 = db_masakin.tblM_Menu.Where(p => p.MenuID == day3).Select(p => p).FirstOrDefault();
                    menu.Add(menu3);

                    var day4 = cat[i].Day4MenuID;
                    var menu4 = db_masakin.tblM_Menu.Where(p => p.MenuID == day4).Select(p => p).FirstOrDefault();
                    menu.Add(menu4);

                    var day5 = cat[i].Day5MenuID;
                    var menu5 = db_masakin.tblM_Menu.Where(p => p.MenuID == day5).Select(p => p).FirstOrDefault();
                    menu.Add(menu5);

                    cat[i].Menu = menu;

                    var delivery = db_master.tblT_Merchant_Delivery_Location.Where(p => p.UserID == MerchId
                        && p.isActive == 1).ToList();

                    for (int j = 0; j < delivery.Count; j++)
                    {
                        var kabupatenId = delivery[j].KabupatenID;
                        var location = db_master.tblM_Kabupaten.Where(p => p.KabupatenID == kabupatenId).FirstOrDefault();
                        if (location != null)
                        {
                            if (j == 0)
                            {
                                cat[i].DeliveryLocation = location.Nama;
                            }
                            else
                            {
                                cat[i].DeliveryLocation = cat[i].DeliveryLocation + "," + location.Nama;
                            }
                        }
                    }
                }
                
                output.status = "success";
                output.message = null;
                output.data = cat;
            }
            catch (Exception ex)
            {
                output.status = "error";
                output.message = ex.Message;
                output.data = null;
            }
            return output;
        }

        [Route("catering/last/week/{weekId}")]
        [HttpGet]
        public OutputModel LastCateringByWeek(int weekId)
        {
            OutputModel output = new OutputModel();

            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday).AddDays(7);
                DateTime dt2 = dt.AddDays(7);
                DateTime dt3 = dt2.AddDays(7);
                DateTime fixDate;

                if (weekId == 1)
                    fixDate = dt;
                else if (weekId == 2)
                    fixDate = dt2;
                else
                    fixDate = dt3;

                var cat = db_masakin.tblT_Catering.Where(p => p.DateFirst == fixDate
                && p.isActive == 1).Select(p => p).OrderByDescending(p => p.CateringID).FirstOrDefault();

                if (cat != null)
                {
                    var MerchId = cat.MerchantID;
                    cat.Merchant = db_master.tblM_User.Where(p => p.UserID == MerchId).FirstOrDefault();

                    List<tblM_Menu> menu = new List<tblM_Menu>();
                    var day1 = cat.Day1MenuID;
                    var menu1 = db_masakin.tblM_Menu.Where(p => p.MenuID == day1).Select(p => p).FirstOrDefault();
                    menu.Add(menu1);

                    var day2 = cat.Day2MenuID;
                    var menu2 = db_masakin.tblM_Menu.Where(p => p.MenuID == day2).Select(p => p).FirstOrDefault();
                    menu.Add(menu2);

                    var day3 = cat.Day3MenuID;
                    var menu3 = db_masakin.tblM_Menu.Where(p => p.MenuID == day3).Select(p => p).FirstOrDefault();
                    menu.Add(menu3);

                    var day4 = cat.Day4MenuID;
                    var menu4 = db_masakin.tblM_Menu.Where(p => p.MenuID == day4).Select(p => p).FirstOrDefault();
                    menu.Add(menu4);

                    var day5 = cat.Day5MenuID;
                    var menu5 = db_masakin.tblM_Menu.Where(p => p.MenuID == day5).Select(p => p).FirstOrDefault();
                    menu.Add(menu5);

                    cat.Menu = menu;

                    var delivery = db_master.tblT_Merchant_Delivery_Location.Where(p => p.UserID == MerchId
                        && p.isActive == 1).ToList();

                    for (int j = 0; j < delivery.Count; j++)
                    {
                        var kabupatenId = delivery[j].KabupatenID;
                        var location = db_master.tblM_Kabupaten.Where(p => p.KabupatenID == kabupatenId).FirstOrDefault();
                        if (location != null)
                        {
                            if (j == 0)
                            {
                                cat.DeliveryLocation = location.Nama;
                            }
                            else
                            {
                                cat.DeliveryLocation = cat.DeliveryLocation + "," + location.Nama;
                            }
                        }
                    }

                    output.status = "success";
                    output.message = null;
                    output.data = cat;
                }
                else
                {
                    output.status = "failed";
                    output.message = null;
                    output.data = null;
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

        //[Route("menu/catering/submit/{merchantId}")]
        //[HttpGet]
        //public OutputModel GetNextCatering(int merchantId)
        //{
        //    OutputModel output = new OutputModel();
        //    try
        //    {
        //        DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
        //        DateTime dt2 = dt.AddDays(7);
        //        DateTime dt3 = dt2.AddDays(7);

        //        List<tblT_Catering> ctr = new List<tblT_Catering>();

        //        var data = db_masakin.tblT_Catering.Where(p => p.MerchantID == merchantId
        //        && p.DateFirst == dt
        //        && p.isActive == 1).Select(p => p).LastOrDefault();

        //        if (data != null) {
        //            if (data.isRepeat == 1) { }
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




        private string checkPhone(string ph)
        {
            string em = "null";
            var data = db_master.tblM_User.Where(p =>
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


        public void GenerateCateringAfterGetAll(int MerchantId)
        {
            try
            {
                DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                DateTime dt1 = dt.AddDays(7);
                DateTime dt2 = dt1.AddDays(7);
                DateTime dt3 = dt2.AddDays(7);
                var list = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt
                && p.MerchantID == MerchantId
                && p.isActive == 1).Select(p => p).FirstOrDefault();
              
                if (list.isRepeat == 1)
                {
                    var merId = list.MerchantID;
                    var f1 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt1
                                            && p.MerchantID == merId
                                            && p.isActive == 1).Select(p => p).FirstOrDefault();
                    if (f1 == null)
                    {
                        f1 = list;
                        f1.Quantity = list.Quantity + list.Sold;
                        f1.Sold = 0;
                        f1.Temporary = 0;
                        f1.DateCreated = DateTime.Now;
                        f1.DateModified = DateTime.Now;
                        f1.DateFirst = dt1;
                        f1.DateLast = dt1.AddDays(4);
                        db_masakin.tblT_Catering.Add(f1);
                        db_masakin.SaveChanges();
                    }

                    var f2 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt2
                                            && p.MerchantID == merId
                                            && p.isActive == 1).Select(p => p).FirstOrDefault();
                    if (f2 == null)
                    {
                        f2 = list;
                        f2.Quantity = list.Quantity + list.Sold;
                        f2.Sold = 0;
                        f2.Temporary = 0;
                        f2.DateCreated = DateTime.Now;
                        f2.DateModified = DateTime.Now;
                        f2.DateFirst = dt2;
                        f2.DateLast = dt2.AddDays(4);
                        db_masakin.tblT_Catering.Add(f2);
                        db_masakin.SaveChanges();
                    }
                    
                    var f3 = db_masakin.tblT_Catering.Where(p => p.DateFirst == dt3
                                            && p.MerchantID == merId
                                            && p.isActive == 1).Select(p => p).FirstOrDefault();
                    if (f3 == null)
                    {
                        f3 = list;
                        f3.Quantity = list.Quantity + list.Sold;
                        f3.Sold = 0;
                        f3.Temporary = 0;
                        f3.DateCreated = DateTime.Now;
                        f3.DateModified = DateTime.Now;
                        f3.DateFirst = dt3;
                        f3.DateLast = dt3.AddDays(4);
                        db_masakin.tblT_Catering.Add(f3);
                        db_masakin.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}