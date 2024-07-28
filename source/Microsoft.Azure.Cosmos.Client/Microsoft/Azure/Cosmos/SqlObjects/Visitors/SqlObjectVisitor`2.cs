// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlObjectVisitor`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal abstract class SqlObjectVisitor<TArg, TOutput>
  {
    public abstract TOutput Visit(SqlAliasedCollectionExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlArrayCreateScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlArrayIteratorCollectionExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlArrayScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlBetweenScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlBinaryScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlBooleanLiteral sqlObject, TArg input);

    public abstract TOutput Visit(SqlCoalesceScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlConditionalScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlExistsScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlFromClause sqlObject, TArg input);

    public abstract TOutput Visit(SqlFunctionCallScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlGroupByClause sqlObject, TArg input);

    public abstract TOutput Visit(SqlIdentifier sqlObject, TArg input);

    public abstract TOutput Visit(SqlIdentifierPathExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlInputPathCollection sqlObject, TArg input);

    public abstract TOutput Visit(SqlInScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlJoinCollectionExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlLikeScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlLimitSpec sqlObject, TArg input);

    public abstract TOutput Visit(SqlLiteralScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlMemberIndexerScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlNullLiteral sqlObject, TArg input);

    public abstract TOutput Visit(SqlNumberLiteral sqlObject, TArg input);

    public abstract TOutput Visit(SqlNumberPathExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlObjectCreateScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlObjectProperty sqlObject, TArg input);

    public abstract TOutput Visit(SqlOffsetLimitClause sqlObject, TArg input);

    public abstract TOutput Visit(SqlOffsetSpec sqlObject, TArg input);

    public abstract TOutput Visit(SqlOrderByClause sqlObject, TArg input);

    public abstract TOutput Visit(SqlOrderByItem sqlObject, TArg input);

    public abstract TOutput Visit(SqlParameter sqlObject, TArg input);

    public abstract TOutput Visit(SqlParameterRefScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlProgram sqlObject, TArg input);

    public abstract TOutput Visit(SqlPropertyName sqlObject, TArg input);

    public abstract TOutput Visit(SqlPropertyRefScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlQuery sqlObject, TArg input);

    public abstract TOutput Visit(SqlSelectClause sqlObject, TArg input);

    public abstract TOutput Visit(SqlSelectItem sqlObject, TArg input);

    public abstract TOutput Visit(SqlSelectListSpec sqlObject, TArg input);

    public abstract TOutput Visit(SqlSelectStarSpec sqlObject, TArg input);

    public abstract TOutput Visit(SqlSelectValueSpec sqlObject, TArg input);

    public abstract TOutput Visit(SqlStringLiteral sqlObject, TArg input);

    public abstract TOutput Visit(SqlStringPathExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlSubqueryCollection sqlObject, TArg input);

    public abstract TOutput Visit(SqlSubqueryScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlTopSpec sqlObject, TArg input);

    public abstract TOutput Visit(SqlUnaryScalarExpression sqlObject, TArg input);

    public abstract TOutput Visit(SqlUndefinedLiteral sqlObject, TArg input);

    public abstract TOutput Visit(SqlWhereClause sqlObject, TArg input);
  }
}
