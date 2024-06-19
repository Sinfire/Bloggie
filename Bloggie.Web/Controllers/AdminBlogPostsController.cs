using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Runtime.InteropServices;
using Bloggie.Web.Models.Domain;

namespace Bloggie.Web.Controllers
{
	public class AdminBlogPostsController : Controller
	{
		private readonly iTagRepository tagRepository;
		private readonly IBlogPostRepository blogPostRepository;

		public AdminBlogPostsController(iTagRepository tagRepository, IBlogPostRepository blogPostRepository)
		{
			this.tagRepository = tagRepository;
			this.blogPostRepository = blogPostRepository;
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{

			// Get tags from Repository
			var tags = await tagRepository.GetAllAsync();

			var model = new AddBlogPostRequest
			{
				Tags = tags.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Add(AddBlogPostRequest addBlogPostRequest)
		{
			// Map view model to Doman model

			var blogPost = new BlogPost
			{
				Heading = addBlogPostRequest.Heading,
				PageTitle = addBlogPostRequest.PageTitle,
				Content = addBlogPostRequest.Content,
				ShortDescription = addBlogPostRequest.ShortDescription,
				FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
				UrlHandle = addBlogPostRequest.UrlHandle,
				PublishedDate = addBlogPostRequest.PublishedDate,
				Author = addBlogPostRequest.Author,
				Visible = addBlogPostRequest.Visible,
			};

			// Map Tags from selected tags

			var selectedTags = new List<Tag>();

			foreach (var selectedTagId in addBlogPostRequest.SelectedTags)
			{
				var selectedTagIdAsGuid = Guid.Parse(selectedTagId);
				var existingTag = await tagRepository.GetAsync(selectedTagIdAsGuid);

				if (existingTag != null)
				{
					selectedTags.Add(existingTag);
				}
			}

			// Map back to Domain model
			blogPost.Tags = selectedTags;

			await blogPostRepository.AddAsync(blogPost);

			return RedirectToAction("Add");
		}

		[HttpGet]
		public async Task<IActionResult> List()
		{
			// Call the repository
			var blogPosts = await blogPostRepository.GetAllAsync();



			return View(blogPosts);
		}

		[HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			// Retrieve the result from the repository
			var blogPost = await blogPostRepository.GetAsync(id);
			var tagsDomainModel = await tagRepository.GetAllAsync();

			if (blogPost != null)
			{


				// Map Domain Model to View Model
				var model = new EditBlogPostRequest
				{
					Id = blogPost.Id,
					Heading = blogPost.Heading,
					PageTitle = blogPost.PageTitle,
					Content = blogPost.Content,
					Author = blogPost.Author,
					FeaturedImageUrl = blogPost.FeaturedImageUrl,
					UrlHandle = blogPost.UrlHandle,
					ShortDescription = blogPost.ShortDescription,
					PublishedDate = blogPost.PublishedDate,
					Visible = blogPost.Visible,
					Tags = tagsDomainModel.Select(x => new SelectListItem
					{
						Text = x.Name,
						Value = x.Id.ToString()
					}),
					SelectedTags = blogPost.Tags.Select(x => x.Id.ToString()).ToArray()
				};
				return View(model);
			}
			// pass data to view
			return View(null);
		}

		[HttpPost]
		public async Task<IActionResult> Edit(EditBlogPostRequest editBlogPostRequest)
		{
			// Map view Model back to Domain Model
			var blogPostDomainModel = new BlogPost
			{
				Id = editBlogPostRequest.Id,
				Heading = editBlogPostRequest.Heading,
				PageTitle = editBlogPostRequest.PageTitle,
				Content = editBlogPostRequest.Content,
				Author = editBlogPostRequest.Author,
				ShortDescription = editBlogPostRequest.ShortDescription,
				FeaturedImageUrl = editBlogPostRequest.FeaturedImageUrl,
				PublishedDate = editBlogPostRequest.PublishedDate,
				UrlHandle = editBlogPostRequest.UrlHandle,
				Visible = editBlogPostRequest.Visible,
			};

			// Map tags into Domain Model
			var selectedTags = new List<Tag>();
			foreach (var selectedTag in editBlogPostRequest.SelectedTags)
			{
				if (Guid.TryParse(selectedTag, out var tag))
				{
					var foundTag = await tagRepository.GetAsync(tag);

					if(foundTag != null)
					{
						selectedTags.Add(foundTag);
					}
				}
			}

			blogPostDomainModel.Tags = selectedTags;

			// Submit information to Repository to Update
			var updatedBlog = await blogPostRepository.UpdateAsync(blogPostDomainModel);

			if (updatedBlog != null)
			{
				// Show success Notification
				return RedirectToAction("Edit");
			}

			// Show error notification
			return RedirectToAction("Edit");
		}

		[HttpPost]
		public async Task<IActionResult> Delete(EditBlogPostRequest editBlogPostRequest)
		{
			// Talk to Repository to delete this blog post
			var deletedBlogPost = await blogPostRepository.DeleteAsync(editBlogPostRequest.Id);

			if (deletedBlogPost != null)
			{
				// Show success Notification
				return RedirectToAction("List");
			}
			// Show error Notification
			return RedirectToAction("Edit", new { id = editBlogPostRequest.Id });

			// Display response
		}
	}
}
