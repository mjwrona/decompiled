// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpandedReferenceSelectItem
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Aggregation;

namespace Microsoft.OData.UriParser
{
  public class ExpandedReferenceSelectItem : SelectItem
  {
    public ExpandedReferenceSelectItem(
      ODataExpandPath pathToNavigationProperty,
      IEdmNavigationSource navigationSource)
      : this(pathToNavigationProperty, navigationSource, (FilterClause) null, (OrderByClause) null, new long?(), new long?(), new bool?(), (SearchClause) null)
    {
    }

    public ExpandedReferenceSelectItem(
      ODataExpandPath pathToNavigationProperty,
      IEdmNavigationSource navigationSource,
      FilterClause filterOption,
      OrderByClause orderByOption,
      long? topOption,
      long? skipOption,
      bool? countOption,
      SearchClause searchOption)
      : this(pathToNavigationProperty, navigationSource, filterOption, orderByOption, topOption, skipOption, countOption, searchOption, (ComputeClause) null)
    {
    }

    public ExpandedReferenceSelectItem(
      ODataExpandPath pathToNavigationProperty,
      IEdmNavigationSource navigationSource,
      FilterClause filterOption,
      OrderByClause orderByOption,
      long? topOption,
      long? skipOption,
      bool? countOption,
      SearchClause searchOption,
      ComputeClause computeOption)
      : this(pathToNavigationProperty, navigationSource, filterOption, orderByOption, topOption, skipOption, countOption, searchOption, computeOption, (ApplyClause) null)
    {
    }

    public ExpandedReferenceSelectItem(
      ODataExpandPath pathToNavigationProperty,
      IEdmNavigationSource navigationSource,
      FilterClause filterOption,
      OrderByClause orderByOption,
      long? topOption,
      long? skipOption,
      bool? countOption,
      SearchClause searchOption,
      ComputeClause computeOption,
      ApplyClause applyOption)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataExpandPath>(pathToNavigationProperty, nameof (pathToNavigationProperty));
      this.PathToNavigationProperty = pathToNavigationProperty;
      this.NavigationSource = navigationSource;
      this.FilterOption = filterOption;
      this.OrderByOption = orderByOption;
      this.TopOption = topOption;
      this.SkipOption = skipOption;
      this.CountOption = countOption;
      this.SearchOption = searchOption;
      this.ComputeOption = computeOption;
      this.ApplyOption = applyOption;
    }

    public ODataExpandPath PathToNavigationProperty { get; private set; }

    public IEdmNavigationSource NavigationSource { get; private set; }

    public FilterClause FilterOption { get; private set; }

    public SearchClause SearchOption { get; private set; }

    public OrderByClause OrderByOption { get; private set; }

    public ComputeClause ComputeOption { get; private set; }

    public ApplyClause ApplyOption { get; private set; }

    public long? TopOption { get; private set; }

    public long? SkipOption { get; private set; }

    public bool? CountOption { get; private set; }

    public override T TranslateWith<T>(SelectItemTranslator<T> translator) => translator.Translate(this);

    public override void HandleWith(SelectItemHandler handler) => handler.Handle(this);
  }
}
