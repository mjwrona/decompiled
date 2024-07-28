// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.WorkItemUpdateWrapperOrderedResultCollection
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal class WorkItemUpdateWrapperOrderedResultCollection : 
    IEnumerable<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>>,
    IEnumerable
  {
    private readonly Lazy<HashSet<Tuple<int, int, int>>> _uniqueWorkItemLinkUpdateResultKeys = new Lazy<HashSet<Tuple<int, int, int>>>();
    private readonly Lazy<Dictionary<Tuple<int, int, int>, WorkItemLinkUpdateResult>> _uniqueLinkUpdateResults = new Lazy<Dictionary<Tuple<int, int, int>, WorkItemLinkUpdateResult>>();
    private readonly List<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>> _collectedResults = new List<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>>();

    public WorkItemUpdateWrapperOrderedResultCollection(
      IVssRequestContext requestContext,
      IEnumerable<WorkItemUpdateResult> results,
      WorkItemUpdateDeserializeResult deserializedPackage)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (results == null)
        throw new ArgumentNullException(nameof (results));
      if (deserializedPackage == null)
        throw new ArgumentNullException(nameof (deserializedPackage));
      WorkItemTrackingLinkService service = requestContext.GetService<WorkItemTrackingLinkService>();
      if (service == null)
        throw new ArgumentException("The link type dictionary can not be found");
      foreach (WorkItemUpdateResult result in results)
      {
        WorkItemUpdateWrapper key1;
        if (!deserializedPackage.TryGetUpdate(result.UpdateId, out key1))
          throw new InvalidOperationException("Cannot locate item in results");
        if (key1.HasResourceLinkUpdates && !key1.IsWorkItem)
          throw new InvalidOperationException("The resource link updates must be part of a work item update");
        this._collectedResults.Add(new KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>(key1, result));
        if (key1.HasLinkUpdates)
        {
          foreach (WorkItemLinkUpdateResult linkUpdate in result.LinkUpdates)
          {
            Tuple<int, int, int> key2 = linkUpdate.ToKey();
            if (this._uniqueWorkItemLinkUpdateResultKeys.Value.Add(key2))
            {
              bool flag = true;
              MDWorkItemLinkType linkTypeById = service.GetLinkTypeById(requestContext, linkUpdate.LinkType);
              Tuple<int, int, int> tuple = (Tuple<int, int, int>) null;
              if (linkTypeById.ForwardId != linkTypeById.ReverseId)
              {
                if (linkUpdate.LinkType != linkTypeById.ForwardId)
                  tuple = linkUpdate.ToReverseKey(linkTypeById);
              }
              else if (linkUpdate.SourceWorkItemId < linkUpdate.TargetWorkItemId)
                tuple = linkUpdate.ToReverseKey(linkTypeById);
              if (tuple != null)
                flag = this._uniqueWorkItemLinkUpdateResultKeys.Value.Add(tuple);
              if (flag)
                this._uniqueLinkUpdateResults.Value.Add(key2, linkUpdate);
            }
          }
        }
      }
    }

    public bool ContainsLink(Tuple<int, int, int> key) => this._uniqueLinkUpdateResults.Value.ContainsKey(key);

    public IEnumerator<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>> GetEnumerator() => this._collectedResults.OrderBy<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>, string>((Func<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>, string>) (x => x.Key.CorrelationId)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this._collectedResults.OrderBy<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>, string>((Func<KeyValuePair<WorkItemUpdateWrapper, WorkItemUpdateResult>, string>) (x => x.Key.CorrelationId)).GetEnumerator();
  }
}
