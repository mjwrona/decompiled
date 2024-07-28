// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.Common.InputDescriptorValidator
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 762B8E87-3651-4560-BE0D-F9006FB93C96
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.Common.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.Common
{
  public static class InputDescriptorValidator
  {
    public static InputDescriptorValidator.BaseInputValidator GetValidator(
      InputValidation validation)
    {
      InputDescriptorValidator.BaseInputValidator validator;
      switch (validation.DataType)
      {
        case InputDataType.String:
          validator = (InputDescriptorValidator.BaseInputValidator) new InputDescriptorValidator.StringInputValidator(validation);
          break;
        case InputDataType.Number:
          validator = (InputDescriptorValidator.BaseInputValidator) new InputDescriptorValidator.NumberInputValidator(validation);
          break;
        case InputDataType.Boolean:
          validator = (InputDescriptorValidator.BaseInputValidator) new InputDescriptorValidator.BooleanValidator(validation);
          break;
        case InputDataType.Guid:
          validator = (InputDescriptorValidator.BaseInputValidator) new InputDescriptorValidator.GuidInputValidator(validation);
          break;
        case InputDataType.Uri:
          validator = (InputDescriptorValidator.BaseInputValidator) new InputDescriptorValidator.UriInputValidator(validation);
          break;
        default:
          validator = new InputDescriptorValidator.BaseInputValidator(validation);
          break;
      }
      return validator;
    }

    public static string ParamName(string paramName, string descriptorId) => string.Format("{0}[{1}]", (object) paramName, (object) descriptorId);

    public class BaseInputValidator
    {
      protected bool IsRequired;
      protected string Pattern;
      protected string PatternMismatchErrorMessage;
      private readonly InputValidation InputValidation;

      public BaseInputValidator(InputValidation validation) => this.InputValidation = validation;

      public void Validate(string value, string paramName)
      {
        if (string.IsNullOrWhiteSpace(value))
        {
          this.ValidateIfRequired(value, paramName);
        }
        else
        {
          this.ValidateIfLengthExceeds(value, paramName);
          this.ValidateData(value, paramName);
        }
      }

      protected virtual void ValidateData(string value, string paramName)
      {
      }

      protected InputValidation GetInputValidation() => this.InputValidation;

      private void ValidateIfRequired(string value, string paramName)
      {
        if (this.IsRequired)
          throw new ArgumentException(CommonResources.EmptyStringNotAllowed(), paramName);
      }

      private void ValidateIfLengthExceeds(string value, string paramName)
      {
        if (!this.InputValidation.MaxLength.HasValue)
          return;
        int length = value.Length;
        int? maxLength = this.InputValidation.MaxLength;
        int valueOrDefault = maxLength.GetValueOrDefault();
        if (length > valueOrDefault & maxLength.HasValue)
          throw new ArgumentException(TFCommonResources.StringLengthExceedsLimit(), paramName);
      }
    }

    private class StringInputValidator : InputDescriptorValidator.BaseInputValidator
    {
      private readonly int MaxLength = int.MaxValue;
      private readonly int MinLength;
      private const int MaxTimeOutInSeconds = 1;

      public StringInputValidator(InputValidation validation)
        : base(validation)
      {
        if (validation.MinLength.HasValue)
          this.MinLength = validation.MinLength.Value;
        if (validation.MaxLength.HasValue)
          this.MaxLength = validation.MaxLength.Value;
        this.Pattern = validation.Pattern;
        this.PatternMismatchErrorMessage = validation.PatternMismatchErrorMessage;
        this.IsRequired = validation.IsRequired;
      }

      protected override void ValidateData(string value, string paramName)
      {
        if ((value == null ? 0 : (value.Length < this.MinLength ? 0 : (value.Length <= this.MaxLength ? 1 : 0))) == 0)
          throw new ArgumentException(string.Format("Error: {0}, Value: {1}", (object) TFCommonResources.StringLengthExceedsLimit(), (object) value), paramName);
        if (!this.ValidatePattern(value, this.Pattern))
          throw new ArgumentException(string.Format("Error: {0}, Value: {1}, Details: {2}", (object) TFCommonResources.StringPatternDidNotMatch((object) this.Pattern), (object) value, (object) this.PatternMismatchErrorMessage), paramName);
      }

      public bool ValidatePattern(string value, string pattern)
      {
        bool flag = true;
        if (!string.IsNullOrWhiteSpace(pattern))
        {
          Regex regex = new Regex(pattern.Trim(), RegexOptions.None, TimeSpan.FromSeconds(1.0));
          try
          {
            flag = regex.IsMatch(value);
          }
          catch (RegexMatchTimeoutException ex)
          {
            throw new InvalidEndpointResponseException(Resources.RegexMatchTimeExceeded((object) value, (object) 1000));
          }
        }
        return flag;
      }
    }

    private class GuidInputValidator : InputDescriptorValidator.StringInputValidator
    {
      public GuidInputValidator(InputValidation validation)
        : base(validation)
      {
        this.IsRequired = validation.IsRequired;
      }

      protected override void ValidateData(string value, string paramName)
      {
        if (!Guid.TryParse(value, out Guid _))
          throw new ArgumentException(string.Format("{0}, Value: {1}", (object) TFCommonResources.EntityModel_BadGuidFormat(), (object) value), paramName);
      }
    }

    private class UriInputValidator : InputDescriptorValidator.StringInputValidator
    {
      public UriInputValidator(InputValidation validation)
        : base(validation)
      {
        this.IsRequired = validation.IsRequired;
        this.Pattern = validation.Pattern;
      }

      protected override void ValidateData(string value, string paramName)
      {
        Uri result;
        if (!(string.IsNullOrWhiteSpace(this.Pattern) ? Uri.TryCreate(value.Trim(), UriKind.Absolute, out result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps) : this.ValidatePattern(value, this.Pattern)))
          throw new ArgumentException(string.Format(Resources.InvalidUrl((object) value)), paramName);
      }
    }

    private class NumberInputValidator : InputDescriptorValidator.BaseInputValidator
    {
      private Decimal _minValue = -2147483647M;
      private Decimal _maxValue = 2147483647M;

      public NumberInputValidator(InputValidation validation)
        : base(validation)
      {
        Decimal? nullable;
        if (validation.MaxValue.HasValue)
        {
          nullable = validation.MaxValue;
          this._maxValue = nullable.Value;
        }
        nullable = validation.MinValue;
        if (nullable.HasValue)
        {
          nullable = validation.MinValue;
          this._minValue = nullable.Value;
        }
        this.IsRequired = validation.IsRequired;
      }

      protected override void ValidateData(string value, string paramName)
      {
        bool flag = false;
        Decimal result;
        if (Decimal.TryParse(value, out result) && result >= this._minValue && result <= this._maxValue)
          flag = true;
        if (!flag)
          throw new ArgumentException(CommonResources.OutOfRange((object) value), paramName);
      }
    }

    private class BooleanValidator : InputDescriptorValidator.BaseInputValidator
    {
      public BooleanValidator(InputValidation validation)
        : base(validation)
      {
        this.IsRequired = validation.IsRequired;
      }

      protected override void ValidateData(string value, string paramName)
      {
        bool result = false;
        if (!bool.TryParse(value, out result))
          throw new ArgumentException(string.Format("Error: {0}, value: {1}", (object) TFCommonResources.EntityModel_BadBooleanFormat(), (object) value), paramName);
      }
    }
  }
}
