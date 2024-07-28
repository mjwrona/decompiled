// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlObjectVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal abstract class SqlObjectVisitor
  {
    public abstract void Visit(SqlAliasedCollectionExpression sqlObject);

    public abstract void Visit(SqlArrayCreateScalarExpression sqlObject);

    public abstract void Visit(SqlArrayIteratorCollectionExpression sqlObject);

    public abstract void Visit(SqlArrayScalarExpression sqlObject);

    public abstract void Visit(SqlBetweenScalarExpression sqlObject);

    public abstract void Visit(SqlBinaryScalarExpression sqlObject);

    public abstract void Visit(SqlBooleanLiteral sqlObject);

    public abstract void Visit(SqlCoalesceScalarExpression sqlObject);

    public abstract void Visit(SqlConditionalScalarExpression sqlObject);

    public abstract void Visit(SqlExistsScalarExpression sqlObject);

    public abstract void Visit(SqlFromClause sqlObject);

    public abstract void Visit(SqlFunctionCallScalarExpression sqlObject);

    public abstract void Visit(SqlGroupByClause sqlObject);

    public abstract void Visit(SqlIdentifier sqlObject);

    public abstract void Visit(SqlIdentifierPathExpression sqlObject);

    public abstract void Visit(SqlInputPathCollection sqlObject);

    public abstract void Visit(SqlInScalarExpression sqlObject);

    public abstract void Visit(SqlJoinCollectionExpression sqlObject);

    public abstract void Visit(SqlLikeScalarExpression sqlObject);

    public abstract void Visit(SqlLimitSpec sqlObject);

    public abstract void Visit(SqlLiteralScalarExpression sqlObject);

    public abstract void Visit(SqlMemberIndexerScalarExpression sqlObject);

    public abstract void Visit(SqlNullLiteral sqlObject);

    public abstract void Visit(SqlNumberLiteral sqlObject);

    public abstract void Visit(SqlNumberPathExpression sqlObject);

    public abstract void Visit(SqlObjectCreateScalarExpression sqlObject);

    public abstract void Visit(SqlObjectProperty sqlObject);

    public abstract void Visit(SqlOffsetLimitClause sqlObject);

    public abstract void Visit(SqlOffsetSpec sqlObject);

    public abstract void Visit(SqlOrderByClause sqlObject);

    public abstract void Visit(SqlOrderByItem sqlObject);

    public abstract void Visit(SqlParameter sqlObject);

    public abstract void Visit(SqlParameterRefScalarExpression sqlObject);

    public abstract void Visit(SqlProgram sqlObject);

    public abstract void Visit(SqlPropertyName sqlObject);

    public abstract void Visit(SqlPropertyRefScalarExpression sqlObject);

    public abstract void Visit(SqlQuery sqlObject);

    public abstract void Visit(SqlSelectClause sqlObject);

    public abstract void Visit(SqlSelectItem sqlObject);

    public abstract void Visit(SqlSelectListSpec sqlObject);

    public abstract void Visit(SqlSelectStarSpec sqlObject);

    public abstract void Visit(SqlSelectValueSpec sqlObject);

    public abstract void Visit(SqlStringLiteral sqlObject);

    public abstract void Visit(SqlStringPathExpression sqlObject);

    public abstract void Visit(SqlSubqueryCollection sqlObject);

    public abstract void Visit(SqlSubqueryScalarExpression sqlObject);

    public abstract void Visit(SqlTopSpec sqlObject);

    public abstract void Visit(SqlUnaryScalarExpression sqlObject);

    public abstract void Visit(SqlUndefinedLiteral sqlObject);

    public abstract void Visit(SqlWhereClause sqlObject);
  }
}
