using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ErogeDaily.Models
{
    public abstract class VerifiableBindableBase : BindableBase, INotifyDataErrorInfo
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        private ErrorsContainer<ValidationResult> errorsContainer;

        public VerifiableBindableBase()
        {
            errorsContainer = new ErrorsContainer<ValidationResult>(RaiseErrorsChanged);
        }

        private void RaiseErrorsChanged([CallerMemberName] string propertyName = null)
            => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));

        public bool HasErrors
            => errorsContainer.HasErrors;

        public IEnumerable GetErrors(string propertyName)
            => errorsContainer.GetErrors(propertyName);

        protected void ValidateProperty(object value, [CallerMemberName] string propertyName = null)
        {
            var context = new ValidationContext(this) { MemberName = propertyName };
            var validationErrors = new List<ValidationResult>();
            if (!Validator.TryValidateProperty(value, context, validationErrors))
            {
                errorsContainer.SetErrors(propertyName, validationErrors);
            }
            else
            {
                errorsContainer.ClearErrors(propertyName);
            }
        }

        public void ClearAllErrors()
        {
            errorsContainer.ClearErrors();
        }
    }
}
