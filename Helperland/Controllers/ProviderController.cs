using Helperland.Models.ViewModels;
using Helperland.Models;
using Helperland.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Globalization;
using BCrypto = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authorization;
using Helperland.ForSendemail;

namespace Helperland.Controllers
{
    [Authorize]
    public class ProviderController : Controller
    {
        private readonly ILogger<ProviderController> _logger;
        private readonly HelperlandContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Sendemail sendemail = new Sendemail();

        public ProviderController(ILogger<ProviderController> logger, HelperlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }


        public IActionResult NewServiceRequest()
        {
            var current_provider_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var current_provider_data = context.Users.Where(x => x.UserId == current_provider_id).FirstOrDefault();
            var requests = context.ServiceRequests.Where(x => x.Status == 1 && x.ZipCode == current_provider_data.ZipCode).ToList();
            var new_requests = new List<object>();
            if (requests.Count() != 0)
            {
                foreach (var item in requests)
                {
                    //for getting extra service 
                    var extra_got = (
                 from r in context.ServiceRequests
                 join e in context.ServiceRequestExtras
                 on r.ServiceRequestId equals e.ServiceRequestId
                 where r.ServiceId == item.ServiceId
                 select e.ServiceExtraId);

                    var x = "";
                    foreach (var service in extra_got)
                    {
                        if (service == 1)
                        {
                            x = x + "Inside cabinets, ";
                        }
                        if (service == 2)
                        {
                            x = x + "Inside fridge, ";
                        }
                        if (service == 3)
                        {
                            x = x + "Inside oven, ";
                        }
                        if (service == 4)
                        {
                            x = x + "Laundry wash & dry, ";
                        }
                        if (service == 5)
                        {
                            x = x + "Interior windows, ";
                        }
                    }

                    var isblock = context.FavoriteAndBlockeds.Where(x => x.UserId == current_provider_id && x.TargetUserId == item.UserId).FirstOrDefault();
                    var check_blocked_by_cust = context.FavoriteAndBlockeds.Where(x => x.UserId == item.UserId && x.TargetUserId == current_provider_id).FirstOrDefault();

                    if ((isblock == null || (isblock != null && isblock.IsBlocked == false)) && (check_blocked_by_cust == null || (check_blocked_by_cust != null && check_blocked_by_cust.IsBlocked == false)))
                    {

                        new_requests.Add(new
                        {
                            //serviceprovider_id = item.ServiceProviderId,
                            myServiceId = item.ServiceId,

                            myservice_start_date = item.ServiceStartDate.Day + "/" + item.ServiceStartDate.Month + "/" + item.ServiceStartDate.Year,
                            myservice_start_time = item.ServiceStartDate.Hour + ":" + item.ServiceStartDate.Minute,
                            myservice_end_time = item.ServiceStartDate.AddHours(Convert.ToDouble(item.ServiceHours + item.ExtraHours)).TimeOfDay.Hours + ":" + item.ServiceStartDate.AddHours(Convert.ToDouble(item.ServiceHours + item.ExtraHours)).TimeOfDay.Minutes,
                            service_duration = item.ServiceHours + item.ExtraHours,

                            extras = x,

                            my_customer_address = (
                                            from r in context.ServiceRequests
                                            join a in context.ServiceRequestAddresses
                                            on r.ServiceRequestId equals a.ServiceRequestId
                                            where r.ServiceId == item.ServiceId
                                            select new { AddressLine1 = a.AddressLine1, AddressLine2 = a.AddressLine2, PostalCode = a.PostalCode, City = a.City }).FirstOrDefault(),
                            my_customer_name = (
                                            from r in context.ServiceRequests
                                            join u in context.Users
                                            on r.UserId equals u.UserId
                                            where r.ServiceId == item.ServiceId
                                            select new { FirstName = u.FirstName, LastName = u.LastName }).FirstOrDefault(),

                            mypayment = item.TotalCost,

                            pet = item.HasPets
                        });
                    }
                }
                ViewBag.current_new_req = new_requests;
            }
            return View();
        }

