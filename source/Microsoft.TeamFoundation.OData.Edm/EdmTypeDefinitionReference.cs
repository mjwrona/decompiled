// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmTypeDefinitionReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmTypeDefinitionReference : 
    EdmTypeReference,
    IEdmTypeDefinitionReference,
    IEdmTypeReference,
    IEdmElement
  {
    public EdmTypeDefinitionReference(IEdmTypeDefinition typeDefinition, bool isNullable)
      : base((IEdmType) typeDefinition, isNullable)
    {
      this.IsUnbounded = false;
      this.MaxLength = new int?();
      this.IsUnicode = EdmTypeDefinitionReference.ComputeDefaultIsUnicode(typeDefinition);
      this.Precision = EdmTypeDefinitionReference.ComputeDefaultPrecision(typeDefinition);
      this.Scale = EdmTypeDefinitionReference.ComputeDefaultScale(typeDefinition);
      this.SpatialReferenceIdentifier = EdmTypeDefinitionReference.ComputeSrid(typeDefinition);
    }

    public EdmTypeDefinitionReference(
      IEdmTypeDefinition typeDefinition,
      bool isNullable,
      bool isUnbounded,
      int? maxLength,
      bool? isUnicode,
      int? precision,
      int? scale,
      int? spatialReferenceIdentifier)
      : base((IEdmType) typeDefinition, isNullable)
    {
      this.IsUnbounded = isUnbounded;
      this.MaxLength = maxLength;
      this.IsUnicode = isUnicode;
      this.Precision = precision;
      this.Scale = scale;
      this.SpatialReferenceIdentifier = spatialReferenceIdentifier;
    }

    public bool IsUnbounded { get; private set; }

    public int? MaxLength { get; private set; }

    public bool? IsUnicode { get; private set; }

    public int? Precision { get; private set; }

    public int? Scale { get; private set; }

    public int? SpatialReferenceIdentifier { get; private set; }

    private static bool? ComputeDefaultIsUnicode(IEdmTypeDefinition typeDefinition) => typeDefinition.UnderlyingType.IsString() ? new bool?(true) : new bool?();

    private static int? ComputeDefaultPrecision(IEdmTypeDefinition typeDefinition) => typeDefinition.UnderlyingType.IsTemporal() ? new int?(0) : new int?();

    private static int? ComputeDefaultScale(IEdmTypeDefinition typeDefinition) => typeDefinition.UnderlyingType.IsDecimal() ? new int?(0) : new int?();

    private static int? ComputeSrid(IEdmTypeDefinition typeDefinition)
    {
      if (typeDefinition.UnderlyingType.IsGeography())
        return new int?(4326);
      return typeDefinition.UnderlyingType.IsGeometry() ? new int?(0) : new int?();
    }
  }
}
