using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nimapinfotech.Models;

namespace Nimapinfotech.Controllers
{
    public class CategoryController : Controller
    {

        private readonly NimapDbContext categoryDb;

        public CategoryController(NimapDbContext categoryDb)
        {
            this.categoryDb = categoryDb;
        }
        public async Task<IActionResult> Index()
        {
            var ctgData = await categoryDb.Categories.ToListAsync();
            return View(ctgData);
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category ctg)
        {
            if (ModelState.IsValid)
            {
                await categoryDb.Categories.AddAsync(ctg);
                await categoryDb.SaveChangesAsync();
                TempData["insert_success"] = "Inserted..";
                return RedirectToAction("Index", "Category");
            }
            return View(ctg);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || categoryDb.Categories == null)
            {
                return NotFound();
            }
            var ctgData = await categoryDb.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);
            if (ctgData == null)
            {
                return NotFound();
            }
            return View(ctgData);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || categoryDb.Categories == null)
            {
                return NotFound();
            }
            var ctgData = await categoryDb.Categories.FindAsync(id);

            if (ctgData == null)
            {
                return NotFound();
            }

            return View(ctgData);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Category ctg)
        {
            if (id != ctg.CategoryId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                categoryDb.Update(ctg);
                await categoryDb.SaveChangesAsync();
                TempData["update_success"] = "Updated..";
                return RedirectToAction("Index", "Category");
            }
            return View(ctg);
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || categoryDb.Categories == null)
            {
                return NotFound();
            }
            var ctgData = await categoryDb.Categories.FirstOrDefaultAsync(x => x.CategoryId == id);

            if (ctgData == null)
            {
                return NotFound();
            }
            return View(ctgData);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var ctgData = await categoryDb.Categories.FindAsync(id);
            if (ctgData != null)
            {
                categoryDb.Categories.Remove(ctgData);
            }
            await categoryDb.SaveChangesAsync();
            TempData["delete_success"] = "Deleted..";
            return RedirectToAction("Index", "Category");
        }
    }
}
