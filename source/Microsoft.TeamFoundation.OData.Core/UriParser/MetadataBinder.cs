// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.MetadataBinder
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.UriParser
{
  [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Keeping the visitor in one place makes sense.")]
  internal class MetadataBinder
  {
    private BindingState bindingState;

    internal MetadataBinder(BindingState initialState)
    {
      ExceptionUtils.CheckArgumentNotNull<BindingState>(initialState, nameof (initialState));
      ExceptionUtils.CheckArgumentNotNull<IEdmModel>(initialState.Model, "initialState.Model");
      this.BindingState = initialState;
    }

    internal BindingState BindingState
    {
      get => this.bindingState;
      private set => this.bindingState = value;
    }

    public static long? ProcessSkip(long? skip)
    {
      if (!skip.HasValue)
        return new long?();
      long? nullable = skip;
      long num = 0;
      if (nullable.GetValueOrDefault() < num & nullable.HasValue)
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_SkipRequiresNonNegativeInteger((object) skip.ToString()));
      return skip;
    }

    public static long? ProcessTop(long? top)
    {
      if (!top.HasValue)
        return new long?();
      long? nullable = top;
      long num = 0;
      if (nullable.GetValueOrDefault() < num & nullable.HasValue)
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_TopRequiresNonNegativeInteger((object) top.ToString()));
      return top;
    }

    public static List<QueryNode> ProcessQueryOptions(
      BindingState bindingState,
      MetadataBinder.QueryTokenVisitor bindMethod)
    {
      if (bindingState == null || bindingState.QueryOptions == null)
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_QueryOptionsBindStateCannotBeNull);
      if (bindMethod == null)
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_QueryOptionsBindMethodCannotBeNull);
      List<QueryNode> queryNodeList = new List<QueryNode>();
      foreach (CustomQueryOptionToken queryOption in bindingState.QueryOptions)
      {
        QueryNode queryNode = bindMethod((QueryToken) queryOption);
        if (queryNode != null)
          queryNodeList.Add(queryNode);
      }
      bindingState.QueryOptions = (List<CustomQueryOptionToken>) null;
      return queryNodeList;
    }

    protected internal QueryNode Bind(QueryToken token)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(token, nameof (token));
      this.BindingState.RecurseEnter();
      QueryNode queryNode;
      switch (token.Kind)
      {
        case QueryTokenKind.BinaryOperator:
          queryNode = this.BindBinaryOperator((BinaryOperatorToken) token);
          break;
        case QueryTokenKind.UnaryOperator:
          queryNode = this.BindUnaryOperator((UnaryOperatorToken) token);
          break;
        case QueryTokenKind.Literal:
          queryNode = this.BindLiteral((LiteralToken) token);
          break;
        case QueryTokenKind.FunctionCall:
          queryNode = this.BindFunctionCall((FunctionCallToken) token);
          break;
        case QueryTokenKind.EndPath:
          queryNode = this.BindEndPath((EndPathToken) token);
          break;
        case QueryTokenKind.Any:
          queryNode = this.BindAnyAll((LambdaToken) token);
          break;
        case QueryTokenKind.InnerPath:
          queryNode = this.BindInnerPathSegment((InnerPathToken) token);
          break;
        case QueryTokenKind.DottedIdentifier:
          queryNode = this.BindCast((DottedIdentifierToken) token);
          break;
        case QueryTokenKind.RangeVariable:
          queryNode = (QueryNode) this.BindRangeVariable((RangeVariableToken) token);
          break;
        case QueryTokenKind.All:
          queryNode = this.BindAnyAll((LambdaToken) token);
          break;
        case QueryTokenKind.FunctionParameter:
          queryNode = this.BindFunctionParameter((FunctionParameterToken) token);
          break;
        case QueryTokenKind.FunctionParameterAlias:
          queryNode = (QueryNode) this.BindParameterAlias((FunctionParameterAliasToken) token);
          break;
        case QueryTokenKind.StringLiteral:
          queryNode = this.BindStringLiteral((StringLiteralToken) token);
          break;
        case QueryTokenKind.In:
          queryNode = this.BindIn((InToken) token);
          break;
        default:
          throw new ODataException(Microsoft.OData.Strings.MetadataBinder_UnsupportedQueryTokenKind((object) token.Kind));
      }
      if (queryNode == null)
        throw new ODataException(Microsoft.OData.Strings.MetadataBinder_BoundNodeCannotBeNull((object) token.Kind));
      this.BindingState.RecurseLeave();
      return queryNode;
    }

    protected virtual SingleValueNode BindParameterAlias(
      FunctionParameterAliasToken functionParameterAliasToken)
    {
      return (SingleValueNode) new ParameterAliasBinder(new MetadataBinder.QueryTokenVisitor(this.Bind)).BindParameterAlias(this.BindingState, functionParameterAliasToken);
    }

    protected virtual QueryNode BindFunctionParameter(FunctionParameterToken token) => token.ParameterName != null ? (QueryNode) new NamedFunctionParameterNode(token.ParameterName, this.Bind(token.ValueToken)) : this.Bind(token.ValueToken);

    protected virtual QueryNode BindInnerPathSegment(InnerPathToken token) => new InnerPathTokenBinder(new MetadataBinder.QueryTokenVisitor(this.Bind), this.BindingState).BindInnerPathSegment(token);

    protected virtual SingleValueNode BindRangeVariable(RangeVariableToken rangeVariableToken) => RangeVariableBinder.BindRangeVariableToken(rangeVariableToken, this.BindingState);

    protected virtual QueryNode BindLiteral(LiteralToken literalToken) => LiteralBinder.BindLiteral(literalToken);

    protected virtual QueryNode BindBinaryOperator(BinaryOperatorToken binaryOperatorToken) => new BinaryOperatorBinder(new Func<QueryToken, QueryNode>(this.Bind), this.BindingState.Configuration.Resolver).BindBinaryOperator(binaryOperatorToken);

    protected virtual QueryNode BindUnaryOperator(UnaryOperatorToken unaryOperatorToken) => new UnaryOperatorBinder(new Func<QueryToken, QueryNode>(this.Bind)).BindUnaryOperator(unaryOperatorToken);

    protected virtual QueryNode BindCast(DottedIdentifierToken dottedIdentifierToken) => new DottedIdentifierBinder(new MetadataBinder.QueryTokenVisitor(this.Bind), this.BindingState).BindDottedIdentifier(dottedIdentifierToken);

    protected virtual QueryNode BindAnyAll(LambdaToken lambdaToken)
    {
      ExceptionUtils.CheckArgumentNotNull<LambdaToken>(lambdaToken, "LambdaToken");
      return (QueryNode) new LambdaBinder(new MetadataBinder.QueryTokenVisitor(this.Bind)).BindLambdaToken(lambdaToken, this.BindingState);
    }

    protected virtual QueryNode BindEndPath(EndPathToken endPathToken) => new EndPathBinder(new MetadataBinder.QueryTokenVisitor(this.Bind), this.BindingState).BindEndPath(endPathToken);

    protected virtual QueryNode BindFunctionCall(FunctionCallToken functionCallToken) => new FunctionCallBinder(new MetadataBinder.QueryTokenVisitor(this.Bind), this.BindingState).BindFunctionCall(functionCallToken);

    protected virtual QueryNode BindStringLiteral(StringLiteralToken stringLiteralToken) => (QueryNode) new SearchTermNode(stringLiteralToken.Text);

    protected virtual QueryNode BindIn(InToken inToken) => new InBinder((Func<QueryToken, QueryNode>) (queryToken =>
    {
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(queryToken, nameof (queryToken));
      return queryToken.Kind == QueryTokenKind.Literal ? LiteralBinder.BindInLiteral((LiteralToken) queryToken) : this.Bind(queryToken);
    })).BindInOperator(inToken, this.BindingState);

    internal delegate QueryNode QueryTokenVisitor(QueryToken token);
  }
}
