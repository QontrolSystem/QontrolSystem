using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QontrolSystem.Data;
using QontrolSystem.Models;

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
        public IActionResult Create(Department department)
        {
            if (!ModelState.IsValid) return View(department);
            _context.Departments.Add(department);
            _context.SaveChanges();
            return RedirectToAction("Index");
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
    }

}
