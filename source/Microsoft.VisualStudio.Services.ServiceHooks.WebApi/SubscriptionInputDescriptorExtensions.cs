// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.SubscriptionInputDescriptorExtensions
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public static class SubscriptionInputDescriptorExtensions
  {
    private const string c_http = "http://";
    private const string c_https = "https://";

    public static void Validate(
      this InputDescriptor inputDescriptor,
      string inputValue,
      string scope = null)
    {
      if (scope != null && inputDescriptor.SupportedScopes != null && !inputDescriptor.SupportedScopes.Contains(scope))
        throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputScopeUnsupportedFormat((object) inputDescriptor.Id, (object) scope));
      if (string.IsNullOrWhiteSpace(inputValue))
      {
        if (inputDescriptor.Validation != null && inputDescriptor.Validation.IsRequired)
          throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_MissingRequiredSubscriptionInputFormat((object) inputDescriptor.Id));
      }
      else
      {
        inputValue = inputValue.Trim();
        if (inputDescriptor.Validation == null)
          return;
        if (!string.IsNullOrEmpty(inputDescriptor.Validation.Pattern))
        {
          bool flag = false;
          try
          {
            flag = Regex.IsMatch(inputValue, inputDescriptor.Validation.Pattern, RegexOptions.None, TimeSpan.FromSeconds(2.0));
          }
          catch (RegexMatchTimeoutException ex)
          {
          }
          if (!flag)
            throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputPatternMismatchFormat((object) inputDescriptor.Id, (object) inputDescriptor.Validation.Pattern));
        }
        int? nullable1 = inputDescriptor.Validation.MinLength;
        if (nullable1.HasValue)
        {
          nullable1 = inputDescriptor.Validation.MaxLength;
          if (nullable1.HasValue)
          {
            int length1 = inputValue.Length;
            nullable1 = inputDescriptor.Validation.MinLength;
            int valueOrDefault1 = nullable1.GetValueOrDefault();
            if (!(length1 < valueOrDefault1 & nullable1.HasValue))
            {
              int length2 = inputValue.Length;
              nullable1 = inputDescriptor.Validation.MaxLength;
              int valueOrDefault2 = nullable1.GetValueOrDefault();
              if (!(length2 > valueOrDefault2 & nullable1.HasValue))
                goto label_24;
            }
            string id = inputDescriptor.Id;
            // ISSUE: variable of a boxed type
            __Boxed<int> length3 = (ValueType) inputValue.Length;
            nullable1 = inputDescriptor.Validation.MinLength;
            // ISSUE: variable of a boxed type
            __Boxed<int> local1 = (ValueType) nullable1.Value;
            nullable1 = inputDescriptor.Validation.MaxLength;
            // ISSUE: variable of a boxed type
            __Boxed<int> local2 = (ValueType) nullable1.Value;
            throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputInvalidLengthFormat((object) id, (object) length3, (object) local1, (object) local2));
          }
        }
        nullable1 = inputDescriptor.Validation.MinLength;
        if (nullable1.HasValue)
        {
          int length4 = inputValue.Length;
          nullable1 = inputDescriptor.Validation.MinLength;
          int valueOrDefault = nullable1.GetValueOrDefault();
          if (length4 < valueOrDefault & nullable1.HasValue)
          {
            string id = inputDescriptor.Id;
            // ISSUE: variable of a boxed type
            __Boxed<int> length5 = (ValueType) inputValue.Length;
            nullable1 = inputDescriptor.Validation.MinLength;
            // ISSUE: variable of a boxed type
            __Boxed<int> local = (ValueType) nullable1.Value;
            throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputInvalidLengthTooSmallFormat((object) id, (object) length5, (object) local));
          }
        }
        else
        {
          nullable1 = inputDescriptor.Validation.MaxLength;
          if (nullable1.HasValue)
          {
            int length6 = inputValue.Length;
            nullable1 = inputDescriptor.Validation.MaxLength;
            int valueOrDefault = nullable1.GetValueOrDefault();
            if (length6 > valueOrDefault & nullable1.HasValue)
            {
              string id = inputDescriptor.Id;
              // ISSUE: variable of a boxed type
              __Boxed<int> length7 = (ValueType) inputValue.Length;
              nullable1 = inputDescriptor.Validation.MaxLength;
              // ISSUE: variable of a boxed type
              __Boxed<int> local = (ValueType) nullable1.Value;
              throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputInvalidLengthTooBigFormat((object) id, (object) length7, (object) local));
            }
          }
        }
