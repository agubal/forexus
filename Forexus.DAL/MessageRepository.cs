using System;
using Forexus.Entities;

namespace Forexus.DAL
{
    /// <summary>
    /// Repository To store Messages. Implemented just for demonstation of next steps
    /// </summary>
    public class MessageRepository : IRepository<Message>
    {
        /// <summary>
        /// Save message to database
        /// </summary>
        /// <param name="entity">Message to save</param>
        /// <returns></returns>
        public Message Save(Message entity)
        {
            entity.Id = Guid.NewGuid();
            return entity;
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
