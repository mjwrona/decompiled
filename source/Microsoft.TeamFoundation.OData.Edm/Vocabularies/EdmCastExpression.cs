// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmCastExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmCastExpression : EdmElement, IEdmCastExpression, IEdmExpression, IEdmElement
  {
    private readonly IEdmExpression operand;
    private readonly IEdmTypeReference type;

    public EdmCastExpression(IEdmExpression operand, IEdmTypeReference type)
    {
      EdmUtil.CheckArgumentNull<IEdmExpression>(operand, nameof (operand));
      EdmUtil.CheckArgumentNull<IEdmTypeReference>(type, nameof (type));
      this.operand = operand;
      this.type = type;
    }

    public IEdmExpression Operand => this.operand;

    public IEdmTypeReference Type => this.type;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.Cast;
  }
}