        [HttpPost]
        public JsonResult Accept([FromBody] ProviderDashboardViewModel provideraccept)
        {
            var Service_id = provideraccept.ServiceId;

            //database row for particulr service id
            var service_data = context.ServiceRequests.Where(x => x.ServiceId == Service_id).FirstOrDefault();

            var total_hours = Convert.ToDouble(service_data.ServiceHours + service_data.ExtraHours);
            var current_sid_start_datetime = service_data.ServiceStartDate.AddMinutes(-60);
            var current_sid_end_datetime = service_data.ServiceStartDate.AddHours(total_hours).AddMinutes(60);

            //all rows from database for particular provider
            var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var current_provider_data = context.ServiceRequests.Where(x => x.ServiceProviderId == current_user_Id).ToList();


            if (service_data.ServiceProviderId != null)
            {
                return Json(new { error = "another" });
            }
            else
            {
                if (current_provider_data.Count == 0)
                {
                    service_data.ServiceProviderId = current_user_Id;
                    service_data.Status = 4;
                    context.SaveChanges();

                    //for send mail of accepted 
                    var provider_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();
                    var provider_at_pincode = context.Users.Where(x => x.ZipCode == provider_data.ZipCode && x.UserTypeId == 2).ToList();

                    foreach (var provider in provider_at_pincode)
                    {
                        if (provider.UserId == current_user_Id)
                        {
                            var acceptence_email = new EmailModel()
                            {
                                To = provider.Email,
                                Subject = "service acceptence mail",
                                Body = "new service has been assigned to you with Service Id " + Service_id,

                            };
                            sendemail.emailSend(acceptence_email);
                        }
                        else
                        {
                            var forbidden_email = new EmailModel()
                            {
                                To = provider.Email,
                                Subject = "service forbidden mail",
                                Body = "service with service id " + Service_id + "is accepted by another service provider",

                            };
                            sendemail.emailSend(forbidden_email);
                        }
                    }

                    return Json(new { error = "success" });
                }
                else
                {
                    foreach (var row in current_provider_data)
                    {
                        var temp_total_hours = Convert.ToDouble(row.ServiceHours + row.ExtraHours);
                        var temp_sid_start_datetime = row.ServiceStartDate;
                        var temp_sid_end_datetime = row.ServiceStartDate.AddHours(temp_total_hours);

                        if ((DateTime.Compare(temp_sid_end_datetime, current_sid_start_datetime) > 0 && DateTime.Compare(temp_sid_end_datetime, current_sid_end_datetime) <= 0) || (DateTime.Compare(temp_sid_start_datetime, current_sid_start_datetime) >= 0 && DateTime.Compare(temp_sid_start_datetime, current_sid_end_datetime) < 0) || (DateTime.Compare(temp_sid_end_datetime, current_sid_end_datetime) > 0 && DateTime.Compare(temp_sid_start_datetime, current_sid_start_datetime) < 0))
                        {

                            return Json(new
                            {
                                error = "conflict",
                                conflict_service_id = row.ServiceId,
                                conflict_service_date = row.ServiceStartDate.Day + "/" + row.ServiceStartDate.Month + "/" + row.ServiceStartDate.Year,
                                conflict_service_starttime = row.ServiceStartDate.Hour + ":" + row.ServiceStartDate.Minute,
                                conflict_service_endtime = row.ServiceStartDate.AddHours(Convert.ToDouble(row.ServiceHours + row.ExtraHours)).TimeOfDay.Hours + ":" + row.ServiceStartDate.AddHours(Convert.ToDouble(row.ServiceHours + row.ExtraHours)).TimeOfDay.Minutes,
                                conflict_service_duration = row.ServiceHours + row.ExtraHours
                            });


                        }
                    }
                    service_data.ServiceProviderId = current_user_Id;
                    service_data.Status = 4;
                    context.SaveChanges();

                    //for send mail of accepted 
                    var provider_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();
                    var provider_at_pincode = context.Users.Where(x => x.ZipCode == provider_data.ZipCode && x.UserTypeId == 2).ToList();

                    foreach (var provider in provider_at_pincode)
                    {
                        if (provider.UserId == current_user_Id)
                        {
                            var acceptence_email = new EmailModel()
                            {
                                To = provider.Email,
                                Subject = "service acceptence mail",
                                Body = "new service has been assigned to you with Service Id " + Service_id,

                            };
                            sendemail.emailSend(acceptence_email);
                        }
                        else
                        {
                            var forbidden_email = new EmailModel()
                            {
                                To = provider.Email,
                                Subject = "service forbidden mail",
                                Body = "service with service id " + Service_id + "is accepted by another service provider",

                            };
                            sendemail.emailSend(forbidden_email);
                        }
                    }

                    return Json(new { error = "success" });
                }
            }
        }



