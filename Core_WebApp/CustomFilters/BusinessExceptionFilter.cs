using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core_WebApp.CustomFilters
{
    public class BusinessExceptionFilter : ExceptionFilterAttribute
    {
        private readonly IModelMetadataProvider modelMetadata;

        /// <summary>
        /// Inject IModelMetadataProvider, this will read
        /// the Current Model Medata in Http request
        /// and will used for Model Processing
        /// Model Binding with view to Read/Write values.
        /// This object will be resilved by ASP.NET Core
        /// MvcOptions class used in 
        /// services.AddControllersWithViews() method
        /// </summary>
        /// <param name="modelMetadata"></param>
        public BusinessExceptionFilter(IModelMetadataProvider modelMetadata)
        {
            this.modelMetadata = modelMetadata;
        }
        public override void OnException(ExceptionContext context)
        {
            // Handle the exception
            context.ExceptionHandled = true;
            // read the exception message
            string message = context.Exception.Message;
            // decide the Result that will be rendered
            // 1. Create a Viewresult object
            var viewResult = new ViewResult();
            // 2. Set the ViewName that will be rendered
            viewResult.ViewName = "CustomError";

            // 3. Define a ViewDataDictionary object
            // this will pass necessary data to View
            var viewDataDict = new ViewDataDictionary(modelMetadata,context.ModelState);
            viewDataDict["controller"] = context.RouteData.Values["controller"].ToString();
            viewDataDict["action"] = context.RouteData.Values["action"].ToString();
            viewDataDict["errorMessage"] = message;
            viewResult.ViewData = viewDataDict;
            context.Result = viewResult;
        }
    }
}
