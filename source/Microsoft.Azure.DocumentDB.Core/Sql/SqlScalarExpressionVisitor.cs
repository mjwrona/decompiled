// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlScalarExpressionVisitor
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Sql
{
  internal abstract class SqlScalarExpressionVisitor
  {
    public abstract void Visit(SqlArrayCreateScalarExpression scalarExpression);

    public abstract void Visit(SqlArrayScalarExpression scalarExpression);

    public abstract void Visit(SqlBetweenScalarExpression scalarExpression);

    public abstract void Visit(SqlBinaryScalarExpression scalarExpression);

    public abstract void Visit(SqlCoalesceScalarExpression scalarExpression);

    public abstract void Visit(SqlConditionalScalarExpression scalarExpression);

    public abstract void Visit(SqlConversionScalarExpression scalarExpression);

    public abstract void Visit(SqlExistsScalarExpression scalarExpression);

    public abstract void Visit(SqlFunctionCallScalarExpression scalarExpression);

    public abstract void Visit(SqlGeoNearCallScalarExpression scalarExpression);

    public abstract void Visit(SqlInScalarExpression scalarExpression);

    public abstract void Visit(SqlLiteralScalarExpression scalarExpression);

    public abstract void Visit(SqlMemberIndexerScalarExpression scalarExpression);

    public abstract void Visit(SqlObjectCreateScalarExpression scalarExpression);

    public abstract void Visit(SqlPropertyRefScalarExpression scalarExpression);

    public abstract void Visit(SqlSubqueryScalarExpression scalarExpression);

    public abstract void Visit(SqlUnaryScalarExpression scalarExpression);
  }
}
