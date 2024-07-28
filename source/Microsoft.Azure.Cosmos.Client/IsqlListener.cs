// Decompiled with JetBrains decompiler
// Type: IsqlListener
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.CodeDom.Compiler;

[GeneratedCode("ANTLR", "4.7.2")]
internal interface IsqlListener : IParseTreeListener
{
  void EnterProgram([NotNull] sqlParser.ProgramContext context);

  void ExitProgram([NotNull] sqlParser.ProgramContext context);

  void EnterSql_query([NotNull] sqlParser.Sql_queryContext context);

  void ExitSql_query([NotNull] sqlParser.Sql_queryContext context);

  void EnterSelect_clause([NotNull] sqlParser.Select_clauseContext context);

  void ExitSelect_clause([NotNull] sqlParser.Select_clauseContext context);

  void EnterTop_spec([NotNull] sqlParser.Top_specContext context);

  void ExitTop_spec([NotNull] sqlParser.Top_specContext context);

  void EnterSelection([NotNull] sqlParser.SelectionContext context);

  void ExitSelection([NotNull] sqlParser.SelectionContext context);

  void EnterSelect_star_spec([NotNull] sqlParser.Select_star_specContext context);

  void ExitSelect_star_spec([NotNull] sqlParser.Select_star_specContext context);

  void EnterSelect_value_spec([NotNull] sqlParser.Select_value_specContext context);

  void ExitSelect_value_spec([NotNull] sqlParser.Select_value_specContext context);

  void EnterSelect_list_spec([NotNull] sqlParser.Select_list_specContext context);

  void ExitSelect_list_spec([NotNull] sqlParser.Select_list_specContext context);

  void EnterSelect_item([NotNull] sqlParser.Select_itemContext context);

  void ExitSelect_item([NotNull] sqlParser.Select_itemContext context);

  void EnterFrom_clause([NotNull] sqlParser.From_clauseContext context);

  void ExitFrom_clause([NotNull] sqlParser.From_clauseContext context);

  void EnterJoinCollectionExpression([NotNull] sqlParser.JoinCollectionExpressionContext context);

  void ExitJoinCollectionExpression([NotNull] sqlParser.JoinCollectionExpressionContext context);

  void EnterAliasedCollectionExpression(
    [NotNull] sqlParser.AliasedCollectionExpressionContext context);

  void ExitAliasedCollectionExpression(
    [NotNull] sqlParser.AliasedCollectionExpressionContext context);

  void EnterArrayIteratorCollectionExpression(
    [NotNull] sqlParser.ArrayIteratorCollectionExpressionContext context);

  void ExitArrayIteratorCollectionExpression(
    [NotNull] sqlParser.ArrayIteratorCollectionExpressionContext context);

  void EnterInputPathCollection([NotNull] sqlParser.InputPathCollectionContext context);

  void ExitInputPathCollection([NotNull] sqlParser.InputPathCollectionContext context);

  void EnterSubqueryCollection([NotNull] sqlParser.SubqueryCollectionContext context);

  void ExitSubqueryCollection([NotNull] sqlParser.SubqueryCollectionContext context);

  void EnterStringPathExpression([NotNull] sqlParser.StringPathExpressionContext context);

  void ExitStringPathExpression([NotNull] sqlParser.StringPathExpressionContext context);

  void EnterEpsilonPathExpression([NotNull] sqlParser.EpsilonPathExpressionContext context);

  void ExitEpsilonPathExpression([NotNull] sqlParser.EpsilonPathExpressionContext context);

  void EnterIdentifierPathExpression([NotNull] sqlParser.IdentifierPathExpressionContext context);

  void ExitIdentifierPathExpression([NotNull] sqlParser.IdentifierPathExpressionContext context);

  void EnterNumberPathExpression([NotNull] sqlParser.NumberPathExpressionContext context);

  void ExitNumberPathExpression([NotNull] sqlParser.NumberPathExpressionContext context);

  void EnterWhere_clause([NotNull] sqlParser.Where_clauseContext context);

  void ExitWhere_clause([NotNull] sqlParser.Where_clauseContext context);

  void EnterGroup_by_clause([NotNull] sqlParser.Group_by_clauseContext context);

  void ExitGroup_by_clause([NotNull] sqlParser.Group_by_clauseContext context);

  void EnterOrder_by_clause([NotNull] sqlParser.Order_by_clauseContext context);

