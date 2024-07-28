// Decompiled with JetBrains decompiler
// Type: sqlBaseVisitor`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.CodeDom.Compiler;

[GeneratedCode("ANTLR", "4.7.2")]
internal class sqlBaseVisitor<Result> : 
  AbstractParseTreeVisitor<Result>,
  IsqlVisitor<Result>,
  IParseTreeVisitor<Result>
{
  public virtual Result VisitProgram([NotNull] sqlParser.ProgramContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSql_query([NotNull] sqlParser.Sql_queryContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSelect_clause([NotNull] sqlParser.Select_clauseContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitTop_spec([NotNull] sqlParser.Top_specContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSelection([NotNull] sqlParser.SelectionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSelect_star_spec([NotNull] sqlParser.Select_star_specContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSelect_value_spec([NotNull] sqlParser.Select_value_specContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSelect_list_spec([NotNull] sqlParser.Select_list_specContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSelect_item([NotNull] sqlParser.Select_itemContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitFrom_clause([NotNull] sqlParser.From_clauseContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitJoinCollectionExpression(
    [NotNull] sqlParser.JoinCollectionExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitAliasedCollectionExpression(
    [NotNull] sqlParser.AliasedCollectionExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitArrayIteratorCollectionExpression(
    [NotNull] sqlParser.ArrayIteratorCollectionExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitInputPathCollection([NotNull] sqlParser.InputPathCollectionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSubqueryCollection([NotNull] sqlParser.SubqueryCollectionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitStringPathExpression([NotNull] sqlParser.StringPathExpressionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitEpsilonPathExpression([NotNull] sqlParser.EpsilonPathExpressionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitIdentifierPathExpression(
    [NotNull] sqlParser.IdentifierPathExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitNumberPathExpression([NotNull] sqlParser.NumberPathExpressionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitWhere_clause([NotNull] sqlParser.Where_clauseContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitGroup_by_clause([NotNull] sqlParser.Group_by_clauseContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitOrder_by_clause([NotNull] sqlParser.Order_by_clauseContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitOrder_by_items([NotNull] sqlParser.Order_by_itemsContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitOrder_by_item([NotNull] sqlParser.Order_by_itemContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSort_order([NotNull] sqlParser.Sort_orderContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitOffset_limit_clause([NotNull] sqlParser.Offset_limit_clauseContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitOffset_count([NotNull] sqlParser.Offset_countContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitLimit_count([NotNull] sqlParser.Limit_countContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitLogicalScalarExpression(
    [NotNull] sqlParser.LogicalScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitConditionalScalarExpression(
    [NotNull] sqlParser.ConditionalScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitCoalesceScalarExpression(
    [NotNull] sqlParser.CoalesceScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitBetweenScalarExpression(
    [NotNull] sqlParser.BetweenScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitLogical_scalar_expression(
    [NotNull] sqlParser.Logical_scalar_expressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitIn_scalar_expression([NotNull] sqlParser.In_scalar_expressionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitLike_scalar_expression([NotNull] sqlParser.Like_scalar_expressionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitEscape_expression([NotNull] sqlParser.Escape_expressionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitBinary_scalar_expression(
    [NotNull] sqlParser.Binary_scalar_expressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitMultiplicative_operator(
    [NotNull] sqlParser.Multiplicative_operatorContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitAdditive_operator([NotNull] sqlParser.Additive_operatorContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitRelational_operator([NotNull] sqlParser.Relational_operatorContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitEquality_operator([NotNull] sqlParser.Equality_operatorContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitBitwise_and_operator([NotNull] sqlParser.Bitwise_and_operatorContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitBitwise_exclusive_or_operator(
    [NotNull] sqlParser.Bitwise_exclusive_or_operatorContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitBitwise_inclusive_or_operator(
    [NotNull] sqlParser.Bitwise_inclusive_or_operatorContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitString_concat_operator([NotNull] sqlParser.String_concat_operatorContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitUnary_scalar_expression(
    [NotNull] sqlParser.Unary_scalar_expressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitUnary_operator([NotNull] sqlParser.Unary_operatorContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitSubqueryScalarExpression(
    [NotNull] sqlParser.SubqueryScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitPropertyRefScalarExpressionBase(
    [NotNull] sqlParser.PropertyRefScalarExpressionBaseContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitFunctionCallScalarExpression(
    [NotNull] sqlParser.FunctionCallScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitLiteralScalarExpression(
    [NotNull] sqlParser.LiteralScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitObjectCreateScalarExpression(
    [NotNull] sqlParser.ObjectCreateScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitParenthesizedScalarExperession(
    [NotNull] sqlParser.ParenthesizedScalarExperessionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitParameterRefScalarExpression(
    [NotNull] sqlParser.ParameterRefScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitArrayCreateScalarExpression(
    [NotNull] sqlParser.ArrayCreateScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitExistsScalarExpression([NotNull] sqlParser.ExistsScalarExpressionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitArrayScalarExpression([NotNull] sqlParser.ArrayScalarExpressionContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitMemberIndexerScalarExpression(
    [NotNull] sqlParser.MemberIndexerScalarExpressionContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitPropertyRefScalarExpressionRecursive(
    [NotNull] sqlParser.PropertyRefScalarExpressionRecursiveContext context)
  {
    return this.VisitChildren((IRuleNode) context);
  }

  public virtual Result VisitScalar_expression_list([NotNull] sqlParser.Scalar_expression_listContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitObject_property_list([NotNull] sqlParser.Object_property_listContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitObject_property([NotNull] sqlParser.Object_propertyContext context) => this.VisitChildren((IRuleNode) context);

  public virtual Result VisitLiteral([NotNull] sqlParser.LiteralContext context) => this.VisitChildren((IRuleNode) context);
}
