// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmGuidConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmGuidConstant : 
    EdmValue,
    IEdmGuidConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmGuidValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly Guid value;

    public EdmGuidConstant(Guid value)
      : this((IEdmPrimitiveTypeReference) null, value)
    {
      this.value = value;
    }

    public EdmGuidConstant(IEdmPrimitiveTypeReference type, Guid value)
      : base((IEdmTypeReference) type)
    {
      this.value = value;
    }

    public Guid Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.GuidConstant;

    public override EdmValueKind ValueKind => EdmValueKind.Guid;
  }
}
