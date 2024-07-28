// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.QueryNodeVisitor`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.UriParser
{
  public abstract class QueryNodeVisitor<T>
  {
    public virtual T Visit(AllNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(AnyNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(BinaryOperatorNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CountNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CollectionNavigationNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CollectionPropertyAccessNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CollectionOpenPropertyAccessNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(ConstantNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CollectionConstantNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(ConvertNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CollectionResourceCastNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(ResourceRangeVariableReferenceNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(NonResourceRangeVariableReferenceNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SingleResourceCastNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SingleNavigationNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SingleResourceFunctionCallNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SingleValueFunctionCallNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CollectionResourceFunctionCallNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CollectionFunctionCallNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SingleValueOpenPropertyAccessNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SingleValuePropertyAccessNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(UnaryOperatorNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(NamedFunctionParameterNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(ParameterAliasNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SearchTermNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SingleComplexNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(CollectionComplexNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(SingleValueCastNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(AggregatedCollectionPropertyNode nodeIn) => throw new NotImplementedException();

    public virtual T Visit(InNode nodeIn) => throw new NotImplementedException();
  }
}
