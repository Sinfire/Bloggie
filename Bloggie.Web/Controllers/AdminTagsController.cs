﻿using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
	public class AdminTagsController : Controller
	{
        private readonly BloggieDBContext bloggieDbContext;

        public AdminTagsController(BloggieDBContext bloggieDbContext)
        {
            this.bloggieDbContext = bloggieDbContext;
        }


        [HttpGet]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[ActionName("Add")]
		public IActionResult Add(AddTagRequest addTagRequest)
		{
			// Mapping AddTagRequest to Tag domain model
			var tag = new Tag
			{
				Name = addTagRequest.Name,
				DisplayName = addTagRequest.DisplayName
			};

			bloggieDbContext.Tags.Add(tag);
			bloggieDbContext.SaveChanges();

			return RedirectToAction("List");
		}

		[HttpGet]
		[ActionName("List")]
        public IActionResult List()
		{
			// Use dbContext to read the tags

			var tags = bloggieDbContext.Tags.ToList();


			return View(tags);
		}

		[ActionName("Edit")]
		public IActionResult Edit()
		{
			return View();
		}

	}
}