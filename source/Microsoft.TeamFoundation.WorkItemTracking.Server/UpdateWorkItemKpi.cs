// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.UpdateWorkItemKpi
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class UpdateWorkItemKpi : WorkItemTrackingKpi
  {
    public override int DefaultSamplingRate => 5;

    public UpdateWorkItemKpi(
      IVssRequestContext requestContext,
      int resultCount,
      bool skipCacheRefresh = false)
      : base(requestContext, skipCacheRefresh ? "UpdateWorkItemNoCacheRefresh" : "UpdateWorkItem", resultCount)
    {
      if (!(!this.Skip & skipCacheRefresh))
        return;
      bool cachedItem = false;
      requestContext.WitContext().TryGetCacheItem<bool>("RulesCacheRefreshed", out cachedItem);
      this.Skip = cachedItem;
    }

    public UpdateWorkItemKpi(
      IVssRequestContext requestContext,
      string kpiName,
      FieldStorageTarget storageTarget,
      IEnumerable<WorkItemUpdateState> workItemUpdates)
      : base(requestContext, kpiName, workItemUpdates.Count<WorkItemUpdateState>())
    {
      this.SetSkipFlag(requestContext, workItemUpdates, (Func<FieldEntry, bool>) (f => f.StorageTarget == storageTarget));
    }

    public UpdateWorkItemKpi(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdateState> workItemUpdates)
      : base(requestContext, "UpdateWorkItemNoIdentity", workItemUpdates.Count<WorkItemUpdateState>())
    {
      this.SetSkipFlag(requestContext, workItemUpdates, (Func<FieldEntry, bool>) (f => f.IsPerson));
    }

    private void SetSkipFlag(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdateState> workItemUpdates,
      Func<FieldEntry, bool> predicate)
    {
      if (this.Skip)
        return;
      foreach (WorkItemUpdateState workItemUpdateState in workItemUpdates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (u => u.Update.Fields != null)))
      {
        this.Skip = workItemUpdateState.Update.Fields.Any<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (f => predicate(requestContext.WitContext().FieldDictionary.GetFieldByNameOrId(f.Key))));
        if (this.Skip)
          break;
      }
    }
  }
}
