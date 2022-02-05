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

      //if (this.Projects.Any(w => w.ProjectNumber == this.ProjectNumber))
      //{
      //  int nextNumber = Enumerable.Range(1, Int32.MaxValue).Except(this.Projects.Select(w => Int32.Parse(w.ProjectNumber))).First();

      //  string errorMessage = string.Format("{0} {1}: {2}", "Entered project number already exists!", "Next available number is", nextNumber);
      //  return new ValidationResult(false, errorMessage);
      //}

      return ValidationResult.ValidResult;
		}
	}
}