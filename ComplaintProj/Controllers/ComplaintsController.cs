using Azure.Core;
using ComplaintProj.Data;
using ComplaintProj.Hubs;
using ComplaintProj.Migrations;
using ComplaintProj.Models;
using ComplaintProj.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Security.Claims;

namespace ComplaintProj.Controllers;

public class ComplaintsController : Controller
{
    private readonly AppDbContext _context;
    //files
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHubContext<ComplaintHub> _hubContext;

    public ComplaintsController(AppDbContext context,
        IWebHostEnvironment webHostEnvironment,
        UserManager<IdentityUser> userManager,
        IHubContext<ComplaintHub> hubContext)
    {
        _context = context;
        _webHostEnvironment = webHostEnvironment;
        _userManager = userManager;
        _hubContext = hubContext;
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult>  Index()
    {
        //GET USER INFO 
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
        var userEmail = User.Identity?.Name;

        // Using AsQueryable() delays database execution so filters run efficiently on the SQL server rather than loading the entire table into RAM like ToList().
        var complaintsQuery = _context.Complaints.AsQueryable();
        if (User.IsInRole("Patient"))
        {
            complaintsQuery = complaintsQuery.Where(c => c.Email == userEmail);
        }
        else if (User.IsInRole("HealthcareProvider"))
        {


            complaintsQuery = complaintsQuery.Where(c => c.AssignedStaffId == userId);
        }

        var filteredComplaints = await complaintsQuery
             .OrderByDescending(c => c.Status == "Reopened,Unresolved") 
             .ThenByDescending(c => c.CreatedAt) 
             .ToListAsync();

        var viewModelList = filteredComplaints.Select(complaint => new ComplaintViewModel
        {
            Id = complaint.Id,
            PatientName = complaint.PatientName,
            CreatedAt = complaint.CreatedAt,
            ComplaintType = complaint.ComplaintType,
            ComplaintLocation = complaint.ComplaintLocation,
            ComplaintCategory = !string.IsNullOrEmpty(complaint.ComplaintCategory)
                                    ? complaint.ComplaintCategory.Split(',').ToList()
                                    : new List<string>(),
            Status = complaint.Status,
            IsSatisfied = complaint.IsSatisfied,
            AssignedStaffId = complaint.AssignedStaffId
            

        }).ToList();

        return View(viewModelList);

    }

    
    ////BEFOR VIEWMODEL FOR CHECK BOX
    //public IActionResult Details(int? id)
    //{

    //    var complaint = _context.Complaints
    //        .FirstOrDefault(x => x.Id == id);   


    //    if (complaint == null) return NotFound();

    //    return View(complaint);
    //}

    //AFTER VIEWMODEL FOR CHECK BOX
    public async Task<IActionResult> Details(int id)
    {

        var complaint = _context.Complaints
            .FirstOrDefault(x => x.Id == id);


        if (complaint == null) return NotFound();

        //check assignmenet
        string? staffName = "No staff assigned yet";
        if (!string.IsNullOrEmpty(complaint.AssignedStaffId))
        {
            var assignedUser = await _userManager.FindByIdAsync(complaint.AssignedStaffId);
            if (assignedUser != null)
            {
                staffName = assignedUser.UserName;
            }
        }


        //only show the healthcare users in list
        var usersInRole = await _userManager.GetUsersInRoleAsync("HealthcareProvider");

        //get it from AspNetUsers
        var staffList = usersInRole
            .Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            })
            .ToList();




