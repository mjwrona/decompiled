// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.PrimitivePropertyConfigurationExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData.Builder
{
  public static class PrimitivePropertyConfigurationExtensions
  {
    public static PrimitivePropertyConfiguration AsDate(this PrimitivePropertyConfiguration property)
    {
      if (property == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (property));
      if (!TypeHelper.IsDateTime(property.RelatedClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (property), SRResources.MustBeDateTimeProperty, (object) property.PropertyInfo.Name, (object) property.DeclaringType.FullName);
      property.TargetEdmTypeKind = new EdmPrimitiveTypeKind?(EdmPrimitiveTypeKind.Date);
      return property;
    }

    public static PrimitivePropertyConfiguration AsTimeOfDay(
      this PrimitivePropertyConfiguration property)
    {
      if (property == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (property));
      if (!TypeHelper.IsTimeSpan(property.RelatedClrType))
        throw Microsoft.AspNet.OData.Common.Error.Argument(nameof (property), SRResources.MustBeTimeSpanProperty, (object) property.PropertyInfo.Name, (object) property.DeclaringType.FullName);
      property.TargetEdmTypeKind = new EdmPrimitiveTypeKind?(EdmPrimitiveTypeKind.TimeOfDay);
      return property;
    }
  }
}
