﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core_WebApp.CustomFilters;
using Core_WebApp.CustomSessions;
using Core_WebApp.Models;
using Core_WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core_WebApp.Controllers
{
    /// <summary>
    /// Contains action methods (methods those will be executed over HttpRequest)
    /// ActionMethos can be either HttpGet (Default) or HttpPost(HttpPut/HttpDelete)
    /// ActionMethod Retuens IActionResult interface
    /// </summary>
    /// 
    // applying filter at controller level
    //[LogFilter]

    
    public class CategoryController : Controller
    {
        private readonly IRepository<Category, int> catRepository;
        /// <summary>
        /// Inject the CategoryRepository as constructor injection
        /// </summary>
        public CategoryController(IRepository<Category, int> catRepository)
        {
            this.catRepository = catRepository;
        }

        // custom filter applied at action level
       // [LogFilter] 

//        [Authorize(Roles = "Manager,Clerk,Operator")]
        [Authorize(Policy = "readpolicy")]
        public async Task<IActionResult> Index()
        {
            var cats = await catRepository.GetAsync();
            return View(cats);
        }
        //   [Authorize(Roles = "Manager,Clerk")]
        [Authorize(Policy = "writepolicy")]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            //try
            //{
                // check if the Cateogry Posted Model is valid
                if (ModelState.IsValid)
                {
                    if (category.BasePrice < 0)
                        throw new Exception("Base Price cannot ne -ve");
                    // create a new Category Record
                    category = await catRepository.CraeteAsync(category);
                    // redirect to Index Page
                    return RedirectToAction("Index");
                }
                else
                {
                    // else stey on the same page with errors
                    return View(category);
                }
            //}
            //catch (Exception ex)
            //{
            //    return View("Error", new ErrorViewModel()
            //    { 
            //       ControllerName =  this.RouteData.Values["controller"].ToString(),
            //       ActionName = this.RouteData.Values["action"].ToString(),
            //       ExceptionMessage = ex.Message
            //    });
            //}
        }

        public async Task<IActionResult> Edit(int id)
        {
            // search the record being edited
            var cat = await catRepository.GetAsync(id);
            // return a view with record being edited
            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            // check if the Cateogry Posted Model is valid
            if (ModelState.IsValid)
            {
                // update the Category Record
                category = await catRepository.UpdateAsync(id,category);
                // redirect to Index Page
                return RedirectToAction("Index");
            }
            else
            {
                // else stey on the same page with errors
                return View(category);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            // search the record being edited
            var res = await catRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult ShowProducts(int id)
        {
            HttpContext.Session.SetInt32("CategoryRowId", id);
            var cat = catRepository.GetAsync(id).Result;

            HttpContext.Session.SetSessionData<Category>("Category", cat);
            return RedirectToAction("Index", "Product");
        }

    }
}
