using System;
using System.Collections.Generic;

namespace MongoDbCore.Models
{
    public class Child : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime Birthday { get; set; }

        public string FamilyId { get; set; }

        public IEnumerable<Inscription> Inscriptions { get; set; } = new List<Inscription>();

        public int Age
        {
            get
            {
                DateTime now = DateTime.Today;
                var age = now.Year - Birthday.Year;
                if (Birthday > now.AddYears(-age))
                {
                    age--;
                }
                return age;
            }
        }

        public string CompleteName => FirstName + " " + LastName;
    }
}
