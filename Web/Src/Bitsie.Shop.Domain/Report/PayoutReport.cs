using Bitsie.Shop.Domain;
using SharpArch.Domain.DomainModel;

namespace Bitsie.Shop.Domain
{
    public class PayoutReport
    {
        public virtual int UserId { get; set; }
        public virtual string CompanyName { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual decimal PayoutBalance { get; set; }
        public virtual string PaymentAddress { get; set; }
    }
}