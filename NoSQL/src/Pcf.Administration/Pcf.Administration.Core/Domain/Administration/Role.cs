using MongoDB.Bson.Serialization.Attributes;
using Pcf.Administration.Core.Attributes;

namespace Pcf.Administration.Core.Domain.Administration
{
    [BsonCollection("roles")]
    public class Role : BaseEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string Description { get; set; }
    }
}
