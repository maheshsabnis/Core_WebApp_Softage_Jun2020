using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core_WebApp.Models;
using Core_WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Core_WebApp.Controllers
{
    /// <summary>
    /// Contains action methods (methods those will be executed over HttpRequest)
    /// ActionMethos can be either HttpGet (Default) or HttpPost(HttpPut/HttpDelete)
    /// ActionMethod Retuens IActionResult interface
    /// </summary>
    public class ProductController : Controller
    {
        private readonly IRepository<Product, int> prdRepository;
        private readonly IRepository<Category, int> catRepository;
        /// <summary>
        /// Inject the ProductRepository as constructor injection
        /// </summary>
        public ProductController(IRepository<Product, int> prdRepository,
             IRepository<Category, int> catRepository)
        {
            this.prdRepository = prdRepository;
            this.catRepository = catRepository;
        }

        public async Task<IActionResult> Index()
        {
            var prds = await prdRepository.GetAsync();
            return View(prds);
        }

        public async Task<IActionResult> Create()
        {
            var prd = new Product();
            // define a ViewBag that will pass the List of
            // Categories to the Create View
            ViewBag.CategoryRowId = 
                new SelectList(await catRepository.GetAsync(),
                "CategoryRowId", 
                "CategoryName");

            return View(prd);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product Product)
        {
            // check if the Product Posted Model is valid
            if (ModelState.IsValid)
            {
                // create a new Product Record
                Product = await prdRepository.CraeteAsync(Product);
                // redirect to Index Page
                return RedirectToAction("Index");
            }
            else
            {
                // else stey on the same page with errors
                return View(Product);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            // search the record being edited
            var prd = await prdRepository.GetAsync(id);
            // return a view with record being edited
            return View(prd);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product Product)
        {
            // check if the Product Posted Model is valid
            if (ModelState.IsValid)
            {
                // update the Product Record
                Product = await prdRepository.UpdateAsync(id,Product);
                // redirect to Index Page
                return RedirectToAction("Index");
            }
            else
            {
                // else stey on the same page with errors
                return View(Product);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            // search the record being edited
            var res = await prdRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
