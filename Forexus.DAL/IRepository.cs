using System;

namespace Forexus.DAL
{
    /// <summary>
    /// Interface to implement data access methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IDisposable where T: class
    {
        /// <summary>
        /// Save entity to database
        /// </summary>
        /// <param name="entity">Entity to save</param>
        /// <returns>Entity with ID</returns>
        T Save(T entity);
    }
}
