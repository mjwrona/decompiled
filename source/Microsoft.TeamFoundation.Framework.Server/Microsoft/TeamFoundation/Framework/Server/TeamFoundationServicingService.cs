// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServicingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.DatabaseReplication;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationServicingService : ITeamFoundationServicingService, IVssFrameworkService
  {
    internal static readonly string[] s_validOperationSuffixes = new string[4]
    {
      "Organization",
      "Collection",
      "Deployment",
      "FinalConfiguration"
    };
    private static readonly byte[] s_databasePseudoHostIdTemplate = new byte[16]
    {
      (byte) 219,
      (byte) 219,
      (byte) 219,
      (byte) 219,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0,
      (byte) 0
    };
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(2.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
    private string m_servicingResourceCookie;
    private IVssServiceHost m_serviceHost;
    internal static readonly string s_jobExtension = "Microsoft.TeamFoundation.JobService.Extensions.Core.ServicingJobExtension";
    private const int AddServicingResourceLockTimeout = 300000;
    private static readonly string s_area = "Servicing";
    private static readonly string s_layer = nameof (TeamFoundationServicingService);

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_serviceHost = systemRequestContext.ServiceHost;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddServicingResource(
      IVssRequestContext requestContext,
      string resourceName,
      byte[] hash,
      Stream fileStream,
      long fileLength,
      long compressedLength,
      long offsetFrom,
      CompressionType compressionType)
    {
      this.ValidateRequestContext(requestContext);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationFileService service = vssRequestContext.GetService<TeamFoundationFileService>();
      bool flag = false;
      int num = 0;
      while (true)
      {
        try
        {
          int fileId = 0;
          service.UploadFile(vssRequestContext, ref fileId, fileStream, hash, compressedLength, fileLength, offsetFrom, compressionType, OwnerId.Servicing, Guid.Empty, resourceName, true, false);
          if (!flag)
            break;
          vssRequestContext.TraceAlways(945010, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "DuplicateFileNameException was thrown, but upload succeeded on retry. FileName: {0}", (object) resourceName);
          break;
        }
        catch (DuplicateFileNameException ex)
        {
          vssRequestContext.TraceException(945011, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, (Exception) ex);
          if (num < 3 && offsetFrom == 0L)
          {
            flag = true;
            Thread.Sleep(new Random().Next(500, 1500));
            service.DeleteNamedFiles(vssRequestContext, OwnerId.Servicing, (IEnumerable<string>) new string[1]
            {
              resourceName
            });
          }
          else
            throw;
        }
        ++num;
      }
    }

    internal void CheckServicingJobs(IVssRequestContext deploymentContext) => this.CheckServicingJobs(deploymentContext, (string) null);

    internal void CheckServicingJobs(IVssRequestContext deploymentContext, string operationClass)
    {
      this.ValidateRequestContext(deploymentContext);
      try
      {
        deploymentContext.TraceEnter(94500, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (CheckServicingJobs));
        using (TeamFoundationLock teamFoundationLock = deploymentContext.GetService<ITeamFoundationLockingService>().AcquireLock(deploymentContext, TeamFoundationLockMode.Exclusive, "ServicingJobsCheck", 0))
        {
          if (teamFoundationLock == null)
          {
            deploymentContext.Trace(94501, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "ServicingJobsCheck lock was not acquired. Another process is performing a check.");
          }
          else
          {
            List<ServicingJobDetail> source1;
            using (ServicingComponent component = deploymentContext.CreateComponent<ServicingComponent>())
            {
              if (!(component is ServicingComponent3 servicingComponent3))
              {
                deploymentContext.Trace(94503, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Configuration database does not support ServicingComponent3. This is expected during upgrade.");
                return;
              }
              source1 = servicingComponent3.QueryQueuedServicingJobs((DateTime) SqlDateTime.MinValue, operationClass);
            }
            deploymentContext.Trace(94504, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Found {0} queued and running jobs.", (object) source1.Count);
            if (source1.Count == 0)
              return;
            Guid deployHostId = deploymentContext.ServiceHost.InstanceId;
            List<ServicingJobDetail> source2 = source1.Where<ServicingJobDetail>((Func<ServicingJobDetail, bool>) (job => TeamFoundationServicingService.GetJobSource(deploymentContext, job) == deployHostId)).ToList<ServicingJobDetail>();
            deploymentContext.Trace(94506, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Found {0} queued and running deployment level jobs.", (object) source2.Count);
            if (source2.Count == 0)
              return;
            List<TeamFoundationJobQueueEntry> source3 = deploymentContext.GetService<TeamFoundationJobService>().QueryJobQueue(deploymentContext, source2.Select<ServicingJobDetail, Guid>((Func<ServicingJobDetail, Guid>) (job => job.JobId)));
            if (!source3.Any<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (jqe => jqe == null)))
              return;
            Dictionary<Guid, ServicingJobDetail> dictionary = new Dictionary<Guid, ServicingJobDetail>();
            for (int index = 0; index < source2.Count; ++index)
            {
              if (source3[index] == null)
                dictionary.Add(source2[index].JobId, source2[index]);
            }
            Thread.Sleep(TimeSpan.FromSeconds(15.0));
            using (ServicingComponent component = deploymentContext.CreateComponent<ServicingComponent>())
              source2 = (component as ServicingComponent3).QueryQueuedServicingJobs((DateTime) SqlDateTime.MinValue, operationClass);
            foreach (ServicingJobDetail servicingJobDetail1 in source2)
            {
              ServicingJobDetail servicingJobDetail2;
              if (dictionary.TryGetValue(servicingJobDetail1.JobId, out servicingJobDetail2) && servicingJobDetail2.QueueTime == servicingJobDetail1.QueueTime && servicingJobDetail2.JobStatus == servicingJobDetail1.JobStatus)
              {
                deploymentContext.Trace(94507, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Setting status of the following job to Failed. JobId: {0}, HostId: {1}", (object) servicingJobDetail1.JobId, (object) servicingJobDetail1.HostId);
                this.UpdateServicingJobDetail(deploymentContext, servicingJobDetail1.HostId, servicingJobDetail1.JobId, (string) null, (string) null, servicingJobDetail1.QueueTime, ServicingJobStatus.Failed, ServicingJobResult.None);
              }
            }
          }
        }
      }
      finally
      {
        deploymentContext.TraceLeave(94500, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (CheckServicingJobs));
      }
    }

    public void DeleteServicingResources(
      IVssRequestContext requestContext,
      List<string> resourceNames)
    {
      this.ValidateRequestContext(requestContext);
      using (this.AcquireServicingLock(requestContext, TeamFoundationLockMode.Exclusive))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<TeamFoundationFileService>().DeleteNamedFiles(vssRequestContext, OwnerId.Servicing, (IEnumerable<string>) resourceNames);
      }
    }

    public ServicingOperation GetServicingOperation(
      IVssRequestContext requestContext,
      string servicingOperation)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(servicingOperation, nameof (servicingOperation));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      this.EnsureCacheFresh(vssRequestContext);
      TeamFoundationServicingService.ServicingOperationCacheService service = vssRequestContext.GetService<TeamFoundationServicingService.ServicingOperationCacheService>();
      ServicingOperation servicingOperation1;
      if (!service.TryGetValue(vssRequestContext, servicingOperation, out servicingOperation1))
      {
        Dictionary<string, ServicingStepGroup> dictionary = new Dictionary<string, ServicingStepGroup>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        using (this.AcquireServicingLock(requestContext, TeamFoundationLockMode.Shared))
        {
          using (ServicingComponent servicingComponent = ServicingComponent.Create(requestContext.To(TeamFoundationHostType.Deployment)))
          {
            using (ResultCollection servicingOperation2 = servicingComponent.GetServicingOperation(servicingOperation))
            {
              servicingOperation1 = servicingOperation2.GetCurrent<ServicingOperation>().Items.FirstOrDefault<ServicingOperation>();
              if (servicingOperation1 != null)
              {
                servicingOperation2.NextResult();
                foreach (ServicingStepGroup servicingStepGroup in servicingOperation2.GetCurrent<ServicingStepGroup>().Items)
                  dictionary.Add(servicingStepGroup.Name, servicingStepGroup);
                servicingOperation1.Groups.AddRange((IEnumerable<ServicingStepGroup>) dictionary.Values);
                servicingOperation2.NextResult();
                foreach (ServicingStep servicingStep in servicingOperation2.GetCurrent<ServicingStep>().Items)
                  dictionary[servicingStep.GroupName].Steps.Add(servicingStep);
                using (vssRequestContext.AcquireExemptionLock())
                  servicingOperation1 = service.Add(vssRequestContext, servicingOperation, servicingOperation1);
              }
            }
          }
        }
      }
      return servicingOperation1;
    }

    public List<string> GetServicingOperationNames(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      requestContext.To(TeamFoundationHostType.Deployment);
      using (ServicingComponent servicingComponent = ServicingComponent.Create(requestContext.To(TeamFoundationHostType.Deployment)))
        return servicingComponent.QueryServicingOperationNames();
    }

    internal int GetServicingResourceFileId(IVssRequestContext requestContext, string resourceName)
    {
      this.ValidateRequestContext(requestContext);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationServicingService.ResourceToFileMappingCacheService service = vssRequestContext.GetService<TeamFoundationServicingService.ResourceToFileMappingCacheService>();
      this.EnsureCacheFresh(vssRequestContext);
      int fileId;
      if (!service.TryGetValue(vssRequestContext, resourceName, out fileId))
      {
        if (!vssRequestContext.GetService<TeamFoundationFileService>().TryGetFileId(vssRequestContext, OwnerId.Servicing, resourceName, out fileId))
          throw new TeamFoundationServicingException("Resource not found");
        service.Add(vssRequestContext, resourceName, fileId);
      }
      return fileId;
    }

    internal Stream GetServicingResource(IVssRequestContext requestContext, string resourceName)
    {
      this.ValidateRequestContext(requestContext);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationServicingService.ResourceCacheService service = vssRequestContext.GetService<TeamFoundationServicingService.ResourceCacheService>();
      this.EnsureCacheFresh(vssRequestContext);
      byte[] buffer;
      if (!service.TryGetValue(vssRequestContext, resourceName, out buffer))
      {
        long contentLength;
        using (Stream stream = vssRequestContext.GetService<TeamFoundationFileService>().RetrieveNamedFile(vssRequestContext, OwnerId.Servicing, resourceName, false, out byte[] _, out contentLength, out CompressionType _))
        {
          if (stream == null)
            return (Stream) null;
          buffer = new byte[(int) contentLength];
          stream.Read(buffer, 0, buffer.Length);
        }
        service.Add(vssRequestContext, resourceName, buffer);
      }
      return (Stream) new MemoryStream(buffer);
    }

    public List<string> GetServicingResources(IVssRequestContext requestContext)
    {
      using (this.AcquireServicingLock(requestContext, TeamFoundationLockMode.Shared))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<TeamFoundationFileService>().QueryNamedFiles(vssRequestContext, OwnerId.Servicing).Select<FileStatistics, string>((Func<FileStatistics, string>) (fileStat => fileStat.FileName)).ToList<string>();
      }
    }

    public List<string> GetServicingStepGroupOperations(
      IVssRequestContext requestContext,
      string stepGroup)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(stepGroup, nameof (stepGroup));
      using (this.AcquireServicingLock(requestContext, TeamFoundationLockMode.Shared))
      {
        using (ServicingComponent servicingComponent = ServicingComponent.Create(requestContext.To(TeamFoundationHostType.Deployment)))
        {
          using (ResultCollection resultCollection = servicingComponent.QueryServicingStepGroupOperations(stepGroup))
            return resultCollection.GetCurrent<ServicingOperation>().Items.Select<ServicingOperation, string>((Func<ServicingOperation, string>) (servingOperation => servingOperation.Name)).ToList<string>();
        }
      }
    }

    public ServicingJobStats GetServicingJobStats(
      IVssRequestContext requestContext,
      DateTime startTime,
      DateTime endTime,
      string operationClass)
    {
      this.ValidateRequestContext(requestContext);
      List<ServicingJobsStatsRaw> servicingJobsStats;
      using (ServicingComponent component = requestContext.CreateComponent<ServicingComponent>())
      {
        if (!(component is ServicingComponent5 servicingComponent5))
        {
          requestContext.Trace(0, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "The database does not support jobs stats");
          return (ServicingJobStats) null;
        }
        servicingJobsStats = servicingComponent5.GetServicingJobsStats(startTime, endTime, operationClass);
      }
      ServicingJobsStatsRaw servicingJobsStatsRaw1 = servicingJobsStats.FirstOrDefault<ServicingJobsStatsRaw>((Func<ServicingJobsStatsRaw, bool>) (js => js.JobStatus == ServicingJobStatus.Queued));
      ServicingJobsStatsRaw servicingJobsStatsRaw2 = servicingJobsStats.FirstOrDefault<ServicingJobsStatsRaw>((Func<ServicingJobsStatsRaw, bool>) (js => js.JobStatus == ServicingJobStatus.Running));
      ServicingJobsStatsRaw servicingJobsStatsRaw3 = servicingJobsStats.FirstOrDefault<ServicingJobsStatsRaw>((Func<ServicingJobsStatsRaw, bool>) (js => js.JobStatus == ServicingJobStatus.Failed));
      ServicingJobsStatsRaw servicingJobsStatsRaw4 = servicingJobsStats.FirstOrDefault<ServicingJobsStatsRaw>((Func<ServicingJobsStatsRaw, bool>) (js => js.JobStatus == ServicingJobStatus.Complete && js.JobResult == ServicingJobResult.Failed));
      ServicingJobsStatsRaw servicingJobsStatsRaw5 = servicingJobsStats.FirstOrDefault<ServicingJobsStatsRaw>((Func<ServicingJobsStatsRaw, bool>) (js => js.JobResult == ServicingJobResult.Succeeded || js.JobResult == ServicingJobResult.PartiallySucceeded));
      ServicingJobsStatsRaw servicingJobsStatsRaw6 = servicingJobsStats.FirstOrDefault<ServicingJobsStatsRaw>((Func<ServicingJobsStatsRaw, bool>) (js => js.JobResult == ServicingJobResult.Succeeded || js.JobResult == ServicingJobResult.PartiallySucceeded || js.JobResult == ServicingJobResult.Failed));
      ServicingJobStats servicingJobStats = new ServicingJobStats();
      if (servicingJobsStatsRaw1 != null)
        servicingJobStats.QueuedCount = servicingJobsStatsRaw1.JobCount;
      if (servicingJobsStatsRaw2 != null)
      {
        servicingJobStats.InProgressCount = servicingJobsStatsRaw2.JobCount;
        servicingJobStats.AverageRunningMilliseconds = servicingJobsStatsRaw2.AvgDurationMilliseconds;
      }
      if (servicingJobsStatsRaw3 != null && servicingJobsStatsRaw4 != null)
      {
        servicingJobStats.FailedCount = servicingJobsStatsRaw3.JobCount + servicingJobsStatsRaw4.JobCount;
        int num = (servicingJobsStatsRaw3.AvgDurationMilliseconds * servicingJobsStatsRaw3.JobCount + servicingJobsStatsRaw4.AvgDurationMilliseconds * servicingJobsStatsRaw4.JobCount) / (servicingJobsStatsRaw3.JobCount / servicingJobsStatsRaw4.JobCount);
        servicingJobStats.AverageFailedMilliseconds = num;
      }
      else if (servicingJobsStatsRaw3 != null)
      {
        servicingJobStats.FailedCount = servicingJobsStatsRaw3.JobCount;
        servicingJobStats.AverageFailedMilliseconds = servicingJobsStatsRaw3.AvgDurationMilliseconds;
      }
      else if (servicingJobsStatsRaw4 != null)
      {
        servicingJobStats.FailedCount = servicingJobsStatsRaw4.JobCount;
        servicingJobStats.AverageFailedMilliseconds = servicingJobsStatsRaw4.AvgDurationMilliseconds;
      }
      if (servicingJobsStatsRaw5 != null)
      {
        servicingJobStats.SuccessCount = servicingJobsStatsRaw5.JobCount;
        servicingJobStats.AverageSuccessfulMilliseconds = servicingJobsStatsRaw5.AvgDurationMilliseconds;
      }
      if (servicingJobsStatsRaw6 != null)
        servicingJobStats.AverageQueueWaitMilliseconds = servicingJobsStatsRaw6.AvgQueueWaitTimeMilliseconds;
      return servicingJobStats;
    }

    public ServicingJobInfo GetServicingJobInfo(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId)
    {
      ServicingJobInfo servicingJobInfo = (ServicingJobInfo) null;
      this.ValidateRequestContext(requestContext);
      using (ServicingComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ServicingComponent>())
      {
        if (component is ServicingComponent5 servicingComponent5)
          servicingJobInfo = servicingComponent5.GetServicingJobInfo(hostId, jobId);
      }
      return servicingJobInfo;
    }

    public List<ServicingJobInfo> GetServicingJobsInfo(
      IVssRequestContext requestContext,
      Guid jobId)
    {
      this.ValidateRequestContext(requestContext);
      using (ServicingComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ServicingComponent>())
      {
        if (component is ServicingComponent5 servicingComponent5)
          return servicingComponent5.GetServicingJobsInfo(jobId);
      }
      return (List<ServicingJobInfo>) null;
    }

    public List<ServicingJobInfo> QueryServicingJobsInfo(
      IVssRequestContext requestContext,
      DateTime queueTimeFrom,
      DateTime queueTimeTo,
      string operationClass,
      ServicingJobResult? jobResult,
      ServicingJobStatus? jobStatus,
      string databaseName,
      int? databaseId,
      Guid? accountId,
      string poolName,
      int? top,
      IList<KeyValuePair<ServicingJobInfoColumn, SortOrder>> sortOrder)
    {
      this.ValidateRequestContext(requestContext);
      using (ServicingComponent component = requestContext.CreateComponent<ServicingComponent>())
      {
        if (component is ServicingComponent5 servicingComponent5)
          return servicingComponent5.QueryServicingJobsInfo(queueTimeFrom.ToUniversalTime(), queueTimeTo.ToUniversalTime(), operationClass, jobResult, jobStatus, databaseName, databaseId, accountId, poolName, top, sortOrder);
        requestContext.Trace(0, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "The database does not support querying job info");
        return (List<ServicingJobInfo>) null;
      }
    }

    public ServicingJobDetail PerformServicingJob(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      Guid? jobId = null,
      ITFLogger logger = null)
    {
      return this.PerformServicingJob(requestContext, servicingJobData, jobId ?? Guid.NewGuid(), DateTime.UtcNow, logger);
    }

    public ServicingJobDetail PerformServicingJob(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      Guid jobId,
      DateTime jobQueueTime,
      ITFLogger logger = null)
    {
      requestContext.SetAuditCorrelationId(servicingJobData.ServicingTokens);
      try
      {
        return this.PerformServicingJobWithPotentialReRun(requestContext, servicingJobData, jobId, jobQueueTime, logger);
      }
      catch (Exception ex)
      {
        new RetryManager(3).Invoke((Action) (() => this.UpdateServicingJobDetail(requestContext.To(TeamFoundationHostType.Deployment), servicingJobData.ServicingHostId, jobId, servicingJobData.OperationClass, string.Join(",", servicingJobData.ServicingOperations), jobQueueTime, ServicingJobStatus.Failed, ServicingJobResult.Failed)));
        throw;
      }
    }

    public ServicingJobDetail QueueMoveHost(
      IVssRequestContext deploymentContext,
      Guid hostId,
      int targetDatabaseId,
      bool deleteSource,
      bool incrementMaxTenantsIfFull,
      bool useReadOnlyMode)
    {
      return this.QueueMoveHost(deploymentContext, hostId, targetDatabaseId, string.Empty, deleteSource, incrementMaxTenantsIfFull, useReadOnlyMode);
    }

    private ServicingJobDetail QueueMoveHost(
      IVssRequestContext deploymentContext,
      Guid hostId,
      int targetDatabaseId,
      string poolName,
      bool deleteSource,
      bool incrementMaxTenantsIfFull,
      bool useReadOnlyMode)
    {
      this.ValidateRequestContext(deploymentContext);
      deploymentContext.TraceEnter(94400, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (QueueMoveHost));
      try
      {
        TeamFoundationHostManagementService service = deploymentContext.GetService<TeamFoundationHostManagementService>();
        deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
        IVssRequestContext requestContext = deploymentContext;
        Guid hostId1 = hostId;
        TeamFoundationServiceHostProperties hostProperties = service.QueryServiceHostProperties(requestContext, hostId1);
        if (hostProperties == null)
          throw new HostDoesNotExistException(hostId);
        if (hostProperties.IsVirtualServiceHost())
          throw new VirtualServiceHostException();
        string str = targetDatabaseId != DatabaseManagementConstants.InvalidDatabaseId ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Move host {0} to db {1}", (object) hostId, (object) targetDatabaseId) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Move host {0} to db pool {1}", (object) hostId, (object) poolName);
        ServicingJobData servicingJobData1 = new ServicingJobData(new string[1]
        {
          ServicingOperationConstants.MoveHost
        });
        servicingJobData1.ServicingHostId = hostId;
        servicingJobData1.OperationClass = "MoveHost";
        servicingJobData1.JobTitle = str;
        servicingJobData1.ServicingOptions = ServicingFlags.HostMustExist;
        servicingJobData1.ServicingLocks = new TeamFoundationLockInfo[2]
        {
          new TeamFoundationLockInfo()
          {
            LockMode = TeamFoundationLockMode.Shared,
            LockName = "Servicing-" + hostId.ToString(),
            LockTimeout = -1
          },
          new TeamFoundationLockInfo()
          {
            LockMode = TeamFoundationLockMode.Shared,
            LockName = "Servicing-Db" + targetDatabaseId.ToString(),
            LockTimeout = -1
          }
        };
        ServicingJobData servicingJobData2 = servicingJobData1;
        MoveHostOptions moveHostOptions = (MoveHostOptions) ((deleteSource ? 1 : 0) | 16);
        if (incrementMaxTenantsIfFull)
          moveHostOptions |= MoveHostOptions.IncrementMaxTenantsIfFull;
        if (useReadOnlyMode)
          moveHostOptions |= MoveHostOptions.UseReadOnlyMode;
        if (hostProperties.HostType == TeamFoundationHostType.Application && hostProperties.DatabaseId == deploymentContext.ServiceHost.ServiceHostInternal().DatabaseId)
          moveHostOptions |= MoveHostOptions.CopyCommonTablesOnly;
        servicingJobData2.ServicingTokens[ServicingTokenConstants.TargetDatabaseId] = targetDatabaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        servicingJobData2.ServicingTokens[ServicingTokenConstants.SourceDatabaseId] = hostProperties.DatabaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        servicingJobData2.ServicingTokens[ServicingTokenConstants.SourceHostId] = hostProperties.Id.ToString("D");
        servicingJobData2.ServicingTokens[ServicingTokenConstants.TargetDatabasePoolName] = poolName;
        servicingJobData2.ServicingTokens[ServicingTokenConstants.TableFilter] = string.Empty;
        servicingJobData2.ServicingTokens[ServicingTokenConstants.MoveHostOptions] = ((int) moveHostOptions).ToString();
        servicingJobData2.ServicingTokens[ServicingTokenConstants.FinalHostState] = hostProperties.Status.ToString();
        return this.QueueServicingJob(deploymentContext, servicingJobData2, JobPriorityClass.AboveNormal, JobPriorityLevel.Normal, new Guid?());
      }
      catch (Exception ex)
      {
        deploymentContext.TraceException(94400, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, ex);
        throw;
      }
      finally
      {
        deploymentContext.TraceLeave(94400, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (QueueMoveHost));
      }
    }

    public List<ServicingJobDetail> QueuePatchCollections(
      IVssRequestContext deploymentContext,
      Guid[] collectionIds,
      bool stopHostsNow)
    {
      this.ValidateRequestContext(deploymentContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) collectionIds, nameof (collectionIds));
      deploymentContext.TraceEnter(94100, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (QueuePatchCollections));
      int length = collectionIds.Length;
      deploymentContext.Trace(94101, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "collectionIds.Length: {0}", (object) length);
      TeamFoundationHostManagementService service = deploymentContext.GetService<TeamFoundationHostManagementService>();
      ServicingJobData[] servicingJobsData = new ServicingJobData[length];
      for (int index = 0; index < length; ++index)
      {
        Guid collectionId = collectionIds[index];
        deploymentContext.Trace(94102, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Processing collection: {0}", (object) collectionId);
        ServicingJobData servicingJobData1 = new ServicingJobData(new string[1]
        {
          ServicingOperationConstants.BringToCurrentServiceLevel
        });
        servicingJobData1.ServicingHostId = collectionId;
        servicingJobData1.JobTitle = FrameworkResources.ServiceCollectionJobTitle((object) collectionId);
        servicingJobData1.OperationClass = "ApplyPatch";
        servicingJobData1.ServicingOptions = ServicingFlags.HostMustExist;
        servicingJobData1.ServicingLocks = new TeamFoundationLockInfo[1]
        {
          new TeamFoundationLockInfo()
          {
            LockMode = TeamFoundationLockMode.Exclusive,
            LockName = "Servicing-" + collectionId.ToString(),
            LockTimeout = -1
          }
        };
        ServicingJobData servicingJobData2 = servicingJobData1;
        Stopwatch stopwatch = Stopwatch.StartNew();
        TeamFoundationServiceHostProperties serviceHostProperties = service.QueryServiceHostProperties(deploymentContext, collectionId, ServiceHostFilterFlags.None);
        deploymentContext.Trace(94103, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "GetCollectionProperties took {0} ms", (object) stopwatch.ElapsedMilliseconds);
        if (serviceHostProperties.Status == TeamFoundationServiceHostStatus.Started)
          servicingJobData2.ServicingTokens[ServicingTokenConstants.FinalHostState] = "Started";
        if (stopHostsNow && serviceHostProperties.Status == TeamFoundationServiceHostStatus.Started)
        {
          stopwatch.Restart();
          service.StopHost(deploymentContext, collectionId, ServiceHostSubStatus.Servicing, FrameworkResources.ProjectCollectionServicingInProgress(), TimeSpan.Zero);
          deploymentContext.Trace(94106, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "StopHost took {0} ms", (object) stopwatch.ElapsedMilliseconds);
        }
        servicingJobData2.ServicingTokens[ServicingTokenConstants.InstanceId] = serviceHostProperties.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
        servicingJobData2.ServicingTokens[ServicingTokenConstants.DatabaseId] = serviceHostProperties.DatabaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        servicingJobsData[index] = servicingJobData2;
      }
      List<ServicingJobDetail> servicingJobDetailList = this.QueueServicingJobs(deploymentContext, (IList<ServicingJobData>) servicingJobsData);
      deploymentContext.TraceLeave(94100, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (QueuePatchCollections));
      return servicingJobDetailList;
    }

    public ServicingJobDetail PerformUpgradeDatabase(
      IVssRequestContext deploymentContext,
      int databaseId,
      Guid jobId,
      ITFLogger logger = null)
    {
      this.ValidateRequestContext(deploymentContext);
      if (jobId.Equals(Guid.Empty))
        jobId = Guid.NewGuid();
      ServicingJobData servicingJobData = ((IEnumerable<ServicingJobData>) this.GetDatabaseUpgradeServicingJobDatas(deploymentContext, new int[1]
      {
        databaseId
      })).SingleOrDefault<ServicingJobData>();
      if (servicingJobData == null)
      {
        TeamFoundationServicingException servicingException = new TeamFoundationServicingException(string.Format("Failed to create UpdateDatabase Servicing Job Data for Database {0}", (object) databaseId));
        logger?.Error((Exception) servicingException);
        throw servicingException;
      }
      ITeamFoundationDatabaseProperties database = deploymentContext.GetService<ITeamFoundationDatabaseManagementService>().GetDatabase(deploymentContext, databaseId);
      if (!database.IsEligibleForUpgrade(true))
      {
        TeamFoundationServicingException servicingException = new TeamFoundationServicingException("Database " + database.DatabaseName + " in " + database.PoolName + " pool with Service Level " + database.ServiceLevel + " is not eligible for upgrade!");
        logger?.Error((Exception) servicingException);
        throw servicingException;
      }
      logger?.Info(string.Format("Running {0} for Database {1} as job {2}", (object) servicingJobData.OperationClass, (object) databaseId, (object) jobId));
      return this.PerformServicingJob(deploymentContext, servicingJobData, new Guid?(jobId), (ITFLogger) null);
    }

    public List<ServicingJobDetail> QueueUpgradeDatabase(
      IVssRequestContext deploymentContext,
      int[] databaseIds)
    {
      return this.QueueUpgradeDatabases(deploymentContext, databaseIds, JobPriorityClass.Normal);
    }

    public List<ServicingJobDetail> QueueUpgradeDatabases(
      IVssRequestContext deploymentContext,
      int[] databaseIds,
      JobPriorityClass jobPriorityClass)
    {
      this.ValidateRequestContext(deploymentContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) databaseIds, nameof (databaseIds));
      deploymentContext.TraceEnter(94200, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (QueueUpgradeDatabases));
      ServicingJobData[] servicingJobDatas = this.GetDatabaseUpgradeServicingJobDatas(deploymentContext, databaseIds);
      List<ServicingJobDetail> servicingJobDetailList = this.QueueServicingJobs(deploymentContext, (IList<ServicingJobData>) servicingJobDatas, jobPriorityClass);
      deploymentContext.TraceLeave(94200, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (QueueUpgradeDatabases));
      return servicingJobDetailList;
    }

    private ServicingJobData[] GetDatabaseUpgradeServicingJobDatas(
      IVssRequestContext deploymentContext,
      int[] databaseIds)
    {
      int length = databaseIds.Length;
      deploymentContext.Trace(94201, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "databaseIds.Length: {0}", (object) length);
      ServicingJobData[] servicingJobDatas = new ServicingJobData[length];
      Dictionary<int, ITeamFoundationDatabaseProperties> dictionary = deploymentContext.GetService<TeamFoundationDatabaseManagementService>().QueryDatabases(deploymentContext, true).ToDictionary<ITeamFoundationDatabaseProperties, int>((Func<ITeamFoundationDatabaseProperties, int>) (dbProperties => dbProperties.DatabaseId));
      DatabaseReplicationConfiguration configuration = deploymentContext.GetService<IDatabaseReplicationService>().Configuration;
      for (int index = 0; index < length; ++index)
      {
        int databaseId = databaseIds[index];
        if (!dictionary.ContainsKey(databaseId))
          throw new DatabaseNotFoundException(databaseId);
      }
      for (int index = 0; index < length; ++index)
      {
        int databaseId = databaseIds[index];
        deploymentContext.Trace(94202, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Processing database: {0}", (object) databaseId);
        ITeamFoundationDatabaseProperties databaseProperties = dictionary[databaseId];
        ServicingJobData servicingJobData = new ServicingJobData(new string[1]
        {
          ServicingOperationConstants.BringToCurrentServiceLevel
        })
        {
          ServicingHostId = TeamFoundationServicingService.GetDatabasePseudoHostId(databaseId),
          JobTitle = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Update db: {0}", (object) databaseId),
          OperationClass = "UpgradeDatabase",
          ServicingOptions = ServicingFlags.None
        };
        servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
        {
          new TeamFoundationLockInfo()
          {
            LockMode = TeamFoundationLockMode.Exclusive,
            LockName = "Servicing-Db" + databaseId.ToString(),
            LockTimeout = -1
          }
        };
        servicingJobData.ServicingTokens[ServicingTokenConstants.DatabaseId] = databaseId.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (configuration != null)
          servicingJobData.ServicingItems[ServicingItemConstants.DatabaseReplicationConfiguration] = (object) TeamFoundationSerializationUtility.SerializeToString<DatabaseReplicationConfigurationWrapper>(new DatabaseReplicationConfigurationWrapper(deploymentContext, configuration));
        servicingJobDatas[index] = servicingJobData;
      }
      return servicingJobDatas;
    }

    public ServicingJobDetail PerformUpgradeHost(
      IVssRequestContext deploymentContext,
      Guid hostId,
      Guid jobId,
      bool alreadyHoldingServicingLock = false,
      List<KeyValuePair<string, string>> additionalTokens = null,
      ITFLogger logger = null)
    {
      this.ValidateRequestContext(deploymentContext);
      this.ValidateHostedDeployment(deploymentContext);
      if (jobId.Equals(Guid.Empty))
        jobId = Guid.NewGuid();
      ServicingJobData servicingJobData = ((IEnumerable<ServicingJobData>) this.GetHostUpgradeServicingJobDatas(deploymentContext, new Guid[1]
      {
        hostId
      }, additionalTokens: additionalTokens, alreadyHoldingServicingLock: (alreadyHoldingServicingLock ? 1 : 0) != 0)).SingleOrDefault<ServicingJobData>();
      if (servicingJobData == null)
      {
        TeamFoundationServicingException servicingException = new TeamFoundationServicingException(string.Format("Failed to create UpgradeHost Servicing Job Data for host {0}", (object) hostId));
        logger?.Error((Exception) servicingException);
        throw servicingException;
      }
      logger?.Info(string.Format("Running {0} for host {1} as job {2}", (object) servicingJobData.OperationClass, (object) hostId, (object) jobId));
      return this.PerformServicingJob(deploymentContext, servicingJobData, new Guid?(jobId), (ITFLogger) null);
    }

    public virtual List<ServicingJobDetail> QueueUpgradeHosts(
      IVssRequestContext deploymentContext,
      Guid[] hostIds,
      JobPriorityClass jobPriorityClass,
      List<List<string>> operations = null,
      List<KeyValuePair<string, string>> additionalTokens = null)
    {
      this.ValidateRequestContext(deploymentContext);
      this.ValidateHostedDeployment(deploymentContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) hostIds, nameof (hostIds));
      deploymentContext.TraceEnter(94210, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "QueueUpdateHosts");
      ServicingJobData[] servicingJobDatas = this.GetHostUpgradeServicingJobDatas(deploymentContext, hostIds, operations, additionalTokens);
      List<ServicingJobDetail> servicingJobDetailList = this.QueueServicingJobs(deploymentContext, (IList<ServicingJobData>) servicingJobDatas, jobPriorityClass);
      deploymentContext.TraceLeave(94210, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "QueueUpdateHosts");
      return servicingJobDetailList;
    }

    public ServicingJobDetail QueueServicingJob(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      JobPriorityClass priorityClass = JobPriorityClass.AboveNormal,
      JobPriorityLevel priorityLevel = JobPriorityLevel.Normal,
      Guid? jobId = null)
    {
      servicingJobData.AddAuditCorrelationId(requestContext);
      TeamFoundationJobService service = requestContext.Elevate().GetService<TeamFoundationJobService>();
      Guid jobId1 = jobId.HasValue ? jobId.Value : Guid.NewGuid();
      TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobId1, servicingJobData.JobTitle, TeamFoundationServicingService.s_jobExtension, TeamFoundationSerializationUtility.SerializeToXml((object) servicingJobData));
      if ((servicingJobData.ServicingOptions & ServicingFlags.UseServicingContextForJobRunner) != ServicingFlags.None)
        foundationJobDefinition.UseServicingContext = true;
      foundationJobDefinition.PriorityClass = priorityClass;
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        foundationJobDefinition
      });
      this.UpdateServicingJobDetail(requestContext.To(TeamFoundationHostType.Deployment), servicingJobData.ServicingHostId, jobId1, servicingJobData.OperationClass, string.Join(",", servicingJobData.ServicingOperations), DateTime.UtcNow, ServicingJobStatus.Queued, ServicingJobResult.None);
      TeamFoundationJobReference[] jobReferences = new TeamFoundationJobReference[1]
      {
        foundationJobDefinition.ToJobReference()
      };
      service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, priorityLevel);
      return new ServicingJobDetail()
      {
        JobId = jobId1,
        HostId = servicingJobData.ServicingHostId,
        OperationClass = servicingJobData.OperationClass,
        Operations = servicingJobData.ServicingOperations,
        JobStatus = ServicingJobStatus.Queued,
        Result = ServicingJobResult.None
      };
    }

    public List<ServicingJobDetail> QueueServicingJobs(
      IVssRequestContext requestContext,
      IList<ServicingJobData> servicingJobsData)
    {
      return this.QueueServicingJobs(requestContext, servicingJobsData, JobPriorityClass.Normal);
    }

    public List<ServicingJobDetail> QueueServicingJobs(
      IVssRequestContext requestContext,
      IList<ServicingJobData> servicingJobsData,
      JobPriorityClass jobPriorityClass)
    {
      this.ValidateRequestContext(requestContext);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) servicingJobsData, nameof (servicingJobsData));
      requestContext.TraceEnter(94200, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (QueueServicingJobs));
      TeamFoundationJobService service = requestContext.Elevate().GetService<TeamFoundationJobService>();
      int count = servicingJobsData.Count;
      requestContext.Trace(94201, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "servicingJobsData.Length: {0}", (object) count);
      List<TeamFoundationJobDefinition> jobUpdates = new List<TeamFoundationJobDefinition>(count);
      List<ServicingJobDetail> servicingJobDetails = new List<ServicingJobDetail>(count);
      for (int index = 0; index < count; ++index)
      {
        ServicingJobData objectToSerialize = servicingJobsData[index];
        if (objectToSerialize == null)
        {
          requestContext.Trace(94206, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Skipping null entry in servicingJobsData");
        }
        else
        {
          Guid jobId = Guid.NewGuid();
          TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition(jobId, objectToSerialize.JobTitle, TeamFoundationServicingService.s_jobExtension, TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize))
          {
            PriorityClass = jobPriorityClass
          };
          jobUpdates.Add(foundationJobDefinition);
          servicingJobDetails.Add(new ServicingJobDetail()
          {
            JobId = jobId,
            HostId = objectToSerialize.ServicingHostId,
            OperationClass = objectToSerialize.OperationClass,
            Operations = objectToSerialize.ServicingOperations,
            OperationString = string.Join(",", objectToSerialize.ServicingOperations),
            JobStatus = ServicingJobStatus.Queued,
            Result = ServicingJobResult.None,
            CompletedStepCount = (short) 0,
            TotalStepCount = (short) -1
          });
        }
      }
      requestContext.Trace(94203, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Adding job definitions.");
      service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) jobUpdates);
      requestContext.Trace(94202, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Adding servicing job details.");
      this.AddServicingJobDetails(requestContext.To(TeamFoundationHostType.Deployment), (IEnumerable<ServicingJobDetail>) servicingJobDetails);
      requestContext.Trace(94204, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Queueing jobs.");
      for (int index1 = 0; index1 < jobUpdates.Count; index1 += 15)
      {
        int length = Math.Min(jobUpdates.Count - index1, 15);
        TeamFoundationJobReference[] foundationJobReferenceArray = new TeamFoundationJobReference[length];
        for (int index2 = 0; index2 < length; ++index2)
          foundationJobReferenceArray[index2] = jobUpdates[index1 + index2].ToJobReference();
        try
        {
          service.QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) foundationJobReferenceArray, index1 / 3, JobPriorityLevel.Highest);
        }
        catch (Exception ex)
        {
          requestContext.Trace(94213, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, string.Format("Failed to queue jobs with exception {0}", (object) ex));
          this.RevertJobDetails(requestContext, foundationJobReferenceArray, servicingJobDetails);
          throw;
        }
      }
      requestContext.Trace(94205, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Jobs queued.");
      return servicingJobDetails;
    }

    private void RevertJobDetails(
      IVssRequestContext requestContext,
      TeamFoundationJobReference[] batchOfJobs,
      List<ServicingJobDetail> servicingJobDetails)
    {
      foreach (TeamFoundationJobReference batchOfJob in batchOfJobs)
      {
        TeamFoundationJobReference job = batchOfJob;
        requestContext.Trace(94214, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, string.Format("Reset servicing jobs detail for job {0}", (object) job.JobId));
        ServicingJobDetail servicingJobDetail = servicingJobDetails.Where<ServicingJobDetail>((Func<ServicingJobDetail, bool>) (d => d.JobId.Equals(job.JobId))).FirstOrDefault<ServicingJobDetail>();
        if (servicingJobDetail != null)
        {
          try
          {
            this.UpdateServicingJobDetail(requestContext.To(TeamFoundationHostType.Deployment), servicingJobDetail.HostId, job.JobId, (string) null, (string) null, servicingJobDetail.QueueTime, ServicingJobStatus.Failed, ServicingJobResult.None);
          }
          catch
          {
            requestContext.Trace(94215, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, string.Format("Failed to update servicing job detail for job {0}.", (object) job.JobId));
          }
        }
        else
          requestContext.Trace(94216, TraceLevel.Error, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, string.Format("Job {0} isn't found in servicing job details. This should NOT happen.", (object) job.JobId));
      }
    }

    public void RequeueServicingJob(IVssRequestContext requestContext, Guid hostId, Guid jobId)
    {
      TeamFoundationJobService service = requestContext.Elevate().GetService<TeamFoundationJobService>();
      this.UpdateServicingJobDetail(requestContext.To(TeamFoundationHostType.Deployment), hostId, jobId, (string) null, (string) null, DateTime.UtcNow, ServicingJobStatus.Queued, ServicingJobResult.None);
      IVssRequestContext requestContext1 = requestContext;
      Guid[] jobIds = new Guid[1]{ jobId };
      service.QueueJobsNow(requestContext1, (IEnumerable<Guid>) jobIds, true);
    }

    internal void FilterInvalidServicingJobs(
      IVssRequestContext deploymentContext,
      List<ServicingJobDetail> servicingJobs)
    {
      if (!servicingJobs.Any<ServicingJobDetail>())
        return;
      List<Guid> list = servicingJobs.Select<ServicingJobDetail, Guid>((Func<ServicingJobDetail, Guid>) (sj => sj.JobId)).ToList<Guid>();
      List<TeamFoundationJobDefinition> foundationJobDefinitionList = deploymentContext.GetService<TeamFoundationJobService>().QueryJobDefinitions(deploymentContext, (IEnumerable<Guid>) list);
      for (int count = foundationJobDefinitionList.Count; count > 0; --count)
      {
        if (foundationJobDefinitionList[count - 1] == null)
          servicingJobs.RemoveAt(count - 1);
      }
    }

    internal List<ServicingJobDetail> GetQueuedJobs(
      IVssRequestContext deploymentContext,
      DateTime minQueueTime,
      string operationClass,
      ITFLogger logger)
    {
      this.ValidateRequestContext(deploymentContext);
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      logger.Info("Calling DetectInactiveProcesses() method.");
      deploymentContext.GetService<TeamFoundationHostManagementService>().DetectInactiveProcesses(deploymentContext);
      List<ServicingJobDetail> source1;
      using (ServicingComponent3 component = (ServicingComponent3) deploymentContext.CreateComponent<ServicingComponent>())
        source1 = component.QueryQueuedServicingJobs(minQueueTime, operationClass);
      if (source1.Count > 0 && deploymentContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        TeamFoundationJobService service = deploymentContext.GetService<TeamFoundationJobService>();
        List<ServicingJobDetail> list = source1.Where<ServicingJobDetail>((Func<ServicingJobDetail, bool>) (job => !((IEnumerable<string>) ServicingOperationClass.CollectionLevelOperationClasses).Contains<string>(job.OperationClass))).ToList<ServicingJobDetail>();
        IVssRequestContext jobSourceRequestContext = deploymentContext;
        IEnumerable<Guid> jobIds = list.Select<ServicingJobDetail, Guid>((Func<ServicingJobDetail, Guid>) (job => job.JobId));
        List<TeamFoundationJobQueueEntry> source2 = service.QueryJobQueue(jobSourceRequestContext, jobIds);
        source2.RemoveAll((Predicate<TeamFoundationJobQueueEntry>) (jqe => jqe == null));
        if (source2.Count < list.Count)
        {
          logger.Info("jobQueueEntries.Count = {0}", (object) source2.Count);
          HashSet<Guid> guidSet = new HashSet<Guid>(source2.Select<TeamFoundationJobQueueEntry, Guid>((Func<TeamFoundationJobQueueEntry, Guid>) (jqe => jqe.JobId)));
          foreach (ServicingJobDetail job in list)
          {
            if (!guidSet.Contains(job.JobId) && job.QueueTime < DateTime.UtcNow.AddMinutes(-1.0))
              this.MarkServicingJobAsFailedIfQueuedOrRunning(deploymentContext, job, logger);
          }
        }
        foreach (ServicingJobDetail job in source1)
        {
          if (((IEnumerable<string>) ServicingOperationClass.CollectionLevelOperationClasses).Contains<string>(job.OperationClass) && job.QueueTime < DateTime.UtcNow.AddMinutes(-1.0))
          {
            using (JobQueueComponent component = deploymentContext.CreateComponent<JobQueueComponent>("Default"))
              source2 = component.QueryJobQueueForOneHost(job.HostId, (IEnumerable<Guid>) new Guid[1]
              {
                job.JobId
              });
            if (source2[0] == null)
              this.MarkServicingJobAsFailedIfQueuedOrRunning(deploymentContext, job, logger);
          }
        }
      }
      return source1;
    }

    private void MarkServicingJobAsFailedIfQueuedOrRunning(
      IVssRequestContext deploymentContext,
      ServicingJobDetail job,
      ITFLogger logger)
    {
      KeyValuePair<Guid, Guid> keyValuePair = new KeyValuePair<Guid, Guid>(job.HostId, job.JobId);
      List<ServicingJobDetail> servicingJobDetailList = this.QueryServicingJobDetails(deploymentContext, (IEnumerable<KeyValuePair<Guid, Guid>>) new KeyValuePair<Guid, Guid>[1]
      {
        keyValuePair
      });
      if (servicingJobDetailList.Count != 1 || servicingJobDetailList[0].JobStatus != ServicingJobStatus.Queued && servicingJobDetailList[0].JobStatus != ServicingJobStatus.Running)
        return;
      string message = string.Format("Discrepancy between Job tables and Servicing tables for job: {0} (host: {1}).  The job is not in the job queue but tbl_ServicingJobDetail status is {2}. Setting the servicing status of the job to Failed.", (object) job.JobId, (object) job.HostId, (object) job.JobStatus);
      logger.Info(message);
      deploymentContext.Trace(94401, TraceLevel.Error, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, message);
      this.UpdateServicingJobDetail(deploymentContext, job.HostId, job.JobId, (string) null, (string) null, servicingJobDetailList[0].QueueTime, ServicingJobStatus.Failed, ServicingJobResult.None);
      job.JobStatus = ServicingJobStatus.Failed;
      job.Result = ServicingJobResult.None;
    }

    internal List<ServicingJobDetail> GetFailedServicingJobs(
      IVssRequestContext deploymentContext,
      DateTime minQueueTime,
      string operationClass,
      ITFLogger logger,
      bool distinctHost = false)
    {
      this.ValidateRequestContext(deploymentContext);
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      deploymentContext = deploymentContext.Elevate();
      this.GetQueuedJobs(deploymentContext, minQueueTime, operationClass, logger);
      List<ServicingJobDetail> failedServicingJobs;
      using (ServicingComponent3 component = (ServicingComponent3) deploymentContext.CreateComponent<ServicingComponent>())
        failedServicingJobs = component.QueryFailedServicingJobs(minQueueTime, operationClass);
      if (distinctHost)
      {
        Dictionary<Guid, ServicingJobDetail> dictionary = new Dictionary<Guid, ServicingJobDetail>();
        foreach (ServicingJobDetail servicingJobDetail in failedServicingJobs)
        {
          if (dictionary.ContainsKey(servicingJobDetail.HostId))
          {
            if (dictionary[servicingJobDetail.HostId].QueueTime < servicingJobDetail.QueueTime)
              dictionary[servicingJobDetail.HostId] = servicingJobDetail;
          }
          else
            dictionary[servicingJobDetail.HostId] = servicingJobDetail;
        }
        failedServicingJobs = dictionary.Values.ToList<ServicingJobDetail>();
      }
      logger.Info("Found {0} failed jobs", (object) failedServicingJobs.Count);
      return failedServicingJobs;
    }

    internal List<ServicingJobDetail> RequeueFailedServicingJobs(
      IVssRequestContext deploymentContext,
      DateTime minQueueTime,
      string operationClass,
      ITFLogger logger)
    {
      this.ValidateRequestContext(deploymentContext);
      if (logger == null)
        logger = (ITFLogger) new NullLogger();
      deploymentContext = deploymentContext.Elevate();
      List<ServicingJobDetail> failedServicingJobs = this.GetFailedServicingJobs(deploymentContext, minQueueTime, operationClass, logger);
      if (failedServicingJobs.Count > 0)
      {
        TeamFoundationJobService service = deploymentContext.GetService<TeamFoundationJobService>();
        Guid[] array = failedServicingJobs.Select<ServicingJobDetail, Guid>((Func<ServicingJobDetail, Guid>) (job => job.JobId)).ToArray<Guid>();
        IVssRequestContext requestContext = deploymentContext;
        Guid[] jobIds = array;
        int num = service.QueueJobsNow(requestContext, (IEnumerable<Guid>) jobIds, true);
        logger.Info("{0} jobs were rescheduled", (object) num);
        try
        {
          foreach (ServicingJobDetail servicingJobDetail in failedServicingJobs)
          {
            logger.Info("Updating servicing job detail. Host Id: {0}, Job Id: {1}", (object) servicingJobDetail.JobId, (object) servicingJobDetail.HostId);
            this.UpdateServicingJobDetail(deploymentContext, servicingJobDetail.HostId, servicingJobDetail.JobId, (string) null, (string) null, DateTime.UtcNow, ServicingJobStatus.Queued, ServicingJobResult.None);
          }
        }
        catch (Exception ex)
        {
          logger.Warning(ex);
          TeamFoundationTrace.Warning(ex.Message);
        }
      }
      return failedServicingJobs;
    }

    public void UploadServicingOperations(
      IVssRequestContext deploymentContext,
      Dictionary<ServicingOperationTarget, ServicingOperation[]> servicingOperationDict)
    {
      List<ServicingOperationData> operations = new List<ServicingOperationData>();
      List<ServicingOperationGroupData> operationGroups = new List<ServicingOperationGroupData>();
      List<ServicingStepGroupData> stepGroups = new List<ServicingStepGroupData>();
      List<ServicingStepData> steps = new List<ServicingStepData>();
      foreach (ServicingOperationTarget key in servicingOperationDict.Keys)
      {
        HashSet<string> stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (ServicingOperation servicingOperation in servicingOperationDict[key])
        {
          ServicingOperationData servicingOperationData = new ServicingOperationData()
          {
            ServicingTarget = key,
            ServicingOperation = servicingOperation.Name,
            Handlers = TeamFoundationServicingService.ToString(servicingOperation.ExecutionHandlers)
          };
          operations.Add(servicingOperationData);
          for (int index1 = 0; index1 < servicingOperation.Groups.Count; ++index1)
          {
            ServicingStepGroup group = servicingOperation.Groups[index1];
            ServicingOperationGroupData operationGroupData = new ServicingOperationGroupData()
            {
              ServicingOperation = servicingOperation.Name,
              GroupName = group.Name,
              GroupOrderNumber = (index1 + 1) * 10
            };
            operationGroups.Add(operationGroupData);
            if (stringSet.Add(group.Name))
            {
              ServicingStepGroupData servicingStepGroupData = new ServicingStepGroupData()
              {
                GroupName = group.Name,
                Handlers = TeamFoundationServicingService.ToString(group.ExecutionHandlers)
              };
              stepGroups.Add(servicingStepGroupData);
              for (int index2 = 0; index2 < group.Steps.Count; ++index2)
              {
                ServicingStep step = group.Steps[index2];
                ServicingStepData servicingStepData = new ServicingStepData()
                {
                  StepName = step.Name,
                  GroupName = group.Name,
                  Options = step.Options,
                  OrderNumber = (index2 + 1) * 10,
                  StepData = step.StepData?.OuterXml,
                  StepPerformer = step.StepPerformer,
                  StepType = step.StepType
                };
                steps.Add(servicingStepData);
              }
            }
          }
        }
      }
      using (ServicingComponent component = deploymentContext.CreateComponent<ServicingComponent>())
      {
        servicingComponent13.CommandTimeout = component is ServicingComponent13 servicingComponent13 ? SqlCommandTimeout.Max(servicingComponent13.CommandTimeout, 900) : throw new TeamFoundationServicingException("Cannot upload servicing operations until database is upgraded to Dev16.M127");
        servicingComponent13.UploadServicingOperations(operations, operationGroups, stepGroups, steps);
      }
    }

    private static string ToString(
      List<ServicingExecutionHandlerData> executionHandlers)
    {
      return executionHandlers != null && executionHandlers.Any<ServicingExecutionHandlerData>() ? string.Join(";", executionHandlers.Select<ServicingExecutionHandlerData, string>((Func<ServicingExecutionHandlerData, string>) (e => e.HandlerType))) : "";
    }

    public void StartDeployment(IVssRequestContext requestContext, string targetServiceLevels)
    {
      using (ServicingComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ServicingComponent>())
        component.StartServicingDeployment(targetServiceLevels);
    }

    public void FinishDeployment(IVssRequestContext requestContext, string targetServiceLevels)
    {
      using (ServicingComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ServicingComponent>())
        component.FinishServicingDeployment(targetServiceLevels);
    }

    public ServicingDeploymentInfo GetServicingDeploymentInfo(
      IVssRequestContext requestContext,
      string targetServiceLevels)
    {
      using (ServicingComponent component = requestContext.To(TeamFoundationHostType.Deployment).CreateComponent<ServicingComponent>())
        return component.GetServicingDeploymentInfo(targetServiceLevels);
    }

    public List<ServicingStepDetail> GetServicingDetails(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid jobId,
      DateTime queueTime,
      ServicingStepDetailFilterOptions filterOptions,
      long minDetailId,
      out ServicingJobDetail jobDetails)
    {
      this.ValidateRequestContext(requestContext);
      using (ServicingComponent servicingComponent = ServicingComponent.Create(requestContext.To(TeamFoundationHostType.Deployment)))
      {
        ResultCollection resultCollection = servicingComponent.QueryServicingStepDetails(hostId, jobId, queueTime, filterOptions, minDetailId);
        jobDetails = resultCollection.GetCurrent<ServicingJobDetail>().Items.FirstOrDefault<ServicingJobDetail>();
        if (minDetailId == long.MaxValue && servicingComponent is ServicingComponent2)
          return new List<ServicingStepDetail>();
        resultCollection.NextResult();
        return resultCollection.GetCurrent<ServicingStepDetail>().Items;
      }
    }

    internal List<ServicingJobDetail> QueryServicingJobDetails(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      return this.QueryServicingJobDetails(requestContext, hostId, (string) null);
    }

    internal List<ServicingJobDetail> QueryServicingJobDetails(
      IVssRequestContext requestContext,
      Guid hostId,
      string operationClass)
    {
      this.ValidateRequestContext(requestContext);
      using (ServicingComponent servicingComponent = ServicingComponent.Create(requestContext.To(TeamFoundationHostType.Deployment)))
        return servicingComponent.QueryServicingJobDetails(hostId, operationClass);
    }

    public List<ServicingJobDetail> QueryServicingJobDetails(
      IVssRequestContext requestContext,
      IEnumerable<KeyValuePair<Guid, Guid>> hostIdJobIdCollection)
    {
      this.ValidateRequestContext(requestContext);
      using (ServicingComponent servicingComponent = ServicingComponent.Create(requestContext))
        return servicingComponent is ServicingComponent7 servicingComponent7 ? servicingComponent7.QueryServicingJobDetails(hostIdJobIdCollection) : throw new ServiceVersionNotSupportedException("servicing", servicingComponent.Version, 7);
    }

    private ServicingJobDetail PerformServicingJobWithPotentialReRun(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      Guid jobId,
      DateTime jobQueueTime,
      ITFLogger logger)
    {
      this.ValidateRequestContext(requestContext);
      string s = string.Empty;
      if (!DatabaseServicingProvider.OperationClassesThatDoNotRecordHistory.Contains<string>(servicingJobData.OperationClass) && !servicingJobData.ServicingTokens.TryGetValue("TFS_REPEAT_SERVICING_STEPS_FROM", out s))
        s = Environment.GetEnvironmentVariable("TFS_REPEAT_SERVICING_STEPS_FROM", EnvironmentVariableTarget.Machine);
      int result = -1;
      if (!int.TryParse(s, out result) || result < 0)
        return this.PerformServicingJob(requestContext, servicingJobData, jobId, jobQueueTime, int.MaxValue, logger);
      if (result < 2)
        result = 2;
      ServicingJobDetail servicingJobDetail1 = this.PerformServicingJob(requestContext, servicingJobData, jobId, jobQueueTime, 1, logger);
      ServicingJobDetail servicingJobDetail2 = requestContext.GetService<TeamFoundationServicingService>().QueryServicingJobDetails(requestContext, servicingJobData.ServicingHostId).Where<ServicingJobDetail>((Func<ServicingJobDetail, bool>) (d => d.JobId == jobId)).FirstOrDefault<ServicingJobDetail>();
      IServicingOperationProvider servicingProvider = (IServicingOperationProvider) new DatabaseServicingProvider(requestContext, jobId);
      int num = ((IEnumerable<string>) servicingJobDetail2.Operations).Aggregate<string, int>(0, (Func<int, string, int>) ((count, servicingOperation) => count + servicingProvider.GetServicingOperation(servicingOperation).StepCount));
      for (int numberOfStepsToPerform = Math.Min(result, num - 1); numberOfStepsToPerform <= num && servicingJobDetail1.Result != ServicingJobResult.Failed; ++numberOfStepsToPerform)
      {
        TeamFoundationJobDefinition foundationJobDefinition = (TeamFoundationJobDefinition) null;
        try
        {
          foundationJobDefinition = requestContext.GetService<ITeamFoundationJobService>().QueryJobDefinitions(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            jobId
          }).SingleOrDefault<TeamFoundationJobDefinition>();
          if (foundationJobDefinition == null)
            logger?.Info(string.Format("Did not locate Job Definition for job {0}, will use the original jobDefintion Data.  This often happen for in proc servicing jobs", (object) jobId));
          else
            servicingJobData = TeamFoundationSerializationUtility.Deserialize<ServicingJobData>(foundationJobDefinition.Data);
        }
        catch (Exception ex)
        {
          if (logger != null)
          {
            logger.Info(string.Format("Encountered an exception while trying get/Deserialize the job data for job {0}, will use the original jobDefinition Data.  This often happen jobs where the Job data is not ServicingJobData or the host is virtual", (object) jobId));
            logger.Info(string.Format("jobDefinition.Data: {0}", (object) foundationJobDefinition?.Data));
            logger.Info(string.Format("Exception Details {0}, Message: {1}. {2}", (object) ex.GetType().Name, (object) ex.Message, (object) ex));
          }
        }
        servicingJobDetail1 = this.PerformServicingJob(requestContext, servicingJobData, jobId, jobQueueTime, numberOfStepsToPerform, logger);
      }
      return servicingJobDetail1;
    }

    private ServicingJobDetail PerformServicingJob(
      IVssRequestContext requestContext,
      ServicingJobData servicingJobData,
      Guid jobId,
      DateTime jobQueueTime,
      int numberOfStepsToPerform,
      ITFLogger logger)
    {
      requestContext.TraceEnter(94305, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "PerformServicingJob." + servicingJobData.OperationClass);
      ServicingJobDetail servicingJobDetail = new ServicingJobDetail()
      {
        JobId = jobId,
        HostId = servicingJobData.ServicingHostId,
        OperationClass = servicingJobData.OperationClass,
        Operations = servicingJobData.ServicingOperations,
        Result = ServicingJobResult.Failed
      };
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      IInternalTeamFoundationHostManagementService service1 = vssRequestContext.GetService<IInternalTeamFoundationHostManagementService>();
      TeamFoundationLock teamFoundationLock1 = (TeamFoundationLock) null;
      if ((servicingJobData.ServicingOptions & ServicingFlags.NotAcquiringServicingLock) == ServicingFlags.None)
        teamFoundationLock1 = this.AcquireServicingLock(requestContext, TeamFoundationLockMode.Shared);
      try
      {
        DatabaseServicingProvider servicingProvider = new DatabaseServicingProvider(vssRequestContext, jobId);
        IServicingOperationProvider operationProvider = (IServicingOperationProvider) servicingProvider;
        DeploymentServiceHost deploymentServiceHost = (DeploymentServiceHost) vssRequestContext.ServiceHost.DeploymentServiceHost;
        using (DatabaseServicingLogger dbLogger = new DatabaseServicingLogger(deploymentServiceHost, servicingJobData.ServicingHostId, jobId, jobQueueTime, servicingJobData.OperationClass))
        {
          using (ServicingContext servicingContext = new ServicingContext(requestContext, deploymentServiceHost.Status == TeamFoundationServiceHostStatus.Started ? deploymentServiceHost.CreateSystemContext(true) : deploymentServiceHost.CreateServicingContext(), (IServicingResourceProvider) servicingProvider, (IServicingStepDetailLogger) dbLogger, logger ?? (ITFLogger) new NullLogger(), servicingJobData.ServicingTokens, servicingJobData.ServicingItems, servicingJobData.OperationClass))
          {
            servicingContext.TargetHostId = servicingJobData.ServicingHostId;
            if (numberOfStepsToPerform != int.MaxValue)
              servicingContext.IsRerunTesting = true;
            servicingContext.JobSourceHost = requestContext.ServiceHost.InstanceId;
            ExtensionStepPerformerProvider stepPerformerProvider = ExtensionStepPerformerProvider.Get(requestContext.ServiceHost.PlugInDirectory, servicingContext.TFLogger);
            if ((servicingJobData.ServicingOptions & ServicingFlags.UseSystemTargetRequestContext) == ServicingFlags.UseSystemTargetRequestContext)
              servicingContext.UseSystemTargetRequestContext = true;
            ITeamFoundationLockingService service2 = vssRequestContext.GetService<ITeamFoundationLockingService>();
            TeamFoundationLock[] teamFoundationLockArray = (TeamFoundationLock[]) null;
            int totalStepCount = 0;
            try
            {
              requestContext.EnterCancelableRegion((ICancelable) servicingContext);
              try
              {
                if (servicingJobData.ServicingLocks != null && servicingJobData.ServicingLocks.Length != 0)
                {
                  teamFoundationLockArray = new TeamFoundationLock[servicingJobData.ServicingLocks.Length];
                  for (int index = 0; index < servicingJobData.ServicingLocks.Length; ++index)
                  {
                    TeamFoundationLockInfo servicingLock = servicingJobData.ServicingLocks[index];
                    if (!string.IsNullOrEmpty(servicingLock.LockName) && servicingLock.LockMode != TeamFoundationLockMode.NoLock)
                    {
                      servicingContext.LogInfo(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Acquiring servicing job lock '{0}'", (object) servicingLock.LockName));
                      teamFoundationLockArray[index] = service2.AcquireLock(vssRequestContext, servicingLock) ?? throw new TeamFoundationLockException(FrameworkResources.FailedToAcquireLock((object) servicingLock.LockName));
                    }
                  }
                }
                servicingContext.Tokens[ServicingTokenConstants.JobId] = jobId.ToString();
                bool hostedDeployment = vssRequestContext.ExecutionEnvironment.IsHostedDeployment;
                if (hostedDeployment)
                {
                  servicingContext.Tokens.Add(ServicingTokenConstants.HostedDeployment, bool.TrueString);
                  Guid guid = vssRequestContext.GetService<CachedRegistryService>().GetValue<Guid>(vssRequestContext, (RegistryQuery) ConfigurationConstants.InstanceType, Guid.Empty);
                  if (Guid.Empty != guid)
                    servicingContext.Tokens[ServicingTokenConstants.InstanceType] = guid.ToString();
                  CachedRegistryService service3 = vssRequestContext.GetService<CachedRegistryService>();
                  string lockTimeInSeconds = FrameworkServerConstants.ServicingMaxLockTimeInSeconds;
                  string s1 = service3.GetValue(vssRequestContext, (RegistryQuery) lockTimeInSeconds, false, (string) null);
                  if (int.TryParse(s1, out int _))
                    servicingContext.Tokens.Add("MaxServicingLockTimeInSeconds", s1);
                  string sessionTimeInSeconds = FrameworkServerConstants.ServicingMaxBlockingSessionTimeInSeconds;
                  string s2 = service3.GetValue(vssRequestContext, (RegistryQuery) sessionTimeInSeconds, false, (string) null);
                  if (int.TryParse(s2, out int _))
                    servicingContext.Tokens.Add("MaxBlockingSessionTimeInSeconds", s2);
                }
                ServicingOperationSet operations;
                this.PerformPreJobWork(requestContext, servicingContext, servicingJobData, operationProvider, (IStepPerformerProvider) stepPerformerProvider, jobId, jobQueueTime, out operations);
                if (servicingJobData.ServicingOperations.Length != 0)
                {
                  using (ServicingStepDriver servicingStepDriver = new ServicingStepDriver(servicingContext, (IStepPerformerProvider) stepPerformerProvider, (IServicingExecutionHandlerProvider) new PluginServicingExecutionHandlerProvider(requestContext.ServiceHost.PlugInDirectory), new ServicingOperationSet[1]
                  {
                    operations
                  }))
                  {
                    servicingStepDriver.HostedDeployment = hostedDeployment;
                    servicingStepDriver.ServicingStepGroupExecutionHandlers.Add((IServicingStepGroupExecutionHandler) servicingProvider);
                    try
                    {
                      servicingStepDriver.Execute(numberOfStepsToPerform);
                    }
                    catch (Exception ex1)
                    {
                      try
                      {
                        string str;
                        if (servicingContext.TryGetToken(ServicingTokenConstants.StopHostOnFailure, out str))
                        {
                          if (bool.TrueString.Equals(str, StringComparison.OrdinalIgnoreCase))
                          {
                            using (System.Timers.Timer timer = new System.Timers.Timer(20000.0))
                            {
                              timer.Elapsed += (ElapsedEventHandler) ((_param1, _param2) => TeamFoundationTracingService.TraceRaw(0, TraceLevel.Warning, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Host '{0}' is not stopping", (object) servicingJobData.ServicingHostId.ToString()));
                              timer.AutoReset = true;
                              service1.StopHost(vssRequestContext, servicingJobData.ServicingHostId, ServiceHostSubStatus.Servicing, FrameworkResources.ProjectCollectionServicingFailureMessage(), TimeSpan.FromMinutes(3.0));
                            }
                          }
                        }
                      }
                      catch (Exception ex2)
                      {
                        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "Error attempting to stop host '{0}' when servicing step failed. Exception details: {1}", servicingJobData.ServicingHostId.ToString(), ex2.ToReadableStackTrace());
                      }
                      throw;
                    }
                    finally
                    {
                      totalStepCount = servicingStepDriver.ServicingStepCount;
                    }
                  }
                }
                ServiceHostSubStatus finalHostSubStatus;
                if (numberOfStepsToPerform >= totalStepCount && TeamFoundationServicingService.NeedToStartHost(servicingContext, out finalHostSubStatus))
                {
                  ArgumentUtility.CheckForEmptyGuid(servicingJobData.ServicingHostId, "hostId");
                  TeamFoundationServiceHostProperties hostProperties = service1.QueryServiceHostProperties(vssRequestContext, servicingJobData.ServicingHostId);
                  if (hostProperties == null)
                    throw new HostDoesNotExistException(servicingJobData.ServicingHostId);
                  if (hostProperties.Status != TeamFoundationServiceHostStatus.Started)
                  {
                    try
                    {
                      requestContext.RootContext.Items.Add(ServicingHostValidatorConstants.SkipHostValidation, (object) true);
                      TeamFoundationHostReadyState foundationHostReadyState = service1.QueryHostReadyState(requestContext.To(TeamFoundationHostType.Deployment), hostProperties);
                      if (!foundationHostReadyState.IsReady)
                        servicingContext.Log(ServicingStepLogEntryKind.Warning, foundationHostReadyState.Message);
                      else
                        service1.StartHostInternal(vssRequestContext, servicingJobData.ServicingHostId, finalHostSubStatus);
                    }
                    finally
                    {
                      requestContext.RootContext.Items.Remove(ServicingHostValidatorConstants.SkipHostValidation);
                    }
                  }
                }
                if (numberOfStepsToPerform >= totalStepCount && ("UpgradeHost".Equals(servicingJobData.OperationClass, StringComparison.Ordinal) || "ApplyPatch".Equals(servicingJobData.OperationClass, StringComparison.Ordinal) || "AttachCollection".Equals(servicingJobData.OperationClass, StringComparison.Ordinal)))
                {
                  Guid hostId = Guid.Parse(servicingContext.Tokens[ServicingTokenConstants.HostId]);
                  TeamFoundationServiceHostProperties hostProperties = service1.QueryServiceHostProperties(vssRequestContext, hostId);
                  string serviceLevel;
                  if (!servicingContext.TryGetToken(ServicingTokenConstants.ServiceLevel, out serviceLevel))
                    serviceLevel = vssRequestContext.ServiceHost.ServiceHostInternal().ServiceLevel;
                  hostProperties.ServiceLevel = serviceLevel;
                  service1.UpdateServiceHost(vssRequestContext, hostProperties);
                }
                servicingJobDetail.Result = servicingJobData.ServicingOperations.Length == 0 ? ServicingJobResult.Skipped : ServicingJobResult.Succeeded;
              }
              catch (Exception ex)
              {
                if (!(ex is TeamFoundationServicingException))
                {
                  servicingContext.Error(ex.Message);
                  servicingContext.LogInfo(ex.ToReadableStackTrace());
                }
                throw;
              }
              finally
              {
                try
                {
                  servicingJobDetail.JobStatus = ServicingJobStatus.Complete;
                  if (operationProvider is IDisposable disposable)
                    disposable.Dispose();
                  dbLogger.Flush(vssRequestContext);
                }
                finally
                {
                  if (numberOfStepsToPerform >= totalStepCount || servicingJobDetail.Result == ServicingJobResult.Failed)
                  {
                    for (int index = 0; index < 5; ++index)
                    {
                      try
                      {
                        this.UpdateServicingJobDetail(vssRequestContext, servicingJobData.ServicingHostId, jobId, servicingJobData.OperationClass, string.Empty, jobQueueTime, ServicingJobStatus.Complete, servicingJobDetail.Result, (short) totalStepCount);
                        break;
                      }
                      catch (Exception ex)
                      {
                        vssRequestContext.TraceException(0, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, ex);
                      }
                    }
                  }
                  else
                    servicingContext.LogInfo("================ Iteration with {0} step(s) completed. ================", new object[1]
                    {
                      (object) numberOfStepsToPerform
                    });
                }
              }
            }
            finally
            {
              requestContext.ExitCancelableRegion((ICancelable) servicingContext);
              if (teamFoundationLockArray != null)
              {
                foreach (TeamFoundationLock teamFoundationLock2 in teamFoundationLockArray)
                  teamFoundationLock2?.Dispose();
              }
            }
          }
        }
      }
      finally
      {
        teamFoundationLock1?.Dispose();
      }
      requestContext.TraceLeave(94305, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "PerformServicingJob." + servicingJobData.OperationClass);
      return servicingJobDetail;
    }

    private void StartUpgradeDatabase(ServicingContext servicingContext)
    {
      string token = servicingContext.Tokens[ServicingTokenConstants.ServiceLevel];
      using (ExtendedAttributeComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<ExtendedAttributeComponent>(servicingContext.GetConnectionInfo(), 300))
      {
        servicingContext.LogInfo("Setting {0} to {1} on the target database.", new object[2]
        {
          (object) TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelToStamp,
          (object) token
        });
        componentRaw.WriteDatabaseAttribute(TeamFoundationSqlResourceComponent.ExtendedPropertyServiceLevelToStamp, token);
      }
    }

    internal TeamFoundationLock AcquireServicingLock(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode)
    {
      return this.AcquireServicingLock(requestContext, lockMode, -1);
    }

    internal TeamFoundationLock AcquireServicingLock(
      IVssRequestContext requestContext,
      TeamFoundationLockMode lockMode,
      int lockTimeout)
    {
      requestContext.TraceEnter(94600, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (AcquireServicingLock));
      requestContext.Elevate().GetService<CachedRegistryService>();
      ITeamFoundationLockingService service = requestContext.Elevate().GetService<ITeamFoundationLockingService>();
      TeamFoundationLock teamFoundationLock = (TeamFoundationLock) null;
      try
      {
        if (lockMode == TeamFoundationLockMode.Exclusive && lockTimeout == -1)
        {
          int lockTimeout1 = 5000;
          int num = 1;
          DateTime now = DateTime.Now;
          while (teamFoundationLock == null)
          {
            requestContext.Trace(94601, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, string.Format("Trying to acquire lock ({0}) on {1} ", (object) lockMode, (object) "CollectionServicing") + string.Format("with timeout: {0} sec. Attempt #{1}. ", (object) (lockTimeout1 / 1000), (object) num) + string.Format("Time passed: {0}", (object) DateTime.Now.Subtract(now)));
            teamFoundationLock = service.AcquireLock(requestContext.Elevate(), lockMode, FrameworkLockResources.CollectionServicing, lockTimeout1);
            lockTimeout1 += lockTimeout1;
            ++num;
            if (lockTimeout1 > 600000)
              lockTimeout1 = 600000;
          }
        }
        else
        {
          requestContext.Trace(94602, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, string.Format("Trying to acquire lock ({0}) on {1} ", (object) lockMode, (object) "CollectionServicing") + string.Format("with timeout: {0} sec.", (object) (lockTimeout / 1000)));
          teamFoundationLock = service.AcquireLock(requestContext.Elevate(), lockMode, FrameworkLockResources.CollectionServicing, lockTimeout);
        }
        if (teamFoundationLock == null)
          throw new TeamFoundationLockException(FrameworkResources.FailedToAcquireLock((object) FrameworkLockResources.CollectionServicing));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(94603, TraceLevel.Error, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, ex);
        teamFoundationLock?.Dispose();
        throw;
      }
      requestContext.TraceLeave(94604, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, nameof (AcquireServicingLock));
      return teamFoundationLock;
    }

    public static List<string> GetDeltaOperations(
      IVssRequestContext requestContext,
      IServicingOperationProvider operationProvider,
      ServicingOperationTarget operationTarget,
      string fromServiceLevel,
      string toServiceLevel,
      string[] operationPrefixes,
      bool isLegacyFinalConfiguration = false)
    {
      Dictionary<string, ServiceLevel> serviceLevelMap1 = ServiceLevel.CreateServiceLevelMap(requestContext, fromServiceLevel);
      Dictionary<string, ServiceLevel> serviceLevelMap2 = ServiceLevel.CreateServiceLevelMap(requestContext, toServiceLevel);
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment && serviceLevelMap2.First<KeyValuePair<string, ServiceLevel>>().Value.PatchNumber > 0)
      {
        foreach (KeyValuePair<string, ServiceLevel> keyValuePair in serviceLevelMap2)
          keyValuePair.Value.PatchNumber = int.MaxValue;
      }
      return TeamFoundationServicingService.GetDeltaOperations(requestContext, operationProvider, operationTarget, serviceLevelMap1, serviceLevelMap2, operationPrefixes, isLegacyFinalConfiguration);
    }

    public static List<string> GetDeltaOperations(
      IVssRequestContext requestContext,
      IServicingOperationProvider operationProvider,
      ServicingOperationTarget operationTarget,
      Dictionary<string, ServiceLevel> fromServiceLevelMap,
      Dictionary<string, ServiceLevel> toServiceLevelMap,
      string[] operationPrefixes,
      bool isLegacyFinalConfiguration = false)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      bool flag = service.GetValue<bool>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.ServicingMode, false);
      string b = service.GetValue<string>(vssRequestContext, (RegistryQuery) FrameworkServerConstants.ServicingAreas, (string) null);
      List<string> deltaOperations1;
      if (string.IsNullOrEmpty(b) || string.Equals(ServicingOperationPrefixes.VisualStudioServices, b, StringComparison.OrdinalIgnoreCase))
      {
        deltaOperations1 = TeamFoundationServicingService.GetDeltaOperations(operationProvider, operationTarget, fromServiceLevelMap[ServicingOperationPrefixes.VisualStudioServices], toServiceLevelMap[ServicingOperationPrefixes.VisualStudioServices], operationPrefixes, isLegacyFinalConfiguration);
      }
      else
      {
        Dictionary<ServiceLevel, List<ServicingOperation>> dictionary = new Dictionary<ServiceLevel, List<ServicingOperation>>();
        deltaOperations1 = new List<string>();
        if (flag)
        {
          foreach (string operationPrefix in operationPrefixes)
          {
            List<string> deltaOperations2 = TeamFoundationServicingService.GetDeltaOperations(operationProvider, operationTarget, TeamFoundationServicingService.GetServiceLevel(fromServiceLevelMap, operationPrefix), TeamFoundationServicingService.GetServiceLevel(toServiceLevelMap, operationPrefix), new string[1]
            {
              operationPrefix
            }, (isLegacyFinalConfiguration ? 1 : 0) != 0);
            deltaOperations1.AddRange((IEnumerable<string>) deltaOperations2);
          }
        }
        else
          deltaOperations1 = TeamFoundationServicingService.GetMergedDeltaOperations(operationProvider, operationTarget, fromServiceLevelMap, toServiceLevelMap, operationPrefixes, isLegacyFinalConfiguration);
      }
      return deltaOperations1;
    }

    private static void AddDeltaOperations(
      Dictionary<ServiceLevel, List<ServicingOperation>> upgradeOperationsByLevel,
      KeyValuePair<ServiceLevel, List<ServicingOperation>> kvp)
    {
      if (upgradeOperationsByLevel.ContainsKey(kvp.Key))
        upgradeOperationsByLevel[kvp.Key].AddRange((IEnumerable<ServicingOperation>) kvp.Value);
      else
        upgradeOperationsByLevel.Add(kvp.Key, kvp.Value);
    }

    internal static ServiceLevel GetServiceLevel(
      Dictionary<string, ServiceLevel> serviceLevelMap,
      string prefix)
    {
      if (prefix.StartsWith(FrameworkServerConstants.PreServiceBinariesOperationsPrefix, StringComparison.OrdinalIgnoreCase))
        prefix = prefix.Substring(FrameworkServerConstants.PreServiceBinariesOperationsPrefix.Length);
      else if (prefix.StartsWith(FrameworkServerConstants.PostPartitionDbsOperationsPrefix, StringComparison.OrdinalIgnoreCase))
        prefix = prefix.Substring(FrameworkServerConstants.PostPartitionDbsOperationsPrefix.Length);
      if (prefix == string.Empty)
        prefix = "Tfs";
      ServiceLevel serviceLevel;
      if (!serviceLevelMap.TryGetValue(prefix, out serviceLevel))
      {
        foreach (string key in serviceLevelMap.Keys)
        {
          if (prefix.StartsWith(key))
          {
            serviceLevel = serviceLevelMap[key];
            break;
          }
        }
      }
      return !(serviceLevel == (ServiceLevel) null) ? serviceLevel : throw new TeamFoundationServicingException(FrameworkResources.InvalidServicingOperationPrefix((object) prefix));
    }

    public static List<string> GetDeltaOperations(
      IServicingOperationProvider operationProvider,
      ServicingOperationTarget operationTarget,
      ServiceLevel fromServiceLevel,
      ServiceLevel toServiceLevel,
      string[] operationPrefixes,
      bool isLegacyFinalConfiguration = false)
    {
      List<string> deltaOperations = new List<string>();
      if (toServiceLevel != (ServiceLevel) null && toServiceLevel != fromServiceLevel && toServiceLevel.PatchNumber > 0)
        toServiceLevel = new ServiceLevel(toServiceLevel.MajorVersion, toServiceLevel.Milestone, 9999);
      foreach (KeyValuePair<ServiceLevel, List<ServicingOperation>> keyValuePair in (IEnumerable<KeyValuePair<ServiceLevel, List<ServicingOperation>>>) TeamFoundationServicingService.GetDeltaOperationsByLevel(operationProvider, operationTarget, fromServiceLevel, toServiceLevel, operationPrefixes, isLegacyFinalConfiguration).OrderBy<KeyValuePair<ServiceLevel, List<ServicingOperation>>, ServiceLevel>((Func<KeyValuePair<ServiceLevel, List<ServicingOperation>>, ServiceLevel>) (kvp => kvp.Key)))
      {
        if (keyValuePair.Key.PatchNumber <= 0 || ServiceLevel.CompareMajorVersions(keyValuePair.Key.MajorVersion, "Dev14") >= 0)
        {
          List<ServicingOperation> source = keyValuePair.Value;
          deltaOperations.AddRange(source.Select<ServicingOperation, string>((Func<ServicingOperation, string>) (op => op.Name)));
        }
      }
      return deltaOperations;
    }

    private static List<string> GetMergedDeltaOperations(
      IServicingOperationProvider operationProvider,
      ServicingOperationTarget operationTarget,
      Dictionary<string, ServiceLevel> fromServiceLevelMap,
      Dictionary<string, ServiceLevel> toServiceLevelMap,
      string[] operationPrefixes,
      bool isLegacyFinalConfiguration = false)
    {
      Dictionary<ServiceLevel, List<ServicingOperation>> dictionary = new Dictionary<ServiceLevel, List<ServicingOperation>>();
      List<string> mergedDeltaOperations = new List<string>();
      foreach (string operationPrefix in operationPrefixes)
      {
        foreach (KeyValuePair<ServiceLevel, List<ServicingOperation>> kvp in (IEnumerable<KeyValuePair<ServiceLevel, List<ServicingOperation>>>) TeamFoundationServicingService.GetDeltaOperationsByLevel(operationProvider, operationTarget, TeamFoundationServicingService.GetServiceLevel(fromServiceLevelMap, operationPrefix), TeamFoundationServicingService.GetServiceLevel(toServiceLevelMap, operationPrefix), new string[1]
        {
          operationPrefix
        }, (isLegacyFinalConfiguration ? 1 : 0) != 0).OrderBy<KeyValuePair<ServiceLevel, List<ServicingOperation>>, ServiceLevel>((Func<KeyValuePair<ServiceLevel, List<ServicingOperation>>, ServiceLevel>) (kvp => kvp.Key)))
          TeamFoundationServicingService.AddDeltaOperations(dictionary, kvp);
      }
      foreach (KeyValuePair<ServiceLevel, List<ServicingOperation>> keyValuePair in (IEnumerable<KeyValuePair<ServiceLevel, List<ServicingOperation>>>) dictionary.OrderBy<KeyValuePair<ServiceLevel, List<ServicingOperation>>, ServiceLevel>((Func<KeyValuePair<ServiceLevel, List<ServicingOperation>>, ServiceLevel>) (kvp => kvp.Key)))
      {
        if (keyValuePair.Key.PatchNumber <= 0 || ServiceLevel.CompareMajorVersions(keyValuePair.Key.MajorVersion, "Dev14") >= 0)
        {
          List<ServicingOperation> source = keyValuePair.Value;
          mergedDeltaOperations.AddRange(source.Select<ServicingOperation, string>((Func<ServicingOperation, string>) (op => op.Name)));
        }
      }
      return mergedDeltaOperations;
    }

    public static Dictionary<ServiceLevel, List<ServicingOperation>> GetDeltaOperationsByLevel(
      IServicingOperationProvider operationProvider,
      ServicingOperationTarget operationTarget,
      ServiceLevel fromServiceLevel,
      ServiceLevel toServiceLevel,
      string[] operationPrefixes,
      bool isLegacyFinalConfiguration = false)
    {
      Dictionary<ServiceLevel, ServicingOperation[]> source = new Dictionary<ServiceLevel, ServicingOperation[]>();
      if (toServiceLevel == (ServiceLevel) null)
        toServiceLevel = new ServiceLevel("Dev20", "M99");
      if (toServiceLevel == fromServiceLevel && (operationTarget != ServicingOperationTarget.PartitionDatabase && operationTarget != ServicingOperationTarget.DeploymentHost && operationTarget != ServicingOperationTarget.ConfigurationDatabase || toServiceLevel.PatchNumber <= 0 && toServiceLevel.Milestone.IndexOf("-part", 0, StringComparison.OrdinalIgnoreCase) <= 0))
        return new Dictionary<ServiceLevel, List<ServicingOperation>>();
      string operationTargetSuffix = TeamFoundationServicingService.GetOperationTargetSuffix(operationTarget);
      foreach (string servicingOperationName in operationProvider.GetServicingOperationNames())
      {
        if (string.IsNullOrEmpty(operationTargetSuffix))
        {
          bool flag = false;
          foreach (string validOperationSuffix in TeamFoundationServicingService.s_validOperationSuffixes)
          {
            if (servicingOperationName.EndsWith(validOperationSuffix, StringComparison.OrdinalIgnoreCase))
            {
              flag = true;
              break;
            }
          }
          if (flag)
            continue;
        }
        else if (!servicingOperationName.EndsWith(operationTargetSuffix, StringComparison.OrdinalIgnoreCase))
          continue;
        for (int index = 0; index < operationPrefixes.Length; ++index)
        {
          string operationPrefix = operationPrefixes[index];
          ServiceLevel level;
          if (servicingOperationName.StartsWith(operationPrefix, StringComparison.OrdinalIgnoreCase) && ServiceLevel.TryGetServiceLevelFromOperation(servicingOperationName, operationPrefix, out level))
          {
            if (TeamFoundationServicingService.ShouldRunOperation(level, fromServiceLevel, toServiceLevel, operationTarget))
            {
              ServicingOperation[] servicingOperationArray;
              if (!source.TryGetValue(level, out servicingOperationArray))
              {
                servicingOperationArray = new ServicingOperation[operationPrefixes.Length];
                source.Add(level, servicingOperationArray);
              }
              servicingOperationArray[index] = operationProvider.GetServicingOperation(servicingOperationName);
              break;
            }
            break;
          }
        }
      }
      return source.ToDictionary<KeyValuePair<ServiceLevel, ServicingOperation[]>, ServiceLevel, List<ServicingOperation>>((Func<KeyValuePair<ServiceLevel, ServicingOperation[]>, ServiceLevel>) (kvp => kvp.Key), (Func<KeyValuePair<ServiceLevel, ServicingOperation[]>, List<ServicingOperation>>) (kvp => ((IEnumerable<ServicingOperation>) kvp.Value).Where<ServicingOperation>((Func<ServicingOperation, bool>) (op => op != null)).ToList<ServicingOperation>()));
    }

    private static bool ShouldRunOperation(
      ServiceLevel level,
      ServiceLevel fromServiceLevel,
      ServiceLevel toServiceLevel,
      ServicingOperationTarget operationTarget)
    {
      return !(level < fromServiceLevel) && !(level > toServiceLevel) && (!(level == fromServiceLevel) || (operationTarget == ServicingOperationTarget.PartitionDatabase || operationTarget == ServicingOperationTarget.DeploymentHost || operationTarget == ServicingOperationTarget.ConfigurationDatabase) && (fromServiceLevel == toServiceLevel && (toServiceLevel.PatchNumber > 0 || toServiceLevel.Milestone.IndexOf("-part", 0, StringComparison.OrdinalIgnoreCase) > 0) || level == fromServiceLevel && fromServiceLevel.MajorVersion == toServiceLevel.MajorVersion && fromServiceLevel.Milestone == toServiceLevel.Milestone && fromServiceLevel.PatchNumber > 0));
    }

    internal static bool IsDatabaseOperationTarget(ServicingOperationTarget target) => target == ServicingOperationTarget.ConfigurationDatabase || target == ServicingOperationTarget.PartitionDatabase;

    internal static string GetOperationTargetSuffix(ServicingOperationTarget target)
    {
      switch (target)
      {
        case ServicingOperationTarget.PartitionDatabase:
        case ServicingOperationTarget.ConfigurationDatabase:
          return string.Empty;
        case ServicingOperationTarget.DeploymentHost:
          return "Deployment";
        case ServicingOperationTarget.OrganizationHost:
          return "Organization";
        case ServicingOperationTarget.CollectionHost:
          return "Collection";
        default:
          throw new TeamFoundationServicingException(FrameworkResources.InvalidServicingTarget((object) target));
      }
    }

    internal ServicingOperationTarget GetHostOperationTarget(TeamFoundationHostType hostType)
    {
      switch (hostType)
      {
        case TeamFoundationHostType.Deployment:
          return ServicingOperationTarget.DeploymentHost;
        case TeamFoundationHostType.Application:
          return ServicingOperationTarget.OrganizationHost;
        case TeamFoundationHostType.ProjectCollection:
          return ServicingOperationTarget.CollectionHost;
        default:
          throw new TeamFoundationServicingException(FrameworkResources.InvalidServicingHostType((object) hostType));
      }
    }

    public static Guid GetDatabasePseudoHostId(int databaseId)
    {
      byte[] b = (byte[]) TeamFoundationServicingService.s_databasePseudoHostIdTemplate.Clone();
      b[15] = (byte) (databaseId & (int) byte.MaxValue);
      b[14] = (byte) (databaseId >> 8 & (int) byte.MaxValue);
      b[13] = (byte) (databaseId >> 16 & (int) byte.MaxValue);
      b[12] = (byte) (databaseId >> 24 & (int) byte.MaxValue);
      string str = databaseId.ToString();
      if (str.Length % 2 == 1)
        str = "0" + str;
      int index1 = 9;
      int index2 = str.Length - 1;
      while (index2 >= 0)
      {
        byte num = (byte) ((int) str[index2] - 48 + ((int) str[index2 - 1] - 48 << 4));
        b[index1] = num;
        index2 -= 2;
        --index1;
      }
      if (str.Length > 4)
      {
        byte num = b[7];
        b[7] = b[6];
        b[6] = num;
      }
      if (str.Length > 8)
      {
        byte num = b[5];
        b[5] = b[4];
        b[4] = num;
      }
      return new Guid(b);
    }

    internal static int GetDatabaseId(Guid databasePseudoHostId)
    {
      byte[] byteArray = databasePseudoHostId.ToByteArray();
      if (!TeamFoundationServicingService.IsPseudoHostId(byteArray))
        throw new ArgumentException("Specified parameter is not a valid database host id.");
      return (int) byteArray[15] + ((int) byteArray[14] << 8) + ((int) byteArray[13] << 16) + ((int) byteArray[12] << 24);
    }

    internal static bool IsPseudoHostId(Guid id) => TeamFoundationServicingService.IsPseudoHostId(id.ToByteArray());

    private static bool IsPseudoHostId(byte[] b)
    {
      for (int index = 0; index < 4; ++index)
      {
        if (b[index] != (byte) 219)
          return false;
      }
      return true;
    }

    internal static Guid GetJobSource(
      IVssRequestContext deploymentContext,
      ServicingJobDetail jobDetail)
    {
      string operationClass = jobDetail.OperationClass;
      if (operationClass != null)
      {
        switch (operationClass.Length)
        {
          case 8:
            if (operationClass == "MoveHost")
              break;
            goto label_36;
          case 10:
            switch (operationClass[0])
            {
              case 'A':
                if (operationClass == "ApplyPatch")
                  break;
                goto label_36;
              case 'D':
                if (operationClass == "DataImport")
                  break;
                goto label_36;
              case 'E':
                if (operationClass == "ExportHost")
                  break;
                goto label_36;
              default:
                goto label_36;
            }
            break;
          case 11:
            if (operationClass == "UpgradeHost")
              break;
            goto label_36;
          case 12:
            if (operationClass == "CreateSchema")
              break;
            goto label_36;
          case 13:
            switch (operationClass[0])
            {
              case 'C':
                if (operationClass == "CreateProject")
                  break;
                goto label_36;
              case 'D':
                if (operationClass == "DeleteProject")
                  break;
                goto label_36;
              default:
                goto label_36;
            }
            return jobDetail.HostId;
          case 14:
            if (operationClass == "MigrateAccount")
              break;
            goto label_36;
          case 15:
            if (operationClass == "UpgradeDatabase")
              break;
            goto label_36;
          case 16:
            switch (operationClass[1])
            {
              case 'e':
                if (operationClass == "DeleteCollection" || operationClass == "DetachCollection")
                  goto label_29;
                else
                  goto label_36;
              case 'r':
                if (operationClass == "CreateCollection")
                  goto label_29;
                else
                  goto label_36;
              case 's':
                if (operationClass == "AssignCollection")
                  goto label_35;
                else
                  goto label_36;
              case 't':
                if (operationClass == "AttachCollection")
                  goto label_29;
                else
                  goto label_36;
              default:
                goto label_36;
            }
          case 17:
            switch (operationClass[0])
            {
              case 'P':
                if (operationClass == "PrepareCollection")
                  goto label_29;
                else
                  goto label_36;
              case 'U':
                if (operationClass == "UpgradeCollection")
                  goto label_29;
                else
                  goto label_36;
              default:
                goto label_36;
            }
          case 18:
            if (operationClass == "ReparentCollection")
              break;
            goto label_36;
          case 20:
            if (operationClass == "AssignHostingAccount")
              goto label_35;
            else
              goto label_36;
          case 21:
            if (operationClass == "PreCreateOrganization")
              break;
            goto label_36;
          case 22:
            if (operationClass == "DeletePrivateArtifacts")
              goto label_29;
            else
              goto label_36;
          case 25:
            if (operationClass == "CreateHostingOrganization")
              break;
            goto label_36;
          case 26:
            if (operationClass == "UpdateCollectionProperties")
              goto label_29;
            else
              goto label_36;
          default:
            goto label_36;
        }
        return deploymentContext.ServiceHost.InstanceId;
label_29:
        if (deploymentContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return deploymentContext.ServiceHost.InstanceId;
        TeamFoundationServiceHostProperties serviceHostProperties = deploymentContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentContext, jobDetail.HostId, ServiceHostFilterFlags.None);
        return serviceHostProperties != null ? serviceHostProperties.ParentId : Guid.Empty;
label_35:
        return Guid.Empty;
      }
label_36:
      return Guid.Empty;
    }

    internal static bool NeedToStartHost(
      ServicingContext servicingContext,
      out ServiceHostSubStatus finalHostSubStatus)
    {
      finalHostSubStatus = ServiceHostSubStatus.None;
      bool startHost = false;
      string str1;
      if (servicingContext.TryGetToken(ServicingTokenConstants.FinalHostState, out str1))
      {
        try
        {
          startHost = (TeamFoundationServiceHostStatus) Enum.Parse(typeof (TeamFoundationServiceHostStatus), str1, true) == TeamFoundationServiceHostStatus.Started;
        }
        catch (Exception ex)
        {
          servicingContext.LogInfo(ex.ToReadableStackTrace());
        }
      }
      if (startHost)
      {
        string str2;
        if (servicingContext.TryGetToken(ServicingTokenConstants.FinalHostSubStatus, out str2))
        {
          try
          {
            finalHostSubStatus = (ServiceHostSubStatus) Enum.Parse(typeof (ServiceHostSubStatus), str2, true);
          }
          catch (Exception ex)
          {
            servicingContext.LogInfo(ex.ToReadableStackTrace());
          }
        }
      }
      return startHost;
    }

    private void ParseServicingOperation(
      Stream xmlStream,
      out string servicingOperation,
      out List<string> stepGroups,
      out List<string> executionHandlers)
    {
      ArgumentUtility.CheckForNull<Stream>(xmlStream, nameof (xmlStream));
      try
      {
        XPathDocument xpathDocument = new XPathDocument(xmlStream);
        servicingOperation = (xpathDocument.CreateNavigator().SelectSingleNode("/ServicingOperation/@name") ?? throw new TeamFoundationServicingException(!(xmlStream is FileStream fileStream) ? FrameworkResources.ServicingOperationConfigDoesNotDefineOperationNameError() : FrameworkResources.ServicingOperationFileDoesNotDefineOperationNameError((object) fileStream.Name))).Value;
        stepGroups = new List<string>();
        XPathNodeIterator source1 = xpathDocument.CreateNavigator().Select("/ServicingOperation/ServicingStepGroup/@name");
        stepGroups = new List<string>(source1.OfType<XPathNavigator>().Select<XPathNavigator, string>((Func<XPathNavigator, string>) (node => node.Value)));
        XPathNodeIterator source2 = xpathDocument.CreateNavigator().Select("/ServicingOperation/ExecutionHandlers/ExecutionHandler/@type");
        executionHandlers = new List<string>(source2.OfType<XPathNavigator>().Select<XPathNavigator, string>((Func<XPathNavigator, string>) (node => node.Value)));
      }
      catch (XmlException ex)
      {
        throw new TeamFoundationServicingException(ex.Message, (Exception) ex);
      }
    }

    private ServicingJobData[] GetHostUpgradeServicingJobDatas(
      IVssRequestContext deploymentContext,
      Guid[] hostIds,
      List<List<string>> operations = null,
      List<KeyValuePair<string, string>> additionalTokens = null,
      bool alreadyHoldingServicingLock = false)
    {
      int length = hostIds.Length;
      deploymentContext.Trace(94211, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "hostIds.Length: {0}", (object) length);
      TeamFoundationHostManagementService service1 = deploymentContext.GetService<TeamFoundationHostManagementService>();
      ServicingJobData[] servicingJobDatas = new ServicingJobData[length];
      TeamFoundationDatabaseManagementService service2 = deploymentContext.GetService<TeamFoundationDatabaseManagementService>();
      for (int index = 0; index < length; ++index)
      {
        Guid hostId = hostIds[index];
        deploymentContext.Trace(94212, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "Processing host: {0}", (object) hostId);
        HostProperties hostProperties = service1.QueryServiceHostPropertiesCached(deploymentContext, hostId);
        if (hostProperties == null)
        {
          deploymentContext.TraceAlways(94212, TraceLevel.Info, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, string.Format("Host {0} was selected as an upgrade candidate, but does not exist.  Was this host deleted?", (object) hostId));
        }
        else
        {
          string[] strArray;
          if (operations != null && operations[index] != null)
            strArray = operations[index].ToArray();
          else
            strArray = new string[1]
            {
              ServicingOperationConstants.BringToCurrentServiceLevel
            };
          ServicingJobData servicingJobData = new ServicingJobData(strArray)
          {
            ServicingHostId = hostId,
            JobTitle = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Update host: {0}", (object) hostId),
            OperationClass = "UpgradeHost",
            ServicingOptions = ServicingFlags.None,
            ServicingOperationTarget = this.GetHostOperationTarget(hostProperties.HostType)
          };
          if (!alreadyHoldingServicingLock)
            servicingJobData.ServicingLocks = new TeamFoundationLockInfo[1]
            {
              new TeamFoundationLockInfo()
              {
                LockMode = TeamFoundationLockMode.Exclusive,
                LockName = "Servicing-" + hostId.ToString(),
                LockTimeout = -1
              }
            };
          if (operations == null || operations[index] == null)
          {
            ITeamFoundationDatabaseProperties database = service2.GetDatabase(deploymentContext, hostProperties.DatabaseId, true);
            TeamFoundationDatabasePool databasePool = service2.GetDatabasePool(deploymentContext, database.PoolName);
            servicingJobData.ServicingTokens[ServicingTokenConstants.DeltaOperationPrefixes] = databasePool.DeltaOperationPrefixes ?? ServicingOperationPrefixes.VisualStudioServices;
          }
          servicingJobData.ServicingTokens[ServicingTokenConstants.HostId] = hostProperties.Id.ToString("D", (IFormatProvider) CultureInfo.InvariantCulture);
          IDictionary<string, string> servicingTokens1 = servicingJobData.ServicingTokens;
          string databaseId = ServicingTokenConstants.DatabaseId;
          int num = hostProperties.DatabaseId;
          string str1 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          servicingTokens1[databaseId] = str1;
          if (hostProperties.ServiceLevel == null)
            hostProperties.ServiceLevel = "Dev14.M67";
          servicingJobData.ServicingTokens[ServicingTokenConstants.ServiceLevelFrom] = hostProperties.ServiceLevel;
          IDictionary<string, string> servicingTokens2 = servicingJobData.ServicingTokens;
          string serveruiculture = ServicingTokenConstants.SERVERUICULTURE;
          num = deploymentContext.ServiceHost.GetCulture(deploymentContext).LCID;
          string str2 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          servicingTokens2[serveruiculture] = str2;
          if (additionalTokens != null)
          {
            foreach (KeyValuePair<string, string> additionalToken in additionalTokens)
              servicingJobData.ServicingTokens.Add(additionalToken);
          }
          servicingJobDatas[index] = servicingJobData;
        }
      }
      return servicingJobDatas;
    }

    private void PerformPreJobWork(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      ServicingJobData servicingJobData,
      IServicingOperationProvider operationProvider,
      IStepPerformerProvider stepPerformerProvider,
      Guid jobId,
      DateTime jobQueueTime,
      out ServicingOperationSet operations)
    {
      TeamFoundationServiceHostProperties hostProperties = (TeamFoundationServiceHostProperties) null;
      ISqlConnectionInfo connectionInfo = this.GetConnectionInfoFromItems(requestContext, servicingContext, ServicingItemConstants.ConnectionInfo);
      ISqlConnectionInfo sqlConnectionInfo = this.GetConnectionInfoFromItems(requestContext, servicingContext, ServicingItemConstants.DboConnectionInfo);
      this.InitializeDatabaseReplicationConfiguration(requestContext, servicingContext, ServicingItemConstants.DatabaseReplicationConfiguration);
      if (sqlConnectionInfo == null && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        sqlConnectionInfo = connectionInfo;
      requestContext = requestContext.Elevate();
      int databaseId1 = -1;
      ServicingOperationTarget servicingOperationTarget = servicingJobData.ServicingOperationTarget;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      TeamFoundationHostManagementService service1 = vssRequestContext.GetService<TeamFoundationHostManagementService>();
      if ((servicingJobData.ServicingOptions & ServicingFlags.HostMustNotExist) == ServicingFlags.HostMustNotExist)
      {
        if (servicingJobData.ServicingHostId == Guid.Empty || (servicingJobData.ServicingOptions & ServicingFlags.HostMustExist) == ServicingFlags.HostMustExist)
          throw new ArgumentException(FrameworkResources.NoHostAvailableForRequest());
        hostProperties = service1.QueryServiceHostProperties(vssRequestContext, servicingJobData.ServicingHostId);
        if (hostProperties != null)
          throw new ArgumentException(FrameworkResources.CollectionWithIdAlreadyExists((object) servicingJobData.ServicingHostId));
      }
      if ((servicingJobData.ServicingOptions & ServicingFlags.HostMustExist) == ServicingFlags.HostMustExist)
      {
        if (servicingJobData.ServicingHostId == Guid.Empty)
          throw new ArgumentException(FrameworkResources.HostDoesNotExistException((object) servicingJobData.ServicingHostId));
        hostProperties = service1.QueryServiceHostProperties(vssRequestContext, servicingJobData.ServicingHostId);
        if (hostProperties == null)
          throw new ArgumentException(FrameworkResources.HostDoesNotExistException((object) servicingJobData.ServicingHostId));
        if (connectionInfo == null || sqlConnectionInfo == null)
        {
          ITeamFoundationDatabaseProperties database = vssRequestContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(vssRequestContext, hostProperties.DatabaseId, true);
          if (connectionInfo == null)
            connectionInfo = database.SqlConnectionInfo;
          if (sqlConnectionInfo == null)
            sqlConnectionInfo = database.DboConnectionInfo;
          if (sqlConnectionInfo == null)
            sqlConnectionInfo = connectionInfo;
        }
      }
      servicingContext.LogInfo("OperationClass: {0}", new object[1]
      {
        (object) servicingContext.OperationClass
      });
      string operationClass = servicingJobData.OperationClass;
      if (operationClass != null)
      {
        switch (operationClass.Length)
        {
          case 8:
            if (operationClass == "MoveHost")
              goto label_37;
            else
              goto label_35;
          case 11:
            if (operationClass == "UpgradeHost")
              break;
            goto label_35;
          case 15:
            if (operationClass == "UpgradeDatabase")
              break;
            goto label_35;
          case 16:
            if (operationClass == "CreateCollection")
            {
              TeamProjectCollectionProperties collectionProperties;
              if (servicingContext.TryGetItem<TeamProjectCollectionProperties>(ServicingItemConstants.CollectionProperties, out collectionProperties) && collectionProperties.DefaultConnectionString != null)
              {
                connectionInfo = collectionProperties.DefaultConnectionInfo;
                goto label_37;
              }
              else
                goto label_37;
            }
            else
              goto label_35;
          case 20:
            if (operationClass == "AssignHostingAccount")
              goto label_37;
            else
              goto label_35;
          case 21:
            if (operationClass == "PreCreateOrganization")
              goto label_37;
            else
              goto label_35;
          case 25:
            if (operationClass == "CreateHostingOrganization")
              goto label_37;
            else
              goto label_35;
          default:
            goto label_35;
        }
        string token = servicingContext.Tokens[ServicingTokenConstants.DatabaseId];
        servicingContext.LogInfo("Database Id: {0}", new object[1]
        {
          (object) token
        });
        databaseId1 = int.Parse(token);
        TeamFoundationDatabaseManagementService service2 = requestContext.GetService<TeamFoundationDatabaseManagementService>();
        servicingContext.LogInfo("Getting database properties.");
        IVssRequestContext requestContext1 = requestContext;
        int databaseId2 = databaseId1;
        ITeamFoundationDatabaseProperties database = service2.GetDatabase(requestContext1, databaseId2, true);
        if (database == null)
          throw new DatabaseNotFoundException(databaseId1);
        servicingContext.LogInfo("Pool name: {0}, IsImport: {1}, IsExport: {2}, IsExternal: {3}.", new object[4]
        {
          (object) database.PoolName,
          (object) database.IsImportDatabase(),
          (object) database.IsExportDatabase(),
          (object) database.IsExternalDatabase()
        });
        if (database.IsImportDatabase())
          servicingContext.Tokens[ServicingTokenConstants.IsImportDatabase] = bool.TrueString;
        connectionInfo = database.SqlConnectionInfo;
        sqlConnectionInfo = database.DboConnectionInfo;
        goto label_37;
      }
label_35:
      if (connectionInfo == null)
        connectionInfo = ((Dictionary<string, ISqlConnectionInfo>) servicingContext.Items["DatabaseMap"])["Framework"];
label_37:
      string serviceLevel;
      switch (servicingJobData.OperationClass)
      {
        case "ApplyPatch":
        case "UpgradeDatabase":
        case "UpgradeCollection":
        case "AttachCollection":
          serviceLevel = TeamFoundationServicingService.GetDatabaseServiceLevel(servicingContext, connectionInfo);
          servicingContext.Tokens[ServicingTokenConstants.ServiceLevelFrom] = serviceLevel.ToString();
          break;
        case "UpgradeHost":
          serviceLevel = TeamFoundationServicingService.GetHostServiceLevel(servicingContext);
          break;
        default:
          serviceLevel = (string) null;
          break;
      }
      if (servicingJobData.OperationClass == "UpgradeDatabase")
      {
        TeamFoundationDatabaseManagementService service3 = vssRequestContext.GetService<TeamFoundationDatabaseManagementService>();
        ITeamFoundationDatabaseProperties database = service3.GetDatabase(vssRequestContext, databaseId1, true);
        TeamFoundationDatabasePool databasePool = service3.GetDatabasePool(vssRequestContext, database.PoolName);
        servicingContext.Tokens[ServicingTokenConstants.DeltaOperationPrefixes] = databasePool.DeltaOperationPrefixes ?? ServicingOperationPrefixes.VisualStudioServices;
      }
      if (connectionInfo != null && !servicingContext.TryGetItem<ISqlConnectionInfo>(ServicingItemConstants.ConnectionInfo, out ISqlConnectionInfo _))
      {
        servicingContext.LogInfo("Framework connection string was not found in servicing items");
        servicingContext.Items[ServicingItemConstants.ConnectionInfo] = (object) connectionInfo;
      }
      if (sqlConnectionInfo == null && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        sqlConnectionInfo = connectionInfo;
      if (sqlConnectionInfo != null && !servicingContext.TryGetItem<ISqlConnectionInfo>(ServicingItemConstants.DboConnectionInfo, out ISqlConnectionInfo _))
      {
        servicingContext.LogInfo("Dbo connection string was not found in servicing items");
        servicingContext.Items[ServicingItemConstants.DboConnectionInfo] = (object) sqlConnectionInfo;
      }
      bool onlineUpdate;
      this.HandlePseudoOperations(vssRequestContext, servicingContext, operationProvider, servicingJobData, serviceLevel, servicingOperationTarget, out onlineUpdate);
      servicingContext.Tokens[ServicingTokenConstants.OnlineUpdate] = XmlConvert.ToString(onlineUpdate);
      if (string.Equals(servicingContext.OperationClass, "UpgradeDatabase", StringComparison.Ordinal))
      {
        string tokenValue1;
        string tokenValue2;
        using (ExtendedAttributeComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<ExtendedAttributeComponent>(vssRequestContext.FrameworkConnectionInfo))
        {
          string[] strArray = componentRaw.ReadDatabaseAttributes(TeamFoundationSqlResourceComponent.ExtendedPropertySchemaVersion, TeamFoundationSqlResourceComponent.ExtendedPropertyProductVersionStamp);
          tokenValue1 = strArray[0];
          tokenValue2 = strArray[1];
        }
        servicingContext.AddTokenIfNotDefined(ServicingTokenConstants.ProductVersion, tokenValue2);
        servicingContext.AddTokenIfNotDefined(ServicingTokenConstants.SchemaVersion, tokenValue1);
        if (requestContext.ServiceInstanceType() != DataImportConstants.DataImportServiceInstanceType)
          servicingJobData.ServicingOperations = ((IEnumerable<string>) servicingJobData.ServicingOperations).Prepend<string>(ServicingOperationConstants.StartInstallPartitionUpdates).ToArray<string>();
        servicingJobData.ServicingOperations = ((IEnumerable<string>) servicingJobData.ServicingOperations).Concat<string>((IEnumerable<string>) new string[1]
        {
          ServicingOperationConstants.FinishInstallPartitionUpdates
        }).ToArray<string>();
        TeamFoundationDatabaseManagementService service4 = requestContext.GetService<TeamFoundationDatabaseManagementService>();
        string statusReason = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Installing {0} update.", (object) servicingContext.Tokens[ServicingTokenConstants.ServiceLevel]);
        ITeamFoundationDatabaseProperties database = service4.GetDatabase(vssRequestContext, databaseId1, true);
        foreach (IDatabaseReplicaInfo replica in vssRequestContext.GetService<IDatabaseReplicationService>().GetDatabaseReplicationContext(vssRequestContext, database, logger: servicingContext.TFLogger).Replicas)
          replica.UpdateDatabaseProperties(vssRequestContext, servicingContext.TFLogger, (Action<TeamFoundationDatabaseProperties>) (editableProperties =>
          {
            editableProperties.Status = TeamFoundationDatabaseStatus.Servicing;
            editableProperties.StatusReason = statusReason;
          }));
        service4.SetDatabaseStatus(vssRequestContext, databaseId1, TeamFoundationDatabaseStatus.Servicing, statusReason);
      }
      servicingContext.LogInfo("Registering the servicing job as running.");
      operations = new ServicingOperationSet(operationProvider, servicingJobData.ServicingOperations);
      short jobTotalStepCount = this.GetServicingJobTotalStepCount(operations);
      this.UpdateServicingJobDetail(requestContext.To(TeamFoundationHostType.Deployment), servicingJobData.ServicingHostId, jobId, servicingJobData.OperationClass, string.Join(",", servicingJobData.ServicingOperations), jobQueueTime, ServicingJobStatus.Running, ServicingJobResult.None, jobTotalStepCount);
      servicingContext.LogInfo("Registered the job.");
      if (servicingJobData.OperationClass == "UpgradeDatabase")
        this.StartUpgradeDatabase(servicingContext);
      if (vssRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        ;
      if ((servicingJobData.ServicingOptions & ServicingFlags.RequiresStoppedHost) != ServicingFlags.RequiresStoppedHost || hostProperties.Status == TeamFoundationServiceHostStatus.Stopped)
        return;
      servicingContext.LogInfo("Stopping host. Host ID: {0}", new object[1]
      {
        (object) hostProperties.Id
      });
      Stopwatch stopwatch = Stopwatch.StartNew();
      using (System.Timers.Timer timer = new System.Timers.Timer(20000.0))
      {
        timer.AutoReset = true;
        timer.Elapsed += (ElapsedEventHandler) ((_param1, _param2) =>
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Host '{0}' is not stopping", (object) hostProperties.Name);
          TeamFoundationTrace.Warning(str);
          servicingContext.Warn(str);
        });
        string reason;
        if (!servicingContext.Tokens.TryGetValue(ServicingTokenConstants.StopHostReason, out reason))
          reason = FrameworkResources.ProjectCollectionServicingInProgress();
        if (!service1.StopHost(vssRequestContext, servicingJobData.ServicingHostId, ServiceHostSubStatus.Servicing, reason, TimeSpan.FromMinutes(16.0)))
          throw new TeamFoundationServicingException(FrameworkResources.FailedToStopHost());
        servicingContext.LogInfo("Host has been stopped. Elapsed: {0} ms.", new object[1]
        {
          (object) stopwatch.ElapsedMilliseconds
        });
      }
    }

    private ISqlConnectionInfo GetConnectionInfoFromItems(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      string servicingItemName)
    {
      ISqlConnectionInfo connectionInfoFromItems = (ISqlConnectionInfo) null;
      object obj;
      if (servicingContext.Items.TryGetValue(servicingItemName, out obj))
      {
        if (obj is SqlConnectionInfoWrapper)
        {
          connectionInfoFromItems = ((SqlConnectionInfoWrapper) obj).ToSqlConnectionInfo(requestContext);
          servicingContext.Items[servicingItemName] = (object) connectionInfoFromItems;
        }
        else
          connectionInfoFromItems = (ISqlConnectionInfo) obj;
      }
      return connectionInfoFromItems;
    }

    private void InitializeDatabaseReplicationConfiguration(
      IVssRequestContext requestContext,
      ServicingContext servicingContext,
      string servicingItemName)
    {
      DatabaseReplicationConfiguration replicationConfiguration = (DatabaseReplicationConfiguration) null;
      object serializedObject;
      if (servicingContext.Items.TryGetValue(servicingItemName, out serializedObject))
      {
        if (serializedObject is string)
        {
          replicationConfiguration = TeamFoundationSerializationUtility.Deserialize<DatabaseReplicationConfigurationWrapper>((string) serializedObject).ToDatabaseReplicationConfiguration(requestContext);
          servicingContext.Items[servicingItemName] = (object) replicationConfiguration;
        }
        else
          replicationConfiguration = (DatabaseReplicationConfiguration) serializedObject;
      }
      if (replicationConfiguration == null)
        return;
      requestContext.To(TeamFoundationHostType.Deployment).GetService<IDatabaseReplicationService>().Configuration = replicationConfiguration;
    }

    private void SetServiceLevelToken(
      IVssRequestContext deploymentContext,
      ServicingContext servicingContext,
      ServicingJobData servicingJobData,
      ServicingOperationTarget operationTarget)
    {
      if (servicingContext.Tokens.ContainsKey(ServicingTokenConstants.ServiceLevel))
        return;
      string serviceLevel1;
      switch (operationTarget)
      {
        case ServicingOperationTarget.PartitionDatabase:
          serviceLevel1 = deploymentContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentContext, deploymentContext.ServiceHost.InstanceId).ServiceLevel;
          string serviceLevel2 = deploymentContext.ServiceHost.DeploymentServiceHost.ServiceHostInternal().ServiceLevel;
          if (!string.Equals(serviceLevel2, serviceLevel1, StringComparison.OrdinalIgnoreCase))
          {
            deploymentContext.Trace(94502, TraceLevel.Error, TeamFoundationServicingService.s_area, TeamFoundationServicingService.s_layer, "DeploymentServiceHost.ServiceHostInternal().ServiceLevel was not updated. Memory: {0}, Database: {1}", (object) serviceLevel2, (object) serviceLevel1);
            break;
          }
          break;
        case ServicingOperationTarget.DeploymentHost:
        case ServicingOperationTarget.OrganizationHost:
        case ServicingOperationTarget.CollectionHost:
          int databaseId = deploymentContext.GetService<TeamFoundationHostManagementService>().QueryServiceHostPropertiesCached(deploymentContext, servicingJobData.ServicingHostId).DatabaseId;
          serviceLevel1 = deploymentContext.GetService<TeamFoundationDatabaseManagementService>().GetDatabase(deploymentContext, databaseId, true).ServiceLevel;
          break;
        default:
          serviceLevel1 = deploymentContext.ServiceHost.DeploymentServiceHost.DatabaseProperties.ServiceLevel;
          break;
      }
      servicingContext.Tokens[ServicingTokenConstants.ServiceLevel] = serviceLevel1;
    }

    private void HandlePseudoOperations(
      IVssRequestContext deploymentContext,
      ServicingContext servicingContext,
      IServicingOperationProvider operationProvider,
      ServicingJobData servicingJobData,
      string serviceLevel,
      ServicingOperationTarget operationTarget,
      out bool onlineUpdate)
    {
      this.SetServiceLevelToken(deploymentContext, servicingContext, servicingJobData, operationTarget);
      onlineUpdate = true;
      bool flag = true;
      while (flag)
      {
        int length = servicingJobData.ServicingOperations.Length;
        flag = false;
        for (int index = 0; index < length; ++index)
        {
          string servicingOperation = servicingJobData.ServicingOperations[index];
          if (!string.IsNullOrEmpty(servicingOperation))
          {
            if (servicingOperation.StartsWith("*", StringComparison.OrdinalIgnoreCase))
            {
              if (servicingOperation.Equals(ServicingOperationConstants.BringToCurrentServiceLevel, StringComparison.Ordinal) || servicingOperation.Equals(ServicingOperationConstants.BringToCurrentServiceLevelAndAttach, StringComparison.Ordinal))
              {
                onlineUpdate = this.HandleBringToCurrentServiceLevel(deploymentContext, servicingContext, operationProvider, servicingJobData, serviceLevel, operationTarget, length, index, servicingOperation);
                flag = true;
                break;
              }
            }
            else if (servicingOperation.EndsWith(".*", StringComparison.OrdinalIgnoreCase))
            {
              string str = servicingOperation.Substring(0, servicingOperation.Length - 1);
              List<string> second = new List<string>();
              foreach (string servicingOperationName in operationProvider.GetServicingOperationNames())
              {
                if (servicingOperationName.Length > str.Length && servicingOperationName.StartsWith(str, StringComparison.OrdinalIgnoreCase))
                  second.Add(servicingOperationName);
              }
              servicingJobData.ServicingOperations = ((IEnumerable<string>) servicingJobData.ServicingOperations).Take<string>(index).Concat<string>((IEnumerable<string>) second).Concat<string>(((IEnumerable<string>) servicingJobData.ServicingOperations).Skip<string>(index + 1)).ToArray<string>();
              flag = true;
              break;
            }
          }
        }
      }
    }

    internal bool HandleBringToCurrentServiceLevel(
      IVssRequestContext deploymentContext,
      ServicingContext servicingContext,
      IServicingOperationProvider operationProvider,
      ServicingJobData servicingJobData,
      string currentServiceLevel,
      ServicingOperationTarget operationTarget,
      int operationCount,
      int operationIndex,
      string currentOperation)
    {
      bool currentServiceLevel1 = true;
      string token = servicingContext.Tokens[ServicingTokenConstants.ServiceLevel];
      Dictionary<string, ServiceLevel> serviceLevelMap1 = ServiceLevel.CreateServiceLevelMap(deploymentContext, currentServiceLevel);
      Dictionary<string, ServiceLevel> serviceLevelMap2 = ServiceLevel.CreateServiceLevelMap(deploymentContext, token);
      bool flag = false;
      foreach (string key in serviceLevelMap1.Keys)
      {
        ServiceLevel serviceLevel1 = serviceLevelMap1[key];
        ServiceLevel serviceLevel2;
        if (serviceLevelMap2.TryGetValue(key, out serviceLevel2))
          flag = ServiceLevel.CompareMajorVersions(serviceLevel2.MajorVersion, serviceLevel1.MajorVersion) > 0;
        if (flag)
          break;
      }
      string deltaOperationPrefixes;
      if (servicingContext.Tokens.TryGetValue(ServicingTokenConstants.DeltaOperationPrefixes, out deltaOperationPrefixes))
      {
        List<string> operationPrefixes = TeamFoundationServicingService.ConfigureDeltaOperationPrefixes(deltaOperationPrefixes);
        List<string> serviceLevelHosted = TeamFoundationServicingService.HandleBringToCurrentServiceLevelHosted(deploymentContext, servicingContext.OperationClass, operationProvider, operationTarget, serviceLevelMap1, serviceLevelMap2, operationPrefixes);
        servicingJobData.ServicingOperations = ((IEnumerable<string>) servicingJobData.ServicingOperations).Take<string>(operationIndex).Concat<string>((IEnumerable<string>) serviceLevelHosted).Concat<string>(((IEnumerable<string>) servicingJobData.ServicingOperations).Skip<string>(operationIndex + 1)).ToArray<string>();
      }
      else
      {
        ServiceLevel serviceLevel3 = serviceLevelMap1.SingleOrDefault<KeyValuePair<string, ServiceLevel>>().Value;
        ServiceLevel serviceLevel4 = serviceLevelMap2.SingleOrDefault<KeyValuePair<string, ServiceLevel>>().Value;
        if (!currentServiceLevel1 && (servicingJobData.ServicingOptions & ServicingFlags.HostMustExist) == ServicingFlags.HostMustExist)
          servicingJobData.ServicingOptions |= ServicingFlags.RequiresStoppedHost;
        List<string> stringList = new List<string>((IEnumerable<string>) OnPremiseServicingConstants.DeltaOperationPrefixes);
        ServiceLevel serviceLevel5 = serviceLevel3;
        if (!(serviceLevel4 >= serviceLevel5))
          throw new TeamFoundationServicingException("Database downgrade is not supported");
        List<string> deltaOperations1 = TeamFoundationServicingService.GetDeltaOperations(deploymentContext, operationProvider, operationTarget, serviceLevel3.ToString(), token, stringList.ToArray());
        if (currentOperation.Equals(ServicingOperationConstants.BringToCurrentServiceLevelAndAttach, StringComparison.Ordinal))
        {
          deltaOperations1.Add(ServicingOperationConstants.SchemaComparisonTest);
          deltaOperations1.Add(ServicingOperationConstants.AttachCollection);
        }
        List<string> deltaOperations2 = TeamFoundationServicingService.GetDeltaOperations(deploymentContext, operationProvider, ServicingOperationTarget.CollectionHost, serviceLevel3.ToString(), token, stringList.ToArray());
        List<string> second1 = new List<string>();
        List<string> second2 = new List<string>();
        second1.Add(ServicingOperationConstants.StartInstallUpdates);
        if (flag)
        {
          second1.Add(ServicingOperationConstants.PrepareForMajorUpgrade);
          second2.Add(ServicingOperationConstants.Repair);
        }
        if (deploymentContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          second2.Add(ServicingOperationConstants.RepairJobQueue);
          second2.Add(ServicingOperationConstants.CleanupSnapshot);
        }
        foreach (string deltaOperationPrefix in OnPremiseServicingConstants.DeltaOperationPrefixes)
        {
          string servicingOperation1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServicingOperationConstants.FinishUpgradeCollectionSuffix, (object) deltaOperationPrefix);
          ServicingOperation servicingOperation2 = operationProvider.GetServicingOperation(servicingOperation1);
          if (servicingOperation2 != null)
            second2.Add(servicingOperation2.Name);
        }
        second2.Add(ServicingOperationConstants.FinishInstallUpdates);
        second2.Add(ServicingOperationConstants.UpdatePartitionDatabaseProperties);
        servicingJobData.ServicingOperations = ((IEnumerable<string>) servicingJobData.ServicingOperations).Take<string>(operationIndex).Concat<string>((IEnumerable<string>) second1).Concat<string>((IEnumerable<string>) deltaOperations1).Concat<string>((IEnumerable<string>) deltaOperations2).Concat<string>(((IEnumerable<string>) servicingJobData.ServicingOperations).Skip<string>(operationIndex + 1).Concat<string>((IEnumerable<string>) second2)).ToArray<string>();
      }
      return currentServiceLevel1;
    }

    internal static List<string> HandleBringToCurrentServiceLevelHosted(
      IVssRequestContext deploymentContext,
      string operationClass,
      IServicingOperationProvider operationProvider,
      ServicingOperationTarget operationTarget,
      Dictionary<string, ServiceLevel> fromServiceLevelMap,
      Dictionary<string, ServiceLevel> toServiceLevelMap,
      List<string> operationPrefixes)
    {
      List<string> deltaOperations = TeamFoundationServicingService.GetDeltaOperations(deploymentContext, operationProvider, operationTarget, fromServiceLevelMap, toServiceLevelMap, operationPrefixes.ToArray());
      if (string.Equals(operationClass, "UpgradeHost", StringComparison.Ordinal))
      {
        foreach (string operationPrefix in operationPrefixes)
        {
          if (fromServiceLevelMap[ServicingOperationPrefixes.VisualStudioServices].Milestone != toServiceLevelMap[ServicingOperationPrefixes.VisualStudioServices].Milestone)
          {
            string servicingOperation1 = (string) null;
            switch (operationTarget)
            {
              case ServicingOperationTarget.OrganizationHost:
                servicingOperation1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServicingOperationConstants.FinishUpgradeOrganizationSuffix, (object) operationPrefix);
                break;
              case ServicingOperationTarget.CollectionHost:
                servicingOperation1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ServicingOperationConstants.FinishUpgradeCollectionSuffix, (object) operationPrefix);
                break;
            }
            ServicingOperation servicingOperation2 = operationProvider.GetServicingOperation(servicingOperation1);
            if (servicingOperation2 != null && servicingOperation2.Groups.Any<ServicingStepGroup>((Func<ServicingStepGroup, bool>) (g => g.Steps.Any<ServicingStep>((Func<ServicingStep, bool>) (step => !step.OnPremOnly)))))
              deltaOperations.Add(servicingOperation2.Name);
          }
        }
      }
      return deltaOperations;
    }

    internal static List<string> ConfigureDeltaOperationPrefixes(string deltaOperationPrefixes)
    {
      List<string> source = new List<string>();
      if (deltaOperationPrefixes != null)
        source.AddRange((IEnumerable<string>) ((IEnumerable<string>) deltaOperationPrefixes.Split(new char[2]
        {
          ';',
          ','
        }, StringSplitOptions.None)).ToList<string>());
      List<string> list = source.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();
      if (list.RemoveAll((Predicate<string>) (prefix => string.Equals(prefix, ServicingOperationPrefixes.NoVisualStudioServices, StringComparison.OrdinalIgnoreCase))) > 0 || list.Any<string>((Func<string, bool>) (prefix => string.Equals(prefix, ServicingOperationPrefixes.VisualStudioServices, StringComparison.OrdinalIgnoreCase))))
        return list;
      list.Insert(0, ServicingOperationPrefixes.VisualStudioServices);
      return list;
    }

    internal static string[] GetCollectionDowngradeOperations(
      IServicingOperationProvider operationProvider,
      string downgradeTargetServiceLevel,
      ITFLogger logger)
    {
      ServiceLevel targetServiceLevel = new ServiceLevel(downgradeTargetServiceLevel);
      string[] array1 = ((IEnumerable<string>) operationProvider.GetServicingOperationNames()).Where<string>((Func<string, bool>) (op => op.StartsWith("From", StringComparison.Ordinal))).ToArray<string>();
      logger.Info("Found {0} downgrade operation(s).", (object) array1.Length);
      ServiceLevel level1;
      string[] array2 = ((IEnumerable<string>) array1).Where<string>((Func<string, bool>) (op => ServiceLevel.TryGetServiceLevelFromOperation(op, "", out level1) && level1 > targetServiceLevel)).OrderByDescending<string, ServiceLevel>((Func<string, ServiceLevel>) (op =>
      {
        ServiceLevel level2;
        ServiceLevel.TryGetServiceLevelFromOperation(op, "", out level2);
        return level2;
      })).ToArray<string>();
      logger.Info("Downgrade operation(s): {0}", array2.Length == 0 ? (object) "None" : (object) string.Join(", ", array2));
      return array2;
    }

    private static IServicingResourceProvider CreateResourceProvider(
      DatabaseServicingProvider fallbackProvider)
    {
      string path1;
      using (IDisposableReadOnlyList<IAdminUtilityExtension> extensionsRaw = VssExtensionManagementService.GetExtensionsRaw<IAdminUtilityExtension>(VssExtensionManagementService.DefaultPluginPath))
        path1 = extensionsRaw.Count >= 1 ? extensionsRaw[0].GetTfsServicingFilesPath() : throw new TeamFoundationExtensionNotFoundException("IAdminUtilityExtension", VssExtensionManagementService.DefaultPluginPath);
      string str = Path.Combine(Path.Combine(path1, "Collection"), "ServicingFiles");
      IServicingResourceProvider fallbackResourceProvider = (IServicingResourceProvider) fallbackProvider;
      if (Directory.Exists(str))
        fallbackResourceProvider = (IServicingResourceProvider) new FileSystemResourceProvider(str, fallbackResourceProvider);
      return fallbackResourceProvider;
    }

    private static string GetDatabaseServiceLevel(
      ServicingContext servicingContext,
      ISqlConnectionInfo connectionInfo)
    {
      using (ExtendedAttributeComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<ExtendedAttributeComponent>(connectionInfo, 300))
      {
        string initialCatalog = componentRaw.InitialCatalog;
        string serviceLevels = componentRaw.ReadServiceLevelStamp();
        servicingContext.LogInfo("TFS_SERVICE_LEVEL is set to '{0}' on {1} database.", new object[2]
        {
          (object) serviceLevels,
          (object) initialCatalog
        });
        if (string.IsNullOrEmpty(serviceLevels))
          throw new TeamFoundationServicingException(FrameworkResources.StampNotSetException((object) initialCatalog));
        try
        {
          ServiceLevel.CreateServiceLevelMap(servicingContext.DeploymentRequestContext, serviceLevels);
          return serviceLevels;
        }
        catch (ServiceLevelException ex)
        {
          throw new TeamFoundationServicingException(FrameworkResources.StampNotSetException((object) initialCatalog), (Exception) ex);
        }
      }
    }

    private static string GetHostServiceLevel(ServicingContext servicingContext) => servicingContext.GetRequiredToken(ServicingTokenConstants.ServiceLevelFrom);

    private static void CheckExlusiveCollectionServicingLock(TeamFoundationLock servicingLock)
    {
      ArgumentUtility.CheckForNull<TeamFoundationLock>(servicingLock, nameof (servicingLock));
      if (servicingLock.IsDisposed)
        throw new ObjectDisposedException(nameof (servicingLock));
      if (servicingLock.LockMode != TeamFoundationLockMode.Exclusive)
        throw new ArgumentException(FrameworkResources.LockIsNotExclusive(), nameof (servicingLock));
      if (servicingLock.Resources.Length != 1)
        throw new ArgumentException(FrameworkResources.MultipleLocksTaken(), nameof (servicingLock));
      if (!string.Equals(servicingLock.Resources[0], FrameworkLockResources.CollectionServicing, StringComparison.Ordinal))
        throw new ArgumentException(FrameworkResources.LockIsNotCollectionServicing(), nameof (servicingLock));
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHost.InstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.ServicingServiceRequestContextHostMessage((object) this.m_serviceHost.InstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    private void ValidateHostedDeployment(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException("This method is not supported in on-premises TFS.");
    }

    private void EnsureCacheFresh(IVssRequestContext deploymentContext)
    {
      string newCookie;
      if (!this.ReloadServicingData(deploymentContext, this.m_servicingResourceCookie, out newCookie))
        return;
      deploymentContext.GetService<TeamFoundationServicingService.ResourceCacheService>().Clear(deploymentContext);
      deploymentContext.GetService<TeamFoundationServicingService.ServicingOperationCacheService>().Clear(deploymentContext);
      deploymentContext.GetService<TeamFoundationServicingService.ResourceToFileMappingCacheService>().Clear(deploymentContext);
      this.m_servicingResourceCookie = newCookie;
    }

    private void UpdateServicingJobDetail(
      IVssRequestContext deploymentContext,
      Guid hostId,
      Guid jobId,
      string operationClass,
      string operationsString,
      DateTime queueTime,
      ServicingJobStatus jobStatus,
      ServicingJobResult result,
      short totalStepCount = -1)
    {
      ServicingJobDetail jobDetail;
      using (ServicingComponent servicingComponent = ServicingComponent.Create(deploymentContext))
        jobDetail = servicingComponent.UpdateServicingJobDetail(hostId, jobId, operationClass, operationsString, queueTime, jobStatus, result, totalStepCount);
      if (jobDetail == null)
        return;
      deploymentContext.GetService<TeamFoundationTracingService>().TraceServicingJobDetail(jobDetail);
    }

    private void AddServicingJobDetails(
      IVssRequestContext deploymentContext,
      IEnumerable<ServicingJobDetail> servicingJobDetails)
    {
      List<ServicingJobDetail> servicingJobDetailList;
      using (ServicingComponent3 servicingComponent3 = (ServicingComponent3) ServicingComponent.Create(deploymentContext))
        servicingJobDetailList = servicingComponent3.AddServicingJobDetails(servicingJobDetails);
      if (servicingJobDetailList.Count <= 0)
        return;
      TeamFoundationTracingService service = deploymentContext.GetService<TeamFoundationTracingService>();
      foreach (ServicingJobDetail jobDetail in servicingJobDetailList)
        service.TraceServicingJobDetail(jobDetail);
    }

    internal bool ReloadServicingData(
      IVssRequestContext deploymentContext,
      string currentCookie,
      out string newCookie)
    {
      IVssRegistryService service = deploymentContext.GetService<IVssRegistryService>();
      newCookie = service.GetValue(deploymentContext, (RegistryQuery) FrameworkServerConstants.ServicingResourceCookie, false, (string) null);
      return !string.Equals(newCookie, currentCookie, StringComparison.Ordinal);
    }

    private short GetServicingJobTotalStepCount(ServicingOperationSet operations)
    {
      int seed = 0;
      foreach (ServicingOperation servicingOperation in operations.GetServicingOperations())
        seed = servicingOperation.Groups.Aggregate<ServicingStepGroup, int>(seed, (Func<int, ServicingStepGroup, int>) ((count, stepGroup) => count + stepGroup.Steps.Count));
      return (short) seed;
    }

    private class ServicingCacheService<TValue> : VssMemoryCacheService<string, TValue>
    {
      public ServicingCacheService()
        : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, TeamFoundationServicingService.s_cacheCleanupInterval)
      {
        this.InactivityInterval.Value = TeamFoundationServicingService.s_maxCacheInactivityAge;
      }

      public TValue Add(IVssRequestContext requestContext, string key, TValue value)
      {
        TValue obj = default (TValue);
        return !this.TryAdd(requestContext, key, value) && this.TryGetValue(requestContext, key, out obj) ? obj : value;
      }
    }

    private class ResourceCacheService : TeamFoundationServicingService.ServicingCacheService<byte[]>
    {
    }

    private class ResourceToFileMappingCacheService : 
      TeamFoundationServicingService.ServicingCacheService<int>
    {
    }

    private class ServicingOperationCacheService : 
      TeamFoundationServicingService.ServicingCacheService<ServicingOperation>
    {
    }
  }
}
