// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlScalarExpressionVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal abstract class SqlScalarExpressionVisitor
  {
    public abstract void Visit(SqlArrayCreateScalarExpression scalarExpression);

    public abstract void Visit(SqlArrayScalarExpression scalarExpression);

    public abstract void Visit(SqlBetweenScalarExpression scalarExpression);

    public abstract void Visit(SqlBinaryScalarExpression scalarExpression);

    public abstract void Visit(SqlCoalesceScalarExpression scalarExpression);

    public abstract void Visit(SqlConditionalScalarExpression scalarExpression);

    public abstract void Visit(SqlExistsScalarExpression scalarExpression);

    public abstract void Visit(SqlFunctionCallScalarExpression scalarExpression);

    public abstract void Visit(SqlInScalarExpression scalarExpression);

    public abstract void Visit(SqlLikeScalarExpression scalarExpression);

    public abstract void Visit(SqlLiteralScalarExpression scalarExpression);

    public abstract void Visit(SqlMemberIndexerScalarExpression scalarExpression);

    public abstract void Visit(SqlObjectCreateScalarExpression scalarExpression);

    public abstract void Visit(SqlParameterRefScalarExpression scalarExpression);

    public abstract void Visit(SqlPropertyRefScalarExpression scalarExpression);

    public abstract void Visit(SqlSubqueryScalarExpression scalarExpression);

    public abstract void Visit(SqlUnaryScalarExpression scalarExpression);
  }
}
