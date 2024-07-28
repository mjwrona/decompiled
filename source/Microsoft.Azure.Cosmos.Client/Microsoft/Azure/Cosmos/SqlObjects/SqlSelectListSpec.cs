// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlSelectListSpec
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlSelectListSpec : SqlSelectSpec
  {
    private SqlSelectListSpec(ImmutableArray<SqlSelectItem> items)
    {
      foreach (SqlObject sqlObject in items)
      {
        if (sqlObject == (SqlObject) null)
          throw new ArgumentException("items must not contain null items.");
      }
      this.Items = items;
    }

    public ImmutableArray<SqlSelectItem> Items { get; }

    public static SqlSelectListSpec Create(params SqlSelectItem[] items) => new SqlSelectListSpec(((IEnumerable<SqlSelectItem>) items).ToImmutableArray<SqlSelectItem>());

    public static SqlSelectListSpec Create(ImmutableArray<SqlSelectItem> items) => new SqlSelectListSpec(items);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlSelectSpecVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlSelectSpecVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlSelectSpecVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
