// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.EntityPropertyExtensions
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class EntityPropertyExtensions
  {
    public static bool IsNull(this EntityProperty property)
    {
      switch (property.PropertyType)
      {
        case EdmType.String:
          return property.StringValue == null;
        case EdmType.Binary:
          return property.BinaryValue == null;
        case EdmType.Boolean:
          return !property.BooleanValue.HasValue;
        case EdmType.DateTime:
          return !property.DateTime.HasValue;
        case EdmType.Double:
          return !property.DoubleValue.HasValue;
        case EdmType.Guid:
          return !property.GuidValue.HasValue;
        case EdmType.Int32:
          return !property.Int32Value.HasValue;
        case EdmType.Int64:
          return !property.Int64Value.HasValue;
        default:
          throw new Exception(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected Edm type value {0}", (object) property.PropertyType));
      }
    }
  }
}
