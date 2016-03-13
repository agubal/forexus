using System;

namespace Forexus.Entities
{
    /// <summary>
    /// Base class for Message entities
    /// </summary>
    public abstract class Entity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
