// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.HostMigration.AdHocJobMover
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.HostMigration
{
  public static class AdHocJobMover
  {
    private static readonly string s_area = "TargetHostMigration";
    private static readonly string s_layer = nameof (AdHocJobMover);
    private static readonly int s_CopyAdhocJobGetDefinitionsHostMigrationTracepoint = 15288132;
    private static readonly int s_CopyAdhocJobQueuedAdhocJobsHostMigrationTracepoint = 15288131;
    private const int c_WaitForJAJobProcessing = 1000;
    private const int c_MaxRetriesWait = 15;

    public static List<Guid> GetAdHocJobDefinitionsFromList(
      IVssRequestContext deploymentContext,
      IList<Guid> jobIds,
      Guid hostId)
    {
      deploymentContext.CheckDeploymentRequestContext();
      TeamFoundationServiceHostProperties serviceHostProperties = deploymentContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(deploymentContext, hostId);
      if (serviceHostProperties.HostType == TeamFoundationHostType.Deployment)
        throw new ArgumentException(string.Format("GetAdHocJobDefinitionsFromList called on deployment host {0}", (object) hostId));
      using (JobQueueComponent component = deploymentContext.CreateComponent<JobQueueComponent>())
      {
        foreach (TeamFoundationJobQueueEntry foundationJobQueueEntry in component.QueryJobQueueForOneHost(hostId, (IEnumerable<Guid>) null))
          deploymentContext.Trace(AdHocJobMover.s_CopyAdhocJobGetDefinitionsHostMigrationTracepoint, TraceLevel.Info, AdHocJobMover.s_area, AdHocJobMover.s_layer, string.Format("Queue Entry, HostId: {0}, JobId: {1}, JobLastResult: {2}, Priority: {3}, ExecutionStartTime: {4}", (object) hostId, (object) foundationJobQueueEntry.JobId, (object) foundationJobQueueEntry.JobLastResult, (object) foundationJobQueueEntry.Priority, (object) foundationJobQueueEntry.ExecutionStartTime));
      }
      int databaseId = serviceHostProperties.DatabaseId;
      DatabasePartition databasePartition;
      using (DatabasePartitionComponent databaseComponent = deploymentContext.GetService<TeamFoundationResourceManagementService>().CreateDatabaseComponent<DatabasePartitionComponent>(deploymentContext, databaseId, (string) null))
        databasePartition = databaseComponent.QueryPartition(serviceHostProperties.Id);
      List<TeamFoundationJobDefinition> foundationJobDefinitionList;
      using (JobDefinitionComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<JobDefinitionComponent>(deploymentContext.GetService<ITeamFoundationDatabaseManagementService>().GetDatabase(deploymentContext, databaseId)))
      {
        componentRaw.PartitionId = databasePartition.PartitionId;
        foundationJobDefinitionList = componentRaw.QueryJobs((IEnumerable<Guid>) jobIds);
      }
      foreach (TeamFoundationJobDefinition foundationJobDefinition in foundationJobDefinitionList)
      {
        if (foundationJobDefinition != null)
          deploymentContext.Trace(AdHocJobMover.s_CopyAdhocJobGetDefinitionsHostMigrationTracepoint, TraceLevel.Info, AdHocJobMover.s_area, AdHocJobMover.s_layer, string.Format("Partition Queried Job Def, JobId: {0}, Name: {1}, ExtensionName: {2}, RunOnce: {3}, Queried: {4}, IsTemplateJob: {5}", (object) foundationJobDefinition.JobId, (object) foundationJobDefinition.Name, (object) foundationJobDefinition.ExtensionName, (object) foundationJobDefinition.RunOnce, (object) foundationJobDefinition.Queried, (object) foundationJobDefinition.IsTemplateJob));
      }
      List<Guid> definitionsFromList = new List<Guid>(jobIds.Count);
      using (JobTemplateComponent component = deploymentContext.CreateComponent<JobTemplateComponent>())
      {
        foreach (int index in Enumerable.Range(0, jobIds.Count))
        {
          TeamFoundationJobDefinitionTemplate jobTemplate = component.QueryJobTemplate(jobIds[index], false, out long _);
          TeamFoundationJobDefinition foundationJobDefinition = foundationJobDefinitionList[index];
          string str = "";
          if (jobTemplate != null)
            str = str + "Job Def Template " + jobTemplate.ToString() + ". ";
          if (foundationJobDefinition != null)
            str += string.Format("Partition Job Def JobId={0}, Name={1}, RunOnce={2}, Queried={3}", (object) foundationJobDefinition.JobId, (object) foundationJobDefinition.Name, (object) foundationJobDefinition.RunOnce, (object) foundationJobDefinition.Queried);
          deploymentContext.Trace(AdHocJobMover.s_CopyAdhocJobGetDefinitionsHostMigrationTracepoint, TraceLevel.Info, AdHocJobMover.s_area, AdHocJobMover.s_layer, string.Format("Checking job id {0}. {1}", (object) jobIds[index], (object) str));
          if (jobTemplate == null || jobTemplate.AllowTemplateOverride() && foundationJobDefinition != null)
          {
            if (foundationJobDefinition == null)
            {
              deploymentContext.Trace(AdHocJobMover.s_CopyAdhocJobGetDefinitionsHostMigrationTracepoint, TraceLevel.Info, nameof (AdHocJobMover), nameof (GetAdHocJobDefinitionsFromList), string.Format("Couldn't find Job Def {0} for host {1} adding job and letting target job runner handle it", (object) jobIds[index], (object) serviceHostProperties.Id));
              definitionsFromList.Add(jobIds[index]);
            }
            else if (foundationJobDefinition.Schedule.Count == 0)
            {
              deploymentContext.Trace(AdHocJobMover.s_CopyAdhocJobGetDefinitionsHostMigrationTracepoint, TraceLevel.Info, nameof (AdHocJobMover), nameof (GetAdHocJobDefinitionsFromList), "Partition Job Def has no schedule adding to jobs to be transferred");
              definitionsFromList.Add(jobIds[index]);
            }
          }
          else if (jobTemplate.Schedules.Count == 0)
          {
            deploymentContext.Trace(AdHocJobMover.s_CopyAdhocJobGetDefinitionsHostMigrationTracepoint, TraceLevel.Info, nameof (AdHocJobMover), nameof (GetAdHocJobDefinitionsFromList), "Job Def Template has no schedule adding to jobs to be transferred");
            definitionsFromList.Add(jobIds[index]);
          }
        }
      }
      return definitionsFromList;
    }

    public static List<AdHocJobInfo> GetQueuedAdHocJobsForHost(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      requestContext.CheckDeploymentRequestContext();
      bool flag1 = true;
      int num = 0;
      while (flag1)
      {
        try
        {
          List<TeamFoundationJobQueueEntry> source1;
          using (JobQueueComponent component = requestContext.CreateComponent<JobQueueComponent>())
            source1 = component.QueryJobQueueForOneHost(hostId, (IEnumerable<Guid>) null);
          requestContext.Trace(AdHocJobMover.s_CopyAdhocJobQueuedAdhocJobsHostMigrationTracepoint, TraceLevel.Info, AdHocJobMover.s_area, AdHocJobMover.s_layer, string.Format("Getting queued jobs for host {0} found {1}", (object) hostId, (object) source1.Count));
          foreach (TeamFoundationJobQueueEntry foundationJobQueueEntry in source1)
            requestContext.Trace(AdHocJobMover.s_CopyAdhocJobQueuedAdhocJobsHostMigrationTracepoint, TraceLevel.Info, AdHocJobMover.s_area, AdHocJobMover.s_layer, string.Format("Queue Entry HostId: {0}, JobId: {1} ,QueueTime: {2},State: {3}, ExecutionStartTime: {4}, JobLastResult:{5}, Priority: {6}", (object) hostId, (object) foundationJobQueueEntry?.JobId, (object) foundationJobQueueEntry?.QueueTime, (object) foundationJobQueueEntry?.State, (object) foundationJobQueueEntry?.ExecutionStartTime, (object) foundationJobQueueEntry?.JobLastResult.ToString(), (object) foundationJobQueueEntry?.Priority));
          List<Guid> definitionsFromList = AdHocJobMover.GetAdHocJobDefinitionsFromList(requestContext, (IList<Guid>) source1.Select<TeamFoundationJobQueueEntry, Guid>((Func<TeamFoundationJobQueueEntry, Guid>) (queueEntry => queueEntry.JobId)).ToList<Guid>(), hostId);
          List<AdHocJobInfo> source2 = new List<AdHocJobInfo>(definitionsFromList.Count);
          List<TeamFoundationJobQueueEntry>.Enumerator enumerator = source1.GetEnumerator();
          enumerator.MoveNext();
          foreach (Guid guid in definitionsFromList)
          {
            while (enumerator.Current.JobId != guid)
              enumerator.MoveNext();
            bool flag2;
            switch (enumerator.Current.State)
            {
              case TeamFoundationJobState.Dormant:
                flag2 = true;
                break;
              case TeamFoundationJobState.QueuedScheduled:
                flag2 = false;
                break;
              default:
                throw new InvalidOperationException(string.Format("Tried to move ad-hoc job {0} for host {1} in state {2}", (object) guid, (object) enumerator.Current.JobSource, (object) enumerator.Current.State));
            }
            requestContext.Trace(AdHocJobMover.s_CopyAdhocJobQueuedAdhocJobsHostMigrationTracepoint, TraceLevel.Info, AdHocJobMover.s_area, AdHocJobMover.s_layer, string.Format("Adding JobId: {0}, JobPriority: {1}, IsDormant: {2}", (object) guid, (object) enumerator.Current.Priority, (object) flag2));
            source2.Add(new AdHocJobInfo()
            {
              JobId = guid,
              JobPriority = enumerator.Current.Priority,
              IsDormant = flag2,
              QueueTime = enumerator.Current.QueueTime
            });
          }
          return source2.OrderBy<AdHocJobInfo, DateTime>((Func<AdHocJobInfo, DateTime>) (x => x.QueueTime)).ToList<AdHocJobInfo>();
        }
        catch (InvalidOperationException ex)
        {
          if (num >= 15)
            throw ex;
          requestContext.TraceAlways(AdHocJobMover.s_CopyAdhocJobQueuedAdhocJobsHostMigrationTracepoint, TraceLevel.Error, AdHocJobMover.s_area, AdHocJobMover.s_layer, ex.Message);
          Thread.Sleep(1000);
          flag1 = true;
          ++num;
        }
      }
      return (List<AdHocJobInfo>) null;
    }
  }
}
