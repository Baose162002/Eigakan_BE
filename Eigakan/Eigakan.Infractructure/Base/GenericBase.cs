using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;



namespace Eigakan.Infractructure.Base
{
	public class GenericBase<TEntity> where TEntity : class
	{
		private readonly EigakanDbContext context;
		private readonly DbSet<TEntity> dbSet;
		private static GenericBase<TEntity> instance = null;

		public static GenericBase<TEntity> Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new GenericBase<TEntity>();
				}
				return instance;
			}
		}

		public GenericBase()
		{
			context = new EigakanDbContext();
			this.dbSet = context.Set<TEntity>();
		}

		private IQueryable<TEntity> GetQueryable(
	Expression<Func<TEntity, bool>> filter = null,
	Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
	string includeProperties = "",
	int? pageIndex = null,
	int? pageSize = null)
		{
			IQueryable<TEntity> query = dbSet;

			if (filter != null)
			{
				query = query.Where(filter);
			}

			foreach (var includeProperty in includeProperties.Split
				(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			// Nếu có hàm orderBy, áp dụng sắp xếp trước
			if (orderBy != null)
			{
				query = orderBy(query);
			}

			// Sau khi sắp xếp, thực hiện phân trang
			if (pageIndex.HasValue && pageSize.HasValue)
			{
				int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
				int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

				query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
			}

			return query;
		}


		public async Task<IEnumerable<TEntity>> Get(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = "",
			int? pageIndex = null,
			int? pageSize = null)
		{
			var query = GetQueryable(filter, orderBy, includeProperties, pageIndex, pageSize);
			return await query.ToListAsync();
		}



		public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
		{
			var query = GetQueryable(predicate);
			return await query.CountAsync();
		}


		public async Task<TEntity> GetSingle(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = "")
		{
			var query = GetQueryable(filter, orderBy, includeProperties, null, null);
			if (orderBy != null)
				query = orderBy(query);
			return await query.FirstOrDefaultAsync();
		}

		public async Task<TEntity> GetSingleAirplane(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = "")
		{
			IQueryable<TEntity> query = context.Set<TEntity>();

			// Nạp các thuộc tính liên quan từ includeProperties
			if (!string.IsNullOrWhiteSpace(includeProperties))
			{
				foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty.Trim());
				}
			}

			if (filter != null)
			{
				query = query.Where(filter);
			}

			if (orderBy != null)
			{
				query = orderBy(query);
			}

			return await query.FirstOrDefaultAsync();
		}


		public async Task<TEntity> GetByID(int id)
		{
			return await dbSet.FindAsync(id);
		}

		public async Task Insert(TEntity entity)
		{

			await dbSet.AddAsync(entity);
			await context.SaveChangesAsync();

		}

		// Phương thức BulkInsertAsync sử dụng EFCore.BulkExtensions
		public async Task BulkInsertAsync(IEnumerable<TEntity> entities)
		{
			if (entities == null || !entities.Any())
				return;

			// Sử dụng BulkInsert từ EFCore.BulkExtensions
			await context.BulkInsertAsync(entities);
		}

		public async Task InsertRange(List<TEntity> entities)
		{
			await dbSet.AddRangeAsync(entities);
			await context.SaveChangesAsync();
		}

		public async Task Delete(TEntity entity)
		{
			dbSet.Remove(entity);
			await context.SaveChangesAsync();
		}

		public async Task Update(TEntity entityToUpdate)
		{
			dbSet.Attach(entityToUpdate);		
			context.Entry(entityToUpdate).State = EntityState.Modified;
			await context.SaveChangesAsync();
		}

		public async Task<int> Count(Expression<Func<TEntity, bool>> filter = null)
		{
			IQueryable<TEntity> query = dbSet;

			if (filter != null)
			{
				query = query.Where(filter);
			}
			return await query.CountAsync();
		}

		public async Task UpdateRange(List<TEntity> entities)
		{
			dbSet.UpdateRange(entities);
			await context.SaveChangesAsync();
		}

		public async Task DeleteRange(List<TEntity> entities)
		{
			dbSet.RemoveRange(entities);
			await context.SaveChangesAsync();
		}

		//Transaction
		public async Task InsertTransaction(TEntity entity)
		{
			await dbSet.AddAsync(entity);
		}

		public async Task UpdateTransaction(TEntity entityToUpdate)
		{
			dbSet.Update(entityToUpdate);
		}


		public async Task SaveChangeTransaction()
		{
			await context.SaveChangesAsync();
		}

		public async Task<IDbContextTransaction> BeginTransactionAsync()
		{
			return await context.Database.BeginTransactionAsync();
		}


	}
}
