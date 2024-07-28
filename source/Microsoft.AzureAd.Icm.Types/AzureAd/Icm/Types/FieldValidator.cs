// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.FieldValidator
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;

namespace Microsoft.AzureAd.Icm.Types
{
  public static class FieldValidator
  {
    public static bool ValidateCorrelationId(
      string text,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateStringField(text, "CorrelationId", 500, out validated, invalidFields);
    }

    public static bool ValidateDataCenter(
      string text,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateStringField(text, "DataCenter", 32, out validated, invalidFields);
    }

    public static bool ValidateDate(
      string fieldName,
      string text,
      out DateTime date,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      if (text == null)
      {
        invalidFields[fieldName] = InvalidReasons.CannotBeNull;
        date = new DateTime();
        return false;
      }
      if (string.IsNullOrWhiteSpace(text))
      {
        invalidFields[fieldName] = InvalidReasons.CannotBeEmpty;
        date = new DateTime();
        return false;
      }
      if (!DateTime.TryParse(text, out date))
      {
        invalidFields[fieldName] = InvalidReasons.IsInAnInvalidFormat;
        date = new DateTime();
        return false;
      }
      if (ParameterValidator.IsWithinValidDateRange(date))
        return true;
      invalidFields[fieldName] = InvalidReasons.IsOutOfRange;
      date = new DateTime();
      return false;
    }

    public static bool ValidateDate(
      string fieldName,
      DateTime date,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      if (ParameterValidator.IsWithinValidDateRange(date))
        return true;
      invalidFields[fieldName] = InvalidReasons.IsOutOfRange;
      date = new DateTime();
      return false;
    }

    public static bool ValidateDeviceGroup(
      string text,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateStringField(text, "DeviceGroup", 64, out validated, invalidFields);
    }

    public static bool ValidateDeviceName(
      string text,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateStringField(text, "DeviceName", 128, out validated, invalidFields);
    }

    public static bool ValidateEnvironment(
      string text,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateStringField(text, "Environment", 32, out validated, invalidFields);
    }

    public static bool ValidateNotNull(
      object field,
      string fieldName,
      IDictionary<string, InvalidReasons> invalidReasons)
    {
      if (field != null)
        return true;
      invalidReasons.Add(fieldName, InvalidReasons.CannotBeNull);
      return false;
    }

    public static bool ValidateField<T>(
      string text,
      string fieldName,
      out T result,
      ConvertFromText<T> converter,
      IDictionary<string, InvalidReasons> invalidFields)
      where T : struct
    {
      InvalidReasons invalidReasons1 = InvalidReasons.None;
      result = default (T);
      if (text == null)
      {
        InvalidReasons invalidReasons2 = invalidReasons1 | InvalidReasons.CannotBeNull;
        invalidFields[fieldName] = invalidReasons2;
        return false;
      }
      text = text.Trim();
      if (string.IsNullOrEmpty(text))
      {
        InvalidReasons invalidReasons3 = invalidReasons1 | InvalidReasons.CannotBeEmpty;
        invalidFields[fieldName] = invalidReasons3;
        return false;
      }
      if (converter(text, out result))
        return true;
      InvalidReasons invalidReasons4 = invalidReasons1 | InvalidReasons.IsInAnInvalidFormat;
      invalidFields[fieldName] = invalidReasons4;
      return false;
    }

    public static bool ValidatePriority(
      string text,
      out int priority,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      InvalidReasons invalidReasons1 = InvalidReasons.None;
      if (text == null)
      {
        InvalidReasons invalidReasons2 = invalidReasons1 | InvalidReasons.CannotBeNull;
        invalidFields["Priority"] = invalidReasons2;
        priority = 0;
        return false;
      }
      text = text.Trim();
      if (string.IsNullOrEmpty(text))
      {
        InvalidReasons invalidReasons3 = invalidReasons1 | InvalidReasons.CannotBeEmpty;
        invalidFields["Priority"] = invalidReasons3;
        priority = 0;
        return false;
      }
      if (int.TryParse(text, out priority))
        return true;
      InvalidReasons invalidReasons4 = invalidReasons1 | InvalidReasons.IsInAnInvalidFormat;
      invalidFields["Priority"] = invalidReasons4;
      priority = 0;
      return false;
    }

    public static bool ValidateRoutingId(
      string text,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateStringField(text, "RoutingId", 200, out validated, invalidFields);
    }