        [HttpGet]
        public IActionResult UpcomingService()
        {
            var requests = context.ServiceRequests;
            var new_requests = new List<object>();
            var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


            foreach (var item1 in requests)
            {
                //for getting extra service 
                var extra_got = (
             from r in context.ServiceRequests
             join e in context.ServiceRequestExtras
             on r.ServiceRequestId equals e.ServiceRequestId
             where r.ServiceId == item1.ServiceId
             select e.ServiceExtraId);

                var x = "";
                foreach (var service in extra_got)
                {
                    if (service == 1)
                    {
                        x = x + "Inside cabinets, ";
                    }
                    if (service == 2)
                    {
                        x = x + "Inside fridge, ";
                    }
                    if (service == 3)
                    {
                        x = x + "Inside oven, ";
                    }
                    if (service == 4)
                    {
                        x = x + "Laundry wash & dry, ";
                    }
                    if (service == 5)
                    {
                        x = x + "Interior windows, ";
                    }
                }

                if (item1.ServiceProviderId == current_user_Id && item1.Status == 4)
                {
                    var for_complete_btn = "";
                    if (DateTime.Now > item1.ServiceStartDate.AddHours(Convert.ToDouble(item1.ServiceHours + item1.ExtraHours)))
                    {
                        for_complete_btn = "true";
                    }
                    else
                    {
                        for_complete_btn = "false";
                    }
                    new_requests.Add(new
                    {
                        //serviceprovider_id = item.ServiceProviderId,
                        myServiceId = item1.ServiceId,

                        myservice_start_date = item1.ServiceStartDate.Day + "/" + item1.ServiceStartDate.Month + "/" + item1.ServiceStartDate.Year,
                        myservice_start_time = item1.ServiceStartDate.Hour + ":" + item1.ServiceStartDate.Minute,
                        myservice_end_time = item1.ServiceStartDate.AddHours(Convert.ToDouble(item1.ServiceHours + item1.ExtraHours)).TimeOfDay.Hours + ":" + item1.ServiceStartDate.AddHours(Convert.ToDouble(item1.ServiceHours + item1.ExtraHours)).TimeOfDay.Minutes,
                        service_duration = item1.ServiceHours + item1.ExtraHours,

                        extras = x,

                        my_customer_address = (
                                        from r in context.ServiceRequests
                                        join a in context.ServiceRequestAddresses
                                        on r.ServiceRequestId equals a.ServiceRequestId
                                        where r.ServiceId == item1.ServiceId
                                        select new { AddressLine1 = a.AddressLine1, AddressLine2 = a.AddressLine2, PostalCode = a.PostalCode, City = a.City }).FirstOrDefault(),
                        my_customer_name = (
                                        from r in context.ServiceRequests
                                        join u in context.Users
                                        on r.UserId equals u.UserId
                                        where r.ServiceId == item1.ServiceId
                                        select new { FirstName = u.FirstName, LastName = u.LastName }).FirstOrDefault(),

                        mypayment = item1.TotalCost,

                        pet = item1.HasPets,

                        show_complete_button = for_complete_btn,
                    });
                }
            }
            ViewBag.current_new_req = new_requests;
            return View();
        }

        [HttpPost]
        public IActionResult UpcomingAction([FromBody] ProviderDashboardViewModel action)
        {
            var cancel = context.ServiceRequests.Where(x => x.ServiceId == action.ServiceId).FirstOrDefault();

            cancel.Status = 1;
            cancel.ServiceProviderId = null;
            cancel.Comments = action.comment;

            context.SaveChanges();

            return Json(true);
        }

