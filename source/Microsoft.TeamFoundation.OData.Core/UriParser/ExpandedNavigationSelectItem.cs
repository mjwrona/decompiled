// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpandedNavigationSelectItem
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser.Aggregation;

namespace Microsoft.OData.UriParser
{
  public sealed class ExpandedNavigationSelectItem : ExpandedReferenceSelectItem
  {
    public ExpandedNavigationSelectItem(
      ODataExpandPath pathToNavigationProperty,
      IEdmNavigationSource navigationSource,
      SelectExpandClause selectExpandOption)
      : this(pathToNavigationProperty, navigationSource, selectExpandOption, (FilterClause) null, (OrderByClause) null, new long?(), new long?(), new bool?(), (SearchClause) null, (LevelsClause) null)
    {
    }

    public ExpandedNavigationSelectItem(
      ODataExpandPath pathToNavigationProperty,
      IEdmNavigationSource navigationSource,
      SelectExpandClause selectAndExpand,
      FilterClause filterOption,
      OrderByClause orderByOption,
      long? topOption,
      long? skipOption,
      bool? countOption,
      SearchClause searchOption,
      LevelsClause levelsOption)
      : base(pathToNavigationProperty, navigationSource, filterOption, orderByOption, topOption, skipOption, countOption, searchOption)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataExpandPath>(pathToNavigationProperty, nameof (pathToNavigationProperty));
      this.SelectAndExpand = selectAndExpand;
      this.LevelsOption = levelsOption;
    }

    public ExpandedNavigationSelectItem(
      ODataExpandPath pathToNavigationProperty,
      IEdmNavigationSource navigationSource,
      SelectExpandClause selectAndExpand,
      FilterClause filterOption,
      OrderByClause orderByOption,
      long? topOption,
      long? skipOption,
      bool? countOption,
      SearchClause searchOption,
      LevelsClause levelsOption,
      ComputeClause computeOption)
      : base(pathToNavigationProperty, navigationSource, filterOption, orderByOption, topOption, skipOption, countOption, searchOption, computeOption)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataExpandPath>(pathToNavigationProperty, nameof (pathToNavigationProperty));
      this.SelectAndExpand = selectAndExpand;
      this.LevelsOption = levelsOption;
    }

    public ExpandedNavigationSelectItem(
      ODataExpandPath pathToNavigationProperty,
      IEdmNavigationSource navigationSource,
      SelectExpandClause selectAndExpand,
      FilterClause filterOption,
      OrderByClause orderByOption,
      long? topOption,
      long? skipOption,
      bool? countOption,
      SearchClause searchOption,
      LevelsClause levelsOption,
      ComputeClause computeOption,
      ApplyClause applyOption)
      : base(pathToNavigationProperty, navigationSource, filterOption, orderByOption, topOption, skipOption, countOption, searchOption, computeOption, applyOption)
    {
      ExceptionUtils.CheckArgumentNotNull<ODataExpandPath>(pathToNavigationProperty, nameof (pathToNavigationProperty));
      this.SelectAndExpand = selectAndExpand;
      this.LevelsOption = levelsOption;
    }

    public SelectExpandClause SelectAndExpand { get; private set; }

    public LevelsClause LevelsOption { get; private set; }

    public override T TranslateWith<T>(SelectItemTranslator<T> translator) => translator.Translate(this);

    public override void HandleWith(SelectItemHandler handler) => handler.Handle(this);
  }
}
