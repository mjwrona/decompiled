// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.ArgumentCheck
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.AzureAd.Icm.Types
{
  public static class ArgumentCheck
  {
    public static void Throw(string message, string name, [CallerMemberName] string callerMethod = null, [CallerFilePath] string callerPath = null) => throw new ArgumentException(message.AppendCaller(callerMethod, callerPath), name);

    public static void ThrowIfNullEmptyOrWhiteSpace(
      string value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      switch (value)
      {
        case null:
          throw new ArgumentNullException(name, ArgumentCheck.GetCallerMessage(callerMethod, callerPath));
        case "":
          throw new ArgumentException("Cannot be empty or consist just of whitespace".AppendCaller(callerMethod, callerPath), name);
        default:
          if (value.Trim().Length != 0)
            break;
          goto case "";
      }
    }

    public static void ThrowIfNullOrEmpty(
      string value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      switch (value)
      {
        case null:
          throw new ArgumentNullException(name, ArgumentCheck.GetCallerMessage(callerMethod, callerPath));
        case "":
          throw new ArgumentException("Cannot be empty".AppendCaller(callerMethod, callerPath), name);
      }
    }

    public static void ThrowIfEmptyOrWhiteSpace(
      string value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      switch (value)
      {
        case null:
          break;
        case "":
          throw new ArgumentException("Cannot be empty or consist just of whitespace".AppendCaller(callerMethod, callerPath), name);
        default:
          if (value.Trim().Length != 0)
            break;
          goto case "";
      }
    }

    public static void ThrowIfNotNullOrEmpty(
      string value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (!string.IsNullOrEmpty(value))
        throw new ArgumentException("Must be null or empty".AppendCaller(callerMethod, callerPath), name);
    }

    public static void ThrowIfNull(
      object value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (value == null)
        throw new ArgumentNullException(ArgumentCheck.GetCallerMessage(callerMethod, callerPath), name);
    }

    public static void ThrowIfNotNull(
      object value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (value != null)
        throw new ArgumentException("Must be null".AppendCaller(callerMethod, callerPath), name);
    }

    public static void ThrowIfFalse(
      bool expression,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (!expression)
        throw new ArgumentOutOfRangeException(name, "expression must be true".AppendCaller(callerMethod, callerPath));
    }

    public static void ThrowIfTrue(
      bool expression,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (expression)
        throw new ArgumentOutOfRangeException(name, "expression must be false".AppendCaller(callerMethod, callerPath));
    }

    public static void ThrowIfStartAfterEnd(
      DateTime start,
      DateTime end,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (start > end)
        throw new ArgumentOutOfRangeException(name, "Start date must be before end date".AppendCaller(callerMethod, callerPath));
    }

    public static void ThrowIfLessThan<T>(
      T value,
      T minValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : IComparable<T>
    {
      if (ParameterValidator.IsLessThan<T>(value, minValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} is less than the minimum value of {1}", (object) value, (object) minValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfLessThan<T>(
      T? value,
      T minValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct, IComparable<T>
    {
      if (value.HasValue && value.Value.CompareTo(minValue) < 0)
      {
        string message = TypeUtility.Format("Parameter value of {0} is less than the minimum value of {1}", (object) value.Value, (object) minValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfLessThanOrEqualTo<T>(
      T value,
      T minValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : IComparable<T>
    {
      if (ParameterValidator.IsLessThanOrEqualTo<T>(value, minValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} is less than or equal to the minimum value of {1}", (object) value, (object) minValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfLessThanOrEqualTo<T>(
      T? value,
      T minValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct, IComparable<T>
    {
      if (value.HasValue && value.Value.CompareTo(minValue) <= 0)
      {
        string message = TypeUtility.Format("Parameter value of {0} is less than or equal to the minimum value of {1}", (object) value.Value, (object) minValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfGreaterThan<T>(
      T value,
      T maxValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : IComparable<T>
    {
      if (ParameterValidator.IsGreaterThan<T>(value, maxValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} is greater than the maximum value of {1}", (object) value, (object) maxValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfGreaterThan<T>(
      T? value,
      T maxValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct, IComparable<T>
    {
      if (value.HasValue && value.Value.CompareTo(maxValue) > 0)
      {
        string message = TypeUtility.Format("Parameter value of {0} is greater than the maximum value of {1}", (object) value.Value, (object) maxValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfGreaterThanOrEqualTo<T>(
      T value,
      T maxValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : IComparable<T>
    {
      if (value.CompareTo(maxValue) >= 0)
      {
        string message = TypeUtility.Format("Parameter value of {0} is greater than or equal to the maximum value of {1}", (object) value, (object) maxValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfGreaterThanOrEqualTo<T>(
      T? value,
      T maxValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct, IComparable<T>
    {
      if (value.HasValue && value.Value.CompareTo(maxValue) >= 0)
      {
        string message = TypeUtility.Format("Parameter value of {0} is greater than or equal to the maximum value of {1}", (object) value.Value, (object) maxValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfNotInRangeExclusive<T>(
      T value,
      T minValue,
      T maxValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : IComparable<T>
    {
      if (ParameterValidator.IsNotInRangeExclusive<T>(value, minValue, maxValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} must be between {1} and {2} (excluding the boundary values", (object) value, (object) minValue, (object) maxValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfNotInRangeExclusive<T>(
      T? value,
      T minValue,
      T maxValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct, IComparable<T>
    {
      if (ParameterValidator.IsNotInRangeExclusive<T>(value, minValue, maxValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} must be between {1} and {2} (excluding the boundary values", (object) value.GetValueOrDefault(), (object) minValue, (object) maxValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfNotInRangeInclusive<T>(
      T value,
      T minValue,
      T maxValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : IComparable<T>
    {
      if (ParameterValidator.IsNotInRangeInclusive<T>(value, minValue, maxValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} must be between {1} and {2} (including the boundary values", (object) value, (object) minValue, (object) maxValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfNotInRangeInclusive<T>(
      T? value,
      T minValue,
      T maxValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct, IComparable<T>
    {
      if (ParameterValidator.IsNotInRangeInclusive<T>(value, minValue, maxValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} must be between {1} and {2} (including the boundary values", (object) value.GetValueOrDefault(), (object) minValue, (object) maxValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfNotEqualTo<T>(
      T value,
      T requiredValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (!value.Equals((object) requiredValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} is not equal to {1}", (object) value, (object) requiredValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfNotEqualTo<T>(
      T? value,
      T requiredValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct
    {
      if (value.HasValue && !value.Value.Equals((object) requiredValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} is not equal to {1}", (object) value, (object) requiredValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfEqualTo<T>(
      T value,
      T forbiddenValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (value.Equals((object) forbiddenValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} should not be equal to {1}", (object) value, (object) forbiddenValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfEqualTo<T>(
      T? value,
      T forbiddenValue,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct
    {
      if (value.HasValue && value.Value.Equals((object) forbiddenValue))
      {
        string message = TypeUtility.Format("Parameter value of {0} is equal to {1}", (object) value, (object) forbiddenValue);
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfHasValue<T>(
      T? value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct
    {
      if (value.HasValue)
      {
        string message = TypeUtility.Format("Parameter has value");
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfHasNoValue<T>(
      T? value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : struct
    {
      if (!value.HasValue)
      {
        string message = TypeUtility.Format("Parameter has no value");
        throw new ArgumentOutOfRangeException(name, message.AppendCaller(callerMethod, callerPath));
      }
    }

    public static void ThrowIfCollectionNullOrEmpty<T>(
      IEnumerable<T> selectColumns,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
    {
      if (selectColumns == null || selectColumns.Equals((object) null) || !selectColumns.Any<T>())
        throw new ArgumentException((name + " cannot be empty or null").AppendCaller(callerMethod, callerPath));
    }

    public static void ThrowIfCollectionNotNullOrEmpty<T>(
      IEnumerable<T> value,
      string name,
      [CallerMemberName] string callerMethod = null,
      [CallerFilePath] string callerPath = null)
      where T : class
    {
      if (value != null && !value.Equals((object) null) && value.Any<T>())
        throw new ArgumentException((name + " is neither null or empty").AppendCaller(callerMethod, callerPath));
    }

    public static void ThrowIfNotUtc(DateTimeOffset date, [CallerMemberName] string callerMethod = null, [CallerFilePath] string callerPath = null)
    {
      if (date.Offset != TimeSpan.Zero)
        throw new ArgumentException("Expecting date in UTC.".AppendCaller(callerMethod, callerPath));
    }

    private static string AppendCaller(this string message, string callerMethod, string callerFile) => message + " " + ArgumentCheck.GetCallerMessage(callerMethod, callerFile);

    private static string GetCallerMessage(string callerMethod, string callerFile) => TypeUtility.Format("Occurred in {0}.{1}", (object) Path.GetFileNameWithoutExtension(callerFile), (object) callerMethod);
  }
}
