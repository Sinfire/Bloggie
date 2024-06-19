using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
	public class BlogPostRepository : IBlogPostRepository
	{
		private readonly BloggieDBContext bloggieDBContext;

		public BlogPostRepository(BloggieDBContext bloggieDBContext)
        {
			this.bloggieDBContext = bloggieDBContext;
		}
        public async Task<BlogPost> AddAsync(BlogPost blogPost)
		{
			await bloggieDBContext.AddAsync(blogPost);
			await bloggieDBContext.SaveChangesAsync();
			return blogPost;
		}

		public async Task<BlogPost?> DeleteAsync(Guid id)
		{
			var existingBlog = await bloggieDBContext.BlogPosts.FindAsync(id);

			if (existingBlog != null)
			{
				bloggieDBContext.BlogPosts.Remove(existingBlog);
				await bloggieDBContext.SaveChangesAsync();
				return existingBlog;
			}
			return null;
		}

		public async Task<IEnumerable<BlogPost>> GetAllAsync()
		{
			return await bloggieDBContext.BlogPosts.Include(x => x.Tags).ToListAsync();
		}

		public async Task<BlogPost?> GetAsync(Guid id)
		{
			return await bloggieDBContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync( x => x.Id == id);
		}

		public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
		{
			return await bloggieDBContext.BlogPosts.Include( x => x.Tags).FirstOrDefaultAsync( x=> x.UrlHandle == urlHandle );

		}

		public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
		{
			var existingBlog = await bloggieDBContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(x => x.Id == blogPost.Id);

			if (existingBlog != null) {
				existingBlog.Id = blogPost.Id;
				existingBlog.Heading = blogPost.Heading;
				existingBlog.PageTitle = blogPost.PageTitle;
				existingBlog.Content = blogPost.Content;
				existingBlog.ShortDescription = blogPost.ShortDescription;
				existingBlog.Author = blogPost.Author;
				existingBlog.FeaturedImageUrl = blogPost.FeaturedImageUrl;
				existingBlog.UrlHandle = blogPost.UrlHandle;
				existingBlog.Visible = blogPost.Visible;
				existingBlog.PublishedDate = blogPost.PublishedDate;
				existingBlog.Tags = blogPost.Tags;

				await bloggieDBContext.SaveChangesAsync();
				return existingBlog;
			}
			return null;
		}
	}
}
