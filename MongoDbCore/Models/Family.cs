using System.Collections.Generic;

namespace MongoDbCore.Models
{
    public class Family : Entity
    {
        public string FamilyName { get; set; }

        public IEnumerable<Parent> Parents { get; set; } = new List<Parent>();

        public IEnumerable<Child> Children { get; set; } = new List<Child>();
    }
}
