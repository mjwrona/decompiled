// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectExpandTermToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;

namespace Microsoft.OData.UriParser
{
  public abstract class SelectExpandTermToken : QueryToken
  {
    protected SelectExpandTermToken(
      PathSegmentToken pathToProperty,
      QueryToken filterOption,
      IEnumerable<OrderByToken> orderByOptions,
      long? topOption,
      long? skipOption,
      bool? countQueryOption,
      QueryToken searchOption,
      SelectToken selectOption,
      ComputeToken computeOption)
    {
      ExceptionUtils.CheckArgumentNotNull<PathSegmentToken>(pathToProperty, "property");
      this.PathToProperty = pathToProperty;
      this.FilterOption = filterOption;
      this.OrderByOptions = orderByOptions;
      this.TopOption = topOption;
      this.SkipOption = skipOption;
      this.CountQueryOption = countQueryOption;
      this.SearchOption = searchOption;
      this.SelectOption = selectOption;
      this.ComputeOption = computeOption;
    }

    public PathSegmentToken PathToProperty { get; internal set; }

    public QueryToken FilterOption { get; private set; }

    public IEnumerable<OrderByToken> OrderByOptions { get; private set; }

    public QueryToken SearchOption { get; private set; }

    public long? TopOption { get; private set; }

    public long? SkipOption { get; private set; }

    public bool? CountQueryOption { get; private set; }

    public SelectToken SelectOption { get; internal set; }

    public ComputeToken ComputeOption { get; private set; }
  }
}
