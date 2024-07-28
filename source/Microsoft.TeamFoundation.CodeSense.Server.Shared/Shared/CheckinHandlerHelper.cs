// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Shared.CheckinHandlerHelper
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server.Data;
using Microsoft.TeamFoundation.CodeSense.Server.Jobs;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Shared
{
  public class CheckinHandlerHelper
  {
    public static void EnqueueCodeSenseJob(
      IVssRequestContext requestContext,
      Guid jobId,
      JobPriorityLevel jobPriority)
    {
      int keepupDelay = requestContext.GetService<IVssRegistryService>().GetKeepupDelay(requestContext);
      if (requestContext.GetService<TeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobId
      }, keepupDelay, jobPriority) <= 0)
        return;
      requestContext.Trace(1023510, TraceLayer.TfsCheckinHandler, "Codesense job with id {0} is queued", (object) jobId);
    }

    public static bool IsCatchupDisabled(IVssRequestContext requestContext)
    {
      TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
      if (service != null)
      {
        TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, JobConstants.CatchupJobId);
        if (foundationJobDefinition != null && foundationJobDefinition.EnabledState.Equals((object) TeamFoundationJobEnabledState.FullyDisabled))
          return true;
      }
      return false;
    }

    public static void AddAssociatedWorkItemsToLookup(
      IVssRequestContext requestContext,
      IEnumerable<int> workItems,
      int changesetId)
    {
      List<WorkItemAssociationLookupEntry> lookupEntries = new List<WorkItemAssociationLookupEntry>();
      if (workItems.Any<int>())
      {
        foreach (int workItem in workItems)
          lookupEntries.Add(new WorkItemAssociationLookupEntry(changesetId, workItem));
      }
      else
        lookupEntries.Add(new WorkItemAssociationLookupEntry(changesetId, -1));
      using (requestContext.AcquireExemptionLock())
      {
        using (ICodeSenseSqlResourceComponent component = requestContext.CreateComponent<ICodeSenseSqlResourceComponent, CodeSenseSqlResourceComponent>())
          component.AddWorkItemAssociationLookupEntries((IEnumerable<WorkItemAssociationLookupEntry>) lookupEntries);
      }
    }
  }
}
