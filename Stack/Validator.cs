using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Stack
{
    public static class Validator
    {
        public static void Validate(IValidatable data)
        {
            List<ValidationResult> errors = new List<ValidationResult>();
            ValidationContext context = new ValidationContext(data, null, null);

            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(data, context, errors, true);

            if (errors.Count > 0)
            {
                ValidationResult first = errors.First();
                throw new ValidationException(new ValidationError(first.ErrorMessage, first.MemberNames.First()));
            }
        }
    }
}
