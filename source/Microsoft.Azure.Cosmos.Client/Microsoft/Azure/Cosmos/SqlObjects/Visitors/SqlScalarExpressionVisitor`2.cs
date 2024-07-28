// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlScalarExpressionVisitor`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal abstract class SqlScalarExpressionVisitor<TArg, TOutput>
  {
    public abstract TOutput Visit(SqlArrayCreateScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlArrayScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlBetweenScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlBinaryScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlCoalesceScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlConditionalScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlExistsScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlFunctionCallScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlInScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlLikeScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlLiteralScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlMemberIndexerScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlObjectCreateScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlParameterRefScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlPropertyRefScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlSubqueryScalarExpression scalarExpression, TArg input);

    public abstract TOutput Visit(SqlUnaryScalarExpression scalarExpression, TArg input);
  }
}
