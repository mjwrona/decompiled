// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmIntegerConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmIntegerConstant : 
    EdmValue,
    IEdmIntegerConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmIntegerValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly long value;

    public EdmIntegerConstant(long value)
      : this((IEdmPrimitiveTypeReference) null, value)
    {
      this.value = value;
    }

    public EdmIntegerConstant(IEdmPrimitiveTypeReference type, long value)
      : base((IEdmTypeReference) type)
    {
      this.value = value;
    }

    public long Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.IntegerConstant;

    public override EdmValueKind ValueKind => EdmValueKind.Integer;
  }
}
