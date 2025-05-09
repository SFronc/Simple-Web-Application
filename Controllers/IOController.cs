using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace WebApp.Controllers;

public class IO : Controller{
    // GET
    public IActionResult ReadForm(){
        if(!HttpContext.Session.Keys.Contains("data"))
            ViewData["data"] += "no data";
        else
            ViewData["data"] += HttpContext.Session.GetString("data");
        return View();
    }

    // POST
    [HttpPost]
    public IActionResult ReadForm(IFormCollection form){
        string data = form["data"].ToString();
        HttpContext.Session.SetString("data",data);
        ViewData["data"] += data;
        return View();
    }
}