// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlObjectVisitor`2
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Sql
{
  internal abstract class SqlObjectVisitor<TInput, TOutput>
  {
    public abstract TOutput Visit(SqlAliasedCollectionExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlArrayCreateScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlArrayIteratorCollectionExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlArrayScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlBetweenScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlBinaryScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlBooleanLiteral sqlObject, TInput input);

    public abstract TOutput Visit(SqlCoalesceScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlConditionalScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlConversionScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlExistsScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlFromClause sqlObject, TInput input);

    public abstract TOutput Visit(SqlFunctionCallScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlGeoNearCallScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlGroupByClause sqlObject, TInput input);

    public abstract TOutput Visit(SqlIdentifier sqlObject, TInput input);

    public abstract TOutput Visit(SqlIdentifierPathExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlInputPathCollection sqlObject, TInput input);

    public abstract TOutput Visit(SqlInScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlJoinCollectionExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlLimitSpec sqlObject, TInput input);

    public abstract TOutput Visit(SqlLiteralArrayCollection sqlObject, TInput input);

    public abstract TOutput Visit(SqlLiteralScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlMemberIndexerScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlNullLiteral sqlObject, TInput input);

    public abstract TOutput Visit(SqlNumberLiteral sqlObject, TInput input);

    public abstract TOutput Visit(SqlNumberPathExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlObjectCreateScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlObjectLiteral sqlObject, TInput input);

    public abstract TOutput Visit(SqlObjectProperty sqlObject, TInput input);

    public abstract TOutput Visit(SqlOffsetLimitClause sqlObject, TInput input);

    public abstract TOutput Visit(SqlOffsetSpec sqlObject, TInput input);

    public abstract TOutput Visit(SqlOrderbyClause sqlObject, TInput input);

    public abstract TOutput Visit(SqlOrderByItem sqlObject, TInput input);

    public abstract TOutput Visit(SqlProgram sqlObject, TInput input);

    public abstract TOutput Visit(SqlPropertyName sqlObject, TInput input);

    public abstract TOutput Visit(SqlPropertyRefScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlQuery sqlObject, TInput input);

    public abstract TOutput Visit(SqlSelectClause sqlObject, TInput input);

    public abstract TOutput Visit(SqlSelectItem sqlObject, TInput input);

    public abstract TOutput Visit(SqlSelectListSpec sqlObject, TInput input);

    public abstract TOutput Visit(SqlSelectStarSpec sqlObject, TInput input);

    public abstract TOutput Visit(SqlSelectValueSpec sqlObject, TInput input);

    public abstract TOutput Visit(SqlStringLiteral sqlObject, TInput input);

    public abstract TOutput Visit(SqlStringPathExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlSubqueryCollection sqlObject, TInput input);

    public abstract TOutput Visit(SqlSubqueryScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlTopSpec sqlObject, TInput input);

    public abstract TOutput Visit(SqlUnaryScalarExpression sqlObject, TInput input);

    public abstract TOutput Visit(SqlUndefinedLiteral sqlObject, TInput input);

    public abstract TOutput Visit(SqlWhereClause sqlObject, TInput input);
  }
}
