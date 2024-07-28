// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ExpandDepthAndCountValidator
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Linq;

namespace Microsoft.OData.UriParser
{
  internal sealed class ExpandDepthAndCountValidator
  {
    private readonly int maxDepth;
    private readonly int maxCount;
    private int currentCount;

    internal ExpandDepthAndCountValidator(int maxDepth, int maxCount)
    {
      this.maxDepth = maxDepth;
      this.maxCount = maxCount;
    }

    internal void Validate(SelectExpandClause expandTree)
    {
      this.currentCount = 0;
      this.EnsureMaximumCountAndDepthAreNotExceeded(expandTree, 0);
    }

    private void EnsureMaximumCountAndDepthAreNotExceeded(
      SelectExpandClause expandTree,
      int currentDepth)
    {
      if (currentDepth > this.maxDepth)
        throw ExceptionUtil.CreateBadRequestError(Strings.UriParser_ExpandDepthExceeded((object) currentDepth, (object) this.maxDepth));
      foreach (ExpandedNavigationSelectItem navigationSelectItem in expandTree.SelectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedNavigationSelectItem))))
      {
        ++this.currentCount;
        if (this.currentCount > this.maxCount)
          throw ExceptionUtil.CreateBadRequestError(Strings.UriParser_ExpandCountExceeded((object) this.currentCount, (object) this.maxCount));
        this.EnsureMaximumCountAndDepthAreNotExceeded(navigationSelectItem.SelectAndExpand, currentDepth + 1);
      }
      this.currentCount += expandTree.SelectedItems.Where<SelectItem>((Func<SelectItem, bool>) (I => I.GetType() == typeof (ExpandedReferenceSelectItem))).Count<SelectItem>();
      if (this.currentCount > this.maxCount)
        throw ExceptionUtil.CreateBadRequestError(Strings.UriParser_ExpandCountExceeded((object) this.currentCount, (object) this.maxCount));
    }
  }
}
