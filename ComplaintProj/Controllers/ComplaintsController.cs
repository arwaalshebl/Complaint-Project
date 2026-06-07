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

            return RedirectToAction("Index", "Home");
        }

        return View(complaint);
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
