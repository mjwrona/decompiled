// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.Internal.PropertyValidation
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PropertyValidation
  {
    private const int c_maxPropertyNameLengthInChars = 400;
    private const int c_maxByteValueSize = 8388608;
    private const int c_maxStringValueLength = 4194304;
    private static readonly DateTime s_minAllowedDateTime = new DateTime(1753, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime s_maxAllowedDateTime = DateTime.SpecifyKind(DateTime.MaxValue, DateTimeKind.Utc).AddDays(-1.0);
    private static double s_minNegative = double.Parse("-1.79E+308", (IFormatProvider) CultureInfo.InvariantCulture);
    private static double s_maxNegative = double.Parse("-2.23E-308", (IFormatProvider) CultureInfo.InvariantCulture);
    private static double s_minPositive = double.Parse("2.23E-308", (IFormatProvider) CultureInfo.InvariantCulture);
    private static double s_maxPositive = double.Parse("1.79E+308", (IFormatProvider) CultureInfo.InvariantCulture);
    private static readonly Dictionary<string, Type> s_validPropertyTypeStrings = new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "System.Boolean",
        typeof (bool)
      },
      {
        "System.Byte",
        typeof (byte)
      },
      {
        "System.Char",
        typeof (char)
      },
      {
        "System.DateTime",
        typeof (DateTime)
      },
      {
        "System.Decimal",
        typeof (Decimal)
      },
      {
        "System.Double",
        typeof (double)
      },
      {
        "System.Int16",
        typeof (short)
      },
      {
        "System.Int32",
        typeof (int)
      },
      {
        "System.Int64",
        typeof (long)
      },
      {
        "System.SByte",
        typeof (sbyte)
      },
      {
        "System.Single",
        typeof (float)
      },
      {
        "System.String",
        typeof (string)
      },
      {
        "System.UInt16",
        typeof (ushort)
      },
      {
        "System.UInt32",
        typeof (uint)
      },
      {
        "System.UInt64",
        typeof (ulong)
      },
      {
        "System.Byte[]",
        typeof (byte[])
      },
      {
        "System.Guid",
        typeof (Guid)
      }
    };

    public static void ValidateDictionary(IDictionary<string, object> source)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(source, nameof (source));
      foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) source)
      {
        PropertyValidation.ValidatePropertyName(keyValuePair.Key);
        PropertyValidation.ValidatePropertyValue(keyValuePair.Key, keyValuePair.Value);
      }
    }

    public static bool IsValidConvertibleType(Type type)
    {
      if (!(type != (Type) null))
        return false;
      return type.GetTypeInfo().IsEnum || type == typeof (object) || type == typeof (byte[]) || type == typeof (Guid) || type == typeof (bool) || type == typeof (char) || type == typeof (sbyte) || type == typeof (byte) || type == typeof (short) || type == typeof (ushort) || type == typeof (int) || type == typeof (uint) || type == typeof (long) || type == typeof (ulong) || type == typeof (float) || type == typeof (double) || type == typeof (Decimal) || type == typeof (DateTime) || type == typeof (string);
    }

    public static bool IsValidTypeString(string type) => PropertyValidation.s_validPropertyTypeStrings.ContainsKey(type);

    public static bool TryGetValidType(string type, out Type result) => PropertyValidation.s_validPropertyTypeStrings.TryGetValue(type, out result);

    public static void ValidatePropertyName(string propertyName)
    {
      PropertyValidation.ValidatePropertyString(propertyName, 400, nameof (propertyName));
      if (char.IsWhiteSpace(propertyName[0]) || char.IsWhiteSpace(propertyName[propertyName.Length - 1]))
        throw new VssPropertyValidationException(propertyName, CommonResources.InvalidPropertyName((object) propertyName));
    }

    public static void ValidatePropertyValue(string propertyName, object value)
    {
      if (value == null)
        return;
      Type type = value.GetType();
      TypeCode typeCode = Type.GetTypeCode(type);
      if (type.IsEnum)
        PropertyValidation.ValidateStringValue(propertyName, ((Enum) value).ToString("D"));
      else if (typeCode == TypeCode.Object && value is byte[])
        PropertyValidation.ValidateByteArray(propertyName, (byte[]) value);
      else if (typeCode == TypeCode.Object && value is Guid guid)
      {
        PropertyValidation.ValidateStringValue(propertyName, guid.ToString("N"));
      }
      else
      {
        switch (typeCode)
        {
          case TypeCode.Empty:
            throw new PropertyTypeNotSupportedException(propertyName, type);
          case TypeCode.Object:
            throw new PropertyTypeNotSupportedException(propertyName, type);
          case TypeCode.DBNull:
            throw new PropertyTypeNotSupportedException(propertyName, type);
          case TypeCode.Int32:
            PropertyValidation.ValidateInt32(propertyName, (int) value);
            break;
          case TypeCode.Double:
            PropertyValidation.ValidateDouble(propertyName, (double) value);
            break;
          case TypeCode.DateTime:
            PropertyValidation.ValidateDateTime(propertyName, (DateTime) value);
            break;
          case TypeCode.String:
            PropertyValidation.ValidateStringValue(propertyName, (string) value);
            break;
          default:
            PropertyValidation.ValidateStringValue(propertyName, value.ToString());
            break;
        }
      }
    }

    private static void ValidateStringValue(string propertyName, string propertyValue)
    {
      if (propertyValue.Length > 4194304)
        throw new VssPropertyValidationException("value", CommonResources.InvalidPropertyValueSize((object) propertyName, (object) typeof (string).FullName, (object) 4194304));
      bool isChineseOs = OSDetails.IsChineseOS;
      ArgumentUtility.CheckStringForInvalidCharacters(propertyValue, "value", true, isChineseOs || propertyName.Equals("Description", StringComparison.Ordinal) || propertyName.Equals("being serialized", StringComparison.Ordinal) || propertyName.Equals("Microsoft.VisualStudio.Services.CodeReview.ItemPath", StringComparison.Ordinal) || propertyName.StartsWith("Microsoft.TeamFoundation.Framework.Server.IdentityFavorites", StringComparison.Ordinal), (string) null);
    }

    private static void ValidateByteArray(string propertyName, byte[] propertyValue)
    {
      if (propertyValue.Length > 8388608)
        throw new VssPropertyValidationException("value", CommonResources.InvalidPropertyValueSize((object) propertyName, (object) typeof (byte[]).FullName, (object) 8388608));
    }

    private static void ValidateDateTime(string propertyName, DateTime propertyValue)
    {
      if (propertyValue != DateTime.MinValue && propertyValue != DateTime.MaxValue)
      {
        if (propertyValue.Kind == DateTimeKind.Unspecified)
          throw new VssPropertyValidationException("value", CommonResources.DateTimeKindMustBeSpecified());
        if (propertyValue.Kind != DateTimeKind.Utc)
          propertyValue = propertyValue.ToUniversalTime();
      }
      PropertyValidation.CheckRange<DateTime>(propertyValue, PropertyValidation.s_minAllowedDateTime, PropertyValidation.s_maxAllowedDateTime, propertyName, "value");
    }

    private static void ValidateDouble(string propertyName, double propertyValue)
    {
      if (double.IsInfinity(propertyValue) || double.IsNaN(propertyValue))
        throw new VssPropertyValidationException("value", CommonResources.DoubleValueOutOfRange((object) propertyName, (object) propertyValue));
      if (propertyValue < PropertyValidation.s_minNegative || propertyValue < 0.0 && propertyValue > PropertyValidation.s_maxNegative || propertyValue > PropertyValidation.s_maxPositive || propertyValue > 0.0 && propertyValue < PropertyValidation.s_minPositive)
        throw new VssPropertyValidationException("value", CommonResources.DoubleValueOutOfRange((object) propertyName, (object) propertyValue));
    }

    private static void ValidateInt32(string propertyName, int propertyValue)
    {
    }

    private static void ValidatePropertyString(
      string propertyString,
      int maxSize,
      string argumentName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(propertyString, argumentName);
      if (propertyString.Length > maxSize)
        throw new VssPropertyValidationException(argumentName, CommonResources.PropertyArgumentExceededMaximumSizeAllowed((object) argumentName, (object) maxSize));
      ArgumentUtility.CheckStringForInvalidCharacters(propertyString, argumentName, true, OSDetails.IsChineseOS, (string) null);
    }

    public static void CheckPropertyLength(
      string propertyValue,
      bool allowNull,
      int minLength,
      int maxLength,
      string propertyName,
      Type containerType,
      string topLevelParamName)
    {
      bool flag = false;
      if (propertyValue == null)
      {
        if (!allowNull)
          flag = true;
      }
      else if (propertyValue.Length < minLength || propertyValue.Length > maxLength)
        flag = true;
      if (!flag)
        return;
      if (propertyValue == null)
        propertyValue = string.Empty;
      if (allowNull)
        throw new ArgumentException(CommonResources.InvalidStringPropertyValueNullAllowed((object) propertyValue, (object) propertyName, (object) containerType.Name, (object) minLength, (object) maxLength), topLevelParamName);
      throw new ArgumentException(CommonResources.InvalidStringPropertyValueNullForbidden((object) propertyValue, (object) propertyName, (object) containerType.Name, (object) minLength, (object) maxLength), topLevelParamName);
    }

    public static void CheckRange<T>(
      T propertyValue,
      T minValue,
      T maxValue,
      string propertyName,
      Type containerType,
      string topLevelParamName)
      where T : IComparable<T>
    {
      if (propertyValue.CompareTo(minValue) < 0 || propertyValue.CompareTo(maxValue) > 0)
        throw new ArgumentOutOfRangeException(topLevelParamName, CommonResources.ValueTypeOutOfRange((object) propertyValue, (object) propertyName, (object) containerType.Name, (object) minValue, (object) maxValue));
    }

    private static void CheckRange<T>(
      T propertyValue,
      T minValue,
      T maxValue,
      string propertyName,
      string topLevelParamName)
      where T : IComparable<T>
    {
      if (propertyValue.CompareTo(minValue) < 0 || propertyValue.CompareTo(maxValue) > 0)
        throw new ArgumentOutOfRangeException(topLevelParamName, CommonResources.VssPropertyValueOutOfRange((object) propertyName, (object) propertyValue, (object) minValue, (object) maxValue));
    }

    public static void ValidatePropertyFilter(string propertyNameFilter) => PropertyValidation.ValidatePropertyString(propertyNameFilter, 400, nameof (propertyNameFilter));
  }
}