        var viewModel = new ComplaintViewModel
        {
            Id = complaint.Id,
            PatientName = complaint.PatientName,
            PhoneNumber = complaint.PhoneNumber,
            DateOfBirth = complaint.DateOfBirth,
            Job = complaint.Job,
            Nationality = complaint.Nationality,
            Email = complaint.Email,
            ComplaintType = complaint.ComplaintType,
            ComplaintLocation = complaint.ComplaintLocation,
            ComplaintSummary = complaint.ComplaintSummary,
            Status = complaint.Status,
            // Convert checkbox list to string
            ComplaintCategory = !string.IsNullOrEmpty(complaint.ComplaintCategory)
                                    ? complaint.ComplaintCategory.Split(',').ToList()
                                    : new List<string>(),

            AttachmentPath = complaint.AttachmentPath,
            AssignedStaffName = staffName,
            AssignedStaffId = complaint.AssignedStaffId,
            HealthcareStaffList = staffList,
            StaffReply = complaint.StaffReply,
            AllRepliesList = !string.IsNullOrEmpty(complaint.StaffReply)
            ? complaint.StaffReply.Split(new[] { "|||" }, StringSplitOptions.RemoveEmptyEntries).ToList()
            : new List<string>(),
            IsSatisfied =complaint.IsSatisfied

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
                Id = viewModel.Id,
                PatientName = viewModel.PatientName,
                PhoneNumber = viewModel.PhoneNumber,
                DateOfBirth = viewModel.DateOfBirth,
                Job = viewModel.Job,
                Nationality = viewModel.Nationality,
                Email = viewModel.Email,
                ComplaintType = viewModel.ComplaintType,
                ComplaintLocation = viewModel.ComplaintLocation,
                ComplaintSummary = viewModel.ComplaintSummary,
                Status= viewModel.Status,

                // Convert it to string
                ComplaintCategory = viewModel.ComplaintCategory != null && viewModel.ComplaintCategory.Any()
                                    ? string.Join(",", viewModel.ComplaintCategory)
                                    : null,

                AttachmentPath = savedFileNames.Any() ? string.Join(",", savedFileNames) : null
            };

            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();


            await _hubContext.Clients.Group("PatientServicesGroup")
                .SendAsync("ReceiveComplaintToast", "NewComplaint", $"New Complaint Submitted! ID: #{complaint.Id}");

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

