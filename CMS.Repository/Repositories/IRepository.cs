namespace CMS.Repository.Repositories
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets all entities of type T from the repository.
        /// </summary>
        /// <returns>returns <see cref="IEnumerable{T}"/></returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Gets an entity of type T from the repository based on the provided search parameters.
        /// </summary>
        /// <param name="predicate">A function that defines the search criteria.</param>
        /// <returns>The entity that matches the search criteria, or null if not found.</returns>
        IEnumerable<T> GetBySearchParams(Func<T, bool> predicate);

        /// <summary>
        /// Adds a new entity of type T to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        T Add(T entity);
        /// <summary>
        /// Updates an existing entity of type T in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        T Update(T entity);
        /// <summary>
        /// Deletes an entity of type T from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(T entity);
    }
}
