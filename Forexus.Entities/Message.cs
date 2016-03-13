using System;
using System.Collections.Generic;

namespace Forexus.Entities
{
    /// <summary>
    /// Represents Message
    /// </summary>
    public class Message
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; }
        public Source Source { get; set; }
        public User User { get; set; }
        public string Event { get; set; }
        public Group Group { get; set; }
        public List<User> Viewers { get; set; }
        public string MessageText { get; set; }
        public string Image { get; set; }
    }
}
