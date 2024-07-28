// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.StructuralPropertyConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Formatter;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public abstract class StructuralPropertyConfiguration : PropertyConfiguration
  {
    protected StructuralPropertyConfiguration(
      PropertyInfo property,
      StructuralTypeConfiguration declaringType)
      : base(property, declaringType)
    {
      this.OptionalProperty = EdmLibHelpers.IsNullable(property.PropertyType);
    }

    public bool OptionalProperty { get; set; }

    public bool ConcurrencyToken { get; set; }
  }
}
