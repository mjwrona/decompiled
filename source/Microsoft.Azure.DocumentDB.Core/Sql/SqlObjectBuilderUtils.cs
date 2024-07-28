// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlObjectBuilderUtils
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents.Sql
{
  internal static class SqlObjectBuilderUtils
  {
    public static SqlMemberIndexerScalarExpression CreateSqlMemberIndexerScalarExpression(
      SqlScalarExpression first,
      SqlScalarExpression second,
      params SqlScalarExpression[] everythingElse)
    {
      List<SqlScalarExpression> source = new List<SqlScalarExpression>(2 + everythingElse.Length);
      source.Add(first);
      source.Add(second);
      source.AddRange((IEnumerable<SqlScalarExpression>) everythingElse);
      SqlMemberIndexerScalarExpression memberExpression = SqlMemberIndexerScalarExpression.Create(first, second);
      foreach (SqlScalarExpression indexExpression in source.Skip<SqlScalarExpression>(2))
        memberExpression = SqlMemberIndexerScalarExpression.Create((SqlScalarExpression) memberExpression, indexExpression);
      return memberExpression;
    }
  }
}
