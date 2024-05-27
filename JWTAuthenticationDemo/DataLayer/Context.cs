using Microsoft.EntityFrameworkCore;

namespace JWTAuthenticationDemo.DataLayer
{
	public class Context : DbContext
	{
		public Context(DbContextOptions<Context> options) : base(options) { }
		public DbSet<User> Users { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//this.Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasData(new User
			{
				Id = 1,
				Username = "testuser",
				Password = "12345"
			});
		}


	}
}
