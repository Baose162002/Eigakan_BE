using Eigakan.Application.Interface.IRepository;
using Eigakan.Infractructure.Base;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Eigakan.Infractructure.Repositories.GenericRepositories
{
	public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
	{
		public async Task<IEnumerable<TEntity>> Get(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = "",
			int? pageIndex = null,
			int? pageSize = null) => await GenericBase<TEntity>.Instance.Get(filter, orderBy, includeProperties, pageIndex, pageSize);

		public async Task<TEntity> GetSingle(
			Expression<Func<TEntity, bool>> filter = null,
			Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
			string includeProperties = "") => await GenericBase<TEntity>.Instance.GetSingle(filter, orderBy, includeProperties);

		public async Task<TEntity> GetByID(int id) => await GenericBase<TEntity>.Instance.GetByID(id);

		public async Task Insert(TEntity entity) => await GenericBase<TEntity>.Instance.Insert(entity);

		public async Task InsertRange(List<TEntity> entities) => await GenericBase<TEntity>.Instance.InsertRange(entities);

		public async Task BulkInsertAsync(IEnumerable<TEntity> entities) => await GenericBase<TEntity>.Instance.BulkInsertAsync(entities);

		public async Task Delete(TEntity entity) => await GenericBase<TEntity>.Instance.Delete(entity);

		public async Task Update(TEntity entityToUpdate) => await GenericBase<TEntity>.Instance.Update(entityToUpdate);

		public async Task<int> Count(Expression<Func<TEntity, bool>> filter = null) => await GenericBase<TEntity>.Instance.Count(filter);

		public async Task UpdateRange(List<TEntity> entities) => await GenericBase<TEntity>.Instance.UpdateRange(entities);

		public async Task DeleteRange(List<TEntity> entities) => await GenericBase<TEntity>.Instance.DeleteRange(entities);

		//Transaction
		public async Task InsertTransaction(TEntity entity) => await GenericBase<TEntity>.Instance.InsertTransaction(entity);

		public async Task UpdateTransaction(TEntity entityToUpdate) => await GenericBase<TEntity>.Instance.UpdateTransaction(entityToUpdate);

		public async Task SaveChangeTransaction() => await GenericBase<TEntity>.Instance.SaveChangeTransaction();

		public async Task<IDbContextTransaction> BeginTransactionAsync() => await GenericBase<TEntity>.Instance.BeginTransactionAsync();


	}
}
