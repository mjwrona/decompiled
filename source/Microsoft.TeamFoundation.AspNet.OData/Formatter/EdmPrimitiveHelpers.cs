// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.EdmPrimitiveHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;
using System.Globalization;
using System.Xml.Linq;

namespace Microsoft.AspNet.OData.Formatter
{
  internal static class EdmPrimitiveHelpers
  {
    public static object ConvertPrimitiveValue(object value, Type type)
    {
      if (value.GetType() == type || value.GetType() == Nullable.GetUnderlyingType(type) || type.IsInstanceOfType(value))
        return value;
      string text = value as string;
      if (type == typeof (char))
        return text != null && text.Length == 1 ? (object) text[0] : throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeStringLengthOne));
      if (type == typeof (char?))
      {
        if (text == null || text.Length > 1)
          throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeStringMaxLengthOne));
        return (object) (text.Length > 0 ? new char?(text[0]) : new char?());
      }
      if (type == typeof (char[]))
        return text != null ? (object) text.ToCharArray() : throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeString));
      if (type == typeof (Binary))
        return (object) new Binary((byte[]) value);
      if (type == typeof (XElement))
        return text != null ? (object) XElement.Parse(text) : throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeString));
      Type type1 = Nullable.GetUnderlyingType(type);
      if ((object) type1 == null)
        type1 = type;
      type = type1;
      if (TypeHelper.IsEnum(type))
        return text != null ? Enum.Parse(type, text) : throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeString));
      if (type == typeof (DateTime))
      {
        switch (value)
        {
          case DateTimeOffset dateTimeOffset:
            return (object) TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfoHelper.TimeZone).DateTime;
          case Date date:
            return (object) (DateTime) date;
          default:
            throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeDateTimeOffsetOrDate));
        }
      }
      else
      {
        if (type == typeof (TimeSpan))
          return value is TimeOfDay timeOfDay ? (object) (TimeSpan) timeOfDay : throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeTimeOfDay));
        if (!(type == typeof (bool)))
          return Convert.ChangeType(value, type, (IFormatProvider) CultureInfo.InvariantCulture);
        bool result;
        if (text != null && bool.TryParse(text, out result))
          return (object) result;
        throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeBoolean));
      }
    }
  }
}
