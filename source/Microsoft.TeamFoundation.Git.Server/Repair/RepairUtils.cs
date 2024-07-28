// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Repair.RepairUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Repair
{
  internal static class RepairUtils
  {
    public static void OdbQuiesce(IVssRequestContext rc, OdbId odbId, ITFLogger log)
    {
      (OdbPointerType pointerType, Guid jobId)[] compute = RepairUtils.BuildThingsToCompute(odbId);
      ITeamFoundationJobService service = rc.GetService<ITeamFoundationJobService>();
      foreach ((OdbPointerType pointerType, Guid jobId) tuple in compute)
      {
        TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(rc, tuple.jobId);
        if (foundationJobDefinition == null)
          throw new InvalidOperationException(string.Format("Definition not found for jobId {0} for pointerType {1}", (object) tuple.jobId, (object) tuple.pointerType));
        foundationJobDefinition.EnabledState = TeamFoundationJobEnabledState.FullyDisabled;
        service.UpdateJobDefinitions(rc, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
        log.Info(string.Format("Disabled jobId {0} for pointerType {1}", (object) tuple.jobId, (object) tuple.pointerType));
        try
        {
          service.StopJob(rc, tuple.jobId);
        }
        catch (JobCannotBeStoppedException ex)
        {
        }
        using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(odbId))
        {
          Sha1Id? oldPointerId = gitOdbComponent.ReadPointer(tuple.pointerType);
          if (oldPointerId.HasValue)
          {
            Sha1Id? nullable = gitOdbComponent.UpdatePointer(tuple.pointerType, oldPointerId, new Sha1Id?());
            if (nullable.HasValue)
              throw new InvalidOperationException(string.Format("Deleting the {0} pointer failed because it was concurrently updated from {1} to {2}", (object) tuple.pointerType, (object) oldPointerId, (object) nullable));
            log.Info(string.Format("Deleted {0} pointer previously set to {1}", (object) tuple.pointerType, (object) oldPointerId));
          }
        }
      }
    }

    public static void OdbUnquiesce(IVssRequestContext rc, OdbId odbId, ITFLogger log)
    {
      (OdbPointerType pointerType, Guid jobId)[] compute = RepairUtils.BuildThingsToCompute(odbId);
      ITeamFoundationJobService service = rc.GetService<ITeamFoundationJobService>();
      foreach ((OdbPointerType pointerType, Guid jobId) tuple in compute)
      {
        TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(rc, tuple.jobId);
        if (foundationJobDefinition == null)
          throw new InvalidOperationException(string.Format("Definition not found for jobId {0} for pointerType {1}", (object) tuple.jobId, (object) tuple.pointerType));
        foundationJobDefinition.EnabledState = TeamFoundationJobEnabledState.Enabled;
        service.UpdateJobDefinitions(rc, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
        log.Info(string.Format("Reenabled jobId {0} for pointerType {1}", (object) tuple.jobId, (object) tuple.pointerType));
      }
      KeyScopedJobUtil.QueueFor(rc, odbId, "GitGraphSerializationJob", "Microsoft.TeamFoundation.Git.Server.Plugins.GitGraphSerializationJob", JobPriorityLevel.Normal, JobPriorityClass.Normal);
    }

    private static (OdbPointerType pointerType, Guid jobId)[] BuildThingsToCompute(OdbId odbId) => new (OdbPointerType, Guid)[3]
    {
      (OdbPointerType.LastPackedIndex, KeyScopedJobUtil.JobIdForKeyScopedJob<OdbJobKey>(new OdbJobKey(odbId), "GitRepackerJob")),
      (OdbPointerType.Graph, KeyScopedJobUtil.JobIdForKeyScopedJob<OdbJobKey>(new OdbJobKey(odbId), "GitGraphSerializationJob")),
      (OdbPointerType.ReachabilityBitmap, KeyScopedJobUtil.JobIdForKeyScopedJob<OdbJobKey>(new OdbJobKey(odbId), "GitBitmapComputationJob"))
    };
  }
}
