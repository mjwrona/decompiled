// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlObjectObfuscator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal sealed class SqlObjectObfuscator : SqlObjectVisitor<SqlObject>
  {
    private static readonly HashSet<string> ExemptedString = new HashSet<string>()
    {
      "GeometryCollection",
      "LineString",
      "MultiLineString",
      "MultiPoint",
      "MultiPolygon",
      "Point",
      "Polygon",
      "_attachments",
      "_etag",
      "_rid",
      "_self",
      "_ts",
      "coordinates",
      "id",
      "name",
      "type"
    };
    private readonly Dictionary<string, string> obfuscatedStrings = new Dictionary<string, string>();
    private readonly Dictionary<Number64, Number64> obfuscatedNumbers = new Dictionary<Number64, Number64>();
    private int numberSequenceNumber;
    private int stringSequenceNumber;
    private int identifierSequenceNumber;
    private int fieldNameSequenceNumber;
    private int paramaterSequenceNumber;

    public override SqlObject Visit(
      SqlAliasedCollectionExpression sqlAliasedCollectionExpression)
    {
      return (SqlObject) SqlAliasedCollectionExpression.Create(sqlAliasedCollectionExpression.Collection.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollection, sqlAliasedCollectionExpression.Alias.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier);
    }

    public override SqlObject Visit(
      SqlArrayCreateScalarExpression sqlArrayCreateScalarExpression)
    {
      List<SqlScalarExpression> items = new List<SqlScalarExpression>();
      foreach (SqlScalarExpression scalarExpression in sqlArrayCreateScalarExpression.Items)
        items.Add(scalarExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
      return (SqlObject) SqlArrayCreateScalarExpression.Create(items.ToImmutableArray<SqlScalarExpression>());
    }

    public override SqlObject Visit(
      SqlArrayIteratorCollectionExpression sqlArrayIteratorCollectionExpression)
    {
      return (SqlObject) SqlArrayIteratorCollectionExpression.Create(sqlArrayIteratorCollectionExpression.Identifier.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier, sqlArrayIteratorCollectionExpression.Collection.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollection);
    }

    public override SqlObject Visit(SqlArrayScalarExpression sqlArrayScalarExpression) => (SqlObject) SqlArrayScalarExpression.Create(sqlArrayScalarExpression.SqlQuery.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlQuery);

    public override SqlObject Visit(
      SqlBetweenScalarExpression sqlBetweenScalarExpression)
    {
      return (SqlObject) SqlBetweenScalarExpression.Create(sqlBetweenScalarExpression.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlBetweenScalarExpression.StartInclusive.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlBetweenScalarExpression.EndInclusive.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlBetweenScalarExpression.Not);
    }

    public override SqlObject Visit(
      SqlBinaryScalarExpression sqlBinaryScalarExpression)
    {
      return (SqlObject) SqlBinaryScalarExpression.Create(sqlBinaryScalarExpression.OperatorKind, sqlBinaryScalarExpression.LeftExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlBinaryScalarExpression.RightExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
    }

    public override SqlObject Visit(SqlBooleanLiteral sqlBooleanLiteral) => (SqlObject) sqlBooleanLiteral;

    public override SqlObject Visit(
      SqlCoalesceScalarExpression sqlCoalesceScalarExpression)
    {
      return (SqlObject) SqlCoalesceScalarExpression.Create(sqlCoalesceScalarExpression.Left.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlCoalesceScalarExpression.Right.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
    }

    public override SqlObject Visit(
      SqlConditionalScalarExpression sqlConditionalScalarExpression)
    {
      return (SqlObject) SqlConditionalScalarExpression.Create(sqlConditionalScalarExpression.Condition.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlConditionalScalarExpression.Consequent.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlConditionalScalarExpression.Alternative.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
    }

    public override SqlObject Visit(
      SqlExistsScalarExpression sqlExistsScalarExpression)
    {
      return (SqlObject) SqlExistsScalarExpression.Create(sqlExistsScalarExpression.Subquery.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlQuery);
    }

    public override SqlObject Visit(SqlFromClause sqlFromClause) => (SqlObject) SqlFromClause.Create(sqlFromClause.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollectionExpression);

    public override SqlObject Visit(
      SqlFunctionCallScalarExpression sqlFunctionCallScalarExpression)
    {
      SqlScalarExpression[] scalarExpressionArray = new SqlScalarExpression[sqlFunctionCallScalarExpression.Arguments.Length];
      for (int index = 0; index < sqlFunctionCallScalarExpression.Arguments.Length; ++index)
        scalarExpressionArray[index] = sqlFunctionCallScalarExpression.Arguments[index].Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression;
      return (SqlObject) SqlFunctionCallScalarExpression.Create(sqlFunctionCallScalarExpression.Name, sqlFunctionCallScalarExpression.IsUdf, scalarExpressionArray);
    }

    public override SqlObject Visit(SqlGroupByClause sqlGroupByClause)
    {
      SqlScalarExpression[] scalarExpressionArray = new SqlScalarExpression[sqlGroupByClause.Expressions.Length];
      for (int index = 0; index < sqlGroupByClause.Expressions.Length; ++index)
        scalarExpressionArray[index] = sqlGroupByClause.Expressions[index].Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression;
      return (SqlObject) SqlGroupByClause.Create(scalarExpressionArray);
    }

    public override SqlObject Visit(SqlIdentifier sqlIdentifier) => (SqlObject) SqlIdentifier.Create(this.GetObfuscatedString(sqlIdentifier.Value, "ident", ref this.identifierSequenceNumber));

    public override SqlObject Visit(
      SqlIdentifierPathExpression sqlIdentifierPathExpression)
    {
      return (SqlObject) SqlIdentifierPathExpression.Create(sqlIdentifierPathExpression.ParentPath?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlPathExpression, sqlIdentifierPathExpression.Value.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier);
    }

    public override SqlObject Visit(SqlInputPathCollection sqlInputPathCollection) => (SqlObject) SqlInputPathCollection.Create(sqlInputPathCollection.Input.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier, sqlInputPathCollection.RelativePath?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlPathExpression);

    public override SqlObject Visit(SqlInScalarExpression sqlInScalarExpression)
    {
      SqlScalarExpression[] scalarExpressionArray = new SqlScalarExpression[sqlInScalarExpression.Haystack.Length];
      for (int index = 0; index < sqlInScalarExpression.Haystack.Length; ++index)
        scalarExpressionArray[index] = sqlInScalarExpression.Haystack[index].Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression;
      return (SqlObject) SqlInScalarExpression.Create(sqlInScalarExpression.Needle.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlInScalarExpression.Not, scalarExpressionArray);
    }

    public override SqlObject Visit(
      SqlJoinCollectionExpression sqlJoinCollectionExpression)
    {
      return (SqlObject) SqlJoinCollectionExpression.Create(sqlJoinCollectionExpression.Left.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollectionExpression, sqlJoinCollectionExpression.Right.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollectionExpression);
    }

    public override SqlObject Visit(SqlLikeScalarExpression sqlLikeScalarExpression) => (SqlObject) SqlLikeScalarExpression.Create(sqlLikeScalarExpression.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlLikeScalarExpression.Pattern.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlLikeScalarExpression.Not, sqlLikeScalarExpression.EscapeSequence?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlStringLiteral);

    public override SqlObject Visit(SqlLimitSpec sqlObject) => (SqlObject) SqlLimitSpec.Create(SqlNumberLiteral.Create((Number64) 0L));

    public override SqlObject Visit(
      SqlLiteralScalarExpression sqlLiteralScalarExpression)
    {
      return (SqlObject) SqlLiteralScalarExpression.Create(sqlLiteralScalarExpression.Literal.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlLiteral);
    }

    public override SqlObject Visit(
      SqlMemberIndexerScalarExpression sqlMemberIndexerScalarExpression)
    {
      return (SqlObject) SqlMemberIndexerScalarExpression.Create(sqlMemberIndexerScalarExpression.Member.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlMemberIndexerScalarExpression.Indexer.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
    }

    public override SqlObject Visit(SqlNullLiteral sqlNullLiteral) => (SqlObject) sqlNullLiteral;

    public override SqlObject Visit(SqlNumberLiteral sqlNumberLiteral) => (SqlObject) SqlNumberLiteral.Create((Number64) Number64.ToDouble(this.GetObfuscatedNumber(sqlNumberLiteral.Value)));

    public override SqlObject Visit(SqlNumberPathExpression sqlNumberPathExpression) => (SqlObject) SqlNumberPathExpression.Create(sqlNumberPathExpression.ParentPath?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlPathExpression, sqlNumberPathExpression.Value.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlNumberLiteral);

    public override SqlObject Visit(
      SqlObjectCreateScalarExpression sqlObjectCreateScalarExpression)
    {
      List<SqlObjectProperty> items = new List<SqlObjectProperty>();
      foreach (SqlObjectProperty property in sqlObjectCreateScalarExpression.Properties)
        items.Add(property.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlObjectProperty);
      return (SqlObject) SqlObjectCreateScalarExpression.Create(items.ToImmutableArray<SqlObjectProperty>());
    }

    public override SqlObject Visit(SqlObjectProperty sqlObjectProperty) => (SqlObject) SqlObjectProperty.Create(sqlObjectProperty.Name.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlPropertyName, sqlObjectProperty.Value.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);

    public override SqlObject Visit(SqlOffsetLimitClause sqlObject) => (SqlObject) SqlOffsetLimitClause.Create(sqlObject.OffsetSpec.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlOffsetSpec, sqlObject.LimitSpec.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlLimitSpec);

    public override SqlObject Visit(SqlOffsetSpec sqlObject) => (SqlObject) SqlOffsetSpec.Create(SqlNumberLiteral.Create((Number64) 0L));

    public override SqlObject Visit(SqlOrderByClause sqlOrderByClause)
    {
      SqlOrderByItem[] sqlOrderByItemArray = new SqlOrderByItem[sqlOrderByClause.OrderByItems.Length];
      for (int index = 0; index < sqlOrderByClause.OrderByItems.Length; ++index)
        sqlOrderByItemArray[index] = sqlOrderByClause.OrderByItems[index].Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlOrderByItem;
      return (SqlObject) SqlOrderByClause.Create(sqlOrderByItemArray);
    }

    public override SqlObject Visit(SqlOrderByItem sqlOrderByItem) => (SqlObject) SqlOrderByItem.Create(sqlOrderByItem.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlOrderByItem.IsDescending);

    public override SqlObject Visit(SqlParameter sqlParameter) => (SqlObject) SqlParameter.Create(this.GetObfuscatedString(sqlParameter.Name, "param", ref this.paramaterSequenceNumber));

    public override SqlObject Visit(SqlParameterRefScalarExpression sqlObject) => (SqlObject) SqlParameterRefScalarExpression.Create(sqlObject.Parameter.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlParameter);

    public override SqlObject Visit(SqlProgram sqlProgram) => (SqlObject) SqlProgram.Create(sqlProgram.Query.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlQuery);

    public override SqlObject Visit(SqlPropertyName sqlPropertyName) => (SqlObject) SqlPropertyName.Create(this.GetObfuscatedString(sqlPropertyName.Value, "p", ref this.fieldNameSequenceNumber));

    public override SqlObject Visit(
      SqlPropertyRefScalarExpression sqlPropertyRefScalarExpression)
    {
      return (SqlObject) SqlPropertyRefScalarExpression.Create(sqlPropertyRefScalarExpression.Member?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlPropertyRefScalarExpression.Identifier.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier);
    }

    public override SqlObject Visit(SqlQuery sqlQuery) => (SqlObject) SqlQuery.Create(sqlQuery.SelectClause.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlSelectClause, sqlQuery.FromClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlFromClause, sqlQuery.WhereClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlWhereClause, sqlQuery.GroupByClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlGroupByClause, sqlQuery.OrderByClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlOrderByClause, sqlQuery.OffsetLimitClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlOffsetLimitClause);

    public override SqlObject Visit(SqlSelectClause sqlSelectClause) => (SqlObject) SqlSelectClause.Create(sqlSelectClause.SelectSpec.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlSelectSpec, sqlSelectClause.TopSpec?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlTopSpec, sqlSelectClause.HasDistinct);

    public override SqlObject Visit(SqlSelectItem sqlSelectItem) => (SqlObject) SqlSelectItem.Create(sqlSelectItem.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlSelectItem.Alias?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier);

    public override SqlObject Visit(SqlSelectListSpec sqlSelectListSpec)
    {
      List<SqlSelectItem> items = new List<SqlSelectItem>();
      foreach (SqlSelectItem sqlSelectItem in sqlSelectListSpec.Items)
        items.Add(sqlSelectItem.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlSelectItem);
      return (SqlObject) SqlSelectListSpec.Create(items.ToImmutableArray<SqlSelectItem>());
    }

    public override SqlObject Visit(SqlSelectStarSpec sqlSelectStarSpec) => (SqlObject) sqlSelectStarSpec;

    public override SqlObject Visit(SqlSelectValueSpec sqlSelectValueSpec) => (SqlObject) SqlSelectValueSpec.Create(sqlSelectValueSpec.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);

    public override SqlObject Visit(SqlStringLiteral sqlStringLiteral) => (SqlObject) SqlStringLiteral.Create(this.GetObfuscatedString(sqlStringLiteral.Value, "str", ref this.stringSequenceNumber));

    public override SqlObject Visit(SqlStringPathExpression sqlStringPathExpression) => (SqlObject) SqlStringPathExpression.Create(sqlStringPathExpression.ParentPath?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlPathExpression, sqlStringPathExpression.Value.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlStringLiteral);

    public override SqlObject Visit(SqlSubqueryCollection sqlSubqueryCollection) => (SqlObject) SqlSubqueryCollection.Create(sqlSubqueryCollection.Query.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlQuery);

    public override SqlObject Visit(
      SqlSubqueryScalarExpression sqlSubqueryScalarExpression)
    {
      return (SqlObject) SqlSubqueryScalarExpression.Create(sqlSubqueryScalarExpression.Query.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlQuery);
    }

    public override SqlObject Visit(SqlTopSpec sqlTopSpec) => (SqlObject) SqlTopSpec.Create(SqlNumberLiteral.Create((Number64) 0L));

    public override SqlObject Visit(SqlUnaryScalarExpression sqlUnaryScalarExpression) => (SqlObject) SqlUnaryScalarExpression.Create(sqlUnaryScalarExpression.OperatorKind, sqlUnaryScalarExpression.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);

    public override SqlObject Visit(SqlUndefinedLiteral sqlUndefinedLiteral) => (SqlObject) sqlUndefinedLiteral;

    public override SqlObject Visit(SqlWhereClause sqlWhereClause) => (SqlObject) SqlWhereClause.Create(sqlWhereClause.FilterExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);

    private Number64 GetObfuscatedNumber(Number64 value)
    {
      Number64 obfuscatedNumber;
      if (value.IsInfinity || value.IsInteger && Number64.ToLong(value) == long.MinValue || value.IsInteger && Math.Abs(Number64.ToLong(value)) < 100L || value.IsDouble && Math.Abs(Number64.ToDouble(value)) < 100.0 && (double) (long) Number64.ToDouble(value) == Number64.ToDouble(value) || value.IsDouble && Math.Abs(Number64.ToDouble(value)) <= double.Epsilon)
        obfuscatedNumber = value;
      else if (!this.obfuscatedNumbers.TryGetValue(value, out obfuscatedNumber))
      {
        double num1 = Number64.ToDouble(value);
        int num2 = ++this.numberSequenceNumber;
        double num3 = Math.Pow(10.0, Math.Floor(Math.Log10(Math.Abs(num1)))) * (double) num2 / 10000.0;
        obfuscatedNumber = (Number64) (Math.Round(num1, 2) + num3);
        this.obfuscatedNumbers.Add(value, obfuscatedNumber);
      }
      return obfuscatedNumber;
    }

    private string GetObfuscatedString(string value, string prefix, ref int sequence)
    {
      string obfuscatedString;
      if (value.Length <= 1)
        obfuscatedString = value;
      else if (SqlObjectObfuscator.ExemptedString.Contains(value))
        obfuscatedString = value;
      else if (!this.obfuscatedStrings.TryGetValue(value, out obfuscatedString))
      {
        ++sequence;
        obfuscatedString = value.Length < 10 ? string.Format("{0}{1}", (object) prefix, (object) sequence) : string.Format("{0}{1}__{2}", (object) prefix, (object) sequence, (object) value.Length);
        this.obfuscatedStrings.Add(value, obfuscatedString);
      }
      return obfuscatedString;
    }
  }
}
