// Decompiled with JetBrains decompiler
// Type: IsqlVisitor`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.CodeDom.Compiler;

[GeneratedCode("ANTLR", "4.7.2")]
internal interface IsqlVisitor<Result> : IParseTreeVisitor<Result>
{
  Result VisitProgram([NotNull] sqlParser.ProgramContext context);

  Result VisitSql_query([NotNull] sqlParser.Sql_queryContext context);

  Result VisitSelect_clause([NotNull] sqlParser.Select_clauseContext context);

  Result VisitTop_spec([NotNull] sqlParser.Top_specContext context);

  Result VisitSelection([NotNull] sqlParser.SelectionContext context);

  Result VisitSelect_star_spec([NotNull] sqlParser.Select_star_specContext context);

  Result VisitSelect_value_spec([NotNull] sqlParser.Select_value_specContext context);

  Result VisitSelect_list_spec([NotNull] sqlParser.Select_list_specContext context);

  Result VisitSelect_item([NotNull] sqlParser.Select_itemContext context);

  Result VisitFrom_clause([NotNull] sqlParser.From_clauseContext context);

  Result VisitJoinCollectionExpression([NotNull] sqlParser.JoinCollectionExpressionContext context);

  Result VisitAliasedCollectionExpression(
    [NotNull] sqlParser.AliasedCollectionExpressionContext context);

  Result VisitArrayIteratorCollectionExpression(
    [NotNull] sqlParser.ArrayIteratorCollectionExpressionContext context);

  Result VisitInputPathCollection([NotNull] sqlParser.InputPathCollectionContext context);

  Result VisitSubqueryCollection([NotNull] sqlParser.SubqueryCollectionContext context);

  Result VisitStringPathExpression([NotNull] sqlParser.StringPathExpressionContext context);

  Result VisitEpsilonPathExpression([NotNull] sqlParser.EpsilonPathExpressionContext context);

  Result VisitIdentifierPathExpression([NotNull] sqlParser.IdentifierPathExpressionContext context);

  Result VisitNumberPathExpression([NotNull] sqlParser.NumberPathExpressionContext context);

  Result VisitWhere_clause([NotNull] sqlParser.Where_clauseContext context);

  Result VisitGroup_by_clause([NotNull] sqlParser.Group_by_clauseContext context);

  Result VisitOrder_by_clause([NotNull] sqlParser.Order_by_clauseContext context);

  Result VisitOrder_by_items([NotNull] sqlParser.Order_by_itemsContext context);

  Result VisitOrder_by_item([NotNull] sqlParser.Order_by_itemContext context);

  Result VisitSort_order([NotNull] sqlParser.Sort_orderContext context);

  Result VisitOffset_limit_clause([NotNull] sqlParser.Offset_limit_clauseContext context);

  Result VisitOffset_count([NotNull] sqlParser.Offset_countContext context);

  Result VisitLimit_count([NotNull] sqlParser.Limit_countContext context);

  Result VisitLogicalScalarExpression([NotNull] sqlParser.LogicalScalarExpressionContext context);

  Result VisitConditionalScalarExpression(
    [NotNull] sqlParser.ConditionalScalarExpressionContext context);

  Result VisitCoalesceScalarExpression([NotNull] sqlParser.CoalesceScalarExpressionContext context);

  Result VisitBetweenScalarExpression([NotNull] sqlParser.BetweenScalarExpressionContext context);

  Result VisitLogical_scalar_expression([NotNull] sqlParser.Logical_scalar_expressionContext context);

  Result VisitIn_scalar_expression([NotNull] sqlParser.In_scalar_expressionContext context);

  Result VisitLike_scalar_expression([NotNull] sqlParser.Like_scalar_expressionContext context);

  Result VisitEscape_expression([NotNull] sqlParser.Escape_expressionContext context);

  Result VisitBinary_scalar_expression([NotNull] sqlParser.Binary_scalar_expressionContext context);

  Result VisitMultiplicative_operator([NotNull] sqlParser.Multiplicative_operatorContext context);

  Result VisitAdditive_operator([NotNull] sqlParser.Additive_operatorContext context);

  Result VisitRelational_operator([NotNull] sqlParser.Relational_operatorContext context);

  Result VisitEquality_operator([NotNull] sqlParser.Equality_operatorContext context);

  Result VisitBitwise_and_operator([NotNull] sqlParser.Bitwise_and_operatorContext context);

  Result VisitBitwise_exclusive_or_operator(
    [NotNull] sqlParser.Bitwise_exclusive_or_operatorContext context);

  Result VisitBitwise_inclusive_or_operator(
    [NotNull] sqlParser.Bitwise_inclusive_or_operatorContext context);

  Result VisitString_concat_operator([NotNull] sqlParser.String_concat_operatorContext context);

  Result VisitUnary_scalar_expression([NotNull] sqlParser.Unary_scalar_expressionContext context);

  Result VisitUnary_operator([NotNull] sqlParser.Unary_operatorContext context);

  Result VisitSubqueryScalarExpression([NotNull] sqlParser.SubqueryScalarExpressionContext context);

  Result VisitPropertyRefScalarExpressionBase(
    [NotNull] sqlParser.PropertyRefScalarExpressionBaseContext context);

  Result VisitFunctionCallScalarExpression(
    [NotNull] sqlParser.FunctionCallScalarExpressionContext context);

  Result VisitLiteralScalarExpression([NotNull] sqlParser.LiteralScalarExpressionContext context);

  Result VisitObjectCreateScalarExpression(
    [NotNull] sqlParser.ObjectCreateScalarExpressionContext context);

  Result VisitParenthesizedScalarExperession(
    [NotNull] sqlParser.ParenthesizedScalarExperessionContext context);

  Result VisitParameterRefScalarExpression(
    [NotNull] sqlParser.ParameterRefScalarExpressionContext context);

  Result VisitArrayCreateScalarExpression(
    [NotNull] sqlParser.ArrayCreateScalarExpressionContext context);

  Result VisitExistsScalarExpression([NotNull] sqlParser.ExistsScalarExpressionContext context);

  Result VisitArrayScalarExpression([NotNull] sqlParser.ArrayScalarExpressionContext context);

  Result VisitMemberIndexerScalarExpression(
    [NotNull] sqlParser.MemberIndexerScalarExpressionContext context);

  Result VisitPropertyRefScalarExpressionRecursive(
    [NotNull] sqlParser.PropertyRefScalarExpressionRecursiveContext context);

  Result VisitScalar_expression_list([NotNull] sqlParser.Scalar_expression_listContext context);

  Result VisitObject_property_list([NotNull] sqlParser.Object_property_listContext context);

  Result VisitObject_property([NotNull] sqlParser.Object_propertyContext context);

  Result VisitLiteral([NotNull] sqlParser.LiteralContext context);
}
