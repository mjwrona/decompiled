// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlScalarExpressionVisitor`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal abstract class SqlScalarExpressionVisitor<TResult>
  {
    public abstract TResult Visit(SqlArrayCreateScalarExpression scalarExpression);

    public abstract TResult Visit(SqlArrayScalarExpression scalarExpression);

    public abstract TResult Visit(SqlBetweenScalarExpression scalarExpression);

    public abstract TResult Visit(SqlBinaryScalarExpression scalarExpression);

    public abstract TResult Visit(SqlCoalesceScalarExpression scalarExpression);

    public abstract TResult Visit(SqlConditionalScalarExpression scalarExpression);

    public abstract TResult Visit(SqlExistsScalarExpression scalarExpression);

    public abstract TResult Visit(SqlFunctionCallScalarExpression scalarExpression);

    public abstract TResult Visit(SqlInScalarExpression scalarExpression);

    public abstract TResult Visit(SqlLikeScalarExpression scalarExpression);

    public abstract TResult Visit(SqlLiteralScalarExpression scalarExpression);

    public abstract TResult Visit(SqlMemberIndexerScalarExpression scalarExpression);

    public abstract TResult Visit(SqlObjectCreateScalarExpression scalarExpression);

    public abstract TResult Visit(SqlParameterRefScalarExpression scalarExpression);

    public abstract TResult Visit(SqlPropertyRefScalarExpression scalarExpression);

    public abstract TResult Visit(SqlSubqueryScalarExpression scalarExpression);

    public abstract TResult Visit(SqlUnaryScalarExpression scalarExpression);
  }
}
