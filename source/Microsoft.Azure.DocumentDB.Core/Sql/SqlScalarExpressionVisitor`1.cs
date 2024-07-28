// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlScalarExpressionVisitor`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Sql
{
  internal abstract class SqlScalarExpressionVisitor<TResult>
  {
    public abstract TResult Visit(SqlArrayCreateScalarExpression scalarExpression);

    public abstract TResult Visit(SqlArrayScalarExpression scalarExpression);

    public abstract TResult Visit(SqlBetweenScalarExpression scalarExpression);

    public abstract TResult Visit(SqlBinaryScalarExpression scalarExpression);

    public abstract TResult Visit(SqlCoalesceScalarExpression scalarExpression);

    public abstract TResult Visit(SqlConditionalScalarExpression scalarExpression);

    public abstract TResult Visit(SqlConversionScalarExpression scalarExpression);

    public abstract TResult Visit(SqlExistsScalarExpression scalarExpression);

    public abstract TResult Visit(SqlFunctionCallScalarExpression scalarExpression);

    public abstract TResult Visit(SqlGeoNearCallScalarExpression scalarExpression);

    public abstract TResult Visit(SqlInScalarExpression scalarExpression);

    public abstract TResult Visit(SqlLiteralScalarExpression scalarExpression);

    public abstract TResult Visit(SqlMemberIndexerScalarExpression scalarExpression);

    public abstract TResult Visit(SqlObjectCreateScalarExpression scalarExpression);

    public abstract TResult Visit(SqlPropertyRefScalarExpression scalarExpression);

    public abstract TResult Visit(SqlSubqueryScalarExpression scalarExpression);

    public abstract TResult Visit(SqlUnaryScalarExpression scalarExpression);
  }
}
