using CMS.Repository.Context;

namespace CMS.Repository.Repositories
{
    /// <summary>
    /// Represents a generic repository for managing entities of type T.
    /// </summary>
    /// <typeparam name="T">The type of entity to manage.</typeparam>
    public class Repository<T>(CMSDBContext context) : IRepository<T> where T : class
    {
        /// <summary>
        /// Adds a new entity of type T to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public T Add(T entity)
        {
            context.Add<T>(entity);
            context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Updates an existing entity of type T in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Delete(T entity)
        {
            context.Remove<T>(entity);
            context.SaveChanges();
        }

        /// <summary>
        /// Gets all entities of type T from the repository.
        /// </summary>
        /// <returns>An IEnumerable of entities of type T.</returns>

        public IEnumerable<T> GetAll()
        {
            return context.Set<T>();
        }

        /// <summary>
        /// Gets an entity of type T from the repository based on the provided search parameters.
        /// </summary>
        /// <param name="predicate">The search predicate.</param>
        /// <returns>An IEnumerable of entities of type T.</returns>
        public IEnumerable<T> GetBySearchParams(Func<T, bool> predicate)
        {
            return context.Set<T>().Where(predicate);
        }

        /// <summary>
        /// Updates an existing entity of type T in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public T Update(T entity)
        {
            context.Update<T>(entity);
            context.SaveChanges();
            return entity;
        }
    }
}
