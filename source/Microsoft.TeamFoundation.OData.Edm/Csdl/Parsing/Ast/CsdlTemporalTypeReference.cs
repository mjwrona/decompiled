// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlTemporalTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlTemporalTypeReference : CsdlPrimitiveTypeReference
  {
    public CsdlTemporalTypeReference(
      EdmPrimitiveTypeKind kind,
      int? precision,
      string typeName,
      bool isNullable,
      CsdlLocation location)
      : base(kind, typeName, isNullable, location)
    {
      this.Precision = precision;
    }
  }
}
