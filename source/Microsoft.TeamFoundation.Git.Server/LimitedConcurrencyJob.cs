// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.LimitedConcurrencyJob
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public abstract class LimitedConcurrencyJob : ITeamFoundationJobExtension
  {
    public abstract int? StaticConcurrencyLimit { get; }

    public abstract LimitedConcurrencyJob.LimitPercentCores PercentageOfCoresConcurrencyLimit { get; }

    public abstract int ConcurrencyRequeueSeconds { get; }

    public abstract LimitedConcurrencyJob.GitJobCategory? JobCategory { get; }

    public virtual TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      int? concurrencyLimit1;
      if (!this.StaticConcurrencyLimit.HasValue)
      {
        LimitedConcurrencyJob.LimitPercentCores concurrencyLimit2 = this.PercentageOfCoresConcurrencyLimit;
        int num;
        if (concurrencyLimit2 == null)
        {
          num = 1;
        }
        else
        {
          concurrencyLimit1 = concurrencyLimit2.Value;
          num = !concurrencyLimit1.HasValue ? 1 : 0;
        }
        if (num != 0 && !this.JobCategory.HasValue)
          return this.Run(requestContext, jobDefinition, out resultMessage);
      }
      int concurrencyLimit3;
      if (Environment.ProcessorCount <= 2)
      {
        concurrencyLimit3 = 1;
      }
      else
      {
        concurrencyLimit1 = this.StaticConcurrencyLimit;
        if (concurrencyLimit1.HasValue)
        {
          concurrencyLimit1 = this.StaticConcurrencyLimit;
          concurrencyLimit3 = concurrencyLimit1.Value;
        }
        else
        {
          LimitedConcurrencyJob.LimitPercentCores concurrencyLimit4 = this.PercentageOfCoresConcurrencyLimit;
          int num1;
          if (concurrencyLimit4 == null)
          {
            num1 = 0;
          }
          else
          {
            concurrencyLimit1 = concurrencyLimit4.Value;
            num1 = concurrencyLimit1.HasValue ? 1 : 0;
          }
          if (num1 != 0)
          {
            Decimal processorCount = (Decimal) Environment.ProcessorCount;
            concurrencyLimit1 = this.PercentageOfCoresConcurrencyLimit.Value;
            Decimal num2 = (Decimal) concurrencyLimit1.Value / 100M;
            concurrencyLimit3 = (int) Math.Ceiling(processorCount * num2);
          }
          else
            concurrencyLimit3 = Environment.ProcessorCount > 2 ? Environment.ProcessorCount - 1 : 1;
        }
      }
      return this.RunWithConcurrencyLimit(requestContext, jobDefinition, queueTime, out resultMessage, concurrencyLimit3);
    }

    private TeamFoundationJobExecutionResult RunWithConcurrencyLimit(
      IVssRequestContext rc,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage,
      int concurrencyLimit)
    {
      string innerResultMsg = (string) null;
      CommandKey commandKey = (CommandKey) (this.JobCategory.HasValue ? this.JobCategory.ToString() : jobDefinition.ExtensionName);
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "VersionControl.").AndCommandKey(commandKey).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionMaxConcurrentRequests(concurrencyLimit));
      int num = (int) new CommandService<TeamFoundationJobExecutionResult>(rc, setter, new Func<TeamFoundationJobExecutionResult>(runJob), new Func<TeamFoundationJobExecutionResult>(requeueLaterFallback)).Execute();
      resultMessage = innerResultMsg;
      return (TeamFoundationJobExecutionResult) num;

      TeamFoundationJobExecutionResult runJob()
      {
        try
        {
          return this.Run(rc, jobDefinition, out innerResultMsg);
        }
        catch (Exception ex)
        {
          ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
          throw;
        }
      }

      TeamFoundationJobExecutionResult requeueLaterFallback()
      {
        rc.GetService<ITeamFoundationJobService>().QueueDelayedJobs(rc, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
        {
          jobDefinition.ToJobReference()
        }, this.ConcurrencyRequeueSeconds);
        innerResultMsg = "Re-queued job because there are too many concurrent jobs running.";
        return TeamFoundationJobExecutionResult.Blocked;
      }
    }

    public abstract TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      out string resultMessage);

    public class LimitPercentCores
    {
      public static readonly LimitedConcurrencyJob.LimitPercentCores JobIsHighCPU = new LimitedConcurrencyJob.LimitPercentCores(new int?(25));
      public static readonly LimitedConcurrencyJob.LimitPercentCores JobIsMediumCPU = new LimitedConcurrencyJob.LimitPercentCores(new int?(50));
      public static readonly LimitedConcurrencyJob.LimitPercentCores JobIsLowCPU = new LimitedConcurrencyJob.LimitPercentCores(new int?(75));

      public LimitPercentCores(int? value) => this.Value = value;

      public int? Value { get; private set; }
    }

    public enum GitJobCategory
    {
      GitNative,
      NonRealtime,
    }
  }
}
