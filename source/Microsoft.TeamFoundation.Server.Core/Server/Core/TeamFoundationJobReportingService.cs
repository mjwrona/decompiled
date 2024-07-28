// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationJobReportingService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.Core
{
  public sealed class TeamFoundationJobReportingService : IVssFrameworkService
  {
    private TeamFoundationJobReportingJobNameCache m_jobNameCache = new TeamFoundationJobReportingJobNameCache(true, true);
    private static readonly string s_jobReportingServiceArea = nameof (TeamFoundationJobReportingService);
    private static readonly string s_jobReportingServiceLayer = "Service";
    private Dictionary<Guid, TeamFoundationServiceHostProcess> m_processInformation = new Dictionary<Guid, TeamFoundationServiceHostProcess>();
    private const int c_backInTimeHoursRegKeyDefault = 48;
    private const float c_imageScaleSizeRegKeyDefault = 1f;
    private const int c_numberOfJobsToShowInChartRegKeyDefault = 25;
    private const int c_maxNumberOfHistoryResultsKeyDefault = 500;

    internal TeamFoundationJobReportingService()
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      string message = "Registering for notifications for RegistrySettingsChanged on ServiceHost {0}";
      systemRequestContext.Trace(1001, TraceLevel.Verbose, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "TeamFoundationJobReportingService.ServiceStart");
      systemRequestContext.Trace(1002, TraceLevel.Verbose, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, message, (object) systemRequestContext.ServiceHost.InstanceId);
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), FrameworkServerConstants.AllowDormantHosts, FrameworkServerConstants.JobReportingRegistryRoot + "/*");
      systemRequestContext.Trace(1004, TraceLevel.Verbose, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "Loading Settings");
      this.LoadSettings(systemRequestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.Trace(1005, TraceLevel.Verbose, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "TeamFoundationJobReportingService.ServiceEnd");
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
    }

    private void OnSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSettings(requestContext);
    }

    private void LoadSettings(IVssRequestContext requestContext)
    {
      requestContext.Trace(1009, TraceLevel.Verbose, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "TeamFoundationJobReportingService.LoadSettings");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.ReportingWindowHours = service.GetValue<int>(requestContext, (RegistryQuery) FrameworkServerConstants.JobReportingBackInTimeHoursRegistryPath, 48);
      if (this.ReportingWindowHours < 1)
      {
        requestContext.Trace(1010, TraceLevel.Warning, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "ReportingWindowHours registry value is out of range ({0}. Using {1}.", (object) this.ReportingWindowHours, (object) 48);
        this.ReportingWindowHours = 48;
      }
      else
        requestContext.Trace(1011, TraceLevel.Info, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "ReportingWindowHours registry value is now ({0}.", (object) this.ReportingWindowHours);
      IVssRegistryService registryService1 = service;
      IVssRequestContext requestContext1 = requestContext;
      RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.JobReportingImageScaleSizeRegPath;
      ref RegistryQuery local1 = ref registryQuery;
      this.ImageScaleSize = registryService1.GetValue<float>(requestContext1, in local1, 1f);
      if ((double) this.ImageScaleSize > 4.0 || (double) this.ImageScaleSize < 0.1)
      {
        requestContext.Trace(1010, TraceLevel.Warning, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "ImageScaleSize registry value is out of range ({0}. Using {1}.  Range value need to be between 0.1 and 4.0", (object) this.ImageScaleSize, (object) 1f);
        this.ImageScaleSize = 1f;
      }
      else
        requestContext.Trace(1011, TraceLevel.Info, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "ImageScaleSize registry value is now ({0}.", (object) this.ImageScaleSize);
      IVssRegistryService registryService2 = service;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.JobReportingNumberOfJobsToShowInChartRegistryPath;
      ref RegistryQuery local2 = ref registryQuery;
      this.NumberOfJobsToShowInChart = registryService2.GetValue<int>(requestContext2, in local2, 25);
      if (this.NumberOfJobsToShowInChart > 25 || this.NumberOfJobsToShowInChart < 1)
      {
        requestContext.Trace(1010, TraceLevel.Warning, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "NumberOfJobsToShowInChart registry value is out of range ({0}. Using {1}.  Range value need to be between 1 and 25", (object) this.NumberOfJobsToShowInChart, (object) 25);
        this.ImageScaleSize = 25f;
      }
      else
        requestContext.Trace(1011, TraceLevel.Info, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "NumberOfJobsToShowInChart registry value is now ({0}.", (object) this.NumberOfJobsToShowInChart);
      IVssRegistryService registryService3 = service;
      IVssRequestContext requestContext3 = requestContext;
      registryQuery = (RegistryQuery) FrameworkServerConstants.JobReportingMaxNumberOfHistoryResultsRegistryPath;
      ref RegistryQuery local3 = ref registryQuery;
      this.MaxNumberOfHistoryResults = registryService3.GetValue<int>(requestContext3, in local3, 500);
      if (this.NumberOfJobsToShowInChart > 10000 || this.NumberOfJobsToShowInChart < 1)
      {
        requestContext.Trace(1010, TraceLevel.Warning, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "MaxNumberOfHistoryResults registry value is out of range ({0}. Using {1}.  Range value need to be between 1 and 10000", (object) this.MaxNumberOfHistoryResults, (object) 500);
        this.ImageScaleSize = 500f;
      }
      else
        requestContext.Trace(1011, TraceLevel.Info, TeamFoundationJobReportingService.s_jobReportingServiceArea, TeamFoundationJobReportingService.s_jobReportingServiceLayer, "MaxNumberOfHistoryResults registry value is now ({0}.", (object) this.MaxNumberOfHistoryResults);
    }

    public TeamFoundationServiceHostProcess GetAgentInformation(
      IVssRequestContext requestContext,
      Guid agentId)
    {
      TeamFoundationServiceHostProcess agentInformation = (TeamFoundationServiceHostProcess) null;
      if (!this.m_processInformation.TryGetValue(agentId, out agentInformation))
      {
        Dictionary<Guid, TeamFoundationServiceHostProcess> dictionary = new Dictionary<Guid, TeamFoundationServiceHostProcess>();
        foreach (TeamFoundationServiceHostProcess serviceHostProcess in requestContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProcesses(requestContext, Guid.Empty))
        {
          if (serviceHostProcess.ProcessId == agentId)
            agentInformation = serviceHostProcess;
          dictionary[agentId] = serviceHostProcess;
        }
        if (agentInformation == null)
          dictionary[agentId] = (TeamFoundationServiceHostProcess) null;
        this.m_processInformation = dictionary;
      }
      return agentInformation;
    }

    public string GetJobName(IVssRequestContext requestContext, Guid jobId)
    {
      string jobName = this.JobNameCache.GetJobName(requestContext, jobId);
      return string.IsNullOrEmpty(jobName) ? jobId.ToString() : jobName;
    }

    public List<TeamFoundationJobReportingResultTypeCount> GetResultTypeCount(
      IVssRequestContext requestContext)
    {
      DateTime utcNow = DateTime.UtcNow;
      DateTime startTime = utcNow.Subtract(new TimeSpan(this.ReportingWindowHours, 0, 0));
      using (JobReportingComponent component = requestContext.CreateComponent<JobReportingComponent>())
        return component.QueryJobResultCount(startTime, utcNow).GetCurrent<TeamFoundationJobReportingResultTypeCount>().Items;
    }

    public List<TeamFoundationJobReportingHistoryQueueTime> QueryJobCountsAndRunTime(
      IVssRequestContext requestContext,
      int? maxRowsToReturn = null)
    {
      DateTime utcNow = DateTime.UtcNow;
      DateTime startTime = utcNow.Subtract(new TimeSpan(this.ReportingWindowHours, 0, 0));
      if (!maxRowsToReturn.HasValue)
        maxRowsToReturn = new int?(this.MaxNumberOfHistoryResults);
      using (JobReportingComponent component = requestContext.CreateComponent<JobReportingComponent>())
      {
        ResultCollection resultCollection = component.QueryRunTime(startTime, utcNow, maxRowsToReturn.Value);
        if (resultCollection != null)
          return resultCollection.GetCurrent<TeamFoundationJobReportingHistoryQueueTime>().Items;
      }
      return (List<TeamFoundationJobReportingHistoryQueueTime>) null;
    }

    public List<TeamFoundationJobReportingHistory> QueryHistory(
      IVssRequestContext requestContext,
      int maxRowsToReturn,
      Guid? jobId,
      int? resultValue)
    {
      DateTime utcNow = DateTime.UtcNow;
      DateTime startTime = utcNow.Subtract(new TimeSpan(this.ReportingWindowHours, 0, 0));
      using (JobReportingComponent component = requestContext.CreateComponent<JobReportingComponent>())
      {
        ResultCollection resultCollection = component.QueryHistory(startTime, utcNow, maxRowsToReturn, jobId, resultValue);
        if (resultCollection != null)
          return resultCollection.GetCurrent<TeamFoundationJobReportingHistory>().Items;
      }
      return (List<TeamFoundationJobReportingHistory>) null;
    }

    public List<TeamFoundationJobReportingJobCountsAndRunTime> QueryQueueTimes(
      IVssRequestContext requestContext,
      int maxRowsToReturn,
      Guid? jobId,
      out DateTime startTime,
      out DateTime endTime)
    {
      endTime = DateTime.UtcNow;
      startTime = endTime.Subtract(new TimeSpan(this.ReportingWindowHours, 0, 0));
      if (maxRowsToReturn < 1 || maxRowsToReturn > 1000)
        maxRowsToReturn = this.MaxNumberOfHistoryResults;
      using (JobReportingComponent component = requestContext.CreateComponent<JobReportingComponent>())
      {
        ResultCollection resultCollection = component.QueryHistoryRunQueueTime(startTime, endTime, maxRowsToReturn, jobId);
        if (resultCollection != null)
          return resultCollection.GetCurrent<TeamFoundationJobReportingJobCountsAndRunTime>().Items;
      }
      return (List<TeamFoundationJobReportingJobCountsAndRunTime>) null;
    }

    public List<TeamFoundationJobReportingQueuePositions> QueryQueueEntries(
      IVssRequestContext requestContext,
      int position)
    {
      using (JobReportingComponent component = requestContext.CreateComponent<JobReportingComponent>())
      {
        ResultCollection resultCollection = component.QueryQueuePositions(position, this.MaxNumberOfHistoryResults);
        if (resultCollection != null)
          return resultCollection.GetCurrent<TeamFoundationJobReportingQueuePositions>().Items;
      }
      return (List<TeamFoundationJobReportingQueuePositions>) null;
    }

    public List<TeamFoundationJobReportingQueuePositionCount> QueryQueuePositionCounts(
      IVssRequestContext requestContext)
    {
      using (JobReportingComponent component = requestContext.CreateComponent<JobReportingComponent>())
      {
        ResultCollection resultCollection = component.QueryQueuePositionCount();
        if (resultCollection != null)
          return resultCollection.GetCurrent<TeamFoundationJobReportingQueuePositionCount>().Items;
      }
      return (List<TeamFoundationJobReportingQueuePositionCount>) null;
    }

    public List<TeamFoundationJobReportingResultsOverTime> QueryResultsOverTime(
      IVssRequestContext requestContext)
    {
      DateTime utcNow = DateTime.UtcNow;
      DateTime startTime = utcNow.Subtract(new TimeSpan(this.ReportingWindowHours, 0, 0));
      using (JobReportingComponent component = requestContext.CreateComponent<JobReportingComponent>())
      {
        ResultCollection resultCollection = component.QueryResultsOverTime(startTime, utcNow, this.NumberOfJobsToShowInChart);
        if (resultCollection != null)
          return resultCollection.GetCurrent<TeamFoundationJobReportingResultsOverTime>().Items;
      }
      return (List<TeamFoundationJobReportingResultsOverTime>) null;
    }

    public TeamFoundationJobReportingJobNameCache JobNameCache => this.m_jobNameCache;

    public int ReportingWindowHours { get; set; }

    public float ImageScaleSize { get; set; }

    public int NumberOfJobsToShowInChart { get; set; }

    public int MaxNumberOfHistoryResults { get; set; }
  }
}
