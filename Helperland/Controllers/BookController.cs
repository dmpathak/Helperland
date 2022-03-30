using Helperland.Models.ViewModels;
using Helperland.Models;
using Helperland.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Helperland.ForSendemail;

namespace Helperland.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly HelperlandContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Sendemail sendemail = new Sendemail();

        public BookController(ILogger<BookController> logger, HelperlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult BookService()
        {
            //in bookservice i've used _layout.cshtml, that's why i used this if condition otherwise i know this should be only for usertype => 1
            if (User.Identity.IsAuthenticated)
            {
                var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var current_user_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();

                //also getting favourite providers list with this get request
                var get_fav_provider = context.FavoriteAndBlockeds.Where(x => x.UserId == current_user_Id && x.IsFavorite == true && x.IsBlocked == false).ToList();
                var my_fav_pro = new List<object>();

                if (get_fav_provider.Count() != 0)
                {
                    foreach (var fav in get_fav_provider)
                    {
                        var check_blocked_by_provider = context.FavoriteAndBlockeds.Where(x => x.UserId == fav.TargetUserId && x.TargetUserId == current_user_Id).FirstOrDefault();
                        if (check_blocked_by_provider == null || check_blocked_by_provider != null && check_blocked_by_provider.IsBlocked == false)
                        {
                            var my_pro = context.Users.Where(x => x.UserId == fav.TargetUserId).FirstOrDefault();
                            my_fav_pro.Add(new
                            {
                                provider_id = my_pro.UserId,
                                avtar = my_pro.UserProfilePicture,
                                name = my_pro.FirstName + " " + my_pro.LastName,


                            });
                        }
                    }
                ViewBag.my_fav_pro = my_fav_pro;
                }

                //nichenu null hoi shake cshtml ma handle karvu
                ViewBag.usertypeid = current_user_data.UserTypeId;
            }
            return View();
        }

        [HttpPost]
        public IActionResult CheckAvailability([FromBody] CheckavailViewModel avail)
        {
            var database_zipcode = context.Users.Where(x => x.ZipCode == avail.MyCheckavail && x.UserTypeId == 2).FirstOrDefault();
            //for getting city from zipcode 
            var zipcode = context.Zipcodes.Where(x => x.ZipcodeValue == avail.MyCheckavail).FirstOrDefault();
            var city = context.Cities.Where(x => x.Id == zipcode.CityId).FirstOrDefault();


            if (avail.MyCheckavail != null && database_zipcode != null)
            {
                var currentuserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var address_data = new List<object>();
                var my_address = context.UserAddresses.Where(x => x.UserId == currentuserId && x.PostalCode == avail.MyCheckavail).ToList();

                if (my_address.Count != 0)
                {

                    foreach (var eachaddress in my_address)
                    {
                        Console.WriteLine(eachaddress.AddressLine2);
                        address_data.Add(new
                        {
                            address = "" + eachaddress.AddressLine1 + " " + eachaddress.AddressLine2 + " " + eachaddress.City + " " + eachaddress.State,
                            phone = eachaddress.Mobile,
                            address_id = eachaddress.AddressId
                        });
                    }

                    var data = new
                    {
                        is_available = true,
                        addresses = address_data,
                        city = city.CityName,
                    };

                    return Json(data);
                }
                else
                {
                    var data = new
                    {
                        is_available = true,
                        addresses = false,
                        city = city.CityName,
                    };

                    return Json(data);
                }
            }
            else
            {
                return Json(false);
            }
        }

        [HttpPost]
        public IActionResult save_new_address([FromBody] CheckavailViewModel avail)
        {
            var add = new UserAddress();
            add.UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            add.AddressLine2 = avail.street_name;
            add.AddressLine1 = avail.house_number;
            add.PostalCode = avail.pincode;
            add.City = avail.city;
            add.Mobile = avail.phone_number;

            context.UserAddresses.Add(add);
            context.SaveChanges();
            return Json(new { address_id = add.AddressId, address = "" + add.AddressLine1 + " " + add.AddressLine2 + " " + add.City + " " + add.State, phone = add.Mobile });
        }


        [HttpPost]
        public IActionResult FinaldataSubmit([FromBody] CheckavailViewModel avail)
        {
            var s1 = new ServiceRequest();
            var s2 = new ServiceRequestAddress();
            var s3 = new ServiceRequestExtra();

            try
            {
                var random_service_id = new Random().Next(1000, 9999);
                var database_service_id = context.ServiceRequests.Where(x => x.ServiceId == random_service_id).FirstOrDefault();
                while (database_service_id != null)
                {
                    random_service_id = new Random().Next(1000, 9999);
                    database_service_id = context.ServiceRequests.Where(x => x.ServiceId == random_service_id).FirstOrDefault();
                }

                var requests = context.ServiceRequests.ToList();

                s1.UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var request_forloop = context.ServiceRequests.Where(x => x.ServiceProviderId == avail.selected_service_pro_id).ToList();

                var s = avail.date.Split("/");
                var d = avail.time.Split(":");
                var datefinal = new DateTime(Int32.Parse(s[2]), Int32.Parse(s[1]), Int32.Parse(s[0]), Int32.Parse(d[0]), Int32.Parse(d[1]), 0);

                var total_hours = Convert.ToDouble(avail.total_time);
                var datefinal_end = datefinal.AddHours(total_hours).AddMinutes(60);

                if (avail.selected_service_pro_id != 0)
                {
                    foreach (var service in request_forloop)
                    {
                        var request_start = service.ServiceStartDate;

                        var request_end = request_start.AddHours(total_hours).AddMinutes(60);

                        if ((DateTime.Compare(request_start, datefinal) <= 0 && DateTime.Compare(request_end, datefinal) > 0) || (DateTime.Compare(request_start, datefinal_end) < 0 && DateTime.Compare(request_end, datefinal_end) >= 0) || (DateTime.Compare(request_end, datefinal_end) > 0 && DateTime.Compare(request_start, datefinal) < 0))
                        {
                            return Json(new { isnoterror = false, conflict_error = "Another service request has already been assigned which has time overlap with this service request. You can’t pick this Provider!" });

                        }
                    }
                    s1.ServiceProviderId = avail.selected_service_pro_id;
                    s1.Status = 4;
                    s1.SpacceptedDate = DateTime.Now;

                    //for send mail directly assigned provider
                    var selected_service_pro_data = context.Users.Where(x => x.UserId == avail.selected_service_pro_id).FirstOrDefault();
                    if (selected_service_pro_data != null)
                    {

                        var direct_acceptence_email = new EmailModel()
                        {
                            To = selected_service_pro_data.Email,
                            Subject = "service acceptence mail",
                            Body = "A service request" + random_service_id + "has been directly assigned to you",

                        };
                        sendemail.emailSend(direct_acceptence_email);
                    }

                }
                else
                {
                    s1.Status = 1;
                }

                s1.ServiceId = random_service_id;
                //var s = avail.date.Split("/");
                //var d = avail.time.Split(":");
                //var datefinal = new DateTime(Int32.Parse(s[2]), Int32.Parse(s[1]), Int32.Parse(s[0]), Int32.Parse(d[0]), Int32.Parse(d[1]), 0);
                s1.ServiceStartDate = datefinal;
                s1.ServiceHours = Convert.ToDouble(avail.basic_time);
                s1.ExtraHours = Convert.ToDouble(avail.total_time) - Convert.ToDouble(avail.basic_time);
                s1.TotalCost = Convert.ToDecimal(avail.total_time) * 18;
                s1.Comments = avail.comments;
                s1.HasPets = avail.have_pets;
                s1.ZipCode = avail.pincode;

                context.ServiceRequests.Add(s1);
                context.SaveChanges();


                var useraddress_database = context.UserAddresses.Where(x => x.AddressId == avail.address_id).FirstOrDefault();
                s2.ServiceRequestId = s1.ServiceRequestId;
                s2.AddressLine1 = useraddress_database.AddressLine1;
                s2.AddressLine2 = useraddress_database.AddressLine2;
                s2.City = useraddress_database.City;
                s2.PostalCode = useraddress_database.PostalCode;
                s2.Mobile = useraddress_database.Mobile;
                context.ServiceRequestAddresses.Add(s2);
                context.SaveChanges();

                var count = 0;
                foreach (var service in avail.extra_services)
                {
                    count = count + 1;

                    if (service == true)
                    {

                        s3.ServiceRequestId = s1.ServiceRequestId;
                        s3.ServiceExtraId = count;
                        context.ServiceRequestExtras.Add(s3);
                    }
                }
                context.SaveChanges();

                //for sending mail 
                var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var provider_at_pin = context.Users.Where(x => x.ZipCode == avail.pincode && x.UserTypeId == 2).ToList();

                foreach (var provider in provider_at_pin)
                {
                    var is_blocked_by_cust = context.FavoriteAndBlockeds.Where(x => x.UserId == current_user_Id && x.TargetUserId == provider.UserId).FirstOrDefault();
                    var is_blocked_by_pro = context.FavoriteAndBlockeds.Where(x => x.UserId == provider.UserId && x.TargetUserId == current_user_Id).FirstOrDefault();

                    if ((is_blocked_by_cust == null || (is_blocked_by_cust != null && is_blocked_by_cust.IsBlocked == false)) && (is_blocked_by_pro == null || (is_blocked_by_pro != null && is_blocked_by_pro.IsBlocked == false)))
                    {
                        var new_task_email = new EmailModel()
                        {
                            To = provider.Email,
                            Subject = "This mail is generated for New Service Request is booked By customer",
                            Body = "New Task Available, Service Request Id:" + s1.ServiceId,

                        };
                        sendemail.emailSend(new_task_email);
                    }
                }
                return Json(new { isnoterror = true, ServiceId = s1.ServiceId });



            }
            catch
            {
                return Json(new { isnoterror = false, server_error = "Internal Server Error" });
            }
        }

    }
}
