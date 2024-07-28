// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitKeyedJob`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel.Composition;

namespace Microsoft.TeamFoundation.Git.Server
{
  [InheritedExport]
  public abstract class GitKeyedJob<TKey> : LimitedConcurrencyJob where TKey : IGitJobKey
  {
    public abstract JobPriorityLevel JobPriorityLevel { get; }

    public abstract JobPriorityClass JobPriorityClass { get; }

    public abstract bool QueueOnRefUpdate { get; }

    public virtual bool QueueOnIndexUpdate { get; }

    public virtual bool ScheduleOnCreate { get; }

    public virtual TimeSpan ScheduleInterval { get; }

    public override int? StaticConcurrencyLimit => new int?();

    public override LimitedConcurrencyJob.LimitPercentCores PercentageOfCoresConcurrencyLimit => (LimitedConcurrencyJob.LimitPercentCores) null;

    public override LimitedConcurrencyJob.GitJobCategory? JobCategory => new LimitedConcurrencyJob.GitJobCategory?();

    public override int ConcurrencyRequeueSeconds => 1;

    public virtual int GetQueueDelaySeconds(
      IVssRequestContext requestContext,
      ITfsGitRepository repo)
    {
      return 0;
    }

    public override sealed TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      out string resultMessage)
    {
      TKey targetKey = default (TKey);
      if (typeof (TKey) != typeof (HostJobKey))
      {
        if (jobDefinition.Data == null)
        {
          resultMessage = "TeamFoundationJobDefinition.data is NULL. It's likely that this job was just converted from collection scope to repo/odb scope. The old scheduled job is blocked while it gets picked up to run by new job agent after VIP swap and before host upgrade. Please check that the deployment was in fact going out at that time of this failure on this scale unit";
          return TeamFoundationJobExecutionResult.Failed;
        }
        targetKey = TeamFoundationSerializationUtility.Deserialize<TKey>(jobDefinition.Data);
      }
      return this.Run(requestContext, targetKey, out resultMessage);
    }

    protected abstract TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TKey targetKey,
      out string resultMessage);
  }
}
