// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmBooleanConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmBooleanConstant : 
    EdmValue,
    IEdmBooleanConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmBooleanValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly bool value;

    public EdmBooleanConstant(bool value)
      : this((IEdmPrimitiveTypeReference) null, value)
    {
    }

    public EdmBooleanConstant(IEdmPrimitiveTypeReference type, bool value)
      : base((IEdmTypeReference) type)
    {
      this.value = value;
    }

    public bool Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.BooleanConstant;

    public override EdmValueKind ValueKind => EdmValueKind.Boolean;
  }
}
