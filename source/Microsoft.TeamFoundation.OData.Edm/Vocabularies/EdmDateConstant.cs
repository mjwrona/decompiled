// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmDateConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmDateConstant : 
    EdmValue,
    IEdmDateConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmDateValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly Date value;

    public EdmDateConstant(Date value)
      : this((IEdmPrimitiveTypeReference) null, value)
    {
    }

    public EdmDateConstant(IEdmPrimitiveTypeReference type, Date value)
      : base((IEdmTypeReference) type)
    {
      this.value = value;
    }

    public Date Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.DateConstant;

    public override EdmValueKind ValueKind => EdmValueKind.Date;
  }
}
