// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmStringConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmStringConstant : 
    EdmValue,
    IEdmStringConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmStringValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly string value;

    public EdmStringConstant(string value)
      : this((IEdmStringTypeReference) null, value)
    {
    }

    public EdmStringConstant(IEdmStringTypeReference type, string value)
      : base((IEdmTypeReference) type)
    {
      EdmUtil.CheckArgumentNull<string>(value, nameof (value));
      this.value = value;
    }

    public string Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.StringConstant;

    public override EdmValueKind ValueKind => EdmValueKind.String;
  }
}
