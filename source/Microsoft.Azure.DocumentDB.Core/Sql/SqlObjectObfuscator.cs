// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlObjectObfuscator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Sql
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
    private int numberSequenceNumber;
    private int stringSequenceNumber;
    private int identifierSequenceNumber;
    private int fieldNameSequenceNumber;
    private readonly Dictionary<string, string> obfuscatedStrings = new Dictionary<string, string>();
    private readonly Dictionary<Number64, Number64> obfuscatedNumbers = new Dictionary<Number64, Number64>();

    public override SqlObject Visit(
      SqlAliasedCollectionExpression sqlAliasedCollectionExpression)
    {
      return (SqlObject) SqlAliasedCollectionExpression.Create(sqlAliasedCollectionExpression.Collection.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollection, sqlAliasedCollectionExpression.Alias.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier);
    }

    public override SqlObject Visit(
      SqlArrayCreateScalarExpression sqlArrayCreateScalarExpression)
    {
      List<SqlScalarExpression> items = new List<SqlScalarExpression>();
      foreach (SqlScalarExpression scalarExpression in (IEnumerable<SqlScalarExpression>) sqlArrayCreateScalarExpression.Items)
        items.Add(scalarExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
      return (SqlObject) SqlArrayCreateScalarExpression.Create((IReadOnlyList<SqlScalarExpression>) items);
    }

    public override SqlObject Visit(
      SqlArrayIteratorCollectionExpression sqlArrayIteratorCollectionExpression)
    {
      return (SqlObject) SqlArrayIteratorCollectionExpression.Create(sqlArrayIteratorCollectionExpression.Alias.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier, sqlArrayIteratorCollectionExpression.Collection.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollection);
    }

    public override SqlObject Visit(SqlArrayScalarExpression sqlArrayScalarExpression) => (SqlObject) SqlArrayScalarExpression.Create(sqlArrayScalarExpression.SqlQuery.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlQuery);

    public override SqlObject Visit(
      SqlBetweenScalarExpression sqlBetweenScalarExpression)
    {
      return (SqlObject) SqlBetweenScalarExpression.Create(sqlBetweenScalarExpression.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlBetweenScalarExpression.LeftExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlBetweenScalarExpression.RightExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlBetweenScalarExpression.IsNot);
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
      return (SqlObject) SqlCoalesceScalarExpression.Create(sqlCoalesceScalarExpression.LeftExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlCoalesceScalarExpression.RightExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
    }

    public override SqlObject Visit(
      SqlConditionalScalarExpression sqlConditionalScalarExpression)
    {
      return (SqlObject) SqlConditionalScalarExpression.Create(sqlConditionalScalarExpression.ConditionExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlConditionalScalarExpression.FirstExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlConditionalScalarExpression.SecondExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
    }

    public override SqlObject Visit(
      SqlExistsScalarExpression sqlExistsScalarExpression)
    {
      return (SqlObject) SqlExistsScalarExpression.Create(sqlExistsScalarExpression.SqlQuery.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlQuery);
    }

    public override SqlObject Visit(SqlFromClause sqlFromClause) => (SqlObject) SqlFromClause.Create(sqlFromClause.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollectionExpression);

    public override SqlObject Visit(
      SqlFunctionCallScalarExpression sqlFunctionCallScalarExpression)
    {
      SqlScalarExpression[] scalarExpressionArray = new SqlScalarExpression[sqlFunctionCallScalarExpression.Arguments.Count];
      for (int index = 0; index < sqlFunctionCallScalarExpression.Arguments.Count; ++index)
        scalarExpressionArray[index] = sqlFunctionCallScalarExpression.Arguments[index].Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression;
      return (SqlObject) SqlFunctionCallScalarExpression.Create(sqlFunctionCallScalarExpression.Name, sqlFunctionCallScalarExpression.IsUdf, scalarExpressionArray);
    }

    public override SqlObject Visit(SqlGroupByClause sqlGroupByClause)
    {
      SqlScalarExpression[] scalarExpressionArray = new SqlScalarExpression[sqlGroupByClause.Expressions.Count];
      for (int index = 0; index < sqlGroupByClause.Expressions.Count; ++index)
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
      SqlScalarExpression[] scalarExpressionArray = new SqlScalarExpression[sqlInScalarExpression.Items.Count];
      for (int index = 0; index < sqlInScalarExpression.Items.Count; ++index)
        scalarExpressionArray[index] = sqlInScalarExpression.Items[index].Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression;
      return (SqlObject) SqlInScalarExpression.Create(sqlInScalarExpression.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlInScalarExpression.Not, scalarExpressionArray);
    }

    public override SqlObject Visit(
      SqlJoinCollectionExpression sqlJoinCollectionExpression)
    {
      return (SqlObject) SqlJoinCollectionExpression.Create(sqlJoinCollectionExpression.LeftExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollectionExpression, sqlJoinCollectionExpression.RightExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlCollectionExpression);
    }

    public override SqlObject Visit(SqlLimitSpec sqlObject) => (SqlObject) SqlLimitSpec.Create(0L);

    public override SqlObject Visit(
      SqlLiteralArrayCollection sqlLiteralArrayCollection)
    {
      SqlScalarExpression[] scalarExpressionArray = new SqlScalarExpression[sqlLiteralArrayCollection.Items.Count];
      for (int index = 0; index < sqlLiteralArrayCollection.Items.Count; ++index)
        scalarExpressionArray[index] = sqlLiteralArrayCollection.Items[index].Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression;
      return (SqlObject) SqlLiteralArrayCollection.Create(scalarExpressionArray);
    }

    public override SqlObject Visit(
      SqlLiteralScalarExpression sqlLiteralScalarExpression)
    {
      return (SqlObject) SqlLiteralScalarExpression.Create(sqlLiteralScalarExpression.Literal.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlLiteral);
    }

    public override SqlObject Visit(
      SqlMemberIndexerScalarExpression sqlMemberIndexerScalarExpression)
    {
      return (SqlObject) SqlMemberIndexerScalarExpression.Create(sqlMemberIndexerScalarExpression.MemberExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlMemberIndexerScalarExpression.IndexExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);
    }

    public override SqlObject Visit(SqlNullLiteral sqlNullLiteral) => (SqlObject) sqlNullLiteral;

    public override SqlObject Visit(SqlNumberLiteral sqlNumberLiteral) => (SqlObject) SqlNumberLiteral.Create(Number64.ToDouble(this.GetObfuscatedNumber(sqlNumberLiteral.Value)));

    public override SqlObject Visit(SqlNumberPathExpression sqlNumberPathExpression) => (SqlObject) SqlNumberPathExpression.Create(sqlNumberPathExpression.ParentPath?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlPathExpression, sqlNumberPathExpression.Value.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlNumberLiteral);

    public override SqlObject Visit(
      SqlObjectCreateScalarExpression sqlObjectCreateScalarExpression)
    {
      List<SqlObjectProperty> properties = new List<SqlObjectProperty>();
      foreach (SqlObjectProperty property in sqlObjectCreateScalarExpression.Properties)
        properties.Add(property.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlObjectProperty);
      return (SqlObject) SqlObjectCreateScalarExpression.Create((IEnumerable<SqlObjectProperty>) properties);
    }

    public override SqlObject Visit(SqlObjectProperty sqlObjectProperty) => (SqlObject) SqlObjectProperty.Create(sqlObjectProperty.Name.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlPropertyName, sqlObjectProperty.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);

    public override SqlObject Visit(SqlOffsetLimitClause sqlObject) => (SqlObject) SqlOffsetLimitClause.Create(sqlObject.OffsetSpec.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlOffsetSpec, sqlObject.LimitSpec.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlLimitSpec);

    public override SqlObject Visit(SqlOffsetSpec sqlObject) => (SqlObject) SqlOffsetSpec.Create(0L);

    public override SqlObject Visit(SqlOrderbyClause sqlOrderByClause)
    {
      SqlOrderByItem[] sqlOrderByItemArray = new SqlOrderByItem[sqlOrderByClause.OrderbyItems.Count];
      for (int index = 0; index < sqlOrderByClause.OrderbyItems.Count; ++index)
        sqlOrderByItemArray[index] = sqlOrderByClause.OrderbyItems[index].Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlOrderByItem;
      return (SqlObject) SqlOrderbyClause.Create(sqlOrderByItemArray);
    }

    public override SqlObject Visit(SqlOrderByItem sqlOrderByItem) => (SqlObject) SqlOrderByItem.Create(sqlOrderByItem.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlOrderByItem.IsDescending);

    public override SqlObject Visit(SqlProgram sqlProgram) => (SqlObject) SqlProgram.Create(sqlProgram.Query.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlQuery);

    public override SqlObject Visit(SqlPropertyName sqlPropertyName) => (SqlObject) SqlPropertyName.Create(this.GetObfuscatedString(sqlPropertyName.Value, "p", ref this.fieldNameSequenceNumber));

    public override SqlObject Visit(
      SqlPropertyRefScalarExpression sqlPropertyRefScalarExpression)
    {
      return (SqlObject) SqlPropertyRefScalarExpression.Create(sqlPropertyRefScalarExpression.MemberExpression?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlPropertyRefScalarExpression.PropertyIdentifier.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier);
    }

    public override SqlObject Visit(SqlQuery sqlQuery) => (SqlObject) SqlQuery.Create(sqlQuery.SelectClause.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlSelectClause, sqlQuery.FromClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlFromClause, sqlQuery.WhereClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlWhereClause, sqlQuery.GroupByClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlGroupByClause, sqlQuery.OrderbyClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlOrderbyClause, sqlQuery.OffsetLimitClause?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlOffsetLimitClause);

    public override SqlObject Visit(SqlSelectClause sqlSelectClause) => (SqlObject) SqlSelectClause.Create(sqlSelectClause.SelectSpec.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlSelectSpec, sqlSelectClause.TopSpec?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlTopSpec, sqlSelectClause.HasDistinct);

    public override SqlObject Visit(SqlSelectItem sqlSelectItem) => (SqlObject) SqlSelectItem.Create(sqlSelectItem.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression, sqlSelectItem.Alias?.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlIdentifier);

    public override SqlObject Visit(SqlSelectListSpec sqlSelectListSpec)
    {
      List<SqlSelectItem> items = new List<SqlSelectItem>();
      foreach (SqlSelectItem sqlSelectItem in (IEnumerable<SqlSelectItem>) sqlSelectListSpec.Items)
        items.Add(sqlSelectItem.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlSelectItem);
      return (SqlObject) SqlSelectListSpec.Create((IReadOnlyList<SqlSelectItem>) items);
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

    public override SqlObject Visit(SqlTopSpec sqlTopSpec) => (SqlObject) SqlTopSpec.Create(0L);

    public override SqlObject Visit(SqlUnaryScalarExpression sqlUnaryScalarExpression) => (SqlObject) SqlUnaryScalarExpression.Create(sqlUnaryScalarExpression.OperatorKind, sqlUnaryScalarExpression.Expression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);

    public override SqlObject Visit(SqlUndefinedLiteral sqlUndefinedLiteral) => (SqlObject) sqlUndefinedLiteral;

    public override SqlObject Visit(SqlWhereClause sqlWhereClause) => (SqlObject) SqlWhereClause.Create(sqlWhereClause.FilterExpression.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) this) as SqlScalarExpression);

    public override SqlObject Visit(
      SqlConversionScalarExpression sqlConversionScalarExpression)
    {
      throw new NotImplementedException("This is not part of the actual grammar");
    }

    public override SqlObject Visit(
      SqlGeoNearCallScalarExpression sqlGeoNearCallScalarExpression)
    {
      throw new NotImplementedException("This is not part of the actual grammar");
    }

    public override SqlObject Visit(SqlObjectLiteral sqlObjectLiteral) => throw new NotImplementedException("This is not part of the actual grammar");

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
