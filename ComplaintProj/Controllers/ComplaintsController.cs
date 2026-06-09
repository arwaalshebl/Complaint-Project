using ComplaintProj.Data;
using ComplaintProj.Models;
using ComplaintProj.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
namespace ComplaintProj.Controllers;

public class ComplaintsController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _webHostEnvironment;


    public ComplaintsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
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
                                    : new List<string>(),

            AttachmentPath = complaint.AttachmentPath

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
            //attachment
            List<string> savedFileNames = new List<string>();

            if (viewModel.Attachments != null && viewModel.Attachments.Any())
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in viewModel.Attachments)
                {
                    if (file.Length > 0)
                    {
                        //create a new uniqu name
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        savedFileNames.Add(uniqueFileName);
                    }
                }
            } 
            //end
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
                                    : null,

                AttachmentPath = savedFileNames.Any() ? string.Join(",", savedFileNames) : null
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
