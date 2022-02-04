using Helperland.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Helperland.Models.ViewModels;
using Helperland.Models.Data;
using Helperland.ForSendemail;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace Helperland.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HelperlandContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly Sendemail sendemail = new Sendemail();


        public HomeController(ILogger<HomeController> logger, HelperlandContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginViewModel)
        {
            var user = new User();
            var usercredentials = context.Users.Where(x => x.Email == loginViewModel.Email && x.Password == loginViewModel.Password).FirstOrDefault();
            if (usercredentials != null)
            {

                var claims = new List<Claim>();
                claims.Add(new Claim("username", loginViewModel.Email));
                claims.Add(new Claim(ClaimTypes.NameIdentifier, loginViewModel.Email));
                claims.Add(new Claim(ClaimTypes.Name, usercredentials.FirstName + usercredentials.LastName));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);

                if (usercredentials.UserTypeId == 1)
                {
                    return RedirectToAction("Prices", "Home");

                }
                else if (usercredentials.UserTypeId == 2)
                {

                    return RedirectToAction("About", "Home");
                }
            }
            TempData["isvalid"] = true;
            return RedirectToAction("Index", "Home", new { login = "true" });
        }
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");

        }
        [Authorize]
        public IActionResult Prices()
        {
            return View();
        }
        [Authorize]
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Faq()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Contact(ContactusViewModel contact)
        {
            var contactu = new ContactU();

            if (ModelState.IsValid)
            {
                contactu.Name = contact.FirstName + contact.LastName;
                contactu.PhoneNumber = contact.PhoneNumber;
                contactu.Email = contact.Email;
                contactu.Subject = contact.Subject;
                contactu.Message = contact.Message;
                contactu.CreatedOn = DateTime.Now;

                string serverfolder = null;
                if (contact.FileUpload != null)
                {
                    string folder = "files/upload/";
                    folder += Guid.NewGuid().ToString() + "_" + contact.FileUpload.FileName;
                    contactu.FileName = folder;
                    serverfolder = Path.Combine(webHostEnvironment.WebRootPath, folder);
                    using (var filestream = new FileStream(serverfolder, FileMode.Create))
                    {
                        contact.FileUpload.CopyTo(filestream);
                    }

                }

                var email = new EmailModel()
                {
                    To = "abc@gmail.com",
                    Subject = "This mail is generated for " + contact.Subject,
                    Body = "FirstName: " + contact.FirstName + ", LastName: " + contact.LastName + ", Email: " + contact.Email + ", PhoneNumber: " + contact.PhoneNumber + ", Message: " + contact.Message,
                    Attachment = serverfolder
                };

                context.ContactUs.Add(contactu);
                context.SaveChanges();
                sendemail.emailSend(email);
            }
            return View();
        }
        [HttpPost]
        public IActionResult Forgotpassword(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                var usercredentials = context.Users.Where(x => x.Email == loginViewModel.ForgetPasswordEmail).FirstOrDefault();

                var email = new EmailModel()
                {

                    To = loginViewModel.ForgetPasswordEmail,
                    Subject = "This mail is generated for Forgot Password",
                    Body = "<a href = 'https://localhost:7053/Home/Resetpassword/"+ usercredentials.UserId + "'> Click Here to ResetPassword </a>"


                };
                sendemail.emailSend(email);

            }
                return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Resetpassword(string id)
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Resetpassword(ResetPassword reset, string id)
        {   
            int myid = Convert.ToInt32(id);
            var user = context.Users.Where(e => e.UserId == myid).FirstOrDefault();
            if (user != null)
            {
            user.Password = reset.NewPassword;
            }

            context.Users.Attach(user);
            context.SaveChanges();

            return View();
        }

        public IActionResult createAccount()
        {
            return View();
        }
        [HttpPost]
        public IActionResult createAccount(CreateAccountViewModel createAccount)
        {
            var user = new User();
            if (ModelState.IsValid)
            {
                user.FirstName = createAccount.FirstName;
                user.LastName = createAccount.LastName;
                user.Email = createAccount.Email;
                user.Mobile = createAccount.Mobile;
                user.Password = createAccount.Password;
                user.UserTypeId = 1;
                user.IsRegisteredUser = false;
                user.WorksWithPets = false;
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = 0;
                user.IsApproved = false;
                user.IsActive = false;
                user.IsDeleted = false;

                context.Users.Add(user);
                context.SaveChanges();
            }

            return View();
        }
        public IActionResult Beacome_a_pro()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Beacome_a_pro(BeacomeaproViewModel beacomeaproViewModel)
        {
            var user = new User();
            if (ModelState.IsValid)
            {

                user.FirstName = beacomeaproViewModel.FirstName;
                user.LastName = beacomeaproViewModel.LastName;
                user.Email = beacomeaproViewModel.Email;
                user.Password = beacomeaproViewModel.Password;
                user.Mobile = beacomeaproViewModel.Mobile;
                user.UserTypeId = 2;
                user.IsRegisteredUser = false;
                user.WorksWithPets = false;
                user.CreatedDate = DateTime.Now;
                user.ModifiedDate = DateTime.Now;
                user.ModifiedBy = 0;
                user.IsApproved = false;
                user.IsActive = false;
                user.IsDeleted = false;

                context.Users.Add(user);
                context.SaveChanges();
            }


            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}