using Microsoft.EntityFrameworkCore;
using MyApp.Entities;
using System.Collections.Generic;

namespace MyApp.Data
{
	public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
	{
		public DbSet<User> Users { get; set; }
	}
}
