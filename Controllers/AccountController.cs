namespace WebApp.Controllers;

using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

public class AccountController : Controller{
    private readonly AppDbContext _context;

    public AccountController(AppDbContext context){
        _context = context;
    }

    [HttpGet]
    public IActionResult Login(){
        return View();
    }

    [HttpPost]
    public IActionResult HandleForm(string username, string password, string action){
        if (action == "login"){
            return Login(username, password);
        }
        else if (action == "register"){
            return Register(username, password);
        }

        return BadRequest("Invalid action");
    }

    [HttpPost]
    public IActionResult Login(string username, string password){
        var user = _context.Users.FirstOrDefault(u => u.login == username && u.password == ComputeHash(password, MD5.Create()));

        if(user != null){
            HttpContext.Session.SetString("IsLoggedIn", "true");
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetInt32("UserId", user.userId);
            return RedirectToAction("Dashboard"); 
        }

        ViewBag.ErrorMessage = "Invalid login or password";
        return View("Login");
    }

    [HttpPost]
    public IActionResult Register(string username, string password){
        if(_context.Users.Any(u => u.login == username)){
            ViewBag.ErrorMessage = "Username already exists!";
            return View("Login");
        }

        var hashedPassword = ComputeHash(password, MD5.Create());

        var newUser = new User{
            login = username,
            password = hashedPassword
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();

        HttpContext.Session.SetString("IsLoggedIn", "true");
        HttpContext.Session.SetString("Username", username);
        HttpContext.Session.SetInt32("UserId", newUser.userId);
        return RedirectToAction("Dashboard");

    }

    [HttpPost]
    public IActionResult Logout(){
        HttpContext.Session.Remove("IsLoggedIn");
        HttpContext.Session.Remove("Username");
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Dashboard(){
        if(HttpContext.Session.GetString("IsLoggedIn") != "true"){
            return RedirectToAction("Login");
        }

        ViewBag.Username = HttpContext.Session.GetString("Username");
        return View();
    }


    [HttpGet]
    public IActionResult Notes(){
        if(HttpContext.Session.GetString("IsLoggedIn") != "true"){
            return RedirectToAction("Login");
        }

        ViewBag.Username = HttpContext.Session.GetString("Username");
        return View();
    }

    [HttpPost]
    public IActionResult DeleteAccount(){
        if(HttpContext.Session.GetString("IsLoggedIn") == "true" && HttpContext.Session.GetInt32("UserId") != null){
            var userId = HttpContext.Session.GetInt32("UserId");

            var recordsToDelete = _context.Notes.Where(e => e.userId == userId).ToList();
            if(recordsToDelete != null){
                _context.Notes.RemoveRange(recordsToDelete);
                _context.SaveChanges();
            }

            var recordToDelete = _context.Users.FirstOrDefault(u => u.userId == userId);
    
            if (recordToDelete != null){
                _context.Users.Remove(recordToDelete);
                _context.SaveChanges();
            }

        }
        return RedirectToAction("Login");
    }

    public static string ComputeHash(string input, HashAlgorithm hasher){
        Encoding enc = Encoding.UTF8;
        var hashBuilder = new StringBuilder();
        byte[] result = hasher.ComputeHash(enc.GetBytes(input));
        foreach(var b in result)
            hashBuilder.Append(b.ToString("x2"));
        return hashBuilder.ToString();
    }
}
