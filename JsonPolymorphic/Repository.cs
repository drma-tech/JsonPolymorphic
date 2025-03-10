using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace JsonPolymorphic
{
    public abstract class AbstractRepository() : IDisposable
    {
        protected readonly MyDbContext context = new();

        public IQueryable<T> GetQueryable<T>(bool tracking) where T : ModelBase
        {
            if (tracking)
                return context.Set<T>().AsQueryable();
            else
                return context.Set<T>().AsNoTracking().AsQueryable();
        }

        public IEnumerable<TR> SqlQueryRaw<TR>(string sql, params object[] parameters)
        {
            return context.Database.SqlQueryRaw<TR>(sql, parameters).AsEnumerable();
        }

        public async Task<int> ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken)
        {
            return await context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }

        public async Task<T?> GetByIdAsync<T>(object?[]? pks, CancellationToken cancellationToken) where T : ModelBase
        {
            return await context.Set<T>().FindAsync(pks, cancellationToken);
        }

        protected void Insert<T>(T model, int? customerId, bool forceCustomerId) where T : ModelBase
        {
            if (forceCustomerId) model.CustomerId = customerId;
            context.Set<T>().Add(model);
        }

        protected void Update<T>(T model, int? customerId, bool forceCustomerId) where T : ModelBase
        {
            if (forceCustomerId) model.CustomerId = customerId;
            var local = context.Set<T>().Local.FirstOrDefault(entry => entry.IdentificationId.Equals(model.IdentificationId));
            if (local != null)
            {
                context.Entry(local).State = EntityState.Detached;
            }

            context.Entry(model).State = EntityState.Modified;
        }

        protected async Task Update<T>(T model, int? customerId, bool forceCustomerId, object?[]? pks, Expression<Func<T, object>> children,
            CancellationToken cancellationToken, bool relationManyToMany = false, bool removeMissingItems = true) where T : ModelBase
        {
            //https://stackoverflow.com/questions/55088933/update-parent-and-child-collections-on-generic-repository-with-ef-core

            //TODO: find a better way to this method

            if (forceCustomerId) model.CustomerId = customerId;
            var dbEntity = await GetByIdAsync<T>(pks, cancellationToken) ?? throw new Exception($"dbEntity not found ({pks})");
            var dbEntry = context.Entry<T>(dbEntity);
            dbEntry.CurrentValues.SetValues(model);

            var propertyName = children.GetPropertyAccess().Name;
            var dbItemsEntry = dbEntry.Collection(propertyName);
            var accessor = dbItemsEntry.Metadata.GetCollectionAccessor();

            await dbItemsEntry.LoadAsync(cancellationToken);

            if (accessor != null && dbItemsEntry.CurrentValue != null)
            {
                var dbItemsMap = ((IEnumerable<ModelBase>)dbItemsEntry.CurrentValue).ToDictionary(e => e.IdentificationId);
                var items = (IEnumerable<ModelBase>)accessor.GetOrCreate(model, true) ?? [];

                foreach (var item in items)
                {
                    if (!dbItemsMap.TryGetValue(item.IdentificationId, out var oldItem))
                        accessor.Add(dbEntity, item, true);
                    else
                    {
                        context.Entry(oldItem).CurrentValues.SetValues(item);
                        dbItemsMap.Remove(item.IdentificationId);
                    }
                }

                if (removeMissingItems)
                {
                    foreach (var oldItem in dbItemsMap.Values)
                    {
                        if (relationManyToMany)
                            accessor.Remove(dbEntity, oldItem);
                        else
                            context.Remove(oldItem);
                    }
                }
            }
        }

        protected async Task DeleteAsync<T>(int pk, int? customerId, CancellationToken cancellationToken) where T : ModelBase
        {
            var model = await GetByIdAsync<T>([pk], cancellationToken);

            if (model != null)
            {
                if (model.CustomerId != customerId) throw new InvalidOperationException("invalid customerId");

                context.Set<T>().Remove(model);
            }
        }

        protected void Delete<T>(T model, int? customerId, bool forceCustomerId) where T : ModelBase
        {
            if (forceCustomerId && model.CustomerId != customerId) throw new InvalidOperationException("invalid customerId");

            context.Set<T>().Remove(model);
        }

        public void DeleteCollection<CT>(ICollection<CT> collection, int? customerId) where CT : ModelBase
        {
            if (customerId != null)
            {
                foreach (var model in collection)
                {
                    if (model.CustomerId != customerId) throw new InvalidOperationException("invalid customerId");
                }
            }

            context.Set<CT>().RemoveRange(collection);
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        #region Disposable

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                context.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion Disposable
    }

    public class GenericRepository() : AbstractRepository()
    {
        public new IQueryable<T> GetQueryable<T>(bool tracking) where T : ModelBase
        {
            return base.GetQueryable<T>(tracking);
        }

        public new void Insert<T>(T model, int? customerId, bool forceCustomerId = true) where T : ModelBase
        {
            base.Insert(model, customerId, forceCustomerId);
        }

        public new void Update<T>(T model, int? customerId, bool forceCustomerId = true) where T : ModelBase
        {
            base.Update(model, customerId, forceCustomerId);
        }

        public new Task Update<T>(T model, int? customerId, bool forceCustomerId, object?[]? pks, Expression<Func<T, object>> children, CancellationToken cancellationToken, bool relationManyToMany = false, bool removeMissingItems = true) where T : ModelBase
        {
            return base.Update(model, customerId, forceCustomerId, pks, children, cancellationToken, relationManyToMany, removeMissingItems);
        }

        public new Task DeleteAsync<T>(int pk, int? customerId, CancellationToken cancellationToken) where T : ModelBase
        {
            return base.DeleteAsync<T>(pk, customerId, cancellationToken);
        }

        public new void Delete<T>(T model, int? customerId, bool forceCustomerId = true) where T : ModelBase
        {
            base.Delete(model, customerId, forceCustomerId);
        }
    }
}