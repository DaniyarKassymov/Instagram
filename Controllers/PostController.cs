using System.ComponentModel.DataAnnotations;
using Instagram.Database;
using Instagram.Models;
using Instagram.Util.Mappers;
using Instagram.Util.Services;
using Instagram.ViewModels.PostVms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace Instagram.Controllers;

public class PostController : Controller
{
    private readonly InstagramDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly IStringLocalizer<UserController> _stringLocalizer;

    public PostController(InstagramDbContext db, UserManager<User> userManager, IMemoryCache cache, IStringLocalizer<UserController> stringLocalizer)
    {
        _db = db;
        _userManager = userManager;
        _stringLocalizer = stringLocalizer;
    }
    
    [HttpGet]
    [Authorize]
    public IActionResult Add()
    {
        var vm = new PostAddVm
        {
            Image = null,
            UserId = _userManager.GetUserId(User)!
        };

        ViewData["Post's description"] = _stringLocalizer["Post's description"];
        ViewData["Create"] = _stringLocalizer["Create"];

        return View(vm);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [Display(Name = "Add")]
    public async Task<IActionResult> AddAsync(PostAddVm vm)
    {
        ViewData["Post's description"] = _stringLocalizer["Post's description"];
        ViewData["Create"] = _stringLocalizer["Create"];
        
        if (!ModelState.IsValid) return View(vm);
        
        var image = await FileUpload.Upload(vm.Description, vm.Image);
        var post = await _db.Posts.AddAsync(PostMapper.PostAddVmPost(vm, image));
        await _db.SaveChangesAsync();
            
        return RedirectToAction("PersonalCabinet", "User"); 
    }

    [HttpGet]
    [Authorize]
    [Display(Name = "Details")]
    public async Task<IActionResult> DetailsAsync(int id)
    {
        var post = await _db.Posts
            .Include(p => p.User)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == id);

        var comments = _db.Comments
            .Include(c => c.User)
            .Include(c => c.Post).ToList();

        var vm = PostMapper.PostPostDetailsVm(post);

        ViewBag.CurrentUser = _userManager.GetUserId(User);
        ViewData["Likes"] = _stringLocalizer["Likes"];
        ViewData["Add comment"] = _stringLocalizer["Add comment"];
        
        return View(vm);
    }

    [HttpPost]
    [Authorize]
    [Display(Name = "Delete")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var post = await _db.Posts.FindAsync(id);

        if (post.UserId != _userManager.GetUserId(User)) return NotFound();
        
        _db.Posts.Remove(post);
        await _db.SaveChangesAsync();
            
        return RedirectToAction("PersonalCabinet", "User");

    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> EditPost(int id)
    {
        var post = await _db.Posts.FindAsync(id);

        if (post == null) return NotFound();
        
        var vm = new EditVm()
        {
            Id = post.Id,
            Description = post.Description
        };

        ViewData["Edit"] = _stringLocalizer["Edit"];
        ViewData["Description"] = _stringLocalizer["Description"];

        return View("EditPost", vm);

    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> EditPost(EditVm vm)
    {
        ViewData["Edit"] = _stringLocalizer["Edit"];
        ViewData["Description"] = _stringLocalizer["Description"];
        if (!ModelState.IsValid) return View(vm);
        
        var exPost = await _db.Posts.FindAsync(vm.Id);
        
        if (exPost == null) return NotFound();
        
        exPost.Description = vm.Description;
        await _db.SaveChangesAsync();
        
        return RedirectToAction("PersonalCabinet", "User");
    }
}