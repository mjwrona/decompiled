// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlSelectListSpec
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlSelectListSpec : SqlSelectSpec
  {
    private SqlSelectListSpec(IReadOnlyList<SqlSelectItem> items)
      : base(SqlObjectKind.SelectListSpec)
    {
      if (items == null)
        throw new ArgumentNullException("items must not be null.");
      foreach (SqlSelectItem sqlSelectItem in (IEnumerable<SqlSelectItem>) items)
      {
        if (sqlSelectItem == null)
          throw new ArgumentException("items must not contain null items.");
      }
      this.Items = (IReadOnlyList<SqlSelectItem>) new List<SqlSelectItem>((IEnumerable<SqlSelectItem>) items);
    }

    public IReadOnlyList<SqlSelectItem> Items { get; }

    public static SqlSelectListSpec Create(params SqlSelectItem[] items) => new SqlSelectListSpec((IReadOnlyList<SqlSelectItem>) items);

    public static SqlSelectListSpec Create(IReadOnlyList<SqlSelectItem> items) => new SqlSelectListSpec(items);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlSelectSpecVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlSelectSpecVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlSelectSpecVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
