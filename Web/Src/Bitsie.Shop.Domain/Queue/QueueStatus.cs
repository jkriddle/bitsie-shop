using System.ComponentModel;
namespace Bitsie.Shop.Domain
{
    public enum QueueStatus
    {
        [Description("Pending")]
        Pending = 1,
        [Description("Complete")]
        Complete,
        [Description("Cancelled")]
        Cancelled,
        [Description("Failed")]
        Failed
    }
}
