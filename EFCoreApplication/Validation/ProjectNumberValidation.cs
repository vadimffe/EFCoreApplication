using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;

namespace EFCoreApplication.Validation
{
  public class ProjectNumberValidation : ValidationRule
	{
		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
      if (string.IsNullOrEmpty((value ?? "").ToString()))
      {
        return new ValidationResult(false, "Enter project number");
      }

      int n;
      bool isNumeric = int.TryParse((value ?? "").ToString(), out n);

      if (!isNumeric)
      {
        return new ValidationResult(false, "Project number should be a number without letters");
      }

      return ValidationResult.ValidResult;
		}
	}
}