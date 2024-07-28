// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlObjectVisitor`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal abstract class SqlObjectVisitor<TResult>
  {
    public abstract TResult Visit(SqlAliasedCollectionExpression sqlObject);

    public abstract TResult Visit(SqlArrayCreateScalarExpression sqlObject);

    public abstract TResult Visit(SqlArrayIteratorCollectionExpression sqlObject);

    public abstract TResult Visit(SqlArrayScalarExpression sqlObject);

    public abstract TResult Visit(SqlBetweenScalarExpression sqlObject);

    public abstract TResult Visit(SqlBinaryScalarExpression sqlObject);

    public abstract TResult Visit(SqlBooleanLiteral sqlObject);

    public abstract TResult Visit(SqlCoalesceScalarExpression sqlObject);

    public abstract TResult Visit(SqlConditionalScalarExpression sqlObject);

    public abstract TResult Visit(SqlExistsScalarExpression sqlObject);

    public abstract TResult Visit(SqlFromClause sqlObject);

    public abstract TResult Visit(SqlFunctionCallScalarExpression sqlObject);

    public abstract TResult Visit(SqlGroupByClause sqlObject);

    public abstract TResult Visit(SqlIdentifier sqlObject);

    public abstract TResult Visit(SqlIdentifierPathExpression sqlObject);

    public abstract TResult Visit(SqlInputPathCollection sqlObject);

    public abstract TResult Visit(SqlJoinCollectionExpression sqlObject);

    public abstract TResult Visit(SqlLikeScalarExpression sqlObject);

    public abstract TResult Visit(SqlInScalarExpression sqlObject);

    public abstract TResult Visit(SqlLimitSpec sqlObject);

    public abstract TResult Visit(SqlLiteralScalarExpression sqlObject);

    public abstract TResult Visit(SqlMemberIndexerScalarExpression sqlObject);

    public abstract TResult Visit(SqlNullLiteral sqlObject);

    public abstract TResult Visit(SqlNumberLiteral sqlObject);

    public abstract TResult Visit(SqlNumberPathExpression sqlObject);

    public abstract TResult Visit(SqlObjectCreateScalarExpression sqlObject);

    public abstract TResult Visit(SqlObjectProperty sqlObject);

    public abstract TResult Visit(SqlOffsetLimitClause sqlObject);

    public abstract TResult Visit(SqlOffsetSpec sqlObject);

    public abstract TResult Visit(SqlOrderByClause sqlObject);

    public abstract TResult Visit(SqlOrderByItem sqlObject);

    public abstract TResult Visit(SqlParameter sqlObject);

    public abstract TResult Visit(SqlParameterRefScalarExpression sqlObject);

    public abstract TResult Visit(SqlProgram sqlObject);

    public abstract TResult Visit(SqlPropertyName sqlObject);

    public abstract TResult Visit(SqlPropertyRefScalarExpression sqlObject);

    public abstract TResult Visit(SqlQuery sqlObject);

    public abstract TResult Visit(SqlSelectClause sqlObject);

    public abstract TResult Visit(SqlSelectItem sqlObject);

    public abstract TResult Visit(SqlSelectListSpec sqlObject);

    public abstract TResult Visit(SqlSelectStarSpec sqlObject);

    public abstract TResult Visit(SqlSelectValueSpec sqlObject);

    public abstract TResult Visit(SqlStringLiteral sqlObject);

    public abstract TResult Visit(SqlStringPathExpression sqlObject);

    public abstract TResult Visit(SqlSubqueryCollection sqlObject);

    public abstract TResult Visit(SqlSubqueryScalarExpression sqlObject);

    public abstract TResult Visit(SqlTopSpec sqlObject);

    public abstract TResult Visit(SqlUnaryScalarExpression sqlObject);

    public abstract TResult Visit(SqlUndefinedLiteral sqlObject);

    public abstract TResult Visit(SqlWhereClause sqlObject);
  }
}
