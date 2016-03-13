using System;
using System.Collections.Concurrent;
using Forexus.Entities;

namespace Forexus.DAL
{
    /// <summary>
    /// Repository To store Entities. Implemented just for demonstation of next steps
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    public class EntityRepository<T> : IRepository<T> where T : Entity
    {
        //Imitation of database:
        private static ConcurrentDictionary<string, Entity> _database;

        static EntityRepository()
        {
            _database = new ConcurrentDictionary<string, Entity>();
        }

        /// <summary>
        /// Save entity or update existed one
        /// </summary>
        /// <param name="entity">Enity to save</param>
        /// <returns></returns>
        public T Save(T entity)
        {
            //Imitation of database logic
            if (!_database.ContainsKey(entity.Name))
            {
                entity.Id = Guid.NewGuid();
                _database.GetOrAdd(entity.Name, entity);
            }

            return _database[entity.Name] as T;

        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
