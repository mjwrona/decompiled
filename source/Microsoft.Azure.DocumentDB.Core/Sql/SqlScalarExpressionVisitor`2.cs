// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlScalarExpressionVisitor`2
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Sql
{
  internal abstract class SqlScalarExpressionVisitor<TInput, TOutput>
  {
    public abstract TOutput Visit(SqlArrayCreateScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlArrayScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlBetweenScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlBinaryScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlCoalesceScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlConditionalScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlConversionScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlExistsScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlFunctionCallScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlGeoNearCallScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlInScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlLiteralScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlMemberIndexerScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlObjectCreateScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlPropertyRefScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlSubqueryScalarExpression scalarExpression, TInput input);

    public abstract TOutput Visit(SqlUnaryScalarExpression scalarExpression, TInput input);
  }
}
