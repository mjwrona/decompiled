// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlObjectEqualityVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal sealed class SqlObjectEqualityVisitor : SqlObjectVisitor<SqlObject, bool>
  {
    public static readonly SqlObjectEqualityVisitor Singleton = new SqlObjectEqualityVisitor();

    private SqlObjectEqualityVisitor()
    {
    }

    public override bool Visit(SqlAliasedCollectionExpression first, SqlObject secondAsObject) => secondAsObject is SqlAliasedCollectionExpression collectionExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Alias, (SqlObject) collectionExpression.Alias) && first.Collection.Accept<SqlObject, bool>((SqlObjectVisitor<SqlObject, bool>) this, (SqlObject) collectionExpression.Collection);

    public override bool Visit(SqlArrayCreateScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlArrayCreateScalarExpression scalarExpression && SqlObjectEqualityVisitor.SequenceEquals((IReadOnlyList<SqlObject>) first.Items, (IReadOnlyList<SqlObject>) scalarExpression.Items);

    public override bool Visit(SqlArrayIteratorCollectionExpression first, SqlObject secondAsObject) => secondAsObject is SqlArrayIteratorCollectionExpression collectionExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Identifier, (SqlObject) collectionExpression.Identifier) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Collection, (SqlObject) collectionExpression.Collection);

    public override bool Visit(SqlArrayScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlArrayScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.SqlQuery, (SqlObject) scalarExpression.SqlQuery);

    public override bool Visit(SqlBetweenScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlBetweenScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Expression, (SqlObject) scalarExpression.Expression) && first.Not == scalarExpression.Not && SqlObjectEqualityVisitor.Equals((SqlObject) first.StartInclusive, (SqlObject) scalarExpression.StartInclusive) && SqlObjectEqualityVisitor.Equals((SqlObject) first.EndInclusive, (SqlObject) scalarExpression.EndInclusive);

    public override bool Visit(SqlBinaryScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlBinaryScalarExpression scalarExpression && first.OperatorKind == scalarExpression.OperatorKind && SqlObjectEqualityVisitor.Equals((SqlObject) first.LeftExpression, (SqlObject) scalarExpression.LeftExpression) && SqlObjectEqualityVisitor.Equals((SqlObject) first.RightExpression, (SqlObject) scalarExpression.RightExpression);

    public override bool Visit(SqlBooleanLiteral first, SqlObject secondAsObject) => secondAsObject is SqlBooleanLiteral sqlBooleanLiteral && first.Value == sqlBooleanLiteral.Value;

    public override bool Visit(SqlCoalesceScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlCoalesceScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Left, (SqlObject) scalarExpression.Left) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Right, (SqlObject) scalarExpression.Right);

    public override bool Visit(SqlConditionalScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlConditionalScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Condition, (SqlObject) scalarExpression.Condition) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Consequent, (SqlObject) scalarExpression.Consequent) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Alternative, (SqlObject) scalarExpression.Alternative);

    public override bool Visit(SqlExistsScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlExistsScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Subquery, (SqlObject) scalarExpression.Subquery);

    public override bool Visit(SqlFromClause first, SqlObject secondAsObject) => secondAsObject is SqlFromClause sqlFromClause && SqlObjectEqualityVisitor.Equals((SqlObject) first.Expression, (SqlObject) sqlFromClause.Expression);

    public override bool Visit(SqlFunctionCallScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlFunctionCallScalarExpression scalarExpression && first.IsUdf == scalarExpression.IsUdf && SqlObjectEqualityVisitor.Equals((SqlObject) first.Name, (SqlObject) scalarExpression.Name) && SqlObjectEqualityVisitor.SequenceEquals((IReadOnlyList<SqlObject>) first.Arguments, (IReadOnlyList<SqlObject>) scalarExpression.Arguments);

    public override bool Visit(SqlGroupByClause first, SqlObject secondAsObject) => secondAsObject is SqlGroupByClause sqlGroupByClause && SqlObjectEqualityVisitor.SequenceEquals((IReadOnlyList<SqlObject>) first.Expressions, (IReadOnlyList<SqlObject>) sqlGroupByClause.Expressions);

    public override bool Visit(SqlIdentifier first, SqlObject secondAsObject) => secondAsObject is SqlIdentifier sqlIdentifier && !(first.Value != sqlIdentifier.Value);

    public override bool Visit(SqlIdentifierPathExpression first, SqlObject secondAsObject) => secondAsObject is SqlIdentifierPathExpression identifierPathExpression && first.Value.Accept<SqlObject, bool>((SqlObjectVisitor<SqlObject, bool>) this, (SqlObject) identifierPathExpression.Value) && SqlObjectEqualityVisitor.Equals((SqlObject) first.ParentPath, (SqlObject) identifierPathExpression.ParentPath);

    public override bool Visit(SqlInputPathCollection first, SqlObject secondAsObject) => secondAsObject is SqlInputPathCollection inputPathCollection && first.Input.Accept<SqlObject, bool>((SqlObjectVisitor<SqlObject, bool>) this, (SqlObject) inputPathCollection.Input) && SqlObjectEqualityVisitor.Equals((SqlObject) first.RelativePath, (SqlObject) inputPathCollection.RelativePath);

    public override bool Visit(SqlInScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlInScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Needle, (SqlObject) scalarExpression.Needle) && first.Not == scalarExpression.Not && SqlObjectEqualityVisitor.SequenceEquals((IReadOnlyList<SqlObject>) first.Haystack, (IReadOnlyList<SqlObject>) scalarExpression.Haystack);

    public override bool Visit(SqlJoinCollectionExpression first, SqlObject secondAsObject) => secondAsObject is SqlJoinCollectionExpression collectionExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Left, (SqlObject) collectionExpression.Left) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Right, (SqlObject) collectionExpression.Right);

    public override bool Visit(SqlLikeScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlLikeScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Expression, (SqlObject) scalarExpression.Expression) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Pattern, (SqlObject) scalarExpression.Pattern) && object.Equals((object) first.Not, (object) scalarExpression.Not) && SqlObjectEqualityVisitor.Equals((SqlObject) first.EscapeSequence, (SqlObject) scalarExpression.EscapeSequence);

    public override bool Visit(SqlLimitSpec first, SqlObject secondAsObject) => secondAsObject is SqlLimitSpec sqlLimitSpec && SqlObjectEqualityVisitor.Equals((SqlObject) first.LimitExpression, (SqlObject) sqlLimitSpec.LimitExpression);

    public override bool Visit(SqlLiteralScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlLiteralScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Literal, (SqlObject) scalarExpression.Literal);

    public override bool Visit(SqlMemberIndexerScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlMemberIndexerScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Member, (SqlObject) scalarExpression.Member) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Indexer, (SqlObject) scalarExpression.Indexer);

    public override bool Visit(SqlNullLiteral first, SqlObject secondAsObject) => secondAsObject is SqlNullLiteral;

    public override bool Visit(SqlNumberLiteral first, SqlObject secondAsObject) => secondAsObject is SqlNumberLiteral sqlNumberLiteral && first.Value.Equals(sqlNumberLiteral.Value);

    public override bool Visit(SqlNumberPathExpression first, SqlObject secondAsObject) => secondAsObject is SqlNumberPathExpression numberPathExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Value, (SqlObject) numberPathExpression.Value);

    public override bool Visit(SqlObjectCreateScalarExpression first, SqlObject secondAsObject)
    {
      if (!(secondAsObject is SqlObjectCreateScalarExpression scalarExpression) || first.Properties.Count<SqlObjectProperty>() != scalarExpression.Properties.Count<SqlObjectProperty>())
        return false;
      foreach (SqlObjectProperty property1 in first.Properties)
      {
        bool flag = false;
        foreach (SqlObjectProperty property2 in scalarExpression.Properties)
          flag |= SqlObjectEqualityVisitor.Equals((SqlObject) property1, (SqlObject) property2);
        if (!flag)
          return false;
      }
      return true;
    }

    public override bool Visit(SqlObjectProperty first, SqlObject secondAsObject) => secondAsObject is SqlObjectProperty sqlObjectProperty && SqlObjectEqualityVisitor.Equals((SqlObject) first.Name, (SqlObject) sqlObjectProperty.Name) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Value, (SqlObject) sqlObjectProperty.Value);

    public override bool Visit(SqlOffsetLimitClause first, SqlObject secondAsObject) => secondAsObject is SqlOffsetLimitClause offsetLimitClause && SqlObjectEqualityVisitor.Equals((SqlObject) first.LimitSpec, (SqlObject) offsetLimitClause.LimitSpec) && SqlObjectEqualityVisitor.Equals((SqlObject) first.OffsetSpec, (SqlObject) offsetLimitClause.OffsetSpec);

    public override bool Visit(SqlOffsetSpec first, SqlObject secondAsObject) => secondAsObject is SqlOffsetSpec sqlOffsetSpec && SqlObjectEqualityVisitor.Equals((SqlObject) first.OffsetExpression, (SqlObject) sqlOffsetSpec.OffsetExpression);

    public override bool Visit(SqlOrderByClause first, SqlObject secondAsObject) => secondAsObject is SqlOrderByClause sqlOrderByClause && SqlObjectEqualityVisitor.SequenceEquals((IReadOnlyList<SqlObject>) first.OrderByItems, (IReadOnlyList<SqlObject>) sqlOrderByClause.OrderByItems);

    public override bool Visit(SqlOrderByItem first, SqlObject secondAsObject) => secondAsObject is SqlOrderByItem sqlOrderByItem && first.IsDescending == sqlOrderByItem.IsDescending && SqlObjectEqualityVisitor.Equals((SqlObject) first.Expression, (SqlObject) sqlOrderByItem.Expression);

    public override bool Visit(SqlParameter first, SqlObject secondAsObject) => secondAsObject is SqlParameter sqlParameter && !(first.Name != sqlParameter.Name);

    public override bool Visit(SqlParameterRefScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlParameterRefScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Parameter, (SqlObject) scalarExpression.Parameter);

    public override bool Visit(SqlProgram first, SqlObject secondAsObject) => secondAsObject is SqlProgram sqlProgram && SqlObjectEqualityVisitor.Equals((SqlObject) first.Query, (SqlObject) sqlProgram.Query);

    public override bool Visit(SqlPropertyName first, SqlObject secondAsObject) => secondAsObject is SqlPropertyName sqlPropertyName && !(first.Value != sqlPropertyName.Value);

    public override bool Visit(SqlPropertyRefScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlPropertyRefScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Identifier, (SqlObject) scalarExpression.Identifier);

    public override bool Visit(SqlQuery first, SqlObject secondAsObject) => secondAsObject is SqlQuery sqlQuery && first.SelectClause.Accept<SqlObject, bool>((SqlObjectVisitor<SqlObject, bool>) this, (SqlObject) sqlQuery.SelectClause) && SqlObjectEqualityVisitor.Equals((SqlObject) first.FromClause, (SqlObject) sqlQuery.FromClause) && SqlObjectEqualityVisitor.Equals((SqlObject) first.WhereClause, (SqlObject) sqlQuery.WhereClause) && SqlObjectEqualityVisitor.Equals((SqlObject) first.GroupByClause, (SqlObject) sqlQuery.GroupByClause) && SqlObjectEqualityVisitor.Equals((SqlObject) first.OrderByClause, (SqlObject) sqlQuery.OrderByClause) && SqlObjectEqualityVisitor.Equals((SqlObject) first.OffsetLimitClause, (SqlObject) sqlQuery.OffsetLimitClause);

    public override bool Visit(SqlSelectClause first, SqlObject secondAsObject) => secondAsObject is SqlSelectClause sqlSelectClause && first.HasDistinct == sqlSelectClause.HasDistinct && SqlObjectEqualityVisitor.Equals((SqlObject) first.SelectSpec, (SqlObject) sqlSelectClause.SelectSpec) && SqlObjectEqualityVisitor.Equals((SqlObject) first.TopSpec, (SqlObject) sqlSelectClause.TopSpec);

    public override bool Visit(SqlSelectItem first, SqlObject secondAsObject) => secondAsObject is SqlSelectItem sqlSelectItem && SqlObjectEqualityVisitor.Equals((SqlObject) first.Alias, (SqlObject) sqlSelectItem.Alias) && SqlObjectEqualityVisitor.Equals((SqlObject) first.Expression, (SqlObject) sqlSelectItem.Expression);

    public override bool Visit(SqlSelectListSpec first, SqlObject secondAsObject) => secondAsObject is SqlSelectListSpec sqlSelectListSpec && SqlObjectEqualityVisitor.SequenceEquals((IReadOnlyList<SqlObject>) first.Items, (IReadOnlyList<SqlObject>) sqlSelectListSpec.Items);

    public override bool Visit(SqlSelectStarSpec first, SqlObject secondAsObject) => secondAsObject is SqlSelectStarSpec;

    public override bool Visit(SqlSelectValueSpec first, SqlObject secondAsObject) => secondAsObject is SqlSelectValueSpec sqlSelectValueSpec && SqlObjectEqualityVisitor.Equals((SqlObject) first.Expression, (SqlObject) sqlSelectValueSpec.Expression);

    public override bool Visit(SqlStringLiteral first, SqlObject secondAsObject) => secondAsObject is SqlStringLiteral sqlStringLiteral && !(first.Value != sqlStringLiteral.Value);

    public override bool Visit(SqlStringPathExpression first, SqlObject secondAsObject) => secondAsObject is SqlStringPathExpression stringPathExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Value, (SqlObject) stringPathExpression.Value) && SqlObjectEqualityVisitor.Equals((SqlObject) first.ParentPath, (SqlObject) stringPathExpression.ParentPath);

    public override bool Visit(SqlSubqueryCollection first, SqlObject secondAsObject) => secondAsObject is SqlSubqueryCollection subqueryCollection && SqlObjectEqualityVisitor.Equals((SqlObject) first.Query, (SqlObject) subqueryCollection.Query);

    public override bool Visit(SqlSubqueryScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlSubqueryScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Query, (SqlObject) scalarExpression.Query);

    public override bool Visit(SqlTopSpec first, SqlObject secondAsObject) => secondAsObject is SqlTopSpec sqlTopSpec && SqlObjectEqualityVisitor.Equals((SqlObject) first.TopExpresion, (SqlObject) sqlTopSpec.TopExpresion);

    public override bool Visit(SqlUnaryScalarExpression first, SqlObject secondAsObject) => secondAsObject is SqlUnaryScalarExpression scalarExpression && SqlObjectEqualityVisitor.Equals((SqlObject) first.Expression, (SqlObject) scalarExpression.Expression);

    public override bool Visit(SqlUndefinedLiteral first, SqlObject secondAsObject) => secondAsObject is SqlUndefinedLiteral;

    public override bool Visit(SqlWhereClause first, SqlObject secondAsObject) => secondAsObject is SqlWhereClause sqlWhereClause && SqlObjectEqualityVisitor.Equals((SqlObject) first.FilterExpression, (SqlObject) sqlWhereClause.FilterExpression);

    private static bool SequenceEquals(
      IReadOnlyList<SqlObject> firstList,
      IReadOnlyList<SqlObject> secondList)
    {
      if (firstList.Count != secondList.Count)
        return false;
      foreach ((SqlObject sqlObject, SqlObject input) in firstList.Zip<SqlObject, SqlObject, (SqlObject, SqlObject)>((IEnumerable<SqlObject>) secondList, (Func<SqlObject, SqlObject, (SqlObject, SqlObject)>) ((first, second) => (first, second))))
      {
        if (!sqlObject.Accept<SqlObject, bool>((SqlObjectVisitor<SqlObject, bool>) SqlObjectEqualityVisitor.Singleton, input))
          return false;
      }
      return true;
    }

    private static bool BothNull(SqlObject first, SqlObject second) => (object) first == null && (object) second == null;

    private static bool DifferentNullality(SqlObject first, SqlObject second)
    {
      if ((object) first == null && (object) second != null)
        return true;
      return (object) first != null && (object) second == null;
    }

    private static bool Equals(SqlObject first, SqlObject second)
    {
      if (SqlObjectEqualityVisitor.BothNull(first, second))
        return true;
      return !SqlObjectEqualityVisitor.DifferentNullality(first, second) && first.Accept<SqlObject, bool>((SqlObjectVisitor<SqlObject, bool>) SqlObjectEqualityVisitor.Singleton, second);
    }
  }
}
