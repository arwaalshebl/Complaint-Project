using ComplaintProj.Data;
using ComplaintProj.Models;
using ComplaintProj.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
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

    //
    ////BEFOR VIEWMODEL FOR CHECK BOX
    //public IActionResult Details(int? id)
    //{

        //    var complaint = _context.Complaints
        //        .FirstOrDefault(x => x.Id == id);   


        //    if (complaint == null) return NotFound();

        //    return View(complaint);
        //}

        //AFTER VIEWMODEL FOR CHECK BOX
        public IActionResult Details(int? id)
    {

        var complaint = _context.Complaints
            .FirstOrDefault(x => x.Id == id);


        if (complaint == null) return NotFound();
        var viewModel = new ComplaintViewModel
        {
            PatientName = complaint.PatientName,
            PhoneNumber = complaint.PhoneNumber,
            DateOfBirth = complaint.DateOfBirth,
            Job = complaint.Job,
            Nationality = complaint.Nationality,
            Email = complaint.Email,
            ComplaintType = complaint.ComplaintType,
            ComplaintLocation = complaint.ComplaintLocation,
            ComplaintSummary = complaint.ComplaintSummary,

            // 💡 التحويل العكسي: تفكيك النص الطويل المفصول بفاصلة (,) وإعادته كقائمة لتفعيل الـ Checkboxes المقفلة
            ComplaintCategory = !string.IsNullOrEmpty(complaint.ComplaintCategory)
                                    ? complaint.ComplaintCategory.Split(',').ToList()
                                    : new List<string>()
        };

        return View(viewModel);
    }


    ////BEFOR VIEWMODEL FOR CHECK BOX
    //public IActionResult Create()
    //{
    //    return View();
    //}
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(ComplaintModel complaint)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        _context.Complaints.Add(complaint);
    //        await _context.SaveChangesAsync();

    //        return RedirectToAction("Index", "Complaints");
    //    }

    //    return View(complaint);
    //}

    //AFTER VIEWMODEL FOR CHECK BOX
    public IActionResult Create()
    {
        return View(new ComplaintViewModel());
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ComplaintViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var complaint = new ComplaintModel
            {
                PatientName = viewModel.PatientName,
                PhoneNumber = viewModel.PhoneNumber,
                DateOfBirth = viewModel.DateOfBirth,
                Job = viewModel.Job,
                Nationality = viewModel.Nationality,
                Email = viewModel.Email,
                ComplaintType = viewModel.ComplaintType,
                ComplaintLocation = viewModel.ComplaintLocation,
                ComplaintSummary = viewModel.ComplaintSummary,

                // Convert it to string
                ComplaintCategory = viewModel.ComplaintCategory != null && viewModel.ComplaintCategory.Any()
                                        ? string.Join(",", viewModel.ComplaintCategory)
                                        : null
            };
            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        return View(viewModel);
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
