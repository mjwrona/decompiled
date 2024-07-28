// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.CollectionPropertyConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public class CollectionPropertyConfiguration : StructuralPropertyConfiguration
  {
    private Type _elementType;

    public CollectionPropertyConfiguration(
      PropertyInfo property,
      StructuralTypeConfiguration declaringType)
      : base(property, declaringType)
    {
      if (!TypeHelper.IsCollection(property.PropertyType, out this._elementType))
        throw Error.Argument(nameof (property), SRResources.CollectionPropertiesMustReturnIEnumerable, (object) property.Name, (object) property.DeclaringType.FullName);
    }

    public override PropertyKind Kind => PropertyKind.Collection;

    public override Type RelatedClrType => this.ElementType;

    public Type ElementType => this._elementType;

    public CollectionPropertyConfiguration IsOptional()
    {
      this.OptionalProperty = true;
      return this;
    }

    public CollectionPropertyConfiguration IsRequired()
    {
      this.OptionalProperty = false;
      return this;
    }
  }
}
