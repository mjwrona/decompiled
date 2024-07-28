// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.PrimitivePropertyConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public class PrimitivePropertyConfiguration : StructuralPropertyConfiguration
  {
    public PrimitivePropertyConfiguration(
      PropertyInfo property,
      StructuralTypeConfiguration declaringType)
      : base(property, declaringType)
    {
    }

    public string DefaultValueString { get; set; }

    public override PropertyKind Kind => PropertyKind.Primitive;

    public override Type RelatedClrType => this.PropertyInfo.PropertyType;

    public EdmPrimitiveTypeKind? TargetEdmTypeKind { get; internal set; }

    public PrimitivePropertyConfiguration IsOptional()
    {
      this.OptionalProperty = true;
      return this;
    }

    public PrimitivePropertyConfiguration IsRequired()
    {
      this.OptionalProperty = false;
      return this;
    }

    public PrimitivePropertyConfiguration IsConcurrencyToken()
    {
      this.ConcurrencyToken = true;
      return this;
    }
  }
}
