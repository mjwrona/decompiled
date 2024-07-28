// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.PrimitiveTypeConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData.Builder
{
  public class PrimitiveTypeConfiguration : IEdmTypeConfiguration
  {
    private Type _clrType;
    private IEdmPrimitiveType _edmType;
    private ODataModelBuilder _builder;

    public PrimitiveTypeConfiguration(
      ODataModelBuilder builder,
      IEdmPrimitiveType edmType,
      Type clrType)
    {
      if (builder == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (builder));
      if (edmType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      if (clrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (clrType));
      this._builder = builder;
      this._clrType = clrType;
      this._edmType = edmType;
    }

    public Type ClrType => this._clrType;

    public string FullName => this._edmType.FullName();

    public string Namespace => this._edmType.Namespace;

    public string Name => this._edmType.Name;

    public EdmTypeKind Kind => EdmTypeKind.Primitive;

    public ODataModelBuilder ModelBuilder => this._builder;

    public IEdmPrimitiveType EdmPrimitiveType => this._edmType;
  }
}
