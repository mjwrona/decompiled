// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlObjectHasher
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlObjectHasher : SqlObjectVisitor<int>
  {
    public static readonly SqlObjectHasher Singleton = new SqlObjectHasher(true);
    private const int SqlAliasedCollectionExpressionHashCode = 1202039781;
    private const int SqlArrayCreateScalarExpressionHashCode = 1760950661;
    private const int SqlArrayIteratorCollectionExpressionHashCode = -468874086;
    private const int SqlArrayScalarExpressionHashCode = -1093553293;
    private const int SqlBetweenScalarExpressionHashCode = -943872277;
    private const int SqlBetweenScalarExpressionNotHashCode = -1283200473;
    private const int SqlBinaryScalarExpressionHashCode = 1667146665;
    private const int SqlBooleanLiteralHashCode = 739161617;
    private const int SqlBooleanLiteralTrueHashCode = 1545461565;
    private const int SqlBooleanLiteralFalseHashCode = -2072875075;
    private const int SqlCoalesceScalarExpressionHashCode = -1400659633;
    private const int SqlConditionalScalarExpressionHashCode = -421337832;
    private const int SqlExistsScalarExpressionHashCode = 1168675587;
    private const int SqlFromClauseHashCode = 52588336;
    private const int SqlFunctionCallScalarExpressionHashCode = 496783446;
    private const int SqlFunctionCallScalarExpressionUdfHashCode = 1547906315;
    private const int SqlGroupByClauseHashCode = 130396242;
    private const int SqlIdentifierHashCode = -1664307981;
    private const int SqlIdentifierPathExpressionHashCode = -1445813508;
    private const int SqlInputPathCollectionHashCode = -209963066;
    private const int SqlInScalarExpressionHashCode = 1439386783;
    private const int SqlInScalarExpressionNotHashCode = -1131398119;
    private const int SqlJoinCollectionExpressionHashCode = 1000382226;
    private const int SqlLimitSpecHashCode = 92601316;
    private const int SqlLiteralArrayCollectionHashCode = 1634639566;
    private const int SqlLiteralScalarExpressionHashCode = -158339101;
    private const int SqlMemberIndexerScalarExpressionHashCode = 1589675618;
    private const int SqlNullLiteralHashCode = -709456592;
    private const int SqlNumberLiteralHashCode = 159836309;
    private const int SqlNumberPathExpressionHashCode = 874210976;
    private const int SqlObjectCreateScalarExpressionHashCode = -131129165;
    private const int SqlObjectPropertyHashCode = 1218972715;
    private const int SqlOffsetLimitClauseHashCode = 150154755;
    private const int SqlOffsetSpecHashCode = 109062001;
    private const int SqlOrderbyClauseHashCode = 1361708336;
    private const int SqlOrderbyItemHashCode = 846566057;
    private const int SqlOrderbyItemAscendingHashCode = -1123129997;
    private const int SqlOrderbyItemDescendingHashCode = -703648622;
    private const int SqlProgramHashCode = -492711050;
    private const int SqlPropertyNameHashCode = 1262661966;
    private const int SqlPropertyRefScalarExpressionHashCode = -1586896865;
    private const int SqlQueryHashCode = 1968642960;
    private const int SqlSelectClauseHashCode = 19731870;
    private const int SqlSelectClauseDistinctHashCode = 1467616881;
    private const int SqlSelectItemHashCode = -611151157;
    private const int SqlSelectListSpecHashCode = -1704039197;
    private const int SqlSelectStarSpecHashCode = -1125875092;
    private const int SqlSelectValueSpecHashCode = 507077368;
    private const int SqlStringLiteralHashCode = -1542874155;
    private const int SqlStringPathExpressionHashCode = -1280625326;
    private const int SqlSubqueryCollectionHashCode = 1175697100;
    private const int SqlSubqueryScalarExpressionHashCode = -1327458193;
    private const int SqlTopSpecHashCode = -791376698;
    private const int SqlUnaryScalarExpressionHashCode = 723832597;
    private const int SqlUndefinedLiteralHashCode = 1290712518;
    private const int SqlWhereClauseHashCode = -516465563;
    private readonly bool isStrict;

    public SqlObjectHasher(bool isStrict) => this.isStrict = isStrict;

    public override int Visit(
      SqlAliasedCollectionExpression sqlAliasedCollectionExpression)
    {
      int lhs = SqlObjectHasher.CombineHashes(1202039781L, (long) sqlAliasedCollectionExpression.Collection.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlAliasedCollectionExpression.Alias != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlAliasedCollectionExpression.Alias.Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(
      SqlArrayCreateScalarExpression sqlArrayCreateScalarExpression)
    {
      int lhs = 1760950661;
      for (int index = 0; index < sqlArrayCreateScalarExpression.Items.Count; ++index)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlArrayCreateScalarExpression.Items[index].Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(
      SqlArrayIteratorCollectionExpression sqlArrayIteratorCollectionExpression)
    {
      return SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(-468874086L, (long) sqlArrayIteratorCollectionExpression.Alias.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlArrayIteratorCollectionExpression.Collection.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlArrayScalarExpression sqlArrayScalarExpression) => SqlObjectHasher.CombineHashes(-1093553293L, (long) sqlArrayScalarExpression.SqlQuery.Accept<int>((SqlObjectVisitor<int>) this));

    public override int Visit(
      SqlBetweenScalarExpression sqlBetweenScalarExpression)
    {
      int lhs = SqlObjectHasher.CombineHashes(-943872277L, (long) sqlBetweenScalarExpression.Expression.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlBetweenScalarExpression.IsNot)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, -1283200473L);
      return SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes((long) lhs, (long) sqlBetweenScalarExpression.LeftExpression.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlBetweenScalarExpression.RightExpression.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(
      SqlBinaryScalarExpression sqlBinaryScalarExpression)
    {
      return SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(1667146665L, (long) sqlBinaryScalarExpression.LeftExpression.Accept<int>((SqlObjectVisitor<int>) this)), (long) SqlObjectHasher.SqlBinaryScalarOperatorKindGetHashCode(sqlBinaryScalarExpression.OperatorKind)), (long) sqlBinaryScalarExpression.RightExpression.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlBooleanLiteral sqlBooleanLiteral) => SqlObjectHasher.CombineHashes(739161617L, sqlBooleanLiteral.Value ? 1545461565L : -2072875075L);

    public override int Visit(
      SqlCoalesceScalarExpression sqlCoalesceScalarExpression)
    {
      return SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(-1400659633L, (long) sqlCoalesceScalarExpression.LeftExpression.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlCoalesceScalarExpression.RightExpression.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(
      SqlConditionalScalarExpression sqlConditionalScalarExpression)
    {
      return SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(-421337832L, (long) sqlConditionalScalarExpression.ConditionExpression.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlConditionalScalarExpression.FirstExpression.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlConditionalScalarExpression.SecondExpression.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(
      SqlExistsScalarExpression sqlExistsScalarExpression)
    {
      return SqlObjectHasher.CombineHashes(1168675587L, (long) sqlExistsScalarExpression.SqlQuery.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlFromClause sqlFromClause) => SqlObjectHasher.CombineHashes(52588336L, (long) sqlFromClause.Expression.Accept<int>((SqlObjectVisitor<int>) this));

    public override int Visit(
      SqlFunctionCallScalarExpression sqlFunctionCallScalarExpression)
    {
      int lhs1 = 496783446;
      if (sqlFunctionCallScalarExpression.IsUdf)
        lhs1 = SqlObjectHasher.CombineHashes((long) lhs1, 1547906315L);
      int lhs2 = SqlObjectHasher.CombineHashes((long) lhs1, (long) sqlFunctionCallScalarExpression.Name.Accept<int>((SqlObjectVisitor<int>) this));
      for (int index = 0; index < sqlFunctionCallScalarExpression.Arguments.Count; ++index)
        lhs2 = SqlObjectHasher.CombineHashes((long) lhs2, (long) sqlFunctionCallScalarExpression.Arguments[index].Accept<int>((SqlObjectVisitor<int>) this));
      return lhs2;
    }

    public override int Visit(SqlGroupByClause sqlGroupByClause)
    {
      int lhs = 130396242;
      for (int index = 0; index < sqlGroupByClause.Expressions.Count; ++index)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlGroupByClause.Expressions[index].Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(SqlIdentifier sqlIdentifier) => SqlObjectHasher.CombineHashes(-1664307981L, (long) sqlIdentifier.Value.GetHashCode());

    public override int Visit(
      SqlIdentifierPathExpression sqlIdentifierPathExpression)
    {
      int lhs = -1445813508;
      if (sqlIdentifierPathExpression.ParentPath != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlIdentifierPathExpression.ParentPath.Accept<int>((SqlObjectVisitor<int>) this));
      return SqlObjectHasher.CombineHashes((long) lhs, (long) sqlIdentifierPathExpression.Value.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlInputPathCollection sqlInputPathCollection)
    {
      int lhs = SqlObjectHasher.CombineHashes(-209963066L, (long) sqlInputPathCollection.Input.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlInputPathCollection.RelativePath != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlInputPathCollection.RelativePath.Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(SqlInScalarExpression sqlInScalarExpression)
    {
      int lhs = SqlObjectHasher.CombineHashes(1439386783L, (long) sqlInScalarExpression.Expression.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlInScalarExpression.Not)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, -1131398119L);
      for (int index = 0; index < sqlInScalarExpression.Items.Count; ++index)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlInScalarExpression.Items[index].Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(SqlLimitSpec sqlObject) => SqlObjectHasher.CombineHashes(92601316L, sqlObject.Limit);

    public override int Visit(
      SqlJoinCollectionExpression sqlJoinCollectionExpression)
    {
      return SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(1000382226L, (long) sqlJoinCollectionExpression.LeftExpression.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlJoinCollectionExpression.RightExpression.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(
      SqlLiteralArrayCollection sqlLiteralArrayCollection)
    {
      int lhs = 1634639566;
      for (int index = 0; index < sqlLiteralArrayCollection.Items.Count; ++index)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlLiteralArrayCollection.Items[index].Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(
      SqlLiteralScalarExpression sqlLiteralScalarExpression)
    {
      return SqlObjectHasher.CombineHashes(-158339101L, (long) sqlLiteralScalarExpression.Literal.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(
      SqlMemberIndexerScalarExpression sqlMemberIndexerScalarExpression)
    {
      return SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(1589675618L, (long) sqlMemberIndexerScalarExpression.MemberExpression.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlMemberIndexerScalarExpression.IndexExpression.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlNullLiteral sqlNullLiteral) => -709456592;

    public override int Visit(SqlNumberLiteral sqlNumberLiteral) => SqlObjectHasher.CombineHashes(159836309L, (long) sqlNumberLiteral.Value.GetHashCode());

    public override int Visit(SqlNumberPathExpression sqlNumberPathExpression)
    {
      int lhs = 874210976;
      if (sqlNumberPathExpression.ParentPath != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlNumberPathExpression.ParentPath.Accept<int>((SqlObjectVisitor<int>) this));
      return SqlObjectHasher.CombineHashes((long) lhs, (long) sqlNumberPathExpression.Value.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(
      SqlObjectCreateScalarExpression sqlObjectCreateScalarExpression)
    {
      int lhs = -131129165;
      foreach (SqlObjectProperty property in sqlObjectCreateScalarExpression.Properties)
      {
        if (this.isStrict)
          lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) property.Accept<int>((SqlObjectVisitor<int>) this));
        else
          lhs += property.Accept<int>((SqlObjectVisitor<int>) this);
      }
      return lhs;
    }

    public override int Visit(SqlObjectProperty sqlObjectProperty) => SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(1218972715L, (long) sqlObjectProperty.Name.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlObjectProperty.Expression.Accept<int>((SqlObjectVisitor<int>) this));

    public override int Visit(SqlOffsetLimitClause sqlObject) => SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(150154755L, (long) sqlObject.OffsetSpec.Accept<int>((SqlObjectVisitor<int>) this)), (long) sqlObject.LimitSpec.Accept<int>((SqlObjectVisitor<int>) this));

    public override int Visit(SqlOffsetSpec sqlObject) => SqlObjectHasher.CombineHashes(109062001L, sqlObject.Offset);

    public override int Visit(SqlOrderbyClause sqlOrderByClause)
    {
      int lhs = 1361708336;
      for (int index = 0; index < sqlOrderByClause.OrderbyItems.Count; ++index)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlOrderByClause.OrderbyItems[index].Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(SqlOrderByItem sqlOrderByItem)
    {
      int lhs = SqlObjectHasher.CombineHashes(846566057L, (long) sqlOrderByItem.Expression.Accept<int>((SqlObjectVisitor<int>) this));
      return !sqlOrderByItem.IsDescending ? SqlObjectHasher.CombineHashes((long) lhs, -1123129997L) : SqlObjectHasher.CombineHashes((long) lhs, -703648622L);
    }

    public override int Visit(SqlProgram sqlProgram) => SqlObjectHasher.CombineHashes(-492711050L, (long) sqlProgram.Query.Accept<int>((SqlObjectVisitor<int>) this));

    public override int Visit(SqlPropertyName sqlPropertyName) => SqlObjectHasher.CombineHashes(1262661966L, (long) sqlPropertyName.Value.GetHashCode());

    public override int Visit(
      SqlPropertyRefScalarExpression sqlPropertyRefScalarExpression)
    {
      int lhs = -1586896865;
      if (sqlPropertyRefScalarExpression.MemberExpression != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlPropertyRefScalarExpression.MemberExpression.Accept<int>((SqlObjectVisitor<int>) this));
      return SqlObjectHasher.CombineHashes((long) lhs, (long) sqlPropertyRefScalarExpression.PropertyIdentifier.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlQuery sqlQuery)
    {
      int lhs = SqlObjectHasher.CombineHashes(1968642960L, (long) sqlQuery.SelectClause.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlQuery.FromClause != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlQuery.FromClause.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlQuery.WhereClause != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlQuery.WhereClause.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlQuery.GroupByClause != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlQuery.GroupByClause.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlQuery.OrderbyClause != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlQuery.OrderbyClause.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlQuery.OffsetLimitClause != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlQuery.OffsetLimitClause.Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(SqlSelectClause sqlSelectClause)
    {
      int lhs = 19731870;
      if (sqlSelectClause.HasDistinct)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, 1467616881L);
      if (sqlSelectClause.TopSpec != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlSelectClause.TopSpec.Accept<int>((SqlObjectVisitor<int>) this));
      return SqlObjectHasher.CombineHashes((long) lhs, (long) sqlSelectClause.SelectSpec.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlSelectItem sqlSelectItem)
    {
      int lhs = SqlObjectHasher.CombineHashes(-611151157L, (long) sqlSelectItem.Expression.Accept<int>((SqlObjectVisitor<int>) this));
      if (sqlSelectItem.Alias != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlSelectItem.Alias.Accept<int>((SqlObjectVisitor<int>) this));
      return lhs;
    }

    public override int Visit(SqlSelectListSpec sqlSelectListSpec)
    {
      int lhs = -1704039197;
      foreach (SqlSelectItem sqlSelectItem in (IEnumerable<SqlSelectItem>) sqlSelectListSpec.Items)
      {
        if (this.isStrict)
          lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlSelectItem.Accept<int>((SqlObjectVisitor<int>) this));
        else
          lhs += sqlSelectItem.Accept<int>((SqlObjectVisitor<int>) this);
      }
      return lhs;
    }

    public override int Visit(SqlSelectStarSpec sqlSelectStarSpec) => -1125875092;

    public override int Visit(SqlSelectValueSpec sqlSelectValueSpec) => SqlObjectHasher.CombineHashes(507077368L, (long) sqlSelectValueSpec.Expression.Accept<int>((SqlObjectVisitor<int>) this));

    public override int Visit(SqlStringLiteral sqlStringLiteral) => SqlObjectHasher.CombineHashes(-1542874155L, (long) sqlStringLiteral.Value.GetHashCode());

    public override int Visit(SqlStringPathExpression sqlStringPathExpression)
    {
      int lhs = -1280625326;
      if (sqlStringPathExpression.ParentPath != null)
        lhs = SqlObjectHasher.CombineHashes((long) lhs, (long) sqlStringPathExpression.ParentPath.Accept<int>((SqlObjectVisitor<int>) this));
      return SqlObjectHasher.CombineHashes((long) lhs, (long) sqlStringPathExpression.Value.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlSubqueryCollection sqlSubqueryCollection) => SqlObjectHasher.CombineHashes(1175697100L, (long) sqlSubqueryCollection.Query.Accept<int>((SqlObjectVisitor<int>) this));

    public override int Visit(
      SqlSubqueryScalarExpression sqlSubqueryScalarExpression)
    {
      return SqlObjectHasher.CombineHashes(-1327458193L, (long) sqlSubqueryScalarExpression.Query.Accept<int>((SqlObjectVisitor<int>) this));
    }

    public override int Visit(SqlTopSpec sqlTopSpec) => SqlObjectHasher.CombineHashes(-791376698L, (long) sqlTopSpec.Count.GetHashCode());

    public override int Visit(SqlUnaryScalarExpression sqlUnaryScalarExpression) => SqlObjectHasher.CombineHashes((long) SqlObjectHasher.CombineHashes(723832597L, (long) SqlObjectHasher.SqlUnaryScalarOperatorKindGetHashCode(sqlUnaryScalarExpression.OperatorKind)), (long) sqlUnaryScalarExpression.Expression.Accept<int>((SqlObjectVisitor<int>) this));

    public override int Visit(SqlUndefinedLiteral sqlUndefinedLiteral) => 1290712518;

    public override int Visit(SqlWhereClause sqlWhereClause) => SqlObjectHasher.CombineHashes(-516465563L, (long) sqlWhereClause.FilterExpression.Accept<int>((SqlObjectVisitor<int>) this));

    private static int SqlUnaryScalarOperatorKindGetHashCode(SqlUnaryScalarOperatorKind kind)
    {
      switch (kind)
      {
        case SqlUnaryScalarOperatorKind.BitwiseNot:
          return 1177827907;
        case SqlUnaryScalarOperatorKind.Not:
          return 1278008063;
        case SqlUnaryScalarOperatorKind.Minus:
          return -1942284846;
        case SqlUnaryScalarOperatorKind.Plus:
          return 251767493;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unsupported operator {0}", (object) kind));
      }
    }

    private static int SqlBinaryScalarOperatorKindGetHashCode(SqlBinaryScalarOperatorKind kind)
    {
      switch (kind)
      {
        case SqlBinaryScalarOperatorKind.Add:
          return 977447154;
        case SqlBinaryScalarOperatorKind.And:
          return -539169937;
        case SqlBinaryScalarOperatorKind.BitwiseAnd:
          return 192594476;
        case SqlBinaryScalarOperatorKind.BitwiseOr:
          return -1494193777;
        case SqlBinaryScalarOperatorKind.BitwiseXor:
          return 140893802;
        case SqlBinaryScalarOperatorKind.Coalesce:
          return -461857726;
        case SqlBinaryScalarOperatorKind.Divide:
          return -1486745780;
        case SqlBinaryScalarOperatorKind.Equal:
          return -69389992;
        case SqlBinaryScalarOperatorKind.GreaterThan:
          return 1643533106;
        case SqlBinaryScalarOperatorKind.GreaterThanOrEqual:
          return 180538014;
        case SqlBinaryScalarOperatorKind.LessThan:
          return -1452081072;
        case SqlBinaryScalarOperatorKind.LessThanOrEqual:
          return -1068434012;
        case SqlBinaryScalarOperatorKind.Modulo:
          return -371220256;
        case SqlBinaryScalarOperatorKind.Multiply:
          return -178990484;
        case SqlBinaryScalarOperatorKind.NotEqual:
          return 65181046;
        case SqlBinaryScalarOperatorKind.Or:
          return -2095255335;
        case SqlBinaryScalarOperatorKind.StringConcat:
          return -525384764;
        case SqlBinaryScalarOperatorKind.Subtract:
          return 2070749634;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Unsupported operator {0}", (object) kind));
      }
    }

    private static int CombineHashes(long lhs, long rhs)
    {
      lhs ^= rhs + 2654435769L + (lhs << 6) + (lhs >> 2);
      return (int) lhs;
    }

    public override int Visit(
      SqlConversionScalarExpression sqlConversionScalarExpression)
    {
      throw new NotImplementedException("This DOM element is being removed.");
    }

    public override int Visit(
      SqlGeoNearCallScalarExpression sqlGeoNearCallScalarExpression)
    {
      throw new NotImplementedException("This DOM element is being removed.");
    }

    public override int Visit(SqlObjectLiteral sqlObjectLiteral) => throw new NotImplementedException("This DOM element is being removed.");

    private static class SqlBinaryScalarOperatorKindHashCodes
    {
      public const int Add = 977447154;
      public const int And = -539169937;
      public const int BitwiseAnd = 192594476;
      public const int BitwiseOr = -1494193777;
      public const int BitwiseXor = 140893802;
      public const int Coalesce = -461857726;
      public const int Divide = -1486745780;
      public const int Equal = -69389992;
      public const int GreaterThan = 1643533106;
      public const int GreaterThanOrEqual = 180538014;
      public const int LessThan = -1452081072;
      public const int LessThanOrEqual = -1068434012;
      public const int Modulo = -371220256;
      public const int Multiply = -178990484;
      public const int NotEqual = 65181046;
      public const int Or = -2095255335;
      public const int StringConcat = -525384764;
      public const int Subtract = 2070749634;
    }

    private static class SqlUnaryScalarOperatorKindHashCodes
    {
      public const int BitwiseNot = 1177827907;
      public const int Not = 1278008063;
      public const int Minus = -1942284846;
      public const int Plus = 251767493;
    }
  }
}
