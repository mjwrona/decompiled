// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.CollectionTypeConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;
using System.Globalization;

namespace Microsoft.AspNet.OData.Builder
{
  public class CollectionTypeConfiguration : IEdmTypeConfiguration
  {
    private IEdmTypeConfiguration _elementType;
    private Type _clrType;

    public CollectionTypeConfiguration(IEdmTypeConfiguration elementType, Type clrType)
    {
      if (elementType == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (elementType));
      if (clrType == (Type) null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (clrType));
      this._elementType = elementType;
      this._clrType = clrType;
    }

    public IEdmTypeConfiguration ElementType => this._elementType;

    public Type ClrType => this._clrType;

    public string FullName => this.Name;

    public string Namespace => "Edm";

    public string Name => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Collection({0})", new object[1]
    {
      (object) this.ElementType.FullName
    });

    public EdmTypeKind Kind => EdmTypeKind.Collection;

    public ODataModelBuilder ModelBuilder => this._elementType.ModelBuilder;
  }
}