    //lang
    public IActionResult ChangeLanguage(string culture)

    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture))
            , new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

        string returnUrl = Request.Headers.Referer.ToString();
        return Redirect(returnUrl);
    }

    //// no need to assign get
    //[HttpGet]
    //public async Task<IActionResult> Assign(int id)
    //{
    //    var complaint = await _context.Complaints.FindAsync(id);
    //    if (complaint == null) return NotFound();

    //    //get it from AspNetUsers
    //    var staffList = await _userManager.Users
    //        .Select(u => new SelectListItem
    //        {
    //            Value = u.Id,       
    //            Text = u.UserName  
    //        })
    //        .ToListAsync();

    //    var viewModel = new ComplaintViewModel
    //    {
    //        Id=complaint.Id,
    //        PatientName = complaint.PatientName,
    //        PhoneNumber = complaint.PhoneNumber,
    //        DateOfBirth = complaint.DateOfBirth,
    //        Job = complaint.Job,
    //        Nationality = complaint.Nationality,
    //        Email = complaint.Email,
    //        ComplaintType = complaint.ComplaintType,
    //        ComplaintLocation = complaint.ComplaintLocation,
    //        ComplaintSummary = complaint.ComplaintSummary,
    //        Status = complaint.Status,
    //        // Convert it to string
    //        ComplaintCategory = !string.IsNullOrEmpty(complaint.ComplaintCategory)
    //                                ? complaint.ComplaintCategory.Split(',').ToList()
    //                                : new List<string>(),

    //        //attach
    //        AttachmentPath = complaint.AttachmentPath,
    //        //assign
    //        AssignedStaffId = complaint.AssignedStaffId,
    //        HealthcareStaffList = staffList 
    //    };

    //    return View(viewModel);
    //}


    [HttpPost]
    [Authorize(Roles = "Admin,PatientServices")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(int id, string assignedStaffId)
    {
            var complaint = await _context.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();

      

        //compare between new staff and current staff
        if (!string.IsNullOrEmpty(complaint.AssignedStaffId) && complaint.AssignedStaffId != assignedStaffId)
        {
            complaint.Status = "In Progress,ReAssigned"; //re assign
        }
        else
        {
            complaint.Status = "In Progress,Assigned"; // first assign or assign to same person
        }
        //Id to the selcted staff
        complaint.AssignedStaffId = assignedStaffId;
        //complaint.Status = "In Progress,Assigned"; // this befor reassign case

        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync();
        ////to all provider
        //await _hubContext.Clients.Group("HealthcareProviderGroup")
        //        .SendAsync("ReceiveComplaintToast", "ComplaintAssigned", $"New Complaint Assigned to you! ID: #{complaint.Id}");

        await _hubContext.Clients.User(assignedStaffId).SendAsync("ReceiveComplaintToast", "ComplaintAssigned", $"New Complaint Assigned to you! ID: #{complaint.Id}");

        // return RedirectToAction("Index");
        return RedirectToAction("Details", new { id = id });


    }



    [HttpPost]
    [Authorize(Roles = "Admin,HealthcareProvider")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string staffReply)
    {
        if (string.IsNullOrWhiteSpace(staffReply))
        {
            ModelState.AddModelError("", "write reply ");
            return RedirectToAction("Details", new { id = id });
        }

        var complaint = await _context.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();

        string currentStaffName = User.Identity?.Name ?? "Healthcare Staff";
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd hh:mm tt");
        string formattedReply = $"[{currentStaffName} - {timestamp}]:\n{staffReply}";
        

        if (string.IsNullOrEmpty(complaint.StaffReply))
        {
            complaint.StaffReply = formattedReply;
        }
        else
        {
            complaint.StaffReply = complaint.StaffReply + "|||" + formattedReply;
        }

        complaint.Status = "In Progress,Replied";




        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync();


        ////to patient
        //var user = await _userManager.FindByEmailAsync(complaint.Email);
        //if (user != null)
        //{
        //    await _hubContext.Clients.User(user.Id).SendAsync("ReceiveComplaintToast", "ComplaintReplied", $"Your c has been replied! ID: #{complaint.Id}");
        //}

        //to PatientServices
        await _hubContext.Clients.Group("PatientServicesGroup")
                .SendAsync("ReceiveComplaintToast", "ComplaintReplied", $"Complaint ID: #{complaint.Id} replied by {complaint.AssignedStaffId}");


        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,PatientServices")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var complaint = await _context.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();

        complaint.Status = "In Progress,Approve";

        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync();


        //notifi patient
        var user = await _userManager.FindByEmailAsync(complaint.Email);
        if (user != null)
        {
            await _hubContext.Clients.User(user.Id).SendAsync("ReceiveComplaintToast", "ComplaintApprove", $"Your complaint has been resolved! ID: #{complaint.Id} \n You can rate your satisfaction ");
        }

        return RedirectToAction("Details", new { id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]

    public IActionResult Delete(int id)
    {
        var complaint = _context.Complaints
            
            .FirstOrDefault(p => p.Id == id);
        if (complaint == null) return NotFound();


        _context.Complaints.Remove(complaint);
        _context.SaveChanges();
        return RedirectToAction("Index");


    }

    [HttpPost]
    [Authorize(Roles = "Admin,Patient")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitFeedback(int id, string satisfaction)
    {
        var complaint = await _context.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();

        complaint.IsSatisfied = satisfaction;

        if (satisfaction == "Satisfied")
        {
            complaint.Status = "Closed";
        }
        else if (satisfaction == "Not Satisfied")
        {
            complaint.Status = "Reopened,Unresolved";


           // var targetGroups = new List<string> { "PatientServicesGroup", "HealthcareProviderGroup" };

            await _hubContext.Clients.Groups("PatientServicesGroup")
                .SendAsync("ReceiveComplaintToast", "ComplaintReopened", $"Complaint #{id} has been Reopened");

        }

        _context.Complaints.Update(complaint);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = id });
    }

   
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Report(int id)
    {
        var complaint = await _context.Complaints.FindAsync(id);
        if (complaint == null) return NotFound();

        string? staffName = "No staff assigned yet";
        if (!string.IsNullOrEmpty(complaint.AssignedStaffId))
        {
            var assignedUser = await _userManager.FindByIdAsync(complaint.AssignedStaffId);
            if (assignedUser != null)
            {
                staffName = assignedUser.UserName;
            }
        }

        var viewModel = new ComplaintViewModel
        {
            Id = complaint.Id,
            PatientName = complaint.PatientName,
            PhoneNumber = complaint.PhoneNumber,
            DateOfBirth = complaint.DateOfBirth,
            Job = complaint.Job,
            Nationality = complaint.Nationality,
            Email = complaint.Email,
            ComplaintType = complaint.ComplaintType,
            ComplaintLocation = complaint.ComplaintLocation,
            ComplaintSummary = complaint.ComplaintSummary,
            Status = complaint.Status,
            IsSatisfied = complaint.IsSatisfied,
            AssignedStaffName = staffName,
            CreatedAt = complaint.CreatedAt,

            AllRepliesList = !string.IsNullOrEmpty(complaint.StaffReply)
                ? complaint.StaffReply.Split(new[] { "|||" }, StringSplitOptions.RemoveEmptyEntries).ToList()
                : new List<string>()
        };

        return View(viewModel);
    }



}
