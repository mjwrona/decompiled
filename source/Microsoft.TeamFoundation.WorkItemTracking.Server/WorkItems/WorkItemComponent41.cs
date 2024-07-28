// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent41
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent41 : WorkItemComponent40
  {
    internal override void BindparametersForGetWorkItemsProc(
      IEnumerable<int> workItemIds,
      bool includeCountFields,
      bool includeCustomFields,
      bool includeTextFields,
      bool includeResourceLinks,
      bool includeWorkItemLinks,
      bool includeHistory,
      bool sortLinks,
      int maxLongTextLength,
      int maxRevisionLongTextLength,
      DateTime? asOf,
      DateTime? revisionsSince,
      bool includeComments,
      bool includeCommentHistory)
    {
      if (revisionsSince.HasValue)
        this.BindNullableDateTime("@revisionsSince", revisionsSince);
      base.BindparametersForGetWorkItemsProc(workItemIds, includeCountFields, includeCustomFields, includeTextFields, includeResourceLinks, includeWorkItemLinks, includeHistory, sortLinks, maxLongTextLength, maxRevisionLongTextLength, asOf, revisionsSince, includeComments, includeCommentHistory);
    }
  }
}
