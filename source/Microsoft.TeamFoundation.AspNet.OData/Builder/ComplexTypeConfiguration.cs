// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.ComplexTypeConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  public class ComplexTypeConfiguration : StructuralTypeConfiguration
  {
    public ComplexTypeConfiguration()
    {
    }

    public ComplexTypeConfiguration(ODataModelBuilder modelBuilder, Type clrType)
      : base(modelBuilder, clrType)
    {
    }

    public override EdmTypeKind Kind => EdmTypeKind.Complex;

    public virtual ComplexTypeConfiguration BaseType
    {
      get => this.BaseTypeInternal as ComplexTypeConfiguration;
      set => this.DerivesFrom(value);
    }

    public virtual ComplexTypeConfiguration Abstract()
    {
      this.AbstractImpl();
      return this;
    }

    public virtual ComplexTypeConfiguration DerivesFromNothing()
    {
      this.DerivesFromNothingImpl();
      return this;
    }

    public virtual ComplexTypeConfiguration DerivesFrom(ComplexTypeConfiguration baseType)
    {
      this.DerivesFromImpl((StructuralTypeConfiguration) baseType);
      return this;
    }
  }
}