label_24:
        switch (inputDescriptor.Validation.DataType)
        {
          case InputDataType.None:
            break;
          case InputDataType.String:
            break;
          case InputDataType.Number:
            Decimal result1;
            if (!Decimal.TryParse(inputValue, out result1))
              throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputDataTypeMismatchFormat((object) inputDescriptor.Id, (object) inputDescriptor.Validation.DataType));
            if (inputDescriptor.Validation == null)
              break;
            Decimal? nullable2 = inputDescriptor.Validation.MinValue;
            if (nullable2.HasValue)
            {
              nullable2 = inputDescriptor.Validation.MaxValue;
              if (nullable2.HasValue)
              {
                Decimal num1 = result1;
                nullable2 = inputDescriptor.Validation.MinValue;
                Decimal valueOrDefault3 = nullable2.GetValueOrDefault();
                if (!(num1 < valueOrDefault3 & nullable2.HasValue))
                {
                  Decimal num2 = result1;
                  nullable2 = inputDescriptor.Validation.MaxValue;
                  Decimal valueOrDefault4 = nullable2.GetValueOrDefault();
                  if (!(num2 > valueOrDefault4 & nullable2.HasValue))
                    break;
                }
                string id = inputDescriptor.Id;
                string str = inputValue;
                nullable2 = inputDescriptor.Validation.MinValue;
                // ISSUE: variable of a boxed type
                __Boxed<Decimal> local3 = (ValueType) nullable2.Value;
                nullable2 = inputDescriptor.Validation.MaxValue;
                // ISSUE: variable of a boxed type
                __Boxed<Decimal> local4 = (ValueType) nullable2.Value;
                throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputOutOfRangeFormat((object) id, (object) str, (object) local3, (object) local4));
              }
            }
            nullable2 = inputDescriptor.Validation.MinValue;
            if (nullable2.HasValue)
            {
              Decimal num = result1;
              nullable2 = inputDescriptor.Validation.MinValue;
              Decimal valueOrDefault = nullable2.GetValueOrDefault();
              if (!(num < valueOrDefault & nullable2.HasValue))
                break;
              string id = inputDescriptor.Id;
              string str = inputValue;
              nullable2 = inputDescriptor.Validation.MinValue;
              // ISSUE: variable of a boxed type
              __Boxed<Decimal> local = (ValueType) nullable2.Value;
              throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputValueTooSmallFormat((object) id, (object) str, (object) local));
            }
            nullable2 = inputDescriptor.Validation.MaxValue;
            if (!nullable2.HasValue)
              break;
            Decimal num3 = result1;
            nullable2 = inputDescriptor.Validation.MaxValue;
            Decimal valueOrDefault5 = nullable2.GetValueOrDefault();
            if (!(num3 > valueOrDefault5 & nullable2.HasValue))
              break;
            string id1 = inputDescriptor.Id;
            string str1 = inputValue;
            nullable2 = inputDescriptor.Validation.MaxValue;
            // ISSUE: variable of a boxed type
            __Boxed<Decimal> local5 = (ValueType) nullable2.Value;
            throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputValueTooBigFormat((object) id1, (object) str1, (object) local5));
          case InputDataType.Boolean:
            if (bool.TryParse(inputValue, out bool _))
              break;
            throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputDataTypeMismatchFormat((object) inputDescriptor.Id, (object) inputDescriptor.Validation.DataType));
          case InputDataType.Guid:
            if (Guid.TryParse(inputValue, out Guid _))
              break;
            throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputDataTypeMismatchFormat((object) inputDescriptor.Id, (object) inputDescriptor.Validation.DataType));
          case InputDataType.Uri:
            Uri result2;
            if ((!Uri.TryCreate(inputValue, UriKind.Absolute, out result2) ? 0 : (result2.Scheme == Uri.UriSchemeHttp ? 1 : (result2.Scheme == Uri.UriSchemeHttps ? 1 : 0))) != 0)
              break;
            throw new SubscriptionInputException(ServiceHooksWebApiResources.Error_SubscriptionInputDataTypeMismatchFormat((object) inputDescriptor.Id, (object) inputDescriptor.Validation.DataType));
          default:
            throw new NotImplementedException(inputDescriptor.Validation.DataType.ToString());
        }
      }
    }
  }
}