    public static bool ValidateServiceInstanceId(
      string text,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateStringField(text, "ServiceInstanceId", 64, out validated, invalidFields);
    }

    public static bool ValidateSeverity(
      string text,
      out int severity,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      if (text == null)
      {
        invalidFields["Severity"] = InvalidReasons.CannotBeNull;
        severity = 0;
        return false;
      }
      text = text.Trim();
      if (string.IsNullOrEmpty(text))
      {
        invalidFields["Severity"] = InvalidReasons.CannotBeEmpty;
        severity = 0;
        return false;
      }
      if (!int.TryParse(text, out severity))
      {
        invalidFields["Severity"] = InvalidReasons.IsInAnInvalidFormat;
        severity = 0;
        return false;
      }
      if (!ParameterValidator.IsNotInRangeInclusive<int>(severity, (int) IcmConstants.Severity.Min, (int) IcmConstants.Severity.Max))
        return true;
      invalidFields["Severity"] = InvalidReasons.IsOutOfRange;
      severity = 0;
      return false;
    }

    public static bool ValidateSeverity(
      int? severity,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      InvalidReasons invalidReasons1 = InvalidReasons.None;
      if (!severity.HasValue || !ParameterValidator.IsNotInRangeInclusive<int>(severity.Value, (int) IcmConstants.Severity.Min, (int) IcmConstants.Severity.Max))
        return true;
      InvalidReasons invalidReasons2 = invalidReasons1 | InvalidReasons.IsOutOfRange;
      invalidFields["Severity"] = invalidReasons2;
      return false;
    }

    public static bool ValidateStringField(
      string text,
      string fieldName,
      int maxLength,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields,
      bool allowNull = true)
    {
      if (text == null)
      {
        validated = (string) null;
        if (allowNull)
          return true;
        invalidFields[fieldName] = InvalidReasons.CannotBeNull;
      }
      text = text.Trim();
      if (string.IsNullOrEmpty(text))
      {
        invalidFields[fieldName] = InvalidReasons.CannotBeEmpty;
        validated = (string) null;
        return false;
      }
      if (ParameterValidator.IsGreaterThan<int>(text.Length, maxLength))
      {
        invalidFields[fieldName] = InvalidReasons.ExceededMaxLength;
        validated = (string) null;
        return false;
      }
      validated = text;
      return true;
    }

    public static bool ValidateMaxLength(
      string text,
      string fieldName,
      int maxLength,
      out string validated,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      validated = (string) null;
      if (text == null)
        return true;
      text = text.Trim();
      if (string.IsNullOrWhiteSpace(text))
        return true;
      if (ParameterValidator.IsGreaterThan<int>(text.Length, maxLength))
      {
        invalidFields[fieldName] = InvalidReasons.ExceededMaxLength;
        return false;
      }
      validated = text;
      return true;
    }

    public static bool ValidateInt(
      string text,
      string fieldName,
      out int validated,
      int? defaultIfNull,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      if (!defaultIfNull.HasValue || text != null)
        return FieldValidator.ValidateField<int>(text, fieldName, out validated, new ConvertFromText<int>(int.TryParse), invalidFields);
      validated = defaultIfNull.Value;
      return true;
    }

    public static bool ValidateMatchesRegex(
      string text,
      string fieldName,
      System.Text.RegularExpressions.Regex regex,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      ArgumentCheck.ThrowIfNull((object) regex, nameof (regex), nameof (ValidateMatchesRegex), "X:\\bt\\1016846\\repo\\src\\Utility\\Types\\FieldValidator.cs");
      if (text == null)
        return false;
      if (regex.IsMatch(text))
        return true;
      invalidFields[fieldName] = InvalidReasons.IsInAnInvalidFormat;
      return false;
    }

    public static bool ValidateUrl(
      string url,
      string fieldName,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateMatchesRegex(url, fieldName, IcmConstants.Regex.Url, invalidFields);
    }

    public static bool ValidateAliasDomain(
      string text,
      string fieldName,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateMatchesRegex(text, fieldName, IcmConstants.Regex.Domain, invalidFields);
    }

    public static bool ValidateEmailAddress(
      string text,
      string fieldName,
      IDictionary<string, InvalidReasons> invalidFields)
    {
      return FieldValidator.ValidateMatchesRegex(text, fieldName, IcmConstants.Regex.EmailAddress, invalidFields);
    }
  }
}
