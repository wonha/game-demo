using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Server.DB
{
	public class AppDbContext : DbContext
	{
		public DbSet<AccountDb> Accounts { get; set; }
		public DbSet<PlayerDb> Players { get; set; }
		public DbSet<ItemDb> Items { get; set; }

		static readonly ILoggerFactory _logger = LoggerFactory.Create(builder => { builder.AddConsole(); });

		private string _connectionString = @"Data Source=[DATABASE_IP];Initial Catalog=GameDB;Encrypt=False;User Id=sqlserver;Password=sqlserver;";

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options
				.UseLoggerFactory(_logger)
				.UseSqlServer(_connectionString);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<AccountDb>()
				.HasIndex(a => a.AccountName)
				.IsUnique();

			builder.Entity<PlayerDb>()
				.HasIndex(p => p.PlayerName)
				.IsUnique();
		}
	}
}
