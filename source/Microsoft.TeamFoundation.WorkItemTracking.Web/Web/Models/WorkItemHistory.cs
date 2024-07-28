// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.WorkItemHistory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  [DataContract]
  public class WorkItemHistory : WorkItemTrackingResource
  {
    [DataMember]
    public int Rev { get; set; }

    [DataMember]
    public string Value { get; set; }

    [DataMember]
    public IdentityReference RevisedBy { get; set; }

    [DataMember]
    public DateTime RevisedDate { get; set; }

    internal static IEnumerable<WorkItemHistory> Create(
      WorkItemTrackingRequestContext witRequestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem)
    {
      IEnumerable<WorkItemRevision> source = workItem.Revisions.Concat<WorkItemRevision>((IEnumerable<WorkItemRevision>) new WorkItemRevision[1]
      {
        (WorkItemRevision) workItem
      });
      IDictionary<string, IdentityReference> allIdentityReferencesByDisplayName = IdentityReferenceBuilder.CreateFromWitIdentityNames(witRequestContext, source.Select<WorkItemRevision, string>((Func<WorkItemRevision, string>) (revision => revision.ModifiedBy)), true);
      foreach (WorkItemRevision workItemRevision in source)
      {
        string fieldValue = workItemRevision.GetFieldValue<string>(witRequestContext, 54);
        if (!string.IsNullOrEmpty(fieldValue))
        {
          WorkItemHistory workItemHistory = new WorkItemHistory();
          workItemHistory.Rev = workItemRevision.Revision;
          workItemHistory.Url = WitUrlHelper.GetWorkItemHistoryUrl(witRequestContext, workItem.Id, new int?(workItemRevision.Revision));
          workItemHistory.RevisedBy = allIdentityReferencesByDisplayName[workItemRevision.ModifiedBy];
          workItemHistory.RevisedDate = workItemRevision.RevisedDate;
          workItemHistory.Value = fieldValue;
          yield return workItemHistory;
        }
      }
    }

    internal static WorkItemHistory Create(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItemFieldData workItem)
    {
      string fieldValue;
      if (string.IsNullOrEmpty(fieldValue = workItem.GetFieldValue<string>(witRequestContext, 54)))
        return (WorkItemHistory) null;
      WorkItemHistory workItemHistory = new WorkItemHistory();
      workItemHistory.Rev = workItem.Revision;
      workItemHistory.Url = WitUrlHelper.GetWorkItemHistoryUrl(witRequestContext, workItem.Id, new int?(workItem.Revision));
      workItemHistory.RevisedBy = IdentityReferenceBuilder.CreateFromWitIdentityName(witRequestContext, workItem.ModifiedBy, true);
      workItemHistory.RevisedDate = workItem.GetFieldValue<DateTime>(witRequestContext, -5);
      workItemHistory.Value = fieldValue;
      return workItemHistory;
    }
  }
}
