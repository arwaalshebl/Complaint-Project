using ComplaintProj.Data;
using ComplaintProj.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
namespace ComplaintProj.Controllers;

public class ComplaintsController : Controller
{
    private readonly AppDbContext _context;

    
    public ComplaintsController(AppDbContext context)
    {
        _context = context;
    }
    public IActionResult Index()
    {

        var complaintsList = _context.Complaints.ToList();

        if (complaintsList == null)
        {
            return NotFound();
        }

        return View(complaintsList);

    }
    public IActionResult Details(int? id)
    {

        var complaint = _context.Complaints
            .FirstOrDefault(x => x.Id == id);   
            

        if (complaint == null) return NotFound();

        return View(complaint);
    }
    public IActionResult Create()
    {
        return View();
    }
    // 2. رابط استقبال البيانات وحفظها (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ComplaintModel complaint)
    {
        if (ModelState.IsValid)
        {
            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Complaints");
        }

        return View(complaint);
    }
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var complaint = await _context.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();

        complaint.Status = status;
        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Complaints");

    }
        ////lang
        //public IActionResult ChangeLanguage(string culture)

        //{
        //    Response.Cookies.Append(
        //        CookieRequestCultureProvider.DefaultCookieName,
        //        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture))
        //        , new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        //        );

        //    string returnUrl = Request.Headers.Referer.ToString();
        //    return Redirect(returnUrl);
        //}
    }
