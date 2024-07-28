// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlNamedTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlNamedTypeReference : CsdlTypeReference
  {
    public CsdlNamedTypeReference(string fullName, bool isNullable, CsdlLocation location)
      : this(false, new int?(), new bool?(), new int?(), new int?(), new int?(), fullName, isNullable, location)
    {
    }

    public CsdlNamedTypeReference(
      bool isUnbounded,
      int? maxLength,
      bool? isUnicode,
      int? precision,
      int? scale,
      int? spatialReferenceIdentifier,
      string fullName,
      bool isNullable,
      CsdlLocation location)
      : base(isNullable, location)
    {
      this.IsUnbounded = isUnbounded;
      this.MaxLength = maxLength;
      this.IsUnicode = isUnicode;
      this.Precision = precision;
      this.Scale = scale;
      this.SpatialReferenceIdentifier = spatialReferenceIdentifier;
      this.FullName = fullName;
    }

    public bool IsUnbounded { get; protected set; }

    public int? MaxLength { get; protected set; }

    public bool? IsUnicode { get; protected set; }

    public int? Precision { get; protected set; }

    public int? Scale { get; protected set; }

    public int? SpatialReferenceIdentifier { get; protected set; }

    public string FullName { get; protected set; }
  }
}
