using Helperland.Models.ViewModels;
using Helperland.Models;
using Helperland.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Helperland.ForSendemail;

namespace Helperland.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly HelperlandContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Sendemail sendemail = new Sendemail();

        public AdminController(ILogger<AdminController> logger, HelperlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult Service_requests()
        {
            var request = context.ServiceRequests;
            var data = new List<object>();
            ViewBag.SP_Names = (
                                from s in context.ServiceRequests
                                join u in context.Users
                                on s.ServiceProviderId equals u.UserId
                                select u.FirstName + " " + u.LastName).Distinct().ToList();
            ViewBag.User_Names = (
                            from u in context.Users
                            join s in context.ServiceRequests
                            on u.UserId equals s.UserId
                            select u.FirstName + " " + u.LastName).Distinct().ToList();
            foreach (var req in request)
            {
                var fname = "";
                var lname = "";
                var avtar = "";
                var provider_user = context.Users.Where(x => x.UserId == req.ServiceProviderId).FirstOrDefault();
                if (provider_user != null)
                {

                    fname = provider_user.FirstName;
                    lname = provider_user.LastName;
                    avtar = provider_user.UserProfilePicture;
                }
                else
                {
                    fname = null;
                    lname = null;
                    avtar = null;
                }


                var extra_got = (
                        from r in context.ServiceRequests
                        join e in context.ServiceRequestExtras
                        on r.ServiceRequestId equals e.ServiceRequestId
                        where r.ServiceId == req.ServiceId
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





                if (req.ServiceProviderId != null)
                {
                    var ratings = context.Ratings.Where(x => x.RatingTo == req.ServiceProviderId).Average(x => x.Ratings);


                    data.Add(new
                    {

                        serviceid = req.ServiceId,

                        servicedate = req.ServiceStartDate.ToString("dd/MM/yyyy").Replace("-", "/"),
                        service_start_time = req.ServiceStartDate.ToString("HH:mm"),
                        service_end_time = req.ServiceStartDate.AddHours(req.ServiceHours).ToString("HH:mm"),
                        service_duration = req.ServiceHours + req.ExtraHours,

                        customer_detail = (
                                        from r in context.ServiceRequests
                                        join u in context.Users
                                        on r.UserId equals u.UserId
                                        where r.UserId == req.UserId
                                        select new { f_name = u.FirstName, l_name = u.LastName }).FirstOrDefault(),

                        service_address = (
                                        from r in context.ServiceRequests
                                        join a in context.ServiceRequestAddresses
                                        on r.ServiceRequestId equals a.ServiceRequestId
                                        where r.ServiceRequestId == req.ServiceRequestId
                                        select new { line1 = a.AddressLine1, line2 = a.AddressLine2, city = a.City, postal = a.PostalCode, mobile = a.Mobile, email = a.Email }).FirstOrDefault(),

                        my_provider_rating = ratings,
                        provider_fname = fname,
                        provider_lname = lname,
                        profile_picture = avtar,

                        status = req.Status,

                        extras = x,
                        payment = req.TotalCost,
                        servicepets = req.HasPets,

                        refunded_amount = (req.RefundedAmount != null) ? req.RefundedAmount : 0,
                        inbalance_amount = Convert.ToDecimal(req.TotalCost - ((req.RefundedAmount != null) ? req.RefundedAmount : 0)),


                    });
                }
                else
                {
                    data.Add(new
                    {

                        serviceid = req.ServiceId,

                        servicedate = req.ServiceStartDate.ToString("dd/MM/yyyy").Replace("-", "/"),
                        service_start_time = req.ServiceStartDate.ToString("HH:mm"),
                        service_end_time = req.ServiceStartDate.AddHours(req.ServiceHours).ToString("HH:mm"),
                        service_duration = req.ServiceHours + req.ExtraHours,

                        customer_detail = (
                                        from r in context.ServiceRequests
                                        join u in context.Users
                                        on r.UserId equals u.UserId
                                        where r.UserId == req.UserId
                                        select new { f_name = u.FirstName, l_name = u.LastName }).FirstOrDefault(),

                        service_address = (
                                        from r in context.ServiceRequests
                                        join a in context.ServiceRequestAddresses
                                        on r.ServiceRequestId equals a.ServiceRequestId
                                        where r.ServiceRequestId == req.ServiceRequestId
                                        select new { line1 = a.AddressLine1, line2 = a.AddressLine2, city = a.City, postal = a.PostalCode, mobile = a.Mobile, email = a.Email }).FirstOrDefault(),

                        my_provider_rating = 0,
                        provider_fname = fname,
                        provider_lname = lname,
                        profile_picture = avtar,

                        status = req.Status,

                        extras = x,
                        payment = req.TotalCost,
                        servicepets = req.HasPets,

                        refunded_amount = Convert.ToDecimal((req.RefundedAmount != null) ? req.RefundedAmount : 0),
                        inbalance_amount = Convert.ToDecimal(req.TotalCost - Convert.ToDecimal((req.RefundedAmount != null) ? req.RefundedAmount : 0)),
                    });
                }
            }
            ViewBag.data = data;
            return View();
        }

        [HttpPost]
        public IActionResult edit_data([FromBody] AdminViewModel admin)
        {


            var request = context.ServiceRequests.Where(x => x.ServiceId == admin.id_for_service).FirstOrDefault();


            var s = admin.date.Split("/");
            var d = admin.time.Split(":");
            DateTime datefinal = new DateTime(Int32.Parse(s[2]), Int32.Parse(s[1]), Int32.Parse(s[0]), Int32.Parse(d[0]), Int32.Parse(d[1]), 0);


            var total_hours = Convert.ToDouble(request.ServiceHours + request.ExtraHours);
            var datefinal_end = datefinal.AddHours(total_hours).AddMinutes(60);

            var request_forloop = context.ServiceRequests.Where(x => x.ServiceProviderId == request.ServiceProviderId).ToList();

            if (request.ServiceProviderId != null)
            {

                foreach (var reschedule in request_forloop)
                {
                    var request_start = reschedule.ServiceStartDate;

                    var request_end = request_start.AddHours(total_hours).AddMinutes(60);

                    if ((DateTime.Compare(request_start, datefinal) <= 0 && DateTime.Compare(request_end, datefinal) > 0) || (DateTime.Compare(request_start, datefinal_end) < 0 && DateTime.Compare(request_end, datefinal_end) >= 0) || (DateTime.Compare(request_end, datefinal_end) > 0 && DateTime.Compare(request_start, datefinal) < 0))
                    {
                        return Json(false);
                    }
                }

                //sendemail mail to provider 
                var pro_data = context.Users.Where(x => x.UserId == request.ServiceProviderId).FirstOrDefault();
                var admin_reschedule_pro_email = new EmailModel()
                {
                    To = pro_data.Email,
                    Subject = "service acceptence mail",
                    Body = "Service Request" + admin.id_for_service + "has been rescheduled by Admin. New date and time are" + admin.date + " , " + admin.time,

                };
                sendemail.emailSend(admin_reschedule_pro_email);

            }

            request.ServiceStartDate = datefinal;
            request.Comments = admin.comment;

            var request_address = context.ServiceRequestAddresses.Where(x => x.ServiceRequestId == request.ServiceRequestId).FirstOrDefault();

            request_address.AddressLine1 = admin.street;
            request_address.AddressLine2 = admin.house;
            request_address.PostalCode = admin.postal;
            request_address.City = admin.city;
            context.SaveChanges();

            //sendemail mail to customer
            var cust_data = context.Users.Where(x => x.UserId == request.UserId).FirstOrDefault();
            var admin_reschedule_cust_email = new EmailModel()
            {
                To = cust_data.Email,
                Subject = "service acceptence mail",
                Body = "Service Request" + admin.id_for_service + "has been rescheduled by Admin. New date and time are" + admin.date + " , " + admin.time,

            };
            sendemail.emailSend(admin_reschedule_cust_email);       
            


            return Json(true);

        }


        [HttpGet]
        public IActionResult User_management()
        {
            var mydata = context.Users;

            var send = new List<object>();

            ViewBag.user_Names = context.Users.Select(x => x.FirstName + " " + x.LastName).ToList();


            foreach (var data in mydata)
            {
                send.Add(new
                {
                    userid = data.UserId,
                    username = data.FirstName + " " + data.LastName,
                    date = data.CreatedDate.ToString("dd/MM/yyyy").Replace("-", "/"),
                    usertype = data.UserTypeId,
                    phone = data.Mobile,
                    postal = data.ZipCode,
                    userstatus = data.IsActive,

                });
            }
            ViewBag.Send = send;
            return View();
        }

        [HttpPost]
        public IActionResult activate([FromBody] AdminViewModel admin)
        {
            var data = context.Users.Where(x => x.UserId == admin.id_for_user).FirstOrDefault();

            data.IsActive = true;

            context.SaveChanges();
            return Json(true);
        }

        [HttpPost]
        public IActionResult deactive([FromBody] AdminViewModel admin)
        {
            var data = context.Users.Where(x => x.UserId == admin.id_for_user).FirstOrDefault();

            data.IsActive = false;
            context.SaveChanges();
            return Json(false);
        }

        [HttpPost]
        public IActionResult refund([FromBody] AdminViewModel admin)
        {
            var data = context.ServiceRequests.Where(x => x.ServiceId == admin.id_for_service).FirstOrDefault();

            data.RefundedAmount = admin.refund_amount;

            context.SaveChanges();
            return Json(true);
        }


        [HttpPost]
        public IActionResult change_postal([FromBody] AdminViewModel setting)
        {

            var zipcode = context.Zipcodes.Where(x => x.ZipcodeValue == setting.postal).FirstOrDefault();

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


    }
}
