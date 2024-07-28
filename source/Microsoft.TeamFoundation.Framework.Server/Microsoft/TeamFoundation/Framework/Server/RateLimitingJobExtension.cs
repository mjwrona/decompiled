// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RateLimitingJobExtension
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class RateLimitingJobExtension : ITeamFoundationJobExtension
  {
    private readonly int _processorCount;

    public RateLimitingJobExtension() => this._processorCount = Environment.ProcessorCount;

    internal RateLimitingJobExtension(int processorCount) => this._processorCount = processorCount;

    public abstract int? StaticConcurrencyLimit { get; }

    public abstract RateLimitingJobExtension.LimitPercentCores? PercentageOfCoresConcurrencyLimit { get; }

    public abstract TimeSpan ConcurrencyRequeueDelay { get; }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      CommandKey commandKey = this.GetCommandKey(jobDefinition);
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) this.GetGroupKey()).AndCommandKey(commandKey).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithMetricsRollingStatisticalWindow(this.RollingWindowSize).WithMetricsRollingStatisticalWindowBuckets(this.RollingWindowBuckets).WithExecutionMaxConcurrentRequests(this.GetConcurrencyLimit()));
      RateLimitingJobExtension.LimitedConcurrencyJobExecutionResult jobExecutionResult = new CommandService<RateLimitingJobExtension.LimitedConcurrencyJobExecutionResult>(requestContext, setter, (Func<RateLimitingJobExtension.LimitedConcurrencyJobExecutionResult>) (() => this.RunJob(requestContext, jobDefinition)), (Func<RateLimitingJobExtension.LimitedConcurrencyJobExecutionResult>) (() => this.RequeueLaterFallback(requestContext, jobDefinition))).Execute();
      resultMessage = jobExecutionResult.ResultMessage;
      return jobExecutionResult.JobResult;
    }

    protected virtual TimeSpan RollingWindowSize => TimeSpan.FromMinutes((double) this.RollingWindowBuckets);

    protected virtual int RollingWindowBuckets => 60;

    public virtual int GetConcurrencyLimit()
    {
      if (this.StaticConcurrencyLimit.HasValue)
        return this.StaticConcurrencyLimit.Value;
      return this.PercentageOfCoresConcurrencyLimit.HasValue ? (int) Math.Ceiling((Decimal) this._processorCount * ((Decimal) (int) this.PercentageOfCoresConcurrencyLimit.Value / 100M)) : 1;
    }

    protected virtual CommandKey GetCommandKey(TeamFoundationJobDefinition jobDefinition) => (CommandKey) jobDefinition.ExtensionName;

    protected virtual string GetGroupKey() => "Framework.";

    private RateLimitingJobExtension.LimitedConcurrencyJobExecutionResult RunJob(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      try
      {
        string resultMessage;
        return new RateLimitingJobExtension.LimitedConcurrencyJobExecutionResult(this.Run(requestContext, jobDefinition, out resultMessage), resultMessage);
      }
      catch (Exception ex)
      {
        ex.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
        throw;
      }
    }

    private RateLimitingJobExtension.LimitedConcurrencyJobExecutionResult RequeueLaterFallback(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        jobDefinition.ToJobReference()
      }, (int) Math.Ceiling(this.ConcurrencyRequeueDelay.TotalSeconds));
      return new RateLimitingJobExtension.LimitedConcurrencyJobExecutionResult(TeamFoundationJobExecutionResult.Blocked, "Re-queued job because there are too many concurrent jobs running.");
    }

    public abstract TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      out string resultMessage);

    public enum LimitPercentCores
    {
      JobIsHighCPU = 25, // 0x00000019
      JobIsMediumCPU = 50, // 0x00000032
      JobIsLowCPU = 75, // 0x0000004B
    }

    private class LimitedConcurrencyJobExecutionResult
    {
      public readonly TeamFoundationJobExecutionResult JobResult;
      public readonly string ResultMessage;

      public LimitedConcurrencyJobExecutionResult(
        TeamFoundationJobExecutionResult jobResult,
        string resultMessage)
      {
        this.JobResult = jobResult;
        this.ResultMessage = resultMessage;
      }
    }
  }
}
