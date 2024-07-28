// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmSpatialTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmSpatialTypeReference : 
    EdmPrimitiveTypeReference,
    IEdmSpatialTypeReference,
    IEdmPrimitiveTypeReference,
    IEdmTypeReference,
    IEdmElement
  {
    private readonly int? spatialReferenceIdentifier;

    public EdmSpatialTypeReference(IEdmPrimitiveType definition, bool isNullable)
      : this(definition, isNullable, new int?())
    {
      EdmUtil.CheckArgumentNull<IEdmPrimitiveType>(definition, nameof (definition));
      switch (definition.PrimitiveKind)
      {
        case EdmPrimitiveTypeKind.Geography:
        case EdmPrimitiveTypeKind.GeographyPoint:
        case EdmPrimitiveTypeKind.GeographyLineString:
        case EdmPrimitiveTypeKind.GeographyPolygon:
        case EdmPrimitiveTypeKind.GeographyCollection:
        case EdmPrimitiveTypeKind.GeographyMultiPolygon:
        case EdmPrimitiveTypeKind.GeographyMultiLineString:
        case EdmPrimitiveTypeKind.GeographyMultiPoint:
          this.spatialReferenceIdentifier = new int?(4326);
          break;
        default:
          this.spatialReferenceIdentifier = new int?(0);
          break;
      }
    }

    public EdmSpatialTypeReference(
      IEdmPrimitiveType definition,
      bool isNullable,
      int? spatialReferenceIdentifier)
      : base(definition, isNullable)
    {
      this.spatialReferenceIdentifier = spatialReferenceIdentifier;
    }

    public int? SpatialReferenceIdentifier => this.spatialReferenceIdentifier;
  }
}
