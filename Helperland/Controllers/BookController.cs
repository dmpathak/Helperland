using Helperland.Models.ViewModels;
using Helperland.Models;
using Helperland.Models.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Globalization;

namespace Helperland.Controllers
{
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly HelperlandContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BookController(ILogger<BookController> logger, HelperlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public IActionResult BookService()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckAvailability([FromBody] CheckavailViewModel avail)
        {
            var database_zipcode = context.Users.Where(x => x.ZipCode == avail.MyCheckavail && x.UserTypeId == 2).FirstOrDefault();
            var database_avail = new Zipcode();
            if (avail.MyCheckavail != null && database_zipcode != null)
            {
                var currentuserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var address_data = new List<object>();
                var my_address = context.UserAddresses.Where(x => x.UserId == currentuserId && x.PostalCode == avail.MyCheckavail);

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
                    addresses = address_data
                };

                return Json(data);
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
            s1.UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            s1.ServiceId = new Random().Next(1000, 9999);
            var s = avail.date.Split("/");
            var d = avail.time.Split(":");
            var datefinal = new DateTime(Int32.Parse(s[2]), Int32.Parse(s[1]), Int32.Parse(s[0]), Int32.Parse(d[0]), Int32.Parse(d[1]), 0);
            s1.ServiceStartDate = datefinal;
            s1.ServiceHours = Convert.ToDouble(avail.basic_time);
            s1.ExtraHours = Convert.ToDouble(avail.total_time) - Convert.ToDouble(avail.basic_time);
            s1.Comments = avail.comments;
            s1.HasPets = avail.have_pets;
            s1.ZipCode = avail.pincode;
            context.ServiceRequests.Add(s1);
            context.SaveChanges();

            var s2 = new ServiceRequestAddress();
            var add = new UserAddress();
            var useraddress_database = context.UserAddresses.Where(x => x.AddressId == avail.address_id).FirstOrDefault();
            s2.ServiceRequestId = s1.ServiceRequestId;
            s2.AddressLine1 = useraddress_database.AddressLine1;
            s2.AddressLine2 = useraddress_database.AddressLine2;
            s2.City = useraddress_database.City;
            s2.PostalCode = useraddress_database.PostalCode;
            s2.Mobile = useraddress_database.Mobile;
            context.ServiceRequestAddresses.Add(s2);
            context.SaveChanges();

            foreach (var service in avail.extra_services)
            {
                var s3 = new ServiceRequestExtra();
                s3.ServiceRequestId = s1.ServiceRequestId;
                if (service == true)
                {
                    s3.ServiceExtraId = 1;
                }
                else
                {
                    s3.ServiceExtraId = 0;
                }
                context.ServiceRequestExtras.Add(s3);
            }
            context.SaveChanges();

            return Json(new { ServiceId = s1.ServiceId });
        }
    }
}
