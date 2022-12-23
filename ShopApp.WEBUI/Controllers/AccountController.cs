using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using ShopApp.WEBUI.EmailServices;
using ShopApp.WEBUI.Extensions;
using ShopApp.WEBUI.Identity;
using ShopApp.WEBUI.Models;
using System;
using System.Threading.Tasks;

namespace ShopApp.WEBUI.Controllers
{
    //[ValidateAntiForgeryToken]
    public class AccountController : Controller
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private IEmailSender _emailSender;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginModel()
            {
                ReturnUrl = returnUrl

            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            //var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null)
            {
                ModelState.AddModelError("", "Bu Kullanıcı Adı İle Daha Önce Hesap Oluşturulmamış.");
                return View(model);
            }
            if(!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("", "Lütfen Mail Hesabınıza Gelen Link İle Hesabınızı Onaylayın.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
            if (result.Succeeded)
            {
                return Redirect(model.ReturnUrl??"~/");
            }
            ModelState.AddModelError("", "Girilen Kullanıcı Adı Veya Paralo Yanlış.");
            return View(model);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.UserName,
                Email = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //generate token
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail", "Account", new {
                    userId= user.Id,
                    token = code
                });

                //email
                await _emailSender.SendEmailAsync(model.Email, "Hesabınızı Onaylayınız.", $"Lütfen Email Hesabınızı Onaylamak İçin Linke <a href='https://localhost:44329{url}'>Tıklayınız</a>");
                return RedirectToAction("Login", "Account");
            }
            ModelState.AddModelError("", "Bilinmeyen Bir Hata Oldu Lütfen Tekrar Deneyiniz.");
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData.Put("message", new AlertMessage()
            {
                Title = "Oturum Kapatıldı.",
                AlertType = "warning",
                Message = "Hesabınız Güvenli Bir Şekilde Kapatıldı."
            });
            return Redirect("~/");
        }

        public async Task<IActionResult> ConfirmEmail(string UserId,string token)
        {
            if (UserId==null || token ==null)
            {
                TempData.Put("message", new AlertMessage()
                {
                    Title= "Geçersiz Token.",
                    AlertType = "danger",
                    Message = "Geçersiz Token"
                });                            
                return View();
            }
            var user = await _userManager.FindByIdAsync(UserId);
            if (user!=null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Hesabınız Onaylandı.",
                        AlertType = "success",
                        Message = "Hesabınız Onaylandı."
                    });                                     
                    return View();
                }    
            }
            TempData.Put("message", new AlertMessage()
            {
                Title = "Hesabınız Onaylanmadı.",
                AlertType = "warning",
                Message = "Hesabınız Onaylanmadı."
            });                   
            return View();            
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return View();

            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return View();
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            //generate token          
            var url = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = code
            });

            //email
            await _emailSender.SendEmailAsync(email, "Reset Password", $"Paralonızı Yebilemek İçin Linke <a href='https://localhost:44329{url}'>Tıklayınız</a>");
            return View();
        }
        public IActionResult ResetPassword(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Home", "Index");
            }
            var model = new ResetPasswordModel
            {
                Token = token

            };
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return RedirectToAction("Home", "Index");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Token,model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login","Account");
            }
            return View(model);
        }       
    }
}
