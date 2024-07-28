// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmFloatingConstant
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmFloatingConstant : 
    EdmValue,
    IEdmFloatingConstantExpression,
    IEdmExpression,
    IEdmElement,
    IEdmFloatingValue,
    IEdmPrimitiveValue,
    IEdmValue
  {
    private readonly double value;

    public EdmFloatingConstant(double value)
      : this((IEdmPrimitiveTypeReference) null, value)
    {
    }

    public EdmFloatingConstant(IEdmPrimitiveTypeReference type, double value)
      : base((IEdmTypeReference) type)
    {
      this.value = value;
    }

    public double Value => this.value;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.FloatingConstant;

    public override EdmValueKind ValueKind => EdmValueKind.Floating;
  }
}
