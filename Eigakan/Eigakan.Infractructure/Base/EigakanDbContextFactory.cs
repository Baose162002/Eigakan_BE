using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Eigakan.Infractructure.Base
{
	public class EigakanDbContextFactory : IDesignTimeDbContextFactory<EigakanDbContext>
	{
		public EigakanDbContext CreateDbContext(string[] args)
		{
			// Đường dẫn tới thư mục chứa appsettings.json trong project API
			var pathToApiProject = Path.Combine(Directory.GetCurrentDirectory(), "../Eigakan.API");

			var configuration = new ConfigurationBuilder()
				.SetBasePath(pathToApiProject)
				.AddJsonFile("appsettings.json", optional: false)
				.Build();

			var optionsBuilder = new DbContextOptionsBuilder<EigakanDbContext>();
			optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

			return new EigakanDbContext(optionsBuilder.Options);
		}
	}
}
