// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmTemporalTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmTemporalTypeReference : 
    EdmPrimitiveTypeReference,
    IEdmTemporalTypeReference,
    IEdmPrimitiveTypeReference,
    IEdmTypeReference,
    IEdmElement
  {
    private readonly int? precision;

    public EdmTemporalTypeReference(IEdmPrimitiveType definition, bool isNullable)
      : this(definition, isNullable, new int?(0))
    {
    }

    public EdmTemporalTypeReference(IEdmPrimitiveType definition, bool isNullable, int? precision)
      : base(definition, isNullable)
    {
      this.precision = precision;
    }

    public int? Precision => this.precision;
  }
}
