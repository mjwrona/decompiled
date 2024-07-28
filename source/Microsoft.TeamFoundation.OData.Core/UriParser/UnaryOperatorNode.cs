// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UnaryOperatorNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class UnaryOperatorNode : SingleValueNode
  {
    private readonly SingleValueNode operand;
    private readonly UnaryOperatorKind operatorKind;
    private IEdmTypeReference typeReference;

    public UnaryOperatorNode(UnaryOperatorKind operatorKind, SingleValueNode operand)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(operand, nameof (operand));
      this.operand = operand;
      this.operatorKind = operatorKind;
      if (operand == null || operand.TypeReference == null)
        this.typeReference = (IEdmTypeReference) null;
      else
        this.typeReference = operand.TypeReference;
    }

    public UnaryOperatorKind OperatorKind => this.operatorKind;

    public SingleValueNode Operand => this.operand;

    public override IEdmTypeReference TypeReference => this.typeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.UnaryOperator;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