        [HttpPost]
        public IActionResult UpcomingComplete([FromBody] ProviderDashboardViewModel action)
        {
            var complete = context.ServiceRequests.Where(x => x.ServiceId == action.ServiceId).FirstOrDefault();
            complete.Status = 2;

            context.SaveChanges();

            return Json(true);
        }



        [HttpGet]
        public IActionResult ServiceSchedule()
        {

            return View();
        }

        [HttpGet]
        public IActionResult ServiceHistory()
        {
            var current_user_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var services = context.ServiceRequests.Where(x => x.ServiceProviderId == current_user_id && x.Status == 2).ToList();
            var current_history = new List<object>();



            foreach (var data in services)
            {

                //for getting extra service 
                var extra_got = (
             from r in context.ServiceRequests
             join e in context.ServiceRequestExtras
             on r.ServiceRequestId equals e.ServiceRequestId
             where r.ServiceId == data.ServiceId
             select e.ServiceExtraId);

                var x = "";
                foreach (var service in extra_got)
                {
                    if (service == 1)
                    {
                        x = x + "Inside cabinets, ";
                    }
                    if (service == 2)
                    {
                        x = x + "Inside fridge, ";
                    }
                    if (service == 3)
                    {
                        x = x + "Inside oven, ";
                    }
                    if (service == 4)
                    {
                        x = x + "Laundry wash & dry, ";
                    }
                    if (service == 5)
                    {
                        x = x + "Interior windows, ";
                    }
                }


                current_history.Add(new
                {
                    myServiceId = data.ServiceId,

                    myservice_start_date = data.ServiceStartDate.Day + "/" + data.ServiceStartDate.Month + "/" + data.ServiceStartDate.Year,
                    myservice_start_time = data.ServiceStartDate.Hour + ":" + data.ServiceStartDate.Minute,
                    myservice_end_time = data.ServiceStartDate.AddHours(Convert.ToDouble(data.ServiceHours + data.ExtraHours)).TimeOfDay.Hours + ":" + data.ServiceStartDate.AddHours(Convert.ToDouble(data.ServiceHours + data.ExtraHours)).TimeOfDay.Minutes,
                    service_duration = data.ServiceHours + data.ExtraHours,

                    extras = x,

                    my_customer_address = (
                                        from r in context.ServiceRequests
                                        join a in context.ServiceRequestAddresses
                                        on r.ServiceRequestId equals a.ServiceRequestId
                                        where r.ServiceId == data.ServiceId
                                        select new { AddressLine1 = a.AddressLine1, AddressLine2 = a.AddressLine2, PostalCode = a.PostalCode, City = a.City }).FirstOrDefault(),
                    my_customer_name = (
                                        from r in context.ServiceRequests
                                        join u in context.Users
                                        on r.UserId equals u.UserId
                                        where r.ServiceId == data.ServiceId
                                        select new { FirstName = u.FirstName, LastName = u.LastName, email = u.Email, phn = u.Mobile }).FirstOrDefault(),

                    mypayment = data.TotalCost,

                    pet = data.HasPets
                });


            }
            ViewBag.current_history = current_history;
            return View();
        }

        [HttpGet]
        public IActionResult MyRatings()
        {
            var current_provider_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var service_data = context.ServiceRequests.Where(x => x.ServiceProviderId == current_provider_id).ToList();
            var rating_data = context.Ratings.Where(x => x.RatingTo == current_provider_id).ToList();

            var mydata = new List<object>();

            foreach (var data in rating_data)
            {

                mydata.Add(new
                {
                    rating = data.Ratings,

                    cmt = data.Comments,

                    date_time = (
                                 from r in context.Ratings
                                 join s in context.ServiceRequests
                                 on r.RatingFrom equals s.UserId
                                 where s.ServiceRequestId == data.ServiceRequestId
                                 select new { serviceid = s.ServiceId, myservice_start_date = s.ServiceStartDate.Day + "/" + s.ServiceStartDate.Month + "/" + s.ServiceStartDate.Year, myservice_start_time = s.ServiceStartDate.Hour + ":" + s.ServiceStartDate.Minute, myservice_end_time = s.ServiceStartDate.AddHours(Convert.ToDouble(s.ServiceHours + s.ExtraHours)).TimeOfDay.Hours + ":" + s.ServiceStartDate.AddHours(Convert.ToDouble(s.ServiceHours + s.ExtraHours)).TimeOfDay.Minutes, service_duration = s.ServiceHours + s.ExtraHours }).FirstOrDefault(),


                    my_customer_name = (
                                        from r in context.ServiceRequests
                                        join u in context.Users
                                        on r.UserId equals u.UserId
                                        where r.UserId == data.RatingFrom
                                        select new { FirstName = u.FirstName, LastName = u.LastName, email = u.Email, phn = u.Mobile }).FirstOrDefault(),


                });

                //return View();
            }
            ViewBag.mydata = mydata;
            return View();
        }

