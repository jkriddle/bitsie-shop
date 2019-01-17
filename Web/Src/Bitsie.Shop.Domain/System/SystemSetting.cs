using SharpArch.Domain.DomainModel;
using System;
using System.ComponentModel;
namespace Bitsie.Shop.Domain
{
    public class SystemSetting : Entity
    {
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }

        public virtual Nullable<T> GetValue<T>() where T : struct
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null && !string.IsNullOrEmpty(Value))
            {
                try
                {
                    return (T)converter.ConvertFrom(Value);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return null;
        }

    }
}
