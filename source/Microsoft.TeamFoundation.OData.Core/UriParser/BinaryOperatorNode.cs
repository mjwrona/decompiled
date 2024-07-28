// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.BinaryOperatorNode
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class BinaryOperatorNode : SingleValueNode
  {
    private readonly BinaryOperatorKind operatorKind;
    private readonly SingleValueNode left;
    private readonly SingleValueNode right;
    private IEdmTypeReference typeReference;

    public BinaryOperatorNode(
      BinaryOperatorKind operatorKind,
      SingleValueNode left,
      SingleValueNode right)
      : this(operatorKind, left, right, (IEdmTypeReference) null)
    {
    }

    internal BinaryOperatorNode(
      BinaryOperatorKind operatorKind,
      SingleValueNode left,
      SingleValueNode right,
      IEdmTypeReference typeReference)
    {
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(left, nameof (left));
      ExceptionUtils.CheckArgumentNotNull<SingleValueNode>(right, nameof (right));
      this.operatorKind = operatorKind;
      this.left = left;
      this.right = right;
      if (typeReference != null)
        this.typeReference = typeReference;
      else if (this.Left == null || this.Right == null || this.Left.TypeReference == null || this.Right.TypeReference == null)
        this.typeReference = (IEdmTypeReference) null;
      else
        this.typeReference = (IEdmTypeReference) QueryNodeUtils.GetBinaryOperatorResultType(this.Left.TypeReference.AsPrimitive(), this.Right.TypeReference.AsPrimitive(), this.OperatorKind);
    }

    public BinaryOperatorKind OperatorKind => this.operatorKind;

    public SingleValueNode Left => this.left;

    public SingleValueNode Right => this.right;

    public override IEdmTypeReference TypeReference => this.typeReference;

    internal override InternalQueryNodeKind InternalKind => InternalQueryNodeKind.BinaryOperator;

    public override T Accept<T>(QueryNodeVisitor<T> visitor)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryNodeVisitor<T>>(visitor, nameof (visitor));
      return visitor.Visit(this);
    }
  }
}
