namespace WebApp.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

[Route("Notes")]
public class NotesController : Controller{
    private readonly AppDbContext _context;

    public NotesController(AppDbContext context){
        _context = context;
    }


    [HttpGet("Private")]
    public async Task<IActionResult> Private(){
        if(HttpContext.Session.GetString("IsLoggedIn") != "true"){
            return RedirectToAction("Login", "Account"); 
        }
        
        var results = await _context.Notes.Where(x => x.userId == HttpContext.Session.GetInt32("UserId"))
        .Select(x => new NoteDto{
            id = x.noteId,
            title = x.title,
            date = x.date
        })
        .ToListAsync();
        return View(results); 
    }

    [HttpGet("Details/{id}")] 
    public async Task<IActionResult> Details(int id){
        if(HttpContext.Session.GetString("IsLoggedIn") != "true"){
            return RedirectToAction("Login", "Account"); 
        }

        HttpContext.Session.SetInt32("CurrentNoteId", id);

        var item = await _context.Notes.FindAsync(id);
        if(item != null && item.userId == HttpContext.Session.GetInt32("UserId")){
            return View(item);
        }
        else return RedirectToAction("Private");

    } 

    [HttpPost("AddNote")]
    public async Task<IActionResult> AddNote(string notetitle, string notetext){

        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null){
            return RedirectToAction("Login", "Account");
        }

        if (string.IsNullOrEmpty(notetitle)){
            ModelState.AddModelError("title", "Tytuł nie może być pusty.");
            return RedirectToAction("Private");
        }
        
        var newData = new Note { title = notetitle, data = notetext, date = DateTime.Now, userId = userId.Value };
        

        await _context.Notes.AddRangeAsync(newData);
        await _context.SaveChangesAsync();

        return RedirectToAction("Private");
    }

    [HttpPost("DeleteNote")]
    public async Task<IActionResult> DeleteNote(){
        var id = HttpContext.Session.GetInt32("CurrentNoteId");

        if(id == null){
            return RedirectToAction("Private");
        }

        var itemToDelete = await _context.Notes.FirstOrDefaultAsync(n => n.noteId == id);

        if (itemToDelete == null){
            return RedirectToAction("Private");
        }

        _context.Notes.Remove(itemToDelete);
        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("CurrentNoteId");

        return RedirectToAction("Private");
    }

    [HttpPost("UpdateNote")]
    public async Task<IActionResult> UpdateNote(string notetext){
        var id = HttpContext.Session.GetInt32("CurrentNoteId");

        if(id == null){
            return RedirectToAction("Private");
        }

        var itemToUpdate = await _context.Notes.FirstOrDefaultAsync(n => n.noteId == id);

        if (itemToUpdate == null){
            return RedirectToAction("Private");
        }

        itemToUpdate.data = notetext;
        itemToUpdate.date = DateTime.Now;

        await _context.SaveChangesAsync();

        HttpContext.Session.Remove("CurrentNoteId");

        return RedirectToAction("Private");
    }


}
