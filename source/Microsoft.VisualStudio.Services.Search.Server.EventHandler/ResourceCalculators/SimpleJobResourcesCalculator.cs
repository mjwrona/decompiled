// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.ResourceCalculators.SimpleJobResourcesCalculator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler.ResourceCalculators
{
  internal sealed class SimpleJobResourcesCalculator : BaseJobResourcesCalculator
  {
    private int m_minThresholdPercent;
    private int m_minThresholdPercentForCode;
    private int m_minThresholdPercentForWorkItem;
    private int m_minAvailableResourcesPercentageThreshold;
    private int m_maxAvailableResourcesPercentageThreshold;
    private int m_callbackDelayInMins;
    private int m_minExpectedAllottedResourcePercentage;
    private int m_totalResourcesForJobExecution;
    private const int IncrementableThreshold = 5;
    private const int DefaultAllottedResources = 1;
    private const int DefaultDelayToQueueJobs = 0;
    private const int DefaultMaxResourcesAllottablePerCollection = 1;
    private const int TracePoint = 1080541;
    private const int AdditionalDelayForCallbackInMins = 5;
    private int m_minThresholdPercentCurrent;
    private int m_minThresholdPercentForCodeCurrent;
    private int m_minThresholdPercentForWorkItemCurrent;
    private int m_maxResoucesAllottablePerCollection;
    private int m_maxResoucesAllottablePerCollectionForCode;
    private int m_maxResoucesAllottablePerCollectionForWorkItem;
    private int m_resourcesUsed;
    private List<int> m_availableResourcesPercentageHistory;
    private const int AvailableResourcesPercentageHistoryCount = 3;
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    private TraceMetaData m_traceMetaData = new TraceMetaData(1080541, "Indexing Pipeline", "Job");
    private bool m_logRusViolations;
    private object m_lock = new object();

    public SimpleJobResourcesCalculator() => this.m_availableResourcesPercentageHistory = new List<int>();

    [Info("InternalForTestPurpose")]
    internal int ResourcesUsed => this.m_resourcesUsed;

    [Info("InternalForTestPurpose")]
    internal int MaxAllottableResourcesPerCollection => this.m_maxResoucesAllottablePerCollection;

    [Info("InternalForTestPurpose")]
    internal int MaxAllottableResourcesPerCollectionForCode => this.m_maxResoucesAllottablePerCollectionForCode;

    [Info("InternalForTestPurpose")]
    internal int MaxAllottableResourcesPerCollectionForWorkItem => this.m_maxResoucesAllottablePerCollectionForWorkItem;

    [Info("InternalForTestPurpose")]
    internal void Initialize(IVssRequestContext requestContext)
    {
      this.RefreshDataFromRegistryService(requestContext);
      this.m_minThresholdPercentCurrent = this.m_minThresholdPercent;
      this.m_minThresholdPercentForCodeCurrent = this.m_minThresholdPercentForCode;
      this.m_minThresholdPercentForWorkItemCurrent = this.m_minThresholdPercentForWorkItem;
      this.ComputeMaxAllottableResourcesPerCollection();
    }

    private void RefreshDataFromRegistryService(IVssRequestContext requestContext)
    {
      ResourceCalculatorRegistryService service = requestContext.GetService<ResourceCalculatorRegistryService>();
      this.m_minThresholdPercent = service.RegistrySettings.MinAllottableResourcesPercentage;
      this.m_minThresholdPercentForCode = service.RegistrySettings.MinAllottableResourcesPercentageForCode;
      this.m_minThresholdPercentForWorkItem = service.RegistrySettings.MinAllottableResourcesPercentageForWorkItem;
      this.m_minAvailableResourcesPercentageThreshold = service.RegistrySettings.MinAvailableResourcesPercentageThreshold;
      this.m_maxAvailableResourcesPercentageThreshold = service.RegistrySettings.MaxAvailableResourcesPercentageThreshold;
      this.m_totalResourcesForJobExecution = service.RegistrySettings.MaxJobsTotal * service.RegistrySettings.TotalJobAgentInstances;
      this.m_callbackDelayInMins = service.RegistrySettings.CallbackDelayInMins;
      this.m_minExpectedAllottedResourcePercentage = service.RegistrySettings.MinExpectedResourcePercentage;
      this.m_logRusViolations = service.RegistrySettings.LogRusViolation;
    }

    private void LogViolationsIfAny(IVssRequestContext requestContext, int allottedResources)
    {
      List<TeamFoundationJobQueueEntry> source = requestContext.GetService<ITeamFoundationJobService>().QueryRunningJobs(requestContext, true);
      if (source == null)
        return;
      int num = this.m_totalResourcesForJobExecution - source.Count<TeamFoundationJobQueueEntry>();
      int availableResources = this.GetCurrentlyAvailableResources();
      if (allottedResources <= num)
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("RUS Info.Violation occurred, actual available resources: {0}, available resources: {1}, allotted resources: {2}", (object) num, (object) availableResources, (object) allottedResources)));
    }

    [Info("InternalForTestPurpose")]
    internal override JobResourcesResponse GetResourcesToQueueJobs(
      ExecutionContext executionContext,
      JobResourcesRequest jobResourcesRequest)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080541, "Indexing Pipeline", "Job", nameof (GetResourcesToQueueJobs));
      int delayToQueueJobs;
      int allottedResources;
      int delayToQueueCallback;
      try
      {
        int availableResources = this.GetCurrentlyAvailableResources();
        int collectionEntityType = this.GetMaxResoucesAllottablePerCollectionEntityType(jobResourcesRequest.EntityType);
        int val2 = collectionEntityType - jobResourcesRequest.UsedResources;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Total available resources: {0}. Max resources allotable for entity: {1} is {2}, remaining resources available for entity: {1} is {3}, requested resources: {4}", (object) availableResources, (object) jobResourcesRequest.EntityType.Name, (object) collectionEntityType, (object) val2, (object) jobResourcesRequest.RequestedResources));
        if (availableResources > 0 && val2 > 0)
        {
          delayToQueueJobs = (int) new JobQueueController(executionContext.RequestContext).GetQueueDelayFactor().TotalSeconds;
          allottedResources = Math.Min(Math.Min(availableResources, jobResourcesRequest.RequestedResources), val2);
          delayToQueueCallback = this.ComputeCallbackDelay(jobResourcesRequest.RequestedResources, allottedResources);
          if (this.m_logRusViolations)
            this.LogViolationsIfAny(executionContext.RequestContext, allottedResources);
        }
        else
        {
          allottedResources = 1;
          delayToQueueJobs = 0;
          delayToQueueCallback = this.ComputeCallbackDelay(jobResourcesRequest.RequestedResources, allottedResources);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Allotting {0} resource as no resources were available at the time of request", (object) 1)));
        }
        delayToQueueCallback = this.CheckAndAddDelayForCallbackIfNeeded(delayToQueueJobs, delayToQueueCallback);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080541, "Indexing Pipeline", "Job", nameof (GetResourcesToQueueJobs));
      }
      return new JobResourcesResponse()
      {
        AllottedResources = allottedResources,
        DelayToQueueJobs = TimeSpan.FromSeconds((double) delayToQueueJobs),
        DelayToQueueCallback = TimeSpan.FromMinutes((double) delayToQueueCallback)
      };
    }

    private int GetMaxResoucesAllottablePerCollectionEntityType(IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          return this.m_maxResoucesAllottablePerCollectionForCode;
        case "WorkItem":
          return this.m_maxResoucesAllottablePerCollectionForWorkItem;
        default:
          return this.m_maxResoucesAllottablePerCollection;
      }
    }

    internal int ComputeCallbackDelay(int requestedResources, int allottedResources) => requestedResources > 0 && allottedResources * 100 / requestedResources < this.m_minExpectedAllottedResourcePercentage ? this.m_callbackDelayInMins : -1;

    [Info("InternalForTestPurpose")]
    internal override void RefreshCurrentResourceConsumptionData(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080541, "Indexing Pipeline", "Job", nameof (RefreshCurrentResourceConsumptionData));
      try
      {
        this.RefreshDataFromRegistryService(requestContext);
        this.RefreshCurrentJobQueueStatus(requestContext);
        this.m_availableResourcesPercentageHistory.Add(this.GetCurrentlyAvailableResources() * 100 / this.m_totalResourcesForJobExecution);
        if (this.m_availableResourcesPercentageHistory.Count > 3)
          this.m_availableResourcesPercentageHistory.RemoveAt(0);
        this.ReCalculateAllocatablePercentages();
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080541, "Indexing Pipeline", "Job", nameof (RefreshCurrentResourceConsumptionData));
      }
    }

    private void ReCalculateAllocatablePercentages()
    {
      int num1 = 0;
      int num2 = 0;
      if (this.m_availableResourcesPercentageHistory.Count<int>() < 3)
        return;
      foreach (int num3 in this.m_availableResourcesPercentageHistory)
      {
        if (num3 < this.m_minAvailableResourcesPercentageThreshold)
          ++num1;
        else if (num3 > this.m_maxAvailableResourcesPercentageThreshold)
          ++num2;
      }
      if (num1 >= 3)
      {
        this.m_minThresholdPercentCurrent = Math.Max(this.m_minThresholdPercentCurrent - 5, this.m_minThresholdPercent);
        this.m_minThresholdPercentForCodeCurrent = Math.Max(this.m_minThresholdPercentForCodeCurrent - 5, this.m_minThresholdPercentForCode);
        this.m_minThresholdPercentForWorkItemCurrent = Math.Max(this.m_minThresholdPercentForWorkItemCurrent - 5, this.m_minThresholdPercentForWorkItem);
        this.ComputeMaxAllottableResourcesPerCollection();
      }
      else
      {
        if (num2 < 3)
          return;
        this.m_minThresholdPercentCurrent = Math.Min(this.m_minThresholdPercentCurrent + 5, 40);
        this.m_minThresholdPercentForCodeCurrent = Math.Min(this.m_minThresholdPercentForCodeCurrent + 5, 40);
        this.m_minThresholdPercentForWorkItemCurrent = Math.Min(this.m_minThresholdPercentForWorkItemCurrent + 5, 40);
        this.ComputeMaxAllottableResourcesPerCollection();
      }
    }

    [Info("InternalForTestPurpose")]
    internal void ComputeMaxAllottableResourcesPerCollection()
    {
      this.m_maxResoucesAllottablePerCollection = this.m_minThresholdPercentCurrent * this.m_totalResourcesForJobExecution / 100;
      this.m_maxResoucesAllottablePerCollection = this.m_maxResoucesAllottablePerCollection == 0 ? 1 : this.m_maxResoucesAllottablePerCollection;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("MaxResoucesAllottablePerCollection", "Indexing Pipeline", (double) this.m_maxResoucesAllottablePerCollection);
      this.m_maxResoucesAllottablePerCollectionForCode = this.m_minThresholdPercentForCodeCurrent * this.m_totalResourcesForJobExecution / 100;
      this.m_maxResoucesAllottablePerCollectionForCode = this.m_maxResoucesAllottablePerCollectionForCode == 0 ? 1 : this.m_maxResoucesAllottablePerCollectionForCode;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("MaxResoucesAllottablePerCollectionForCode", "Indexing Pipeline", (double) this.m_maxResoucesAllottablePerCollectionForCode);
      this.m_maxResoucesAllottablePerCollectionForWorkItem = this.m_minThresholdPercentForWorkItemCurrent * this.m_totalResourcesForJobExecution / 100;
      this.m_maxResoucesAllottablePerCollectionForWorkItem = this.m_maxResoucesAllottablePerCollectionForWorkItem == 0 ? 1 : this.m_maxResoucesAllottablePerCollectionForWorkItem;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("MaxResoucesAllottablePerCollectionForWorkItem", "Indexing Pipeline", (double) this.m_maxResoucesAllottablePerCollectionForWorkItem);
    }

    [Info("InternalForTestPurpose")]
    internal int GetCurrentlyAvailableResources() => this.m_totalResourcesForJobExecution - this.m_resourcesUsed;

    [Info("InternalForTestPurpose")]
    internal int CheckAndAddDelayForCallbackIfNeeded(int delayToQueueJobs, int delayToQueueCallback)
    {
      if (delayToQueueJobs > 0)
        delayToQueueCallback += 5;
      return delayToQueueCallback;
    }

    [Info("InternalForTestPurpose")]
    internal void RefreshCurrentJobQueueStatus(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080541, "Indexing Pipeline", "Job", nameof (RefreshCurrentJobQueueStatus));
      try
      {
        List<TeamFoundationJobQueueEntry> source = requestContext.GetService<ITeamFoundationJobService>().QueryRunningJobs(requestContext, true);
        int num1 = 0;
        if (source != null)
        {
          foreach (Guid guid in source.Select<TeamFoundationJobQueueEntry, Guid>((Func<TeamFoundationJobQueueEntry, Guid>) (x => x.JobSource)).Distinct<Guid>())
          {
            Guid host = guid;
            int num2 = source.Count<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (x => x.JobSource == host));
            num1 += num2;
            if (this.m_logRusViolations)
            {
              double num3 = (double) num2 / (double) this.m_totalResourcesForJobExecution * 100.0;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("RUS Info. Collection {0} occupying {1}% resources when thresholds are collection - {2}, code - {3}, workitem - {4}", (object) host, (object) num3, (object) this.m_minThresholdPercentCurrent, (object) this.m_minThresholdPercentForCodeCurrent, (object) this.m_minThresholdPercentForWorkItemCurrent)));
            }
          }
          lock (this.m_lock)
            this.m_resourcesUsed = num1;
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Current job queue data has been fetched successfully and CollectionResources map has been created. Resources Used {0}", (object) this.m_resourcesUsed)));
        }
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, "Current job queue data has been fetched but running jobs was found to be null");
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080541, "Indexing Pipeline", "Job", nameof (RefreshCurrentJobQueueStatus));
      }
    }
  }
}
