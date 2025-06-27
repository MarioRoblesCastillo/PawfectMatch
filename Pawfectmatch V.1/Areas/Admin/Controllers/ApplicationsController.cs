using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Pawfectmatch_V._1.Models;
using Pawfectmatch_V._1.Data;
using Rotativa.AspNetCore;

namespace Pawfectmatch_V._1.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Admin/Applications/Requests
        public async Task<IActionResult> Requests(string status = null, int page = 1)
        {
            int pageSize = 10;

            if (string.IsNullOrEmpty(status))
                status = "To Review";

            var query = _context.AdoptionApplications
                .Include(a => a.Pet)
                .Include(a => a.Admin)
                .AsQueryable();

            // Filter by status
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(a => a.Status == status);
            }

            // Notification badge: count of pending applications
            ViewBag.PendingCount = await _context.AdoptionApplications.CountAsync(a => a.Status == "To Review");

            // Order by submission date (newest first)
            query = query.OrderByDescending(a => a.SubmittedAt);

            var applications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;

            return View(applications);
        }

        // GET: Admin/Applications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var application = await _context.AdoptionApplications
                .Include(a => a.Pet)
                .Include(a => a.Admin)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            return View(application);
        }

        // POST: Admin/Applications/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus, string? adminNotes = null)
        {
            var application = await _context.AdoptionApplications
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            // Update application status
            application.Status = newStatus;
            application.AdminId = currentUser.Id;
            application.UpdatedAt = DateTime.Now;

            // If approved, update pet status to adopted
            if (newStatus == "Approved" && application.Pet != null)
            {
                application.Pet.Status = "Adopted";
                application.Pet.DateOfRelease = DateTime.Now;
            }

            // If declined, make sure pet is available again
            if (newStatus == "Declined" && application.Pet != null && application.Pet.Status == "Adopted")
            {
                application.Pet.Status = "Available";
                application.Pet.DateOfRelease = null;
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Application status updated to {newStatus} successfully!";
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: Admin/Applications/BulkUpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdateStatus(List<int> selectedIds, string bulkAction)
        {
            if (selectedIds == null || !selectedIds.Any() || string.IsNullOrEmpty(bulkAction))
            {
                TempData["ErrorMessage"] = "Please select applications and an action.";
                return RedirectToAction(nameof(Requests));
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            int updated = 0, deleted = 0;
            var applications = await _context.AdoptionApplications
                .Include(a => a.Pet)
                .Where(a => selectedIds.Contains(a.Id))
                .ToListAsync();

            foreach (var application in applications)
            {
                if (bulkAction == "Delete")
                {
                    _context.AdoptionApplications.Remove(application);
                    deleted++;
                }
                else if (bulkAction == "Approve" || bulkAction == "Declined")
                {
                    application.Status = bulkAction;
                    application.AdminId = currentUser.Id;
                    application.UpdatedAt = DateTime.Now;

                    if (bulkAction == "Approve" && application.Pet != null)
                    {
                        application.Pet.Status = "Adopted";
                        application.Pet.DateOfRelease = DateTime.Now;
                    }
                    if (bulkAction == "Declined" && application.Pet != null && application.Pet.Status == "Adopted")
                    {
                        application.Pet.Status = "Available";
                        application.Pet.DateOfRelease = null;
                    }
                    updated++;
                }
            }

            await _context.SaveChangesAsync();

            if (deleted > 0 && updated > 0)
                TempData["SuccessMessage"] = $"{updated} applications updated, {deleted} deleted.";
            else if (deleted > 0)
                TempData["SuccessMessage"] = $"{deleted} applications deleted.";
            else if (updated > 0)
                TempData["SuccessMessage"] = $"{updated} applications updated.";
            else
                TempData["ErrorMessage"] = "No applications were updated.";

            return RedirectToAction(nameof(Requests));
        }

        // GET: Admin/Applications/Form
        public IActionResult Form()
        {
            return View();
        }

        // POST: Admin/Applications/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var application = await _context.AdoptionApplications.FindAsync(id);
            if (application == null)
                return NotFound();

            _context.AdoptionApplications.Remove(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Application deleted successfully!";
            return RedirectToAction(nameof(Requests));
        }

        // GET: Admin/Applications/Export
        public async Task<IActionResult> Export(string format = "excel")
        {
            var applications = await _context.AdoptionApplications
                .Include(a => a.Pet)
                .Include(a => a.Admin)
                .OrderByDescending(a => a.SubmittedAt)
                .ToListAsync();

            if (format.ToLower() == "excel")
            {
                return await ExportToExcel(applications);
            }
            else
            {
                return await ExportToPdf(applications);
            }
        }

        private async Task<IActionResult> ExportToExcel(List<AdoptionApplication> applications)
        {
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Adoption Applications");

            // Add headers
            worksheet.Cell("A1").Value = "ID";
            worksheet.Cell("B1").Value = "Applicant Name";
            worksheet.Cell("C1").Value = "Email";
            worksheet.Cell("D1").Value = "Address";
            worksheet.Cell("E1").Value = "Pet Name";
            worksheet.Cell("F1").Value = "Pet Type";
            worksheet.Cell("G1").Value = "Status";
            worksheet.Cell("H1").Value = "Submitted Date";
            worksheet.Cell("I1").Value = "Reviewed By";

            // Add data
            for (int i = 0; i < applications.Count; i++)
            {
                var app = applications[i];
                int row = i + 2;
                worksheet.Cell($"A{row}").Value = app.Id;
                worksheet.Cell($"B{row}").Value = app.ApplicantName;
                worksheet.Cell($"C{row}").Value = app.Email;
                worksheet.Cell($"D{row}").Value = app.Address;
                worksheet.Cell($"E{row}").Value = app.Pet?.Name ?? "N/A";
                worksheet.Cell($"F{row}").Value = app.Pet?.PetType ?? "N/A";
                worksheet.Cell($"G{row}").Value = app.Status;
                worksheet.Cell($"H{row}").Value = app.SubmittedAt.ToString("yyyy-MM-dd HH:mm");
                worksheet.Cell($"I{row}").Value = app.Admin?.UserName ?? "N/A";
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Position = 0;

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "adoption_applications.xlsx");
        }

        private async Task<IActionResult> ExportToPdf(List<AdoptionApplication> applications)
        {
            // This would require a PDF library like iTextSharp or similar
            // For now, return a simple text response
            var content = "Adoption Applications Report\n\n";
            foreach (var app in applications)
            {
                content += $"ID: {app.Id}\n";
                content += $"Applicant: {app.ApplicantName}\n";
                content += $"Email: {app.Email}\n";
                content += $"Pet: {app.Pet?.Name ?? "N/A"}\n";
                content += $"Status: {app.Status}\n";
                content += $"Submitted: {app.SubmittedAt:yyyy-MM-dd HH:mm}\n";
                content += "---\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(content);
            return File(bytes, "text/plain", "adoption_applications.txt");
        }

        // GET: Admin/Applications/Certificate/{id}
        public async Task<IActionResult> Certificate(int id)
        {
            var application = await _context.AdoptionApplications
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null || application.Status != "Approved")
                return NotFound();

            return new ViewAsPdf("Certificate", application)
            {
                FileName = $"AdoptionCertificate_{application.Pet?.Name}_{application.ApplicantName}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }
    }
}