        [HttpGet]
        public IActionResult BlockCustomer()
        {

            var current_provider_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            List<int> service_data = context.ServiceRequests.Where(x => x.ServiceProviderId == current_provider_id && x.Status == 2).Select(x => x.UserId).Distinct().ToList();
            var blockdata = new List<object>();


            foreach (var data in service_data)
            {
                var isblock = context.FavoriteAndBlockeds.Where(x => x.UserId == current_provider_id && x.TargetUserId == data).FirstOrDefault();

                if (isblock != null)
                {

                    blockdata.Add(new
                    {
                        customer_data = (

                        from r in context.ServiceRequests
                        join u in context.Users
                        on r.UserId equals u.UserId
                        where r.UserId == data
                        select new { userid = u.UserId, FirstName = u.FirstName, LastName = u.LastName }).FirstOrDefault(),

                        is_block = isblock.IsBlocked,


                    });
                }
                else
                {
                    blockdata.Add(new
                    {
                        customer_data = (

                        from r in context.ServiceRequests
                        join u in context.Users
                        on r.UserId equals u.UserId
                        where r.UserId == data
                        select new { userid = u.UserId, FirstName = u.FirstName, LastName = u.LastName }).FirstOrDefault(),

                        is_block = false,


                    });
                }
            }

            ViewBag.blockdata = blockdata;
            return View();
        }

