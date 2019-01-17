using System;

namespace Bitsie.Shop.Services
{
    public interface ISystemService
    {
        Nullable<T> GetValue<T>(string name) where T : struct;
        void SetValue(string name, object value);
    }
}
