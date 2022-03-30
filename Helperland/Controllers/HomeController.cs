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
using BCrypto = BCrypt.Net.BCrypt;

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
            if (User.Identity.IsAuthenticated)
            {
                var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var current_user_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();

                ViewBag.usertypeid = current_user_data.UserTypeId;
            }
            return View();
        }

        //usertypeid => 1  user 
        //usertypeid => 2  serviceprovider
        //usertypeid => 3  admin


        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel loginViewModel)
        {
            var user = new User();
            var usercredentials = context.Users.Where(x => x.Email == loginViewModel.Email).FirstOrDefault();
            if (usercredentials != null && BCrypt.Net.BCrypt.Verify(loginViewModel.Password, usercredentials.Password))
            {

                if (usercredentials.UserTypeId == 1)
                {
                    if (usercredentials.IsActive)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim("username", loginViewModel.Email));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, usercredentials.UserId.ToString()));
                        claims.Add(new Claim(ClaimTypes.Name, usercredentials.FirstName + usercredentials.LastName));

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("CustomerDashboard", "Customer");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { login = "true" });
                    }

                }
                if (usercredentials.UserTypeId == 2)
                {
                    if (usercredentials.IsActive)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim("username", loginViewModel.Email));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, usercredentials.UserId.ToString()));
                        claims.Add(new Claim(ClaimTypes.Name, usercredentials.FirstName + usercredentials.LastName));

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        return RedirectToAction("NewServiceRequest", "Provider");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { login = "true" });
                    }
                }
                if (usercredentials.UserTypeId == 3)
                {
                    var claims = new List<Claim>();
                    claims.Add(new Claim("username", loginViewModel.Email));
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, usercredentials.UserId.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, usercredentials.FirstName + usercredentials.LastName));

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    return RedirectToAction("Service_requests", "Admin");
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

        public IActionResult Prices()
        {
            if (User.Identity.IsAuthenticated)
            {
                var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var current_user_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();

                ViewBag.usertypeid = current_user_data.UserTypeId;
            }
            return View();
        }

        public IActionResult About()
        {
            if (User.Identity.IsAuthenticated)
            {
                var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var current_user_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();

                ViewBag.usertypeid = current_user_data.UserTypeId;
            }
            return View();
        }
        public IActionResult Faq()
        {
            if (User.Identity.IsAuthenticated)
            {
                var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var current_user_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();

                ViewBag.usertypeid = current_user_data.UserTypeId;
            }
            return View();
        }
        public IActionResult Contact()
        {
            if (User.Identity.IsAuthenticated)
            {
                var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var current_user_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();

                ViewBag.usertypeid = current_user_data.UserTypeId;
            }
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
                    Body = "<a href = 'https://localhost:7053/Home/Resetpassword/" + usercredentials.UserId + "'> Click Here to ResetPassword </a>"


                };
                sendemail.emailSend(email);

            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Resetpassword(string id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var current_user_Id = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var current_user_data = context.Users.Where(x => x.UserId == current_user_Id).FirstOrDefault();

                ViewBag.usertypeid = current_user_data.UserTypeId;
            }
            return View();
        }
        [HttpPost]
        public IActionResult Resetpassword(ResetPassword reset, string id)
        {
            int myid = Convert.ToInt32(id);
            var user = context.Users.Where(e => e.UserId == myid).FirstOrDefault();
            if (user != null)
            {
                var passwordHash = BCrypto.HashPassword(reset.NewPassword);
                user.Password = passwordHash;
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
            var passwordHash = BCrypto.HashPassword(createAccount.Password);
            var user = new User();

            var email = context.Users.Where(x => x.Email == createAccount.Email).ToList();

            if (ModelState.IsValid)
            {
                if (email.Count() == 0)
                {
                    user.FirstName = createAccount.FirstName;
                    user.LastName = createAccount.LastName;
                    user.Email = createAccount.Email;
                    user.Mobile = createAccount.Mobile;
                    user.Password = passwordHash;
                    user.UserTypeId = 1;
                    user.IsRegisteredUser = false;
                    user.WorksWithPets = false;
                    user.CreatedDate = DateTime.Now;
                    user.ModifiedDate = DateTime.Now;
                    user.ModifiedBy = 0;
                    user.IsApproved = false;
                    user.IsActive = true;
                    user.IsDeleted = false;

                    context.Users.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    ViewBag.Errormsg = "This E Mail Address is already Exists";
                }
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
            var passwordHash = BCrypto.HashPassword(beacomeaproViewModel.Password);
            var user = new User();

            var email = context.Users.Where(x => x.Email == beacomeaproViewModel.Email).ToList();

            if (ModelState.IsValid)
            {
                if (email.Count() == 0)
                {
                    user.FirstName = beacomeaproViewModel.FirstName;
                    user.LastName = beacomeaproViewModel.LastName;
                    user.Email = beacomeaproViewModel.Email;
                    user.Password = passwordHash;
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
                else
                {
                    ViewBag.Errormsg = "This E Mail Address is already Exists";
                }
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