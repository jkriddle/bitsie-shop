using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Bitsie.Shop.Services;

namespace Bitsie.Shop.Web.Wrappers
{
    public class ModelStateWrapper : IValidationDictionary
    {
        #region Fields

        private readonly ModelStateDictionary _modelState;

        #endregion

        #region Constructor

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        #endregion

        #region IValidationDictionary Members

        public override void AddError(string key, string errorMessage)
        {
            _modelState.AddModelError(key, errorMessage);
        }

        public override bool IsValid
        {
            get { return _modelState.IsValid; }
        }

        public override IList<string> Errors
        {
            get 
            { 
                var errors = new List<string>();
                foreach(var key in _modelState.Keys)
                {
                    errors.AddRange(_modelState[key].Errors.Select(error => error.ErrorMessage));
                }
                return errors;
            }
        }

        public override void Remove(string propertyName)
        {
            var newErrorList = new List<ValidationError>();
            List<string> matchingErrors = _modelState.Keys.Where(errorKey => errorKey == propertyName).ToList();
            foreach (var errorKey in matchingErrors)
            {
                _modelState.Remove(errorKey);
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