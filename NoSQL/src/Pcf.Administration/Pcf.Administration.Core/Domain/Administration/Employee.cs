using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Pcf.Administration.Core.Attributes;

namespace Pcf.Administration.Core.Domain.Administration
{
    [BsonCollection("employees")]
    public class Employee : BaseEntity
    {
        [BsonElement("first_name")]
        public string FirstName { get; set; }

        [BsonElement("last_name")]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("role_id")]
        public string RoleId { get; set; }

        [BsonElement("applied_promocodes_count")]
        public int AppliedPromocodesCount { get; set; }
    }
}
