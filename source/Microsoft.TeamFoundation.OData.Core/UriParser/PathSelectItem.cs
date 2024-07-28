// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.PathSelectItem
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;

namespace Microsoft.OData.UriParser
{
  public sealed class PathSelectItem : SelectItem
  {
    public PathSelectItem(ODataSelectPath selectedPath)
      : this(selectedPath, (IEdmNavigationSource) null, (SelectExpandClause) null, (FilterClause) null, (OrderByClause) null, new long?(), new long?(), new bool?(), (SearchClause) null, (ComputeClause) null)
    {
    }

    public PathSelectItem(
      ODataSelectPath selectedPath,
      IEdmNavigationSource navigationSource,
      SelectExpandClause selectAndExpand,
      FilterClause filterOption,
      OrderByClause orderByOption,
      long? topOption,
      long? skipOption,
      bool? countOption,
      SearchClause searchOption,
      ComputeClause computeOption)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataSelectPath>(selectedPath, nameof (selectedPath));
      this.SelectedPath = selectedPath;
      this.NavigationSource = navigationSource;
      this.SelectAndExpand = selectAndExpand;
      this.FilterOption = filterOption;
      this.OrderByOption = orderByOption;
      this.TopOption = topOption;
      this.SkipOption = skipOption;
      this.CountOption = countOption;
      this.SearchOption = searchOption;
      this.ComputeOption = computeOption;
    }

    public ODataSelectPath SelectedPath { get; private set; }

    public IEdmNavigationSource NavigationSource { get; internal set; }

    public SelectExpandClause SelectAndExpand { get; internal set; }

    public FilterClause FilterOption { get; internal set; }

    public OrderByClause OrderByOption { get; internal set; }

    public long? TopOption { get; internal set; }

    public long? SkipOption { get; internal set; }

    public bool? CountOption { get; internal set; }

    public SearchClause SearchOption { get; internal set; }

    public ComputeClause ComputeOption { get; internal set; }

    public bool HasOptions
    {
      get
      {
        if (this.FilterOption != null || this.ComputeOption != null || this.SearchOption != null || this.TopOption.HasValue || this.SkipOption.HasValue || this.CountOption.HasValue || this.OrderByOption != null)
          return true;
        return this.SelectAndExpand != null && !this.SelectAndExpand.AllSelected;
      }
    }

    public override T TranslateWith<T>(SelectItemTranslator<T> translator) => translator.Translate(this);

    public override void HandleWith(SelectItemHandler handler) => handler.Handle(this);
  }
}
