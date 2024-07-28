// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.Visitors.SqlLiteralVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.SqlObjects.Visitors
{
  internal abstract class SqlLiteralVisitor
  {
    public abstract void Visit(SqlBooleanLiteral literal);

    public abstract void Visit(SqlNullLiteral literal);

    public abstract void Visit(SqlNumberLiteral literal);

    public abstract void Visit(SqlStringLiteral literal);

    public abstract void Visit(SqlUndefinedLiteral literal);
  }
}