        [HttpPost]
        public IActionResult block([FromBody] ProviderDashboardViewModel action)
        {
            var current_provider_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var rate_data = context.FavoriteAndBlockeds.Where(x => x.UserId == current_provider_id && x.TargetUserId == action.UserId).FirstOrDefault();

            if (rate_data == null)
            {
                var rate = new FavoriteAndBlocked();

                rate.UserId = current_provider_id;
                rate.TargetUserId = action.UserId;
                rate.IsBlocked = true;

                context.Add(rate);
            }
            else
            {
                rate_data.IsBlocked = true;
            }
            context.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public IActionResult Unblock([FromBody] ProviderDashboardViewModel action)
        {
            var current_provider_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var block_data = context.FavoriteAndBlockeds.Where(x => x.UserId == current_provider_id && x.TargetUserId == action.UserId).FirstOrDefault();

            block_data.IsBlocked = false;
            context.SaveChanges();
            return Json(true);
        }

        [HttpGet]
        public IActionResult MySettings()
        {
            var current_provider_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user_data = context.Users.Where(x => x.UserId == current_provider_id).FirstOrDefault();
            var tab1_data = new List<object>();

            var add = context.UserAddresses.Where(x => x.UserId == current_provider_id).FirstOrDefault();
            if (user_data.DateOfBirth != null && add != null)
            {
                tab1_data.Add(new
                {
                    f_name = user_data.FirstName,
                    l_name = user_data.LastName,
                    email = user_data.Email,
                    mobile = user_data.Mobile,
                    bdate = user_data.DateOfBirth.Value.Day,
                    bmonth = user_data.DateOfBirth.Value.Month,
                    byear = user_data.DateOfBirth.Value.Year,
                    street = add.AddressLine1,
                    house = add.AddressLine2,
                    postal = add.PostalCode,
                    city = add.City,
                    nationality = user_data.NationalityId,
                    gender = user_data.Gender,
                    avtar = user_data.UserProfilePicture,
                });
            }
            else
            {
                tab1_data.Add(new
                {
                    f_name = user_data.FirstName,
                    l_name = user_data.LastName,
                    email = user_data.Email,
                    mobile = user_data.Mobile,
                    bdate = "",
                    bmonth = "",
                    byear = "",
                    street = "",
                    house = "",
                    postal = "",
                    city = "",
                    nationality = user_data.NationalityId,
                    gender = user_data.Gender,
                    avtar = user_data.UserProfilePicture,
                });
            }

            ViewBag.tab1_data = tab1_data;
            return View();
        }

        [HttpPost]
        public IActionResult settingtab1(ProviderDashboardViewModel provider)
        {
            var current_provider_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = context.Users.Where(x => x.UserId == current_provider_id).FirstOrDefault();

            user.FirstName = provider.first_name;
            user.LastName = provider.last_name;
            user.Mobile = provider.mobile;
            user.DateOfBirth = new DateTime(Int32.Parse(provider.year), Int32.Parse(provider.month), Int32.Parse(provider.date));
            user.NationalityId = provider.nationality;
            user.Gender = provider.gender;
            if (provider.avtar != null)
            {
                user.UserProfilePicture = provider.avtar;
            }

            var provider_add = context.UserAddresses.Where(x => x.UserId == current_provider_id).FirstOrDefault();
            var provider_add_usertable = context.Users.Where(x => x.UserId == current_provider_id).FirstOrDefault();

            if (provider_add != null)
            {
                provider_add.AddressLine1 = provider.street;
                provider_add.AddressLine2 = provider.house;
                provider_add.PostalCode = provider.postcode;
                provider_add_usertable.ZipCode = provider.postcode;
                provider_add.City = provider.city;
                provider_add.Mobile = provider.mobile;
            }
            else
            {
                var provider_new_add = new UserAddress();

                provider_new_add.UserId = current_provider_id;
                provider_new_add.AddressLine1 = provider.street;
                provider_new_add.AddressLine2 = provider.house;
                provider_new_add.PostalCode = provider.postcode;
                provider_add_usertable.ZipCode = provider.postcode;
                provider_new_add.City = provider.city;
                provider_new_add.Mobile = provider.mobile;

                context.Add(provider_new_add);
            }

            context.SaveChanges();
            return RedirectToAction("MySettings", "Provider");
        }

        [HttpPost]
        public IActionResult settingtab2([FromBody] ProviderDashboardViewModel provider)
        {
            var current_provider_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var pass_data = context.Users.Where(x => x.UserId == current_provider_id).FirstOrDefault();
            if (provider.oldpassword != "" && provider.newpassword != "" && provider.confirmpassword != "")
            {

                if (pass_data != null && BCrypto.Verify(provider.oldpassword, pass_data.Password))
                {
                    pass_data.Password = BCrypto.HashPassword(provider.newpassword);

                    context.SaveChanges();
                    return Json("true");
                }
                else
                {
                    return Json("false");
                }
            }
            return Json("datanone");
        }

        [HttpPost]

        public IActionResult change_pin_new([FromBody] ProviderDashboardViewModel setting)
        {

            var zipcode = context.Zipcodes.Where(x => x.ZipcodeValue == setting.postcode).FirstOrDefault();

            if (zipcode != null)
            {
                var mycity = context.Cities.Where(x => x.Id == zipcode.CityId).FirstOrDefault();
                return Json(new { city = mycity.CityName });
            }
            else
            {
                return Json(new { city = "" });
            }
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var current_user_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var data = new List<object>();

            var services = context.ServiceRequests.Where(x => x.ServiceProviderId == current_user_id && (x.Status == 2 || x.Status == 4)).ToList();
            foreach (var service in services)
            {
                data.Add(new
                {
                    id = service.ServiceId,
                    title = service.ServiceStartDate.ToString("HH:mm") + " - " + service.ServiceStartDate.AddHours(service.ServiceHours).ToString("HH:mm"),
                    start = service.ServiceStartDate.ToString("yyyy-MM-ddTHH:mm"),
                    color = service.Status == 2 ? "#86858b" : "#1d7a8c",
                    postalcode = service.ZipCode,

                });
            }
            return Json(data);
        }

        public class EventViewModel
        {
#nullable disable
            public int id { get; set; }
            public string start { get; set; }
            public string title { get; set; }
            public string backgroundColor { get; set; }
            public bool allDay { get; set; }
        }

    }
}