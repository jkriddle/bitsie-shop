using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitsie.Shop.Services
{
    public class ValidationDictionary : IValidationDictionary
    {
        #region Fields

        #endregion

        #region Constructor

        public ValidationDictionary()
        {
        }

        #endregion
        
        #region Overrides of IValidationDictionary

        public override void AddError(string key, string errorMessage)
        {
            ErrorDictionary.Add(new ValidationError(key, errorMessage));
        }

        public override bool IsValid
        {
            get { return ErrorDictionary.Count == 0; }
        }

        public override IList<string> Errors
        {
            get { return ErrorDictionary.Select(e => e.Message).ToList(); }
        }

        public override void Remove(string key)
        {
            var item = ErrorDictionary.FirstOrDefault(e => e.PropertyName == key);
            if (item != null)
            {
                ErrorDictionary.Remove(item);
            }
        }

        public override void Merge(IValidationDictionary vd)
        {
            foreach (var err in vd.ErrorDictionary)
            {
                AddError(err.PropertyName, err.Message);
            }
        }

        #endregion
    }
}
