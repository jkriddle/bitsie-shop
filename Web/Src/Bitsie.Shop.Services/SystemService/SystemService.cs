using Bitsie.Shop.Domain;
using Bitsie.Shop.Infrastructure;
using System;
using System.Linq;

namespace Bitsie.Shop.Services
{
    public class SystemService : ISystemService
    {
        private readonly ISystemSettingRepository _systemSettingRepository;

        public SystemService(ISystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
        }

        /// <summary>
        /// Retrieve a system setting value
        /// </summary>
        /// <typeparam name="T">Type to retrieve setting as</typeparam>
        /// <param name="name">Setting name</param>
        /// <returns></returns>
        public Nullable<T> GetValue<T>(string name) where T : struct
        {
            var setting = _systemSettingRepository.FindAll().FirstOrDefault(s => s.Name == name);
            if (setting == null) return default(T);
            return setting.GetValue<T>();
        }

        /// <summary>
        /// Set the value of a system setting
        /// </summary>
        /// <param name="name">Setting name</param>
        /// <param name="value">Setting value</param>
        public void SetValue(string name, object value)
        {
            var setting = _systemSettingRepository.FindAll().FirstOrDefault(s => s.Name == name);
            if (setting == null)
            {
                setting = new SystemSetting();
                setting.Name = name;
            }
            setting.Value = value.ToString();
            _systemSettingRepository.Save(setting);
        }
    }
}
