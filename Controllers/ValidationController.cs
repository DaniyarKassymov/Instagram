using Instagram.Database;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers;

public class ValidationController : Controller
{
    private readonly InstagramDbContext _db;

    public ValidationController(InstagramDbContext db)
    {
        _db = db;
    }

    public bool CheckUserNameOrEmail(string emailOrUserName)
    {
        return _db.Users.Any(u => u.Email == emailOrUserName 
                                      || u.UserName == emailOrUserName);
    }

    public bool CheckUserName(string userName)
    {
        return !_db.Users.Any(u => u.UserName == userName);
    }
    
    public bool CheckEmail(string email)
    {
        return !_db.Users.Any(u => u.Email == email);
    }
}