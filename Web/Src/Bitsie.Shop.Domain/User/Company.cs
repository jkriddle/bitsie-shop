using SharpArch.Domain.DomainModel;
namespace Bitsie.Shop.Domain
{
    public class Company : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Street { get; set; }
        public virtual string Street2 { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Zip { get; set; }
        public virtual string Website { get; set; }
        public virtual string Industry { get; set; }
    }
}
