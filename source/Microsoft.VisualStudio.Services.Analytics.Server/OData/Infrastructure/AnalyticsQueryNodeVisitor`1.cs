// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsQueryNodeVisitor`1
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.UriParser;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public abstract class AnalyticsQueryNodeVisitor<T> : QueryNodeVisitor<T>
  {
    public override T Visit(SingleComplexNode nodeIn) => default (T);

    public override T Visit(SearchTermNode nodeIn) => default (T);

    public override T Visit(ParameterAliasNode nodeIn) => default (T);

    public override T Visit(NamedFunctionParameterNode nodeIn) => default (T);

    public override T Visit(UnaryOperatorNode nodeIn) => default (T);

    public override T Visit(SingleValuePropertyAccessNode nodeIn) => default (T);

    public override T Visit(SingleValueOpenPropertyAccessNode nodeIn) => default (T);

    public override T Visit(CollectionFunctionCallNode nodeIn) => default (T);

    public override T Visit(CollectionResourceFunctionCallNode nodeIn) => default (T);

    public override T Visit(SingleValueFunctionCallNode nodeIn) => default (T);

    public override T Visit(SingleResourceFunctionCallNode nodeIn) => default (T);

    public override T Visit(CollectionComplexNode nodeIn) => default (T);

    public override T Visit(SingleNavigationNode nodeIn) => default (T);

    public override T Visit(NonResourceRangeVariableReferenceNode nodeIn) => default (T);

    public override T Visit(ResourceRangeVariableReferenceNode nodeIn) => default (T);

    public override T Visit(CollectionResourceCastNode nodeIn) => default (T);

    public override T Visit(ConvertNode nodeIn) => default (T);

    public override T Visit(ConstantNode nodeIn) => default (T);

    public override T Visit(CollectionOpenPropertyAccessNode nodeIn) => default (T);

    public override T Visit(CollectionPropertyAccessNode nodeIn) => default (T);

    public override T Visit(CollectionNavigationNode nodeIn) => default (T);

    public override T Visit(CountNode nodeIn) => default (T);

    public override T Visit(BinaryOperatorNode nodeIn) => default (T);

    public override T Visit(AnyNode nodeIn) => default (T);

    public override T Visit(SingleResourceCastNode nodeIn) => default (T);

    public override T Visit(AllNode nodeIn) => default (T);

    public override T Visit(CollectionConstantNode nodeIn) => default (T);

    public override T Visit(SingleValueCastNode nodeIn) => default (T);

    public override T Visit(AggregatedCollectionPropertyNode nodeIn) => default (T);

    public override T Visit(InNode nodeIn) => default (T);
  }
}
