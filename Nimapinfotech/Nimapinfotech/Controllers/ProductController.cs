using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nimapinfotech.Models;

namespace Nimapinfotech.Controllers
{
    public class ProductController : Controller
    {

        private readonly NimapDbContext productDb;

        public ProductController(NimapDbContext productDb)
        {
            this.productDb = productDb;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 12)
        {
            var productsQuery = productDb.Products
                .Include(p => p.Category)
                .OrderBy(p => p.ProductId);

            var totalRecords = await productsQuery.CountAsync();
            Console.WriteLine($"Total Records: {totalRecords}");

            int skip = Math.Max(0, (page - 1) * pageSize);
            Console.WriteLine($"Skipping {skip} records, Taking {pageSize} records");

         
            var products = await productsQuery
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            Console.WriteLine($"Fetched Records: {products.Count}");

            var paginatedResult = new PaginatedList<Product>(products, totalRecords, page, pageSize);

            return View(paginatedResult);
        }




        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(await productDb.Categories.ToListAsync(), "CategoryId", "CategoryName");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, string categoryName)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryId = new SelectList(await productDb.Categories.ToListAsync(), "CategoryId", "CategoryName", product.CategoryId);
                return View(product);
            }

            Console.WriteLine($"Product Name: {product.ProductName}, Category Name: {categoryName}");

            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                var existingCategory = await productDb.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());

                if (existingCategory == null)
                {
                    var newCategory = new Category { CategoryName = categoryName };
                    productDb.Categories.Add(newCategory);
                    await productDb.SaveChangesAsync();

                    product.CategoryId = newCategory.CategoryId;
                }
                else
                {
                    product.CategoryId = existingCategory.CategoryId;
                }
            }

            Console.WriteLine($"Final Product Details: {product.ProductName}, CategoryId: {product.CategoryId}");

            productDb.Products.Add(product);
            await productDb.SaveChangesAsync();

            TempData["insert_success"] = "Product created successfully.";
            return RedirectToAction("Index");
        }




        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || productDb.Products == null)
            {
                return NotFound();
            }

            var prdData = await productDb.Products
                .Include(p => p.Category) 
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (prdData == null)
            {
                return NotFound();
            }

            return View(prdData);
        }




        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || productDb.Products == null)
            {
                return NotFound();
            }

            var prdData = await productDb.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (prdData == null)
            {
                return NotFound();
            }

            ViewBag.CategoryId = new SelectList(productDb.Categories, "CategoryId", "CategoryName", prdData.CategoryId);

            return View(prdData);
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Product prd, string categoryName)
        {
            if (id != prd.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var category = await productDb.Categories
                        .FirstOrDefaultAsync(c => c.CategoryId == prd.CategoryId);

                    if (category != null && !string.IsNullOrEmpty(categoryName))
                    {
                        category.CategoryName = categoryName;
                        productDb.Categories.Update(category);
                    }

                    productDb.Products.Update(prd);

                    await productDb.SaveChangesAsync();

                    TempData["update_success"] = "Product and Category updated successfully.";
                    return RedirectToAction("Index", "Product");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(prd.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.CategoryId = new SelectList(productDb.Categories, "CategoryId", "CategoryName", prd.CategoryId);

            return View(prd);
        }

        private bool ProductExists(int id)
        {
            return productDb.Products.Any(e => e.ProductId == id);
        }






        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || productDb.Products == null)
            {
                return NotFound();
            }

            var prdData = await productDb.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(x => x.ProductId == id);

            if (prdData == null)
            {
                return NotFound();
            }

            return View(prdData);
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prdData = await productDb.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (prdData != null)
            {
                productDb.Products.Remove(prdData);

                await productDb.SaveChangesAsync();

                TempData["delete_success"] = "Product deleted successfully.";
            }

            return RedirectToAction("Index", "Product");
        }

    }
}
