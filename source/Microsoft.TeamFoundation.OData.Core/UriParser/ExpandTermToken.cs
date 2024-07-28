// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpandTermToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public sealed class ExpandTermToken : SelectExpandTermToken
  {
    public ExpandTermToken(PathSegmentToken pathToNavigationProp)
      : this(pathToNavigationProp, (SelectToken) null, (ExpandToken) null)
    {
    }

    public ExpandTermToken(
      PathSegmentToken pathToNavigationProp,
      SelectToken selectOption,
      ExpandToken expandOption)
      : this(pathToNavigationProp, (QueryToken) null, (IEnumerable<OrderByToken>) null, new long?(), new long?(), new bool?(), new long?(), (QueryToken) null, selectOption, expandOption)
    {
    }

    public ExpandTermToken(
      PathSegmentToken pathToNavigationProp,
      QueryToken filterOption,
      IEnumerable<OrderByToken> orderByOptions,
      long? topOption,
      long? skipOption,
      bool? countQueryOption,
      long? levelsOption,
      QueryToken searchOption,
      SelectToken selectOption,
      ExpandToken expandOption)
      : this(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, levelsOption, searchOption, selectOption, expandOption, (ComputeToken) null)
    {
    }

    public ExpandTermToken(
      PathSegmentToken pathToNavigationProp,
      QueryToken filterOption,
      IEnumerable<OrderByToken> orderByOptions,
      long? topOption,
      long? skipOption,
      bool? countQueryOption,
      long? levelsOption,
      QueryToken searchOption,
      SelectToken selectOption,
      ExpandToken expandOption,
      ComputeToken computeOption)
      : this(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, levelsOption, searchOption, selectOption, expandOption, computeOption, (IEnumerable<QueryToken>) null)
    {
    }

    public ExpandTermToken(
      PathSegmentToken pathToNavigationProp,
      QueryToken filterOption,
      IEnumerable<OrderByToken> orderByOptions,
      long? topOption,
      long? skipOption,
      bool? countQueryOption,
      long? levelsOption,
      QueryToken searchOption,
      SelectToken selectOption,
      ExpandToken expandOption,
      ComputeToken computeOption,
      IEnumerable<QueryToken> applyOptions)
      : base(pathToNavigationProp, filterOption, orderByOptions, topOption, skipOption, countQueryOption, searchOption, selectOption, computeOption)
    {
      this.ExpandOption = expandOption;
      this.LevelsOption = levelsOption;
      this.ApplyOptions = applyOptions;
    }

    public PathSegmentToken PathToNavigationProp => this.PathToProperty;

    public ExpandToken ExpandOption { get; internal set; }

    public long? LevelsOption { get; private set; }

    public IEnumerable<QueryToken> ApplyOptions { get; private set; }

    public override QueryTokenKind Kind => QueryTokenKind.ExpandTerm;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
