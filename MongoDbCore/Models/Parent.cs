namespace MongoDbCore.Models
{
    public class Parent : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string Cp { get; set; }
        public string City { get; set; }

        public string FamilyId { get; set; }
    }
}
