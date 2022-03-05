using Helperland.Models.ViewModels;
using Helperland.Models;
using Helperland.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Globalization;
using BCrypto = BCrypt.Net.BCrypt;

namespace Helperland.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly HelperlandContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CustomerController(ILogger<CustomerController> logger, HelperlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult CustomerDashboard()
        {
            var New = 1;
            var completed = 2;
            var cancelled = 3;
            var refunded = 4;
            var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var current_user_data = new List<object>();
            var my_adata = context.ServiceRequests.Where(x => x.UserId == current_user_Id && x.Status == 1);


            foreach (var eachdata in my_adata)
            {
                current_user_data.Add(new
                {

                    myServiceId = eachdata.ServiceId,


                    service_start_date_time = eachdata.ServiceStartDate,
                    myservice_start_date = eachdata.ServiceStartDate.Day + "/" + eachdata.ServiceStartDate.Month + "/" + eachdata.ServiceStartDate.Year,
                    myservice_start_time = eachdata.ServiceStartDate.Hour + ":" + eachdata.ServiceStartDate.Minute,
                    service_duration = eachdata.ServiceHours + eachdata.ExtraHours,


                    //myservice_end_time = Convert.ToDecimal(eachdata.ServiceStartDate.Hour + "." + eachdata.ServiceStartDate.Minute) + Convert.ToDecimal(eachdata.ServiceHours + "." + eachdata.ExtraHours),
                    myservice_end_time = eachdata.ServiceStartDate.AddHours(Convert.ToDouble(eachdata.ServiceHours + eachdata.ExtraHours)).TimeOfDay.Hours + ":" + eachdata.ServiceStartDate.AddHours(Convert.ToDouble(eachdata.ServiceHours + eachdata.ExtraHours)).TimeOfDay.Minutes,

                    my_provider_rating = (
                                    from r in context.ServiceRequests
                                    join e in context.Ratings
                                    on r.ServiceRequestId equals e.ServiceRequestId
                                    where r.ServiceId == eachdata.ServiceId
                                    select e.Ratings).FirstOrDefault(),

                    serviceprovider_id = eachdata.ServiceProviderId,
                    mysericeprovider_name = "Name of provider",

                    mypayment = eachdata.TotalCost

                });
            }

            Console.WriteLine(current_user_data);
            ViewBag.current_user_data = current_user_data;
            return View();
        }

        [HttpPost]
        public IActionResult CustomerDashboard1([FromBody] CustomerDashboardViewModel dashboard)
        {
            var data_for_first_modal = context.ServiceRequests.Where(x => x.ServiceId == dashboard.ServiceId).FirstOrDefault();

            //var data_send = new List<object>();

            var address_got = (
             from r in context.ServiceRequests
             join a in context.ServiceRequestAddresses
             on r.ServiceRequestId equals a.ServiceRequestId
             where r.ServiceId == dashboard.ServiceId
             select new { line1 = a.AddressLine1, line2 = a.AddressLine2, city = a.City, postal = a.PostalCode, mobile = a.Mobile, email = a.Email }).FirstOrDefault();

            var extra_got = (
             from r in context.ServiceRequests
             join e in context.ServiceRequestExtras
             on r.ServiceRequestId equals e.ServiceRequestId
             where r.ServiceId == dashboard.ServiceId
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

            var data_send = new
            {
                service_date = data_for_first_modal.ServiceStartDate.Day + "/" + data_for_first_modal.ServiceStartDate.Month + "/" + data_for_first_modal.ServiceStartDate.Year,
                service_start_time = data_for_first_modal.ServiceStartDate.Hour + ":" + data_for_first_modal.ServiceStartDate.Minute,
                service_end_date = data_for_first_modal.ServiceStartDate.AddHours(Convert.ToDouble(data_for_first_modal.ServiceHours + data_for_first_modal.ExtraHours)).TimeOfDay.Hours + ":" + data_for_first_modal.ServiceStartDate.AddHours(Convert.ToDouble(data_for_first_modal.ServiceHours + data_for_first_modal.ExtraHours)).TimeOfDay.Minutes,
                duration = data_for_first_modal.ServiceHours + data_for_first_modal.ExtraHours,
                service_id = dashboard.ServiceId,

                extras = x,

                payment = data_for_first_modal.TotalCost,

                //address get form another table
                serviceaddres = address_got.line1 + " " + address_got.line2 + ", " + address_got.postal + " " + address_got.city,
                servicephone = address_got.mobile,
                serviceemail = address_got.email,

                servicepets = data_for_first_modal.HasPets
            };

            return Json(data_send);
        }

        [HttpPost]
        public IActionResult CustomerDashboard2(CustomerDashboardViewModel dashboard)
        {
            var request = context.ServiceRequests.Where(x => x.ServiceId == dashboard.ServiceId).FirstOrDefault();


            var s = dashboard.date.Split("/");
            var d = dashboard.start_time.Split(":");
            DateTime datefinal = new DateTime(Int32.Parse(s[2]), Int32.Parse(s[1]), Int32.Parse(s[0]), Int32.Parse(d[0]), Int32.Parse(d[1]), 0);


            var total_hours = Convert.ToDouble(request.ServiceHours + request.ExtraHours);
            var datefinal_end = datefinal.AddHours(total_hours).AddMinutes(30);

            var request_forloop = context.ServiceRequests.Where(x => x.ServiceProviderId == request.ServiceProviderId).ToList();

            foreach (var reschedule in request_forloop)
            {
                var request_start = reschedule.ServiceStartDate;

                var request_end = request_start.AddHours(total_hours).AddMinutes(30);

                if ((DateTime.Compare(request_start, datefinal) <= 0 && DateTime.Compare(request_end, datefinal) > 0) || (DateTime.Compare(request_start, datefinal_end) < 0 && DateTime.Compare(request_end, datefinal_end) >= 0))
                {
                    //return Json(false);
                    return RedirectToAction("CustomerDashboard", "Customer");
                }
            }

            request.ServiceStartDate = datefinal;
            context.SaveChanges();
            return RedirectToAction("CustomerDashboard", "Customer");
            //return Json(true);



        }

        [HttpPost]
        public IActionResult CustomerDashboard3(CustomerDashboardViewModel dashboard)
        {
            var cancel = context.ServiceRequests.Where(x => x.ServiceId == dashboard.ServiceId).FirstOrDefault();

            cancel.Status = 3;
            cancel.Comments = dashboard.why_cancel;


            context.SaveChanges();

            return RedirectToAction("CustomerDashboard", "Customer");
        }



        public IActionResult CustomerServiceHistory()
        {
            var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var current_user_history = new List<object>();
            var my_adata = context.ServiceRequests.Where(x => x.UserId == current_user_Id && (x.Status == 2 || x.Status == 3));



            foreach (var eachdata in my_adata)
            {
                var sericeprovider_name = "";
                var user_my = context.Users.Where(x => x.UserId == eachdata.ServiceProviderId).FirstOrDefault();
                if (user_my != null)
                {

                    sericeprovider_name = user_my.FirstName;
                }
                else
                {
                    sericeprovider_name = "Name of provider";
                }
                current_user_history.Add(new
                {

                    myserviceid = eachdata.ServiceId,

                    service_start_date_time = eachdata.ServiceStartDate,
                    myservice_start_date = eachdata.ServiceStartDate.Day + "/" + eachdata.ServiceStartDate.Month + "/" + eachdata.ServiceStartDate.Year,
                    myservice_start_time = eachdata.ServiceStartDate.Hour + ":" + eachdata.ServiceStartDate.Minute,
                    service_duration = eachdata.ServiceHours + eachdata.ExtraHours,

                    myservice_end_time = eachdata.ServiceStartDate.AddHours(Convert.ToDouble(eachdata.ServiceHours + eachdata.ExtraHours)).TimeOfDay.Hours + ":" + eachdata.ServiceStartDate.AddHours(Convert.ToDouble(eachdata.ServiceHours + eachdata.ExtraHours)).TimeOfDay.Minutes,

                    // rating should particular service provider
                    my_provider_rating = (
                                    from r in context.ServiceRequests
                                    join e in context.Ratings
                                    on r.ServiceRequestId equals e.ServiceRequestId
                                    where r.ServiceId == eachdata.ServiceId
                                    select e.Ratings).FirstOrDefault(),

                    serviceprovider_id = eachdata.ServiceProviderId,

                    mysericeprovider_name = sericeprovider_name,
                    mypayment = eachdata.TotalCost,

                    status = eachdata.Status,

                });
            }
            Console.WriteLine(current_user_history);
            ViewBag.current_user_history = current_user_history;
            return View();
        }


        [HttpPost]
        public IActionResult SubmitRate(CustomerDashboardViewModel dashboard)
        {
            var service_req = context.ServiceRequests.Where(x => x.ServiceId == dashboard.ServiceId).FirstOrDefault();
            var current_service_rate = context.Ratings.Where(x => x.ServiceRequestId == service_req.ServiceRequestId).FirstOrDefault();
            var Add_new_service_rating = new Rating();
            if (current_service_rate != null)
            {

                current_service_rate.OnTimeArrival = dashboard.OnTimeArrival;
                current_service_rate.Friendly = dashboard.Friendly;
                current_service_rate.QualityOfService = dashboard.QualityOfService;
                current_service_rate.Comments = dashboard.Comments;
                current_service_rate.Ratings = Convert.ToDecimal((current_service_rate.Ratings + (dashboard.OnTimeArrival + dashboard.Friendly + dashboard.QualityOfService) / 3) / 2);
                current_service_rate.RatingFrom = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                //aahiya dynamic value nakvi atyare service id 1466 mate j 6e 
                current_service_rate.RatingTo = 6;
                current_service_rate.RatingDate = DateTime.Now;

            }
            else
            {
                Add_new_service_rating.ServiceRequestId = service_req.ServiceRequestId;
                Add_new_service_rating.OnTimeArrival = dashboard.OnTimeArrival;
                Add_new_service_rating.Friendly = dashboard.Friendly;
                Add_new_service_rating.QualityOfService = dashboard.QualityOfService;
                Add_new_service_rating.Comments = dashboard.Comments;
                Add_new_service_rating.Ratings = Convert.ToDecimal((dashboard.OnTimeArrival + dashboard.Friendly + dashboard.QualityOfService) / 3);
                Add_new_service_rating.RatingFrom = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));


                //aahiya dynamic value nakvi atyare service id 1466 mate j 6e etle aahiya 6 ni badle je user hoi eni userid nakhvi (user table mi col1)
                //5 -> helper, 6-> sp2 
                Add_new_service_rating.RatingTo = 5;
                Add_new_service_rating.RatingDate = DateTime.Now;
                context.Ratings.Add(Add_new_service_rating);
            }

            context.SaveChanges();
            return RedirectToAction("CustomerServiceHistory", "Customer");
        }

        public IActionResult CustomerFavouritePros()
        {
            return View();
        }

        public IActionResult CustomerMySettings()
        {
            var currentuserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var address_data = new List<object>();
            var current_cust_address_data = context.UserAddresses.Where(x => x.UserId == currentuserId);
            var current_cust_data = context.Users.Where(x => x.UserId == currentuserId).FirstOrDefault();

            foreach (var address in current_cust_address_data)
            {
                address_data.Add(new
                {
                    address = "" + address.AddressLine1 + " " + address.AddressLine2 + ", " + address.PostalCode + " " + address.City,
                    phone = address.Mobile,
                    address_id = address.AddressId,
                    street = address.AddressLine1,
                    house = address.AddressLine2,
                    pin = address.PostalCode,
                    city = address.City,

                });
            }


            var data_send = new List<object>();

            if (current_cust_data.DateOfBirth != null)
            {

                data_send.Add(new
                {
                    f_name = current_cust_data.FirstName,
                    l_name = current_cust_data.LastName,
                    mail = current_cust_data.Email,
                    phone = current_cust_data.Mobile,
                    bdate = current_cust_data.DateOfBirth.Value.Day,
                    bmonth = current_cust_data.DateOfBirth.Value.Month,
                    byear = current_cust_data.DateOfBirth.Value.Year,
                });
            }
            else
            {
                data_send.Add(new
                {
                    f_name = current_cust_data.FirstName,
                    l_name = current_cust_data.LastName,
                    mail = current_cust_data.Email,
                    phone = current_cust_data.Mobile,
                    bdate = "",
                    bmonth = "",
                    byear = ""
                });
            }

            ViewBag.address_data = address_data;
            ViewBag.data_send = data_send;
            return View();
        }

        [HttpPost]
        public IActionResult CustomerMySettings(CustomerSettingsViewModel setting)
        {
            return View();
        }


        [HttpPost]
        public IActionResult settingtab1([FromBody] settingtab1ViewModel setting)
        {

            var for_valid = ModelState.IsValid;

            var currentuserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = context.Users.Where(x => x.UserId == currentuserId).FirstOrDefault();



            user.FirstName = setting.first_name;
            user.LastName = setting.last_name;
            user.Email = setting.email;
            user.Mobile = setting.mobile;
            user.DateOfBirth = new DateTime(Int32.Parse(setting.year), Int32.Parse(setting.month), Int32.Parse(setting.date));
            user.LanguageId = Convert.ToInt32(setting.language);

            context.SaveChanges();
            return Json(new { fname = user.FirstName, lname = user.LastName, mob = user.Mobile, date = setting.date, month = setting.month, year = setting.year, lang = setting.language });
        }

        [HttpPost]
        public IActionResult settingtab2_edit([FromBody] settingtab2ViewModel setting)
        {

            var current_cust_address_data = context.UserAddresses.Where(x => x.AddressId == setting.id).FirstOrDefault();

            current_cust_address_data.AddressLine1 = setting.street;
            current_cust_address_data.City = setting.city;
            current_cust_address_data.AddressLine2 = setting.house;
            current_cust_address_data.PostalCode = setting.postcode;
            current_cust_address_data.Mobile = setting.mobile;


            context.SaveChanges();
            return Json(new { id = setting.id, address = "" + current_cust_address_data.AddressLine1 + " " + current_cust_address_data.AddressLine2 + ", " + current_cust_address_data.PostalCode + " " + current_cust_address_data.City, phone = current_cust_address_data.Mobile });
        }

        [HttpPost]
        public IActionResult settingtab2_new([FromBody] settingtab2ViewModel setting)
        {

            var address = new UserAddress();
            address.UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            address.AddressLine1 = setting.street;
            address.AddressLine2 = setting.house;
            address.City = setting.city;
            address.PostalCode = setting.postcode;
            address.Mobile = setting.mobile;

            context.UserAddresses.Add(address);
            context.SaveChanges();
            return Json(new { id = address.UserId, address = "" + address.AddressLine1 + " " + address.AddressLine2 + ", " + address.PostalCode + " " + address.City, phone = address.Mobile });
        }

        [HttpPost]
        public IActionResult settingtab3([FromBody] settingtab3ViewModel setting)

        {
            var user_my_id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var mypass_check = context.Users.Where(x => x.UserId == user_my_id).FirstOrDefault();

            if (mypass_check != null && BCrypto.Verify(setting.oldpassword, mypass_check.Password))
            {
                mypass_check.Password = BCrypto.HashPassword(setting.newpassword); ;

                context.SaveChanges();
                return Json("true");
            }
            else
            {
                return Json("false");
            }
        }
    }
}
