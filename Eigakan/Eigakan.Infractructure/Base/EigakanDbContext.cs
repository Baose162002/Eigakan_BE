using Eigakan.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Eigakan.Infractructure.Base
{
	public class EigakanDbContext : DbContext
	{
		public EigakanDbContext()
		{

		}
		public EigakanDbContext(DbContextOptions<EigakanDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
		public DbSet<SubscriptionPurchase> SubscriptionPurchases { get; set; }
		public DbSet<Movie> Movies { get; set; }
		public DbSet<Genre> Genres { get; set; }
		public DbSet<MovieGenre> MovieGenres { get; set; }
		public DbSet<Person> Persons { get; set; }
		public DbSet<MoviePerson> MoviePersons { get; set; }
		public DbSet<Media> Media { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Contract> Contracts { get; set; }
		public DbSet<AdPurchaseTransaction> AdPurchasesTransaction { get; set; }
		public DbSet<AdPackage> AdPackages { get; set; }
		public DbSet<News> News { get; set; }
        public DbSet<MovieRating> MovieRating { get; set; }
        public DbSet<AdMedia> AdMedia { get; set; }
		public DbSet<MovieHistory> MovieHistories { get; set; }
		public DbSet<UserEarning> UserEarnings { get; set; }
		public DbSet<UserRegister> UserRegisters { get; set; }
		public DbSet<MovieCount> MovieCounts { get; set; }
		public DbSet<AdMediaCount> AdMediaCounts { get; set; }
		public DbSet<AdMedia> AdMedias { get; set; }
		public DbSet<ViewPaymentPolicy> ViewPaymentPolicies { get; set; }
		public DbSet<MovieEarning> MovieEarnings { get; set; }
		public DbSet<WithdrawRequest> WithdrawRequests { get; set; }
		public DbSet<AdPurchaseItems> AdPurchaseItems { get; set; }
		public DbSet<UserWallet> UserWallets { get; set; }
		public DbSet<WalletTransaction> WalletTransactions { get; set; }




		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var configuration = new ConfigurationBuilder()
					.SetBasePath(AppContext.BaseDirectory) // Hoặc .SetBasePath(Directory.GetCurrentDirectory())
					.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
					.Build();

				var connectionString = configuration.GetConnectionString("DefaultConnection");
				optionsBuilder.UseSqlServer(connectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);


			modelBuilder.Entity<MovieGenre>()
				.HasKey(mg => new { mg.MovieId, mg.GenreId });
			modelBuilder.Entity<MovieGenre>()
				.HasOne(mg => mg.Movie)
				.WithMany(m => m.MovieGenres)
				.HasForeignKey(mg => mg.MovieId);
			modelBuilder.Entity<MovieGenre>()
				.HasOne(mg => mg.Genre)
				.WithMany(g => g.MovieGenres)
				.HasForeignKey(mg => mg.GenreId);

			modelBuilder.Entity<MoviePerson>()
				.HasKey(mp => new { mp.MovieId, mp.PersonId });
			modelBuilder.Entity<MoviePerson>()
				.HasOne(mp => mp.Movie)
				.WithMany(m => m.MoviePersons)
				.HasForeignKey(mp => mp.MovieId);
			modelBuilder.Entity<MoviePerson>()
				.HasOne(mp => mp.Person)
				.WithMany(p => p.MoviePersons)
				.HasForeignKey(mp => mp.PersonId);
            
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });
            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId);
            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId);
            modelBuilder.Entity<SubscriptionPurchase>()
			  .HasOne(sp => sp.SubscriptionPackage)
			  .WithMany(s => s.SubscriptionPurchases)
			  .HasForeignKey(sp => sp.SubscriptionId);
            modelBuilder.Entity<MoviePerson>()
                .HasKey(mp => new { mp.MovieId, mp.PersonId });
            modelBuilder.Entity<MoviePerson>()
                .HasOne(mp => mp.Movie)
                .WithMany(m => m.MoviePersons)
                .HasForeignKey(mp => mp.MovieId);
            modelBuilder.Entity<MoviePerson>()
                .HasOne(mp => mp.Person)
                .WithMany(p => p.MoviePersons)
                .HasForeignKey(mp => mp.PersonId);

			modelBuilder.Entity<Movie>()
				.HasMany(m => m.Comments)
				.WithOne(c => c.Movie)
				.HasForeignKey(c => c.MovieId);

			// Cấu hình mối quan hệ one-to-many giữa Movie và Contract
			modelBuilder.Entity<Movie>()
				.HasMany(m => m.contracts)   
				.WithOne(c => c.Movie)        
				.HasForeignKey(c => c.MovieId)  
				.OnDelete(DeleteBehavior.Cascade); 


			//one - to - one giữa User và UserRegister
			modelBuilder.Entity<User>()
				.HasOne(u => u.UserRegister)
				.WithOne(ur => ur.User)
				.HasForeignKey<User>(u => u.UserRegisterId)
				.OnDelete(DeleteBehavior.Cascade);

			// Thiết lập ràng buộc unique
			modelBuilder.Entity<User>()
				.HasIndex(u => u.UserRegisterId)
				.IsUnique();

			//one - to - one giữa User và UserWallet
			modelBuilder.Entity<UserWallet>()
					.HasOne(w => w.User)
					.WithOne(u => u.UserWallet)
					.HasForeignKey<UserWallet>(w => w.UserId)
					.OnDelete(DeleteBehavior.Cascade);

		}
	}
}