  void ExitOrder_by_clause([NotNull] sqlParser.Order_by_clauseContext context);

  void EnterOrder_by_items([NotNull] sqlParser.Order_by_itemsContext context);

  void ExitOrder_by_items([NotNull] sqlParser.Order_by_itemsContext context);

  void EnterOrder_by_item([NotNull] sqlParser.Order_by_itemContext context);

  void ExitOrder_by_item([NotNull] sqlParser.Order_by_itemContext context);

  void EnterSort_order([NotNull] sqlParser.Sort_orderContext context);

  void ExitSort_order([NotNull] sqlParser.Sort_orderContext context);

  void EnterOffset_limit_clause([NotNull] sqlParser.Offset_limit_clauseContext context);

  void ExitOffset_limit_clause([NotNull] sqlParser.Offset_limit_clauseContext context);

  void EnterOffset_count([NotNull] sqlParser.Offset_countContext context);

  void ExitOffset_count([NotNull] sqlParser.Offset_countContext context);

  void EnterLimit_count([NotNull] sqlParser.Limit_countContext context);

  void ExitLimit_count([NotNull] sqlParser.Limit_countContext context);

  void EnterLogicalScalarExpression([NotNull] sqlParser.LogicalScalarExpressionContext context);

  void ExitLogicalScalarExpression([NotNull] sqlParser.LogicalScalarExpressionContext context);

  void EnterConditionalScalarExpression(
    [NotNull] sqlParser.ConditionalScalarExpressionContext context);

  void ExitConditionalScalarExpression(
    [NotNull] sqlParser.ConditionalScalarExpressionContext context);

  void EnterCoalesceScalarExpression([NotNull] sqlParser.CoalesceScalarExpressionContext context);

  void ExitCoalesceScalarExpression([NotNull] sqlParser.CoalesceScalarExpressionContext context);

  void EnterBetweenScalarExpression([NotNull] sqlParser.BetweenScalarExpressionContext context);

  void ExitBetweenScalarExpression([NotNull] sqlParser.BetweenScalarExpressionContext context);

  void EnterLogical_scalar_expression([NotNull] sqlParser.Logical_scalar_expressionContext context);

  void ExitLogical_scalar_expression([NotNull] sqlParser.Logical_scalar_expressionContext context);

  void EnterIn_scalar_expression([NotNull] sqlParser.In_scalar_expressionContext context);

  void ExitIn_scalar_expression([NotNull] sqlParser.In_scalar_expressionContext context);

  void EnterLike_scalar_expression([NotNull] sqlParser.Like_scalar_expressionContext context);

  void ExitLike_scalar_expression([NotNull] sqlParser.Like_scalar_expressionContext context);

  void EnterEscape_expression([NotNull] sqlParser.Escape_expressionContext context);

  void ExitEscape_expression([NotNull] sqlParser.Escape_expressionContext context);

  void EnterBinary_scalar_expression([NotNull] sqlParser.Binary_scalar_expressionContext context);

  void ExitBinary_scalar_expression([NotNull] sqlParser.Binary_scalar_expressionContext context);

  void EnterMultiplicative_operator([NotNull] sqlParser.Multiplicative_operatorContext context);

  void ExitMultiplicative_operator([NotNull] sqlParser.Multiplicative_operatorContext context);

  void EnterAdditive_operator([NotNull] sqlParser.Additive_operatorContext context);

  void ExitAdditive_operator([NotNull] sqlParser.Additive_operatorContext context);

  void EnterRelational_operator([NotNull] sqlParser.Relational_operatorContext context);

  void ExitRelational_operator([NotNull] sqlParser.Relational_operatorContext context);

  void EnterEquality_operator([NotNull] sqlParser.Equality_operatorContext context);

  void ExitEquality_operator([NotNull] sqlParser.Equality_operatorContext context);

  void EnterBitwise_and_operator([NotNull] sqlParser.Bitwise_and_operatorContext context);

  void ExitBitwise_and_operator([NotNull] sqlParser.Bitwise_and_operatorContext context);

  void EnterBitwise_exclusive_or_operator(
    [NotNull] sqlParser.Bitwise_exclusive_or_operatorContext context);

  void ExitBitwise_exclusive_or_operator(
    [NotNull] sqlParser.Bitwise_exclusive_or_operatorContext context);

  void EnterBitwise_inclusive_or_operator(
    [NotNull] sqlParser.Bitwise_inclusive_or_operatorContext context);

