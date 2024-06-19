using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Data
{
	public class AuthDbContext : IdentityDbContext
	{
		public AuthDbContext(DbContextOptions options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			// Seed Roles ( User, Admin, SuperAdmin)

			var adminRoleId			= "c6f5fed0-f499-48b0-9e83-91ef8bd5ef88";
			var superAdminRoleId	= "781f5dd8-59e1-4d79-9fed-ab7121b5a807";
			var userRoleId			= "4b9dfd69-d0a6-4630-b604-70477155f625";



			var roles = new List<IdentityRole>
			{
				new IdentityRole
				{
					Name = "Admin",
					NormalizedName = "Admin",
					Id = adminRoleId,
					ConcurrencyStamp = adminRoleId
				},
				new IdentityRole
				{
					Name = "SuperAdmin",
					NormalizedName = "SuperAdmin",
					Id = superAdminRoleId,
					ConcurrencyStamp = superAdminRoleId
				},
				new IdentityRole
				{
					Name = "User",
					NormalizedName = "User",
					Id = userRoleId,
					ConcurrencyStamp = userRoleId
				}
			};

			builder.Entity<IdentityRole>().HasData(Roles);


			// Seed SuperAdminUser

			var superAdminId = "ecce1477-8457-455d-a48d-fa9aca606941";
			
			var superAdminUser = new IdentityUser
			{
				UserName = "superadmin@bloggie.com",
				Email = "superadmin@bloggie.com",
				NormalizedEmail = "superadmin@bloggie.com".ToUpper(),
				NormalizedUserName = "superadmin@bloggie.com".ToUpper(),
				Id = superAdminId
			};

			superAdminUser.PasswordHash = new PasswordHasher<IdentityUser>()
				.HashPassword(superAdminUser, "Superadmin123");

			builder.Entity<IdentityUser>().HasData(superAdminUser);

			// Add all roles to SuperAdmin User

			var superAdminRoles = new List<IdentityUserRole<string>>
			{
				new IdentityUserRole<string>
				{
					RoleId = adminRoleId,
					UserId = superAdminId
				},
				new IdentityUserRole<string>
				{
					RoleId = superAdminRoleId,
					UserId = superAdminId
				},
				new IdentityUserRole<string>
				{
					RoleId = userRoleId,
					UserId = superAdminId
				}
			};

			builder.Entity<IdentityUserRole<string>>().HasData(superAdminRoles);
		}
	}
}
