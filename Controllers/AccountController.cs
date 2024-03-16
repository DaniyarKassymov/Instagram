using System.ComponentModel.DataAnnotations;
using Instagram.Database;
using Instagram.Models;
using Instagram.Util.Enums;
using Instagram.Util.Mappers;
using Instagram.Util.Services;
using Instagram.ViewModels.UserVms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace Instagram.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly InstagramDbContext _db;
    private readonly IStringLocalizer<AccountController> _stringLocalizer;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, InstagramDbContext db, IMemoryCache cache, IStringLocalizer<AccountController> stringLocalizer)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        ViewBag.GenderList = new List<Gender>()
        {
            Gender.Male,
            Gender.Female,
        };

        ViewData["Register"] = _stringLocalizer["Register"];
        ViewData["Email"] = _stringLocalizer["Email"];
        ViewData["Username"] = _stringLocalizer["Username"];
        ViewData["Password"] = _stringLocalizer["Password"];
        ViewData["Confirm"] = _stringLocalizer["Confirm"];
        ViewData["Name"] = _stringLocalizer["Name"];
        ViewData["Description"] = _stringLocalizer["Description"];
        ViewData["Phone"] = _stringLocalizer["Phone"];
        ViewData["All"] = _stringLocalizer["All"];
        ViewData["Authorize"] = _stringLocalizer["Authorize"];
        ViewData["Have account?"] = _stringLocalizer["Have account?"];
        
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Display(Name = "Register")]
    public async Task<IActionResult> RegisterAsync(RegisterVm vm)
    {
        if (ModelState.IsValid)
        {
            var avatar = await FileUpload.Upload(vm.UserName, vm.Avatar);
            var user = UserMapper.RegisterVmUser(vm, avatar);
            var result = await _userManager.CreateAsync(user, vm.Password);
            
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "User");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }
        
        ViewBag.GenderList = new List<Gender>()
        {
            Gender.Male,
            Gender.Female,
        };
        
        ViewData["Register"] = _stringLocalizer["Register"];
        ViewData["Email"] = _stringLocalizer["Email"];
        ViewData["Username"] = _stringLocalizer["Username"];
        ViewData["Password"] = _stringLocalizer["Password"];
        ViewData["Confirm"] = _stringLocalizer["Confirm"];
        ViewData["Name"] = _stringLocalizer["Name"];
        ViewData["Description"] = _stringLocalizer["Description"];
        ViewData["Phone"] = _stringLocalizer["Phone"];
        ViewData["All"] = _stringLocalizer["All"];
        ViewData["Authorize"] = _stringLocalizer["Authorize"];
        ViewData["Have account?"] = _stringLocalizer["Have account?"];

        return View(vm);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        var vm = new LoginVm
        {
            EmailOrUserName = null,
            Password = null
        };
        
        ViewData["Don't you have an account yet?"] = _stringLocalizer["Don't you have an account yet?"];
        ViewData["Password"] = _stringLocalizer["Password"];
        ViewData["Register"] = _stringLocalizer["Register"];
        ViewData["Username or email"] = _stringLocalizer["Username or email"];
        ViewData["Authorize"] = _stringLocalizer["Authorize"];

        return View(vm);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Display(Name = "Login")]
    public async Task<IActionResult> LoginAsync(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);
        
        var user = await _userManager.FindByEmailAsync(vm.EmailOrUserName) 
                   ?? await _db.Users.FirstOrDefaultAsync(u => u.UserName == vm.EmailOrUserName);

        var result = await _signInManager.PasswordSignInAsync(
            user!,
            vm.Password,
            false,
            false);

        if (result.Succeeded)
            return RedirectToAction("Index", "User");
            
        ModelState.AddModelError(string.Empty, "Неверно введены данные");
        
        ViewData["Don't you have an account yet?"] = _stringLocalizer["Don't you have an account yet?"];
        ViewData["Password"] = _stringLocalizer["Password"];
        ViewData["Register"] = _stringLocalizer["Register"];
        ViewData["Username or email"] = _stringLocalizer["Username or email"];
        ViewData["Authorize"] = _stringLocalizer["Authorize"];

        return View(vm);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [Display(Name = "LogOut")]
    public async Task<IActionResult> LogOutAsync()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "User");
    }
}