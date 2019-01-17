using SharpArch.Domain.DomainModel;

namespace Bitsie.Shop.Domain
{
    public class Product : Entity
    {
        public virtual User User { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual string ShortDescription { get; set; }
        public virtual decimal Price { get; set; }
        public virtual ProductStatus Status { get; set; }
        public virtual string RedirectUrl { get; set; }
    }
}
