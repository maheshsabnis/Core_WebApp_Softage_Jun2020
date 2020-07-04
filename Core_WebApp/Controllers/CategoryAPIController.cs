using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Core_WebApp.Models;
using Core_WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Core_WebApp.Controllers
{

    // WEB API Controller contains all HTTP Action Methods
    // Each Http Action method must return Http Status Result(?)
    // Each method is be defualt is HTTP-GET Request request
    // To define the Http Type for methods in API apply 
    // HTTP Method Attributes on these methods so that they are mapped with
    // Http Request Type (GET / POST/  PUT  /DELETE)
    // HttpGet / HttpGet("{<TEMPLATE-STRING>}")
    // HttpPost / HttpPost("{TEMPLATE-ASTRING}") 
    // HttpPut("{TEMPLATE-ASTRING}") 
    // HttpDelete("{TEMPLATE-ASTRING}")

    // Route --> Used to define the Http Endpoint Routing
    // to access HTTP Action methods from API controller class
    // HTTP Action Methods --> GET/POST/PUT/DELETE
    // http://localhost:5000/api/CategoryAPI
    [Route("api/[controller]")]
    // ApiController --> Class used to read Data from HTTP POST/PUT
    // request body and map with CLR Object
    // The data to be created (post) / updated (put) can be send in 
    // HTTP Headers
    // HTTP Routing
    // HTTP Form Post
    [ApiController]
    public class CategoryAPIController : ControllerBase
    {
        private readonly IRepository<Category, int> catRepo;

        public CategoryAPIController(IRepository<Category, int> catRepo)
        {
            this.catRepo = catRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var cats = await catRepo.GetAsync();
            return Ok(cats); // serializde the response in JSON
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var cat = await catRepo.GetAsync(id);
            return Ok(cat); // serializde the response in JSON
        }
        //[HttpPost("{categoryId}/{categoryName}/{basePrice}")]
        // public async Task<IActionResult> Post([FromBody]Category cat)
        //  public async Task<IActionResult> Post(string categoryId, string categoryName, int basePrice)
        // public async Task<IActionResult> Post([FromQuery] Category cat)

        //public async Task<IActionResult> Post([FromRoute] Category cat)
        [HttpPost]
        //public async Task<IActionResult> Post([FromForm] Category cat)
        public async Task<IActionResult> Post(Category cat)
        {
            //var cat = new Category()
            //{
            //     CategoryId = categoryId,
            //     CategoryName = categoryName,
            //     BasePrice = basePrice
            //};
            if (ModelState.IsValid)
            {
                if (cat.BasePrice < 0)
                    throw new Exception("Please check Price");
                 cat = await catRepo.CraeteAsync(cat);
                return Ok(cat); // serializde the response in JSON
            }
            return BadRequest(cat);
        }
        // id (premptive type) will always send through Http request header
        // Complex-Type is always send through Http request Body
         [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Category cat)
        {
            if (ModelState.IsValid)
            {
                cat = await catRepo.UpdateAsync(id,cat);
                return Ok(cat); // serializde the response in JSON
            }
            return BadRequest(cat);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var cat = await catRepo.DeleteAsync(id);
            return Ok(cat); // serializde the response in JSON
        }
    }
}
