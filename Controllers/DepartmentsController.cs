using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models.Accounts;

namespace QontrolSystem.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly AppDbContext _context;

        public DepartmentsController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string search, int page = 1, int pageSize = 10)
        {
            var query = _context.Departments
                .Include(d => d.Users)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.DepartmentName.Contains(search));
            }

            var totalItems = query.Count();
            var departments = query
                .OrderBy(d => d.DepartmentName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.Search = search;

            return View(departments);
        }

        public IActionResult Create() => View();

        
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Department department)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is NOT valid. Errors:");

                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Field: {key} — Error: {error.ErrorMessage}");
                    }
                }

                return View(department);
            }

            else
            {
                bool departmentExists = _context.Departments
     .Any(d => d.DepartmentName.Trim().ToLower() == department.DepartmentName.Trim().ToLower());


                if (departmentExists)
                {
                    // Add a validation error to ModelState so it displays in the view
                    ModelState.AddModelError("DepartmentName", "A department with this name already exists.");
                    return View(department);
                }

                // ✅ Add new department
                _context.Departments.Add(department);
                _context.SaveChanges();

                return RedirectToAction("Index", "Loading", new
                {
                    returnUrl = Url.Action("Index", "Departments"),
                    duration = 3000,
                    message = "Creating department",
                });
            }
                
        }

        public IActionResult Edit(int id)
        {
            var dept = _context.Departments.Find(id);
            if (dept == null) return NotFound();
            return View(dept);
        }

        [HttpPost]
        public IActionResult Edit(Department department)
        {
            if (!ModelState.IsValid) return View(department);
            _context.Departments.Update(department);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var dept = _context.Departments.Find(id);
            if (dept == null) return NotFound();
            _context.Departments.Remove(dept);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Departments/SubDepartments/5
        public async Task<IActionResult> SubDepartments(int? departmentId)
        {
            if (departmentId == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentID == departmentId);

            if (department == null)
            {
                return NotFound();
            }

            var subDepartments = await _context.ITSubDepartments
                .Where(sd => sd.DepartmentID == departmentId)
                .ToListAsync();

            ViewBag.DepartmentName = department.DepartmentName;
            ViewBag.DepartmentId = departmentId;

            return View(subDepartments);
        }

        public async Task<IActionResult> CreateSubDepartment(int? departmentId)
        {
            if (departmentId == null)
                return NotFound();

            var department = await _context.Departments.FindAsync(departmentId);
            if (department == null)
                return NotFound();

            ViewBag.DepartmentName = department.DepartmentName;
            ViewBag.DepartmentId = departmentId;

            var SubDepartment = new ITSubDepartmentCreateViewModel
            {
                DepartmentID = departmentId.Value
            };

            return View(SubDepartment);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSubDepartment(ITSubDepartmentCreateViewModel SubDepartment)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicates
                var existingSubDept = await _context.ITSubDepartments
                    .FirstOrDefaultAsync(sd =>
                        sd.SubDepartmentName.ToLower() == SubDepartment.SubDepartmentName.ToLower()
                        && sd.DepartmentID == SubDepartment.DepartmentID);

                if (existingSubDept != null)
                {
                    ModelState.AddModelError("SubDepartmentName", "A sub-department with this name already exists.");
                    var dept = await _context.Departments.FindAsync(SubDepartment.DepartmentID);
                    ViewBag.DepartmentName = dept?.DepartmentName;
                    ViewBag.DepartmentId = SubDepartment.DepartmentID;
                    return View(SubDepartment);
                }

                // Map ViewModel to Entity
                var subDepartment = new ITSubDepartment
                {
                    SubDepartmentName = SubDepartment.SubDepartmentName,
                    DepartmentID = SubDepartment.DepartmentID
                };

                _context.ITSubDepartments.Add(subDepartment);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sub-department created successfully!";
                return RedirectToAction("SubDepartments", new { departmentId = SubDepartment.DepartmentID });
            }

            var department = await _context.Departments.FindAsync(SubDepartment.DepartmentID);
            ViewBag.DepartmentName = department?.DepartmentName;
            ViewBag.DepartmentId = SubDepartment.DepartmentID;
            return View(SubDepartment);
        }


        // GET: Departments/EditSubDepartment/5
        public async Task<IActionResult> EditSubDepartment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subDepartment = await _context.ITSubDepartments
                .Include(sd => sd.Department)
                .FirstOrDefaultAsync(sd => sd.ITSubDepartmentID == id);

            if (subDepartment == null)
            {
                return NotFound();
            }

            ViewBag.DepartmentName = subDepartment.Department?.DepartmentName;
            return View(subDepartment);
        }

        // POST: Departments/EditSubDepartment/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSubDepartment(int id, [Bind("ITSubDepartmentID,SubDepartmentName,DepartmentID")] ITSubDepartment subDepartment)
        {
            if (id != subDepartment.ITSubDepartmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Check if sub-department name already exists (excluding current one)
                    var existingSubDept = await _context.ITSubDepartments
                        .FirstOrDefaultAsync(sd => sd.SubDepartmentName.ToLower() == subDepartment.SubDepartmentName.ToLower()
                                           && sd.DepartmentID == subDepartment.DepartmentID
                                           && sd.ITSubDepartmentID != subDepartment.ITSubDepartmentID);

                    if (existingSubDept != null)
                    {
                        ModelState.AddModelError("SubDepartmentName", "A sub-department with this name already exists.");
                        var dept = await _context.Departments.FindAsync(subDepartment.DepartmentID);
                        ViewBag.DepartmentName = dept?.DepartmentName;
                        return View(subDepartment);
                    }

                    _context.Update(subDepartment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Sub-department updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ITSubDepartmentExists(subDepartment.ITSubDepartmentID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var department = await _context.Departments.FindAsync(subDepartment.DepartmentID);
            ViewBag.DepartmentName = department?.DepartmentName;
            return View(subDepartment);
        }

        // GET: Departments/DeleteSubDepartment/5
        public async Task<IActionResult> DeleteSubDepartment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subDepartment = await _context.ITSubDepartments
                .Include(sd => sd.Department)
                .FirstOrDefaultAsync(sd => sd.ITSubDepartmentID == id);

            if (subDepartment == null)
            {
                return NotFound();
            }

            return View(subDepartment);
        }

        // POST: Departments/DeleteSubDepartment/5
        [HttpPost, ActionName("DeleteSubDepartment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubDepartmentConfirmed(int id)
        {
            var subDepartment = await _context.ITSubDepartments.FindAsync(id);
            if (subDepartment != null)
            {
                _context.ITSubDepartments.Remove(subDepartment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Sub-department deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // Helper methods
        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentID == id);
        }

        private bool ITSubDepartmentExists(int id)
        {
            return _context.ITSubDepartments.Any(e => e.ITSubDepartmentID == id);
        }
    }
}
    