  void ExitBitwise_inclusive_or_operator(
    [NotNull] sqlParser.Bitwise_inclusive_or_operatorContext context);

  void EnterString_concat_operator([NotNull] sqlParser.String_concat_operatorContext context);

  void ExitString_concat_operator([NotNull] sqlParser.String_concat_operatorContext context);

  void EnterUnary_scalar_expression([NotNull] sqlParser.Unary_scalar_expressionContext context);

  void ExitUnary_scalar_expression([NotNull] sqlParser.Unary_scalar_expressionContext context);

  void EnterUnary_operator([NotNull] sqlParser.Unary_operatorContext context);

  void ExitUnary_operator([NotNull] sqlParser.Unary_operatorContext context);

  void EnterSubqueryScalarExpression([NotNull] sqlParser.SubqueryScalarExpressionContext context);

  void ExitSubqueryScalarExpression([NotNull] sqlParser.SubqueryScalarExpressionContext context);

  void EnterPropertyRefScalarExpressionBase(
    [NotNull] sqlParser.PropertyRefScalarExpressionBaseContext context);

  void ExitPropertyRefScalarExpressionBase(
    [NotNull] sqlParser.PropertyRefScalarExpressionBaseContext context);

  void EnterFunctionCallScalarExpression(
    [NotNull] sqlParser.FunctionCallScalarExpressionContext context);

  void ExitFunctionCallScalarExpression(
    [NotNull] sqlParser.FunctionCallScalarExpressionContext context);

  void EnterLiteralScalarExpression([NotNull] sqlParser.LiteralScalarExpressionContext context);

  void ExitLiteralScalarExpression([NotNull] sqlParser.LiteralScalarExpressionContext context);

  void EnterObjectCreateScalarExpression(
    [NotNull] sqlParser.ObjectCreateScalarExpressionContext context);

  void ExitObjectCreateScalarExpression(
    [NotNull] sqlParser.ObjectCreateScalarExpressionContext context);

  void EnterParenthesizedScalarExperession(
    [NotNull] sqlParser.ParenthesizedScalarExperessionContext context);

  void ExitParenthesizedScalarExperession(
    [NotNull] sqlParser.ParenthesizedScalarExperessionContext context);

  void EnterParameterRefScalarExpression(
    [NotNull] sqlParser.ParameterRefScalarExpressionContext context);

  void ExitParameterRefScalarExpression(
    [NotNull] sqlParser.ParameterRefScalarExpressionContext context);

  void EnterArrayCreateScalarExpression(
    [NotNull] sqlParser.ArrayCreateScalarExpressionContext context);

  void ExitArrayCreateScalarExpression(
    [NotNull] sqlParser.ArrayCreateScalarExpressionContext context);

  void EnterExistsScalarExpression([NotNull] sqlParser.ExistsScalarExpressionContext context);

  void ExitExistsScalarExpression([NotNull] sqlParser.ExistsScalarExpressionContext context);

  void EnterArrayScalarExpression([NotNull] sqlParser.ArrayScalarExpressionContext context);

  void ExitArrayScalarExpression([NotNull] sqlParser.ArrayScalarExpressionContext context);

  void EnterMemberIndexerScalarExpression(
    [NotNull] sqlParser.MemberIndexerScalarExpressionContext context);

  void ExitMemberIndexerScalarExpression(
    [NotNull] sqlParser.MemberIndexerScalarExpressionContext context);

  void EnterPropertyRefScalarExpressionRecursive(
    [NotNull] sqlParser.PropertyRefScalarExpressionRecursiveContext context);

  void ExitPropertyRefScalarExpressionRecursive(
    [NotNull] sqlParser.PropertyRefScalarExpressionRecursiveContext context);

  void EnterScalar_expression_list([NotNull] sqlParser.Scalar_expression_listContext context);

  void ExitScalar_expression_list([NotNull] sqlParser.Scalar_expression_listContext context);

  void EnterObject_property_list([NotNull] sqlParser.Object_property_listContext context);

  void ExitObject_property_list([NotNull] sqlParser.Object_property_listContext context);

  void EnterObject_property([NotNull] sqlParser.Object_propertyContext context);

  void ExitObject_property([NotNull] sqlParser.Object_propertyContext context);

  void EnterLiteral([NotNull] sqlParser.LiteralContext context);

  void ExitLiteral([NotNull] sqlParser.LiteralContext context);
}
