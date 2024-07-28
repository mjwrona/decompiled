// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Parser.CstToAstVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.Query.Core.Parser
{
  internal sealed class CstToAstVisitor : sqlBaseVisitor<SqlObject>
  {
    public static readonly CstToAstVisitor Singleton = new CstToAstVisitor();
    private static readonly IReadOnlyDictionary<string, SqlBinaryScalarOperatorKind> binaryOperatorKindLookup = (IReadOnlyDictionary<string, SqlBinaryScalarOperatorKind>) new Dictionary<string, SqlBinaryScalarOperatorKind>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "+",
        SqlBinaryScalarOperatorKind.Add
      },
      {
        "AND",
        SqlBinaryScalarOperatorKind.And
      },
      {
        "&",
        SqlBinaryScalarOperatorKind.BitwiseAnd
      },
      {
        "|",
        SqlBinaryScalarOperatorKind.BitwiseOr
      },
      {
        "^",
        SqlBinaryScalarOperatorKind.BitwiseXor
      },
      {
        "/",
        SqlBinaryScalarOperatorKind.Divide
      },
      {
        "=",
        SqlBinaryScalarOperatorKind.Equal
      },
      {
        ">",
        SqlBinaryScalarOperatorKind.GreaterThan
      },
      {
        ">=",
        SqlBinaryScalarOperatorKind.GreaterThanOrEqual
      },
      {
        "<",
        SqlBinaryScalarOperatorKind.LessThan
      },
      {
        "<=",
        SqlBinaryScalarOperatorKind.LessThanOrEqual
      },
      {
        "%",
        SqlBinaryScalarOperatorKind.Modulo
      },
      {
        "*",
        SqlBinaryScalarOperatorKind.Multiply
      },
      {
        "!=",
        SqlBinaryScalarOperatorKind.NotEqual
      },
      {
        "OR",
        SqlBinaryScalarOperatorKind.Or
      },
      {
        "||",
        SqlBinaryScalarOperatorKind.StringConcat
      },
      {
        "-",
        SqlBinaryScalarOperatorKind.Subtract
      }
    };
    private static readonly IReadOnlyDictionary<string, SqlUnaryScalarOperatorKind> unaryOperatorKindLookup = (IReadOnlyDictionary<string, SqlUnaryScalarOperatorKind>) new Dictionary<string, SqlUnaryScalarOperatorKind>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "~",
        SqlUnaryScalarOperatorKind.BitwiseNot
      },
      {
        "-",
        SqlUnaryScalarOperatorKind.Minus
      },
      {
        "NOT",
        SqlUnaryScalarOperatorKind.Not
      },
      {
        "+",
        SqlUnaryScalarOperatorKind.Plus
      }
    };

    private CstToAstVisitor()
    {
    }

    public override SqlObject VisitProgram([NotNull] sqlParser.ProgramContext context) => this.Visit((IParseTree) context.sql_query());

    public override SqlObject VisitSql_query([NotNull] sqlParser.Sql_queryContext context)
    {
      SqlSelectClause selectClause = (SqlSelectClause) this.Visit((IParseTree) context.select_clause());
      SqlFromClause sqlFromClause = context.from_clause() == null ? (SqlFromClause) null : (SqlFromClause) this.Visit((IParseTree) context.from_clause());
      SqlWhereClause sqlWhereClause = context.where_clause() == null ? (SqlWhereClause) null : (SqlWhereClause) this.Visit((IParseTree) context.where_clause());
      SqlOrderByClause sqlOrderByClause = context.order_by_clause() == null ? (SqlOrderByClause) null : (SqlOrderByClause) this.Visit((IParseTree) context.order_by_clause());
      SqlGroupByClause sqlGroupByClause = context.group_by_clause() == null ? (SqlGroupByClause) null : (SqlGroupByClause) this.Visit((IParseTree) context.group_by_clause());
      SqlOffsetLimitClause offsetLimitClause1 = context.offset_limit_clause() == null ? (SqlOffsetLimitClause) null : (SqlOffsetLimitClause) this.Visit((IParseTree) context.offset_limit_clause());
      SqlFromClause fromClause = sqlFromClause;
      SqlWhereClause whereClause = sqlWhereClause;
      SqlGroupByClause groupByClause = sqlGroupByClause;
      SqlOrderByClause orderByClause = sqlOrderByClause;
      SqlOffsetLimitClause offsetLimitClause2 = offsetLimitClause1;
      return (SqlObject) SqlQuery.Create(selectClause, fromClause, whereClause, groupByClause, orderByClause, offsetLimitClause2);
    }

    public override SqlObject VisitSelect_clause([NotNull] sqlParser.Select_clauseContext context)
    {
      SqlSelectSpec selectSpec = (SqlSelectSpec) this.Visit((IParseTree) context.selection());
      SqlTopSpec sqlTopSpec = context.top_spec() == null ? (SqlTopSpec) null : (SqlTopSpec) this.Visit((IParseTree) context.top_spec());
      bool flag = context.K_DISTINCT() != null;
      SqlTopSpec topSpec = sqlTopSpec;
      int num = flag ? 1 : 0;
      return (SqlObject) SqlSelectClause.Create(selectSpec, topSpec, num != 0);
    }

    public override SqlObject VisitSelect_star_spec([NotNull] sqlParser.Select_star_specContext context) => (SqlObject) SqlSelectStarSpec.Create();

    public override SqlObject VisitSelect_value_spec([NotNull] sqlParser.Select_value_specContext context) => (SqlObject) SqlSelectValueSpec.Create((SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression()));

    public override SqlObject VisitSelect_list_spec([NotNull] sqlParser.Select_list_specContext context)
    {
      List<SqlSelectItem> items = new List<SqlSelectItem>();
      foreach (IParseTree tree in context.select_item())
      {
        SqlSelectItem sqlSelectItem = (SqlSelectItem) this.Visit(tree);
        items.Add(sqlSelectItem);
      }
      return (SqlObject) SqlSelectListSpec.Create(items.ToImmutableArray<SqlSelectItem>());
    }

    public override SqlObject VisitSelect_item([NotNull] sqlParser.Select_itemContext context) => (SqlObject) SqlSelectItem.Create((SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression()), context.IDENTIFIER() == null ? (SqlIdentifier) null : SqlIdentifier.Create(context.IDENTIFIER().GetText()));

    public override SqlObject VisitTop_spec([NotNull] sqlParser.Top_specContext context)
    {
      if (context.NUMERIC_LITERAL() != null)
        return (SqlObject) SqlTopSpec.Create(SqlNumberLiteral.Create(CstToAstVisitor.GetNumber64ValueFromNode((IParseTree) context.NUMERIC_LITERAL())));
      return context.PARAMETER() != null ? (SqlObject) SqlTopSpec.Create(Microsoft.Azure.Cosmos.SqlObjects.SqlParameter.Create(context.PARAMETER().GetText())) : throw new InvalidOperationException();
    }

    public override SqlObject VisitFrom_clause([NotNull] sqlParser.From_clauseContext context) => (SqlObject) SqlFromClause.Create((SqlCollectionExpression) this.Visit((IParseTree) context.collection_expression()));

    public override SqlObject VisitAliasedCollectionExpression(
      [NotNull] sqlParser.AliasedCollectionExpressionContext context)
    {
      return (SqlObject) SqlAliasedCollectionExpression.Create((SqlCollection) this.Visit((IParseTree) context.collection()), context.IDENTIFIER() == null ? (SqlIdentifier) null : SqlIdentifier.Create(context.IDENTIFIER().GetText()));
    }

    public override SqlObject VisitArrayIteratorCollectionExpression(
      [NotNull] sqlParser.ArrayIteratorCollectionExpressionContext context)
    {
      SqlCollection collection = (SqlCollection) this.Visit((IParseTree) context.collection());
      return (SqlObject) SqlArrayIteratorCollectionExpression.Create(SqlIdentifier.Create(context.IDENTIFIER().GetText()), collection);
    }

    public override SqlObject VisitJoinCollectionExpression(
      [NotNull] sqlParser.JoinCollectionExpressionContext context)
    {
      return (SqlObject) SqlJoinCollectionExpression.Create((SqlCollectionExpression) this.Visit((IParseTree) context.collection_expression(0)), (SqlCollectionExpression) this.Visit((IParseTree) context.collection_expression(1)));
    }

    public override SqlObject VisitInputPathCollection([NotNull] sqlParser.InputPathCollectionContext context) => (SqlObject) SqlInputPathCollection.Create(SqlIdentifier.Create(context.IDENTIFIER().GetText()), context.path_expression() == null ? (SqlPathExpression) null : (SqlPathExpression) this.Visit((IParseTree) context.path_expression()));

    public override SqlObject VisitSubqueryCollection([NotNull] sqlParser.SubqueryCollectionContext context) => (SqlObject) SqlSubqueryCollection.Create((SqlQuery) this.Visit((IParseTree) context.sql_query()));

    public override SqlObject VisitEpsilonPathExpression(
      [NotNull] sqlParser.EpsilonPathExpressionContext context)
    {
      return (SqlObject) null;
    }

    public override SqlObject VisitIdentifierPathExpression(
      [NotNull] sqlParser.IdentifierPathExpressionContext context)
    {
      return (SqlObject) SqlIdentifierPathExpression.Create((SqlPathExpression) this.Visit((IParseTree) context.path_expression()), SqlIdentifier.Create(context.IDENTIFIER().GetText()));
    }

    public override SqlObject VisitNumberPathExpression(
      [NotNull] sqlParser.NumberPathExpressionContext context)
    {
      return (SqlObject) SqlNumberPathExpression.Create((SqlPathExpression) this.Visit((IParseTree) context.path_expression()), SqlNumberLiteral.Create(CstToAstVisitor.GetNumber64ValueFromNode((IParseTree) context.NUMERIC_LITERAL())));
    }

    public override SqlObject VisitStringPathExpression(
      [NotNull] sqlParser.StringPathExpressionContext context)
    {
      return (SqlObject) SqlStringPathExpression.Create((SqlPathExpression) this.Visit((IParseTree) context.path_expression()), SqlStringLiteral.Create(CstToAstVisitor.GetStringValueFromNode((IParseTree) context.STRING_LITERAL())));
    }

    public override SqlObject VisitWhere_clause([NotNull] sqlParser.Where_clauseContext context) => (SqlObject) SqlWhereClause.Create((SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression()));

    public override SqlObject VisitGroup_by_clause([NotNull] sqlParser.Group_by_clauseContext context)
    {
      List<SqlScalarExpression> items = new List<SqlScalarExpression>();
      foreach (sqlParser.Scalar_expressionContext tree in context.scalar_expression_list().scalar_expression())
        items.Add((SqlScalarExpression) this.Visit((IParseTree) tree));
      return (SqlObject) SqlGroupByClause.Create(items.ToImmutableArray<SqlScalarExpression>());
    }

    public override SqlObject VisitOrder_by_clause([NotNull] sqlParser.Order_by_clauseContext context)
    {
      List<SqlOrderByItem> items = new List<SqlOrderByItem>();
      foreach (sqlParser.Order_by_itemContext context1 in context.order_by_items().order_by_item())
      {
        SqlOrderByItem sqlOrderByItem = (SqlOrderByItem) this.VisitOrder_by_item(context1);
        items.Add(sqlOrderByItem);
      }
      return (SqlObject) SqlOrderByClause.Create(items.ToImmutableArray<SqlOrderByItem>());
    }

    public override SqlObject VisitOrder_by_item([NotNull] sqlParser.Order_by_itemContext context)
    {
      SqlScalarExpression expression = (SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression());
      bool flag = false;
      if (context.sort_order() != null)
      {
        if (context.sort_order().K_ASC() != null)
        {
          flag = false;
        }
        else
        {
          if (context.sort_order().K_DESC() == null)
            throw new ArgumentOutOfRangeException(string.Format("Unknown sort order : {0}.", (object) context.sort_order()));
          flag = true;
        }
      }
      int num = flag ? 1 : 0;
      return (SqlObject) SqlOrderByItem.Create(expression, num != 0);
    }

    public override SqlObject VisitOffset_limit_clause([NotNull] sqlParser.Offset_limit_clauseContext context) => (SqlObject) SqlOffsetLimitClause.Create((SqlOffsetSpec) this.Visit((IParseTree) context.offset_count()), (SqlLimitSpec) this.Visit((IParseTree) context.limit_count()));

    public override SqlObject VisitOffset_count([NotNull] sqlParser.Offset_countContext context)
    {
      if (context.NUMERIC_LITERAL() != null)
        return (SqlObject) SqlOffsetSpec.Create(SqlNumberLiteral.Create(CstToAstVisitor.GetNumber64ValueFromNode((IParseTree) context.NUMERIC_LITERAL())));
      return context.PARAMETER() != null ? (SqlObject) SqlOffsetSpec.Create(Microsoft.Azure.Cosmos.SqlObjects.SqlParameter.Create(context.PARAMETER().GetText())) : throw new NotImplementedException();
    }

    public override SqlObject VisitLimit_count([NotNull] sqlParser.Limit_countContext context)
    {
      if (context.NUMERIC_LITERAL() != null)
        return (SqlObject) SqlLimitSpec.Create(SqlNumberLiteral.Create(CstToAstVisitor.GetNumber64ValueFromNode((IParseTree) context.NUMERIC_LITERAL())));
      return context.PARAMETER() != null ? (SqlObject) SqlLimitSpec.Create(Microsoft.Azure.Cosmos.SqlObjects.SqlParameter.Create(context.PARAMETER().GetText())) : throw new NotImplementedException();
    }

    public override SqlObject VisitArrayCreateScalarExpression(
      [NotNull] sqlParser.ArrayCreateScalarExpressionContext context)
    {
      List<SqlScalarExpression> items = new List<SqlScalarExpression>();
      if (context.scalar_expression_list() != null)
      {
        foreach (sqlParser.Scalar_expressionContext tree in context.scalar_expression_list().scalar_expression())
          items.Add((SqlScalarExpression) this.Visit((IParseTree) tree));
      }
      return (SqlObject) SqlArrayCreateScalarExpression.Create(items.ToImmutableArray<SqlScalarExpression>());
    }

    public override SqlObject VisitArrayScalarExpression(
      [NotNull] sqlParser.ArrayScalarExpressionContext context)
    {
      return (SqlObject) SqlArrayScalarExpression.Create((SqlQuery) this.Visit((IParseTree) context.sql_query()));
    }

    public override SqlObject VisitBetweenScalarExpression(
      [NotNull] sqlParser.BetweenScalarExpressionContext context)
    {
      SqlScalarExpression expression = (SqlScalarExpression) this.Visit((IParseTree) context.binary_scalar_expression(0));
      bool flag = context.K_NOT() != null;
      SqlScalarExpression scalarExpression1 = (SqlScalarExpression) this.Visit((IParseTree) context.binary_scalar_expression(1));
      SqlScalarExpression scalarExpression2 = (SqlScalarExpression) this.Visit((IParseTree) context.binary_scalar_expression(2));
      SqlScalarExpression startInclusive = scalarExpression1;
      SqlScalarExpression endInclusive = scalarExpression2;
      int num = flag ? 1 : 0;
      return (SqlObject) SqlBetweenScalarExpression.Create(expression, startInclusive, endInclusive, num != 0);
    }

    public override SqlObject VisitBinary_scalar_expression(
      [NotNull] sqlParser.Binary_scalar_expressionContext context)
    {
      SqlObject sqlObject;
      if (context.unary_scalar_expression() != null)
      {
        sqlObject = this.Visit((IParseTree) context.unary_scalar_expression());
      }
      else
      {
        SqlScalarExpression left = (SqlScalarExpression) this.Visit((IParseTree) context.binary_scalar_expression(0));
        SqlBinaryScalarOperatorKind operatorKind;
        if (!CstToAstVisitor.binaryOperatorKindLookup.TryGetValue(context.children[1].GetText(), out operatorKind))
          throw new ArgumentOutOfRangeException("Unknown binary operator: " + context.children[1].GetText() + ".");
        SqlScalarExpression right = (SqlScalarExpression) this.Visit((IParseTree) context.binary_scalar_expression(1));
        sqlObject = (SqlObject) SqlBinaryScalarExpression.Create(operatorKind, left, right);
      }
      return sqlObject;
    }

    public override SqlObject VisitCoalesceScalarExpression(
      [NotNull] sqlParser.CoalesceScalarExpressionContext context)
    {
      return (SqlObject) SqlCoalesceScalarExpression.Create((SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression(0)), (SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression(1)));
    }

    public override SqlObject VisitConditionalScalarExpression(
      [NotNull] sqlParser.ConditionalScalarExpressionContext context)
    {
      SqlScalarExpression condition = (SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression(0));
      SqlScalarExpression scalarExpression1 = (SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression(1));
      SqlScalarExpression scalarExpression2 = (SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression(2));
      SqlScalarExpression consequent = scalarExpression1;
      SqlScalarExpression alternative = scalarExpression2;
      return (SqlObject) SqlConditionalScalarExpression.Create(condition, consequent, alternative);
    }

    public override SqlObject VisitExistsScalarExpression(
      [NotNull] sqlParser.ExistsScalarExpressionContext context)
    {
      return (SqlObject) SqlExistsScalarExpression.Create((SqlQuery) this.Visit(context.children[2]));
    }

    public override SqlObject VisitFunctionCallScalarExpression(
      [NotNull] sqlParser.FunctionCallScalarExpressionContext context)
    {
      bool isUdf = context.K_UDF() != null;
      SqlIdentifier name = SqlIdentifier.Create(context.IDENTIFIER().GetText());
      List<SqlScalarExpression> items = new List<SqlScalarExpression>();
      if (context.scalar_expression_list() != null)
      {
        foreach (sqlParser.Scalar_expressionContext tree in context.scalar_expression_list().scalar_expression())
          items.Add((SqlScalarExpression) this.Visit((IParseTree) tree));
      }
      return (SqlObject) SqlFunctionCallScalarExpression.Create(name, isUdf, items.ToImmutableArray<SqlScalarExpression>());
    }

    public override SqlObject VisitIn_scalar_expression(
      [NotNull] sqlParser.In_scalar_expressionContext context)
    {
      SqlScalarExpression needle = (SqlScalarExpression) this.Visit((IParseTree) context.binary_scalar_expression());
      bool not = context.K_NOT() != null;
      List<SqlScalarExpression> items = new List<SqlScalarExpression>();
      foreach (sqlParser.Scalar_expressionContext tree in context.scalar_expression_list().scalar_expression())
        items.Add((SqlScalarExpression) this.Visit((IParseTree) tree));
      return (SqlObject) SqlInScalarExpression.Create(needle, not, items.ToImmutableArray<SqlScalarExpression>());
    }

    public override SqlObject VisitLike_scalar_expression(
      [NotNull] sqlParser.Like_scalar_expressionContext context)
    {
      SqlScalarExpression expression = (SqlScalarExpression) this.Visit((IParseTree) context.binary_scalar_expression()[0]);
      SqlScalarExpression scalarExpression = (SqlScalarExpression) this.Visit((IParseTree) context.binary_scalar_expression()[1]);
      bool flag = context.K_NOT() != null;
      SqlStringLiteral sqlStringLiteral = context.escape_expression() != null ? SqlStringLiteral.Create(CstToAstVisitor.GetStringValueFromNode((IParseTree) context.escape_expression().STRING_LITERAL())) : (SqlStringLiteral) null;
      SqlScalarExpression pattern = scalarExpression;
      int num = flag ? 1 : 0;
      SqlStringLiteral escapeSequence = sqlStringLiteral;
      return (SqlObject) SqlLikeScalarExpression.Create(expression, pattern, num != 0, escapeSequence);
    }

    public override SqlObject VisitEscape_expression([NotNull] sqlParser.Escape_expressionContext context) => this.Visit((IParseTree) context.STRING_LITERAL());

    public override SqlObject VisitLiteralScalarExpression(
      [NotNull] sqlParser.LiteralScalarExpressionContext context)
    {
      return (SqlObject) SqlLiteralScalarExpression.Create((SqlLiteral) this.Visit((IParseTree) context.literal()));
    }

    public override SqlObject VisitLiteral([NotNull] sqlParser.LiteralContext context)
    {
      TerminalNodeImpl child = (TerminalNodeImpl) context.children[0];
      switch (child.Symbol.Type)
      {
        case 38:
          return (SqlObject) SqlBooleanLiteral.Create(false);
        case 46:
          return (SqlObject) SqlNullLiteral.Create();
        case 52:
          return (SqlObject) SqlBooleanLiteral.Create(true);
        case 54:
          return (SqlObject) SqlUndefinedLiteral.Create();
        case 58:
          return (SqlObject) SqlNumberLiteral.Create(CstToAstVisitor.GetNumber64ValueFromNode((IParseTree) child));
        case 59:
          return (SqlObject) SqlStringLiteral.Create(CstToAstVisitor.GetStringValueFromNode((IParseTree) child));
        default:
          throw new ArgumentOutOfRangeException(string.Format("Unknown symbol type: {0}", (object) child.Symbol.Type));
      }
    }

    public override SqlObject VisitMemberIndexerScalarExpression(
      [NotNull] sqlParser.MemberIndexerScalarExpressionContext context)
    {
      return (SqlObject) SqlMemberIndexerScalarExpression.Create((SqlScalarExpression) this.Visit((IParseTree) context.primary_expression()), (SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression()));
    }

    public override SqlObject VisitObjectCreateScalarExpression(
      [NotNull] sqlParser.ObjectCreateScalarExpressionContext context)
    {
      List<SqlObjectProperty> items = new List<SqlObjectProperty>();
      if (context.object_property_list() != null)
      {
        foreach (IParseTree tree in context.object_property_list().object_property())
        {
          SqlObjectProperty sqlObjectProperty = (SqlObjectProperty) this.Visit(tree);
          items.Add(sqlObjectProperty);
        }
      }
      return (SqlObject) SqlObjectCreateScalarExpression.Create(items.ToImmutableArray<SqlObjectProperty>());
    }

    public override SqlObject VisitObject_property([NotNull] sqlParser.Object_propertyContext context)
    {
      string stringValueFromNode = CstToAstVisitor.GetStringValueFromNode((IParseTree) context.STRING_LITERAL());
      SqlScalarExpression expression = (SqlScalarExpression) this.Visit((IParseTree) context.scalar_expression());
      return (SqlObject) SqlObjectProperty.Create(SqlPropertyName.Create(stringValueFromNode), expression);
    }

    public override SqlObject VisitParameterRefScalarExpression(
      [NotNull] sqlParser.ParameterRefScalarExpressionContext context)
    {
      return (SqlObject) SqlParameterRefScalarExpression.Create(Microsoft.Azure.Cosmos.SqlObjects.SqlParameter.Create(context.PARAMETER().GetText()));
    }

    public override SqlObject VisitPropertyRefScalarExpressionBase(
      [NotNull] sqlParser.PropertyRefScalarExpressionBaseContext context)
    {
      return (SqlObject) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(context.IDENTIFIER().GetText()));
    }

    public override SqlObject VisitPropertyRefScalarExpressionRecursive(
      [NotNull] sqlParser.PropertyRefScalarExpressionRecursiveContext context)
    {
      return (SqlObject) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) this.Visit((IParseTree) context.primary_expression()), SqlIdentifier.Create(context.IDENTIFIER().GetText()));
    }

    public override SqlObject VisitSubqueryScalarExpression(
      [NotNull] sqlParser.SubqueryScalarExpressionContext context)
    {
      return (SqlObject) SqlSubqueryScalarExpression.Create((SqlQuery) this.Visit((IParseTree) context.sql_query()));
    }

    public override SqlObject VisitUnary_scalar_expression(
      [NotNull] sqlParser.Unary_scalar_expressionContext context)
    {
      if (context.primary_expression() != null)
        return this.Visit((IParseTree) context.primary_expression());
      string text = context.unary_operator().GetText();
      SqlUnaryScalarOperatorKind operatorKind;
      if (!CstToAstVisitor.unaryOperatorKindLookup.TryGetValue(text, out operatorKind))
        throw new ArgumentOutOfRangeException("Unknown unary operator: " + text + ".");
      SqlScalarExpression expression = (SqlScalarExpression) this.Visit((IParseTree) context.unary_scalar_expression());
      return (SqlObject) SqlUnaryScalarExpression.Create(operatorKind, expression);
    }

    public override SqlObject VisitParenthesizedScalarExperession(
      [NotNull] sqlParser.ParenthesizedScalarExperessionContext context)
    {
      return this.Visit(context.children[1]);
    }

    public override SqlObject VisitLogical_scalar_expression(
      [NotNull] sqlParser.Logical_scalar_expressionContext context)
    {
      SqlObject sqlObject;
      if (context.binary_scalar_expression() != null)
        sqlObject = this.Visit((IParseTree) context.binary_scalar_expression());
      else if (context.in_scalar_expression() != null)
        sqlObject = this.Visit((IParseTree) context.in_scalar_expression());
      else if (context.like_scalar_expression() != null)
      {
        sqlObject = this.Visit((IParseTree) context.like_scalar_expression());
      }
      else
      {
        SqlScalarExpression left = (SqlScalarExpression) this.Visit((IParseTree) context.logical_scalar_expression(0));
        SqlBinaryScalarOperatorKind operatorKind;
        if (!CstToAstVisitor.binaryOperatorKindLookup.TryGetValue(context.children[1].GetText(), out operatorKind))
          throw new ArgumentOutOfRangeException("Unknown logical operator: " + context.children[1].GetText() + ".");
        SqlScalarExpression right = (SqlScalarExpression) this.Visit((IParseTree) context.logical_scalar_expression(1));
        sqlObject = (SqlObject) SqlBinaryScalarExpression.Create(operatorKind, left, right);
      }
      return sqlObject;
    }

    public override SqlObject VisitAdditive_operator([NotNull] sqlParser.Additive_operatorContext context) => throw new NotSupportedException();

    public override SqlObject VisitBitwise_and_operator(
      [NotNull] sqlParser.Bitwise_and_operatorContext context)
    {
      throw new NotSupportedException();
    }

    public override SqlObject VisitBitwise_exclusive_or_operator(
      [NotNull] sqlParser.Bitwise_exclusive_or_operatorContext context)
    {
      throw new NotSupportedException();
    }

    public override SqlObject VisitBitwise_inclusive_or_operator(
      [NotNull] sqlParser.Bitwise_inclusive_or_operatorContext context)
    {
      throw new NotSupportedException();
    }

    public override SqlObject VisitEquality_operator([NotNull] sqlParser.Equality_operatorContext context) => throw new NotSupportedException();

    public override SqlObject VisitMultiplicative_operator(
      [NotNull] sqlParser.Multiplicative_operatorContext context)
    {
      throw new NotSupportedException();
    }

    public override SqlObject VisitRelational_operator([NotNull] sqlParser.Relational_operatorContext context) => throw new NotSupportedException();

    public override SqlObject VisitString_concat_operator(
      [NotNull] sqlParser.String_concat_operatorContext context)
    {
      throw new NotSupportedException();
    }

    public override SqlObject VisitUnary_operator([NotNull] sqlParser.Unary_operatorContext context) => throw new NotSupportedException();

    public override SqlObject VisitObject_property_list(
      [NotNull] sqlParser.Object_property_listContext context)
    {
      throw new NotSupportedException();
    }

    public override SqlObject VisitOrder_by_items([NotNull] sqlParser.Order_by_itemsContext context) => throw new NotSupportedException();

    public override SqlObject VisitSort_order([NotNull] sqlParser.Sort_orderContext context) => throw new NotSupportedException();

    public override SqlObject VisitScalar_expression_list(
      [NotNull] sqlParser.Scalar_expression_listContext context)
    {
      throw new NotSupportedException();
    }

    private static string GetStringValueFromNode(IParseTree parseTree)
    {
      string text = parseTree.GetText();
      return text.Substring(1, text.Length - 2).Replace("\\\"", "\"");
    }

    private static Number64 GetNumber64ValueFromNode(IParseTree parseTree)
    {
      string text = parseTree.GetText();
      long result;
      return !long.TryParse(text, out result) ? (Number64) double.Parse(text) : (Number64) result;
    }

    private sealed class UnknownSqlObjectException : ArgumentOutOfRangeException
    {
      public UnknownSqlObjectException(SqlObject sqlObject, Exception innerException = null)
        : base("Unknown SqlObject: " + (sqlObject?.GetType()?.ToString() ?? "<NULL>"), innerException)
      {
      }
    }
  }
}
