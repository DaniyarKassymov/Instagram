using System.ComponentModel.DataAnnotations;
using Instagram.Database;
using Instagram.Models;
using Instagram.Util.Mappers;
using Instagram.ViewModels.UserVms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace Instagram.Controllers;

public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly InstagramDbContext _db;
    private readonly IMemoryCache _cache;
    private readonly IStringLocalizer<UserController> _stringLocalizer;

    public UserController(UserManager<User> userManager, InstagramDbContext db, IMemoryCache cache, IStringLocalizer<UserController> stringLocalizer)
    {
        _userManager = userManager;
        _db = db;
        _cache = cache;
        _stringLocalizer = stringLocalizer;
    }

    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
        if (!User.Identity.IsAuthenticated)
            return RedirectToAction("Login", "Account");

        var user = _db.Users
            .Include(u => u.Subscribed)
            .FirstOrDefault(u => u.Id == _userManager.GetUserId(User));

        var vm = new UserIndexVm{Subscribed = user.Subscribed};

        var posts = _db.Posts
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .Include(p => p.User)
            .OrderBy(p => p.UserId);

        ViewBag.CurrentUser = _userManager.GetUserId(User);
        ViewData["Likes"] = _stringLocalizer["Likes"];
        ViewData["Main page"] = _stringLocalizer["Main page"];

        return View(vm);
    }
    
    [HttpGet]
    [Authorize]
    [Display(Name = "PersonalCabinet")]
    public async Task<IActionResult> PersonalCabinetAsync()
    {
        var userId = _userManager.GetUserId(User);

        User user = null;
        
        if (!_cache.TryGetValue(userId, out user))
        {
            user = await _db.Users
                .Include(u => u.Subscribers)
                .Include(u => u.Subscribed)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user != null)
                _cache.Set(user.Id, user, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10)));
        }
        
        var userUserIndexVm = UserMapper.UserPersonalCabinetVm(user);
        
        ViewBag.CurrentUser = _userManager.GetUserId(User);
        ViewData["Posts"] = _stringLocalizer["Posts"];
        ViewData["Subscribers"] = _stringLocalizer["Subscribers"];
        ViewData["Personal cabinet"] = _stringLocalizer["Personal cabinet"];
        ViewData["Subscriptions"] = _stringLocalizer["Subscriptions"];
        ViewData["LogOut"] = _stringLocalizer["LogOut"];

        return View(userUserIndexVm);
    }
    
    [HttpGet]
    [Authorize]
    [Display(Name = "OtherUserCabinet")]
    public async Task<IActionResult> OtherUserCabinetAsync(string id)
    {
        if (!_cache.TryGetValue(id, out User? user))
        {
           user = await _db.Users
                .Include(u => u.Subscribers)
                .Include(u => u.Subscribed)
                .Include(u => u.Posts)
                .FirstOrDefaultAsync(u => u.Id == id);

           if (user != null)
               _cache.Set(user.Id, user, new MemoryCacheEntryOptions()
                   .SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
        }
        
        var vm = UserMapper.UserPersonalCabinetVm(user);

        ViewBag.CurrentUser = _userManager.GetUserId(User);
        ViewData["Subscribe"] = _stringLocalizer["Subscribe"];
        ViewData["Unsubscribe"] = _stringLocalizer["Unsubscribe"];
        
        return View(vm);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult FoundedUsers(string fieldName)
    {
        var userId = _userManager.GetUserId(User);

        var users = _db.Users.Where(u => (u.UserName.Contains(fieldName) ||
                                              u.Email.Contains(fieldName) ||
                                              u.Name.Contains(fieldName)) &&
                                             u.Id != userId).ToList();

        var vm = users.Select(UserMapper.UserPersonalCabinetVm).ToList();

        return View(vm);
    }

    [HttpPost]
    [Authorize]
    [Display(Name = "Subscribe")]
    public async Task<IActionResult> SubscribeAsync(string id)
    {
        var subscribed = _db.Users.FirstOrDefault(u => u.Id == id);
        var subscriber = _db.Users.Include(user => user.Subscribed)
            .FirstOrDefault(u => u.Id == _userManager.GetUserId(User));

        if (subscribed == null)
            return Problem("Такого пользователя нет");

        if (subscriber == null)
            return Unauthorized("Пользователь не авторизован");
        
        if (subscriber.Subscribed.Contains(subscribed))
        {
            subscriber.Subscribed.Remove(subscribed);
            subscribed.Subscribers.Remove(subscriber);
        }
        else
        {
            subscriber.Subscribed.Add(subscribed);
            subscribed.Subscribers.Add(subscriber);
        }

        await _db.SaveChangesAsync();
        
        return RedirectToAction("OtherUserCabinet", "User", new {id = id});
    }

    [Authorize]
    [HttpPost]
    [Display(Name = "Like")]
    public async Task<IActionResult> LikeAsync(int postId)
    {
        var post = _db.Posts.FirstOrDefault(p => p.Id == postId);
        var user = _db.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));

        if (post == null)
            return NotFound();

        if (user == null)
            return Unauthorized();
        
        var like = new Like
        {
            PostId = post.Id,
            UserId = user.Id
        };

        var likeInDb = _db.Likes.FirstOrDefault(l => l.PostId == post.Id && l.UserId == user.Id);

        if (likeInDb != null)
        {
            _db.Likes.Remove(likeInDb);
            await _db.SaveChangesAsync();
            
            return RedirectToAction("Index", "User");
        }

        _db.Likes.Add(like);
        await _db.SaveChangesAsync();

        return RedirectToAction("Index", "User");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Comment(string commentText, int postId)
    {
        var post = _db.Posts.FirstOrDefault(p => p.Id == postId);
        var user = _db.Users.FirstOrDefault(u => u.Id == _userManager.GetUserId(User));
        
        if (post == null)
            return NotFound();

        if (user == null)
            return Unauthorized();

        if (commentText == null)
            return NotFound();

        var comment = new Comment
        {
            Text = commentText,
            PostId = post.Id,
            UserId = user.Id
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync(); 
        
        return RedirectToAction("Index", "User"); 
    }
}