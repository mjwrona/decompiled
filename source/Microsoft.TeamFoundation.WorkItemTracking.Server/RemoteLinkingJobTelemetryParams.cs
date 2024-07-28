// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.RemoteLinkingJobTelemetryParams
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class RemoteLinkingJobTelemetryParams
  {
    internal DateTime QueueDateTime;
    internal string ResultMessage;
    internal IEnumerable<WorkItemLinkUpdateResultRecord> PendingWorkItemRemoteLinks;
    internal IEnumerable<WorkItemLinkUpdateRecord> LocalWorkItemLinkUpdates;
    internal IEnumerable<RemoteLinkE2EData> RemoteWorkItemLinkE2EData;
    internal TeamFoundationJobExecutionResult JobResult;
    internal Guid AuthorizeById;

    internal RemoteLinkingJobTelemetryParams(
      DateTime queueDateTime,
      string resultMessage,
      Guid authorizeById,
      IEnumerable<WorkItemLinkUpdateResultRecord> pendingWorkItemRemoteLinks,
      IEnumerable<WorkItemLinkUpdateRecord> localWorkItemLinkUpdates,
      TeamFoundationJobExecutionResult jobResult)
    {
      this.QueueDateTime = queueDateTime;
      this.ResultMessage = resultMessage;
      this.AuthorizeById = authorizeById;
      this.PendingWorkItemRemoteLinks = pendingWorkItemRemoteLinks;
      this.LocalWorkItemLinkUpdates = localWorkItemLinkUpdates;
      this.RemoteWorkItemLinkE2EData = this.GetRemoteLinkTelemetryData(pendingWorkItemRemoteLinks);
      this.JobResult = jobResult;
    }

    private IEnumerable<RemoteLinkE2EData> GetRemoteLinkTelemetryData(
      IEnumerable<WorkItemLinkUpdateResultRecord> pendingWorkItemRemoteLinks)
    {
      DateTime currentTimeInUTC = DateTime.UtcNow;
      return pendingWorkItemRemoteLinks == null ? (IEnumerable<RemoteLinkE2EData>) null : (IEnumerable<RemoteLinkE2EData>) pendingWorkItemRemoteLinks.Select<WorkItemLinkUpdateResultRecord, RemoteLinkE2EData>((Func<WorkItemLinkUpdateResultRecord, RemoteLinkE2EData>) (x => new RemoteLinkE2EData()
      {
        AuthorizedByTfid = x.AuthorizedByTfid,
        LinkType = x.LinkType,
        E2EElapsedTime = (currentTimeInUTC - x.AuthorizedDate).Milliseconds,
        LocalProjectId = x.LocalProjectId,
        RemoteHostId = x.RemoteHostId,
        RemoteProjectId = x.RemoteProjectId,
        RemoteStatus = x.RemoteStatus,
        RemoteStatusMessage = x.RemoteStatusMessage,
        SourceId = x.SourceId,
        TargetId = x.TargetId
      })).ToList<RemoteLinkE2EData>();
    }
  }
}
