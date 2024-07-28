// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.EnumDeserializationHelpers
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData;
using System;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  internal static class EnumDeserializationHelpers
  {
    public static object ConvertEnumValue(object value, Type type)
    {
      if (value == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (value));
      Type type1 = !(type == (Type) null) ? TypeHelper.GetUnderlyingTypeOrSelf(type) : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (type));
      if (value.GetType() == type1)
        return value;
      if (!(value is ODataEnumValue odataEnumValue))
        throw new ValidationException(Microsoft.AspNet.OData.Common.Error.Format(SRResources.PropertyMustBeEnum, (object) value.GetType().Name, (object) "ODataEnumValue"));
      if (!TypeHelper.IsEnum(type1))
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(Microsoft.AspNet.OData.Common.Error.Format(SRResources.TypeMustBeEnumOrNullableEnum, (object) type.Name));
      return Enum.Parse(type1, odataEnumValue.Value);
    }
  }
}
