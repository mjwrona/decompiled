// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectTermToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public sealed class SelectTermToken : SelectExpandTermToken
  {
    public SelectTermToken(PathSegmentToken pathToProperty)
      : this(pathToProperty, (SelectToken) null)
    {
    }

    public SelectTermToken(PathSegmentToken pathToProperty, SelectToken selectOption)
      : this(pathToProperty, (QueryToken) null, (IEnumerable<OrderByToken>) null, new long?(), new long?(), new bool?(), (QueryToken) null, selectOption, (ComputeToken) null)
    {
    }

    public SelectTermToken(
      PathSegmentToken pathToProperty,
      QueryToken filterOption,
      IEnumerable<OrderByToken> orderByOptions,
      long? topOption,
      long? skipOption,
      bool? countQueryOption,
      QueryToken searchOption,
      SelectToken selectOption,
      ComputeToken computeOption)
      : base(pathToProperty, filterOption, orderByOptions, topOption, skipOption, countQueryOption, searchOption, selectOption, computeOption)
    {
    }

    public override QueryTokenKind Kind => QueryTokenKind.SelectTerm;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
