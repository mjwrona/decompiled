// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationJobReportingJobNameCache
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationJobReportingJobNameCache
  {
    private Dictionary<Guid, string> m_jobs = new Dictionary<Guid, string>();
    private bool m_alwaysReadInstanceJobNames;
    private bool m_alwaysReadCollectionJobNames;
    private const string c_servicingJobExtensionName = "Microsoft.TeamFoundation.JobService.Extensions.Core.ServicingJobExtension";

    internal TeamFoundationJobReportingJobNameCache(
      bool alwaysReadInstanceJobNames,
      bool alwaysReadCollectionJobNames)
      : this()
    {
      this.m_alwaysReadInstanceJobNames = alwaysReadInstanceJobNames;
      this.m_alwaysReadCollectionJobNames = alwaysReadCollectionJobNames;
    }

    internal TeamFoundationJobReportingJobNameCache()
    {
      lock (this.m_jobs)
      {
        this.m_jobs.Add(new Guid("F7CBA1A0-51AE-42E0-BEE9-67BCBD800C16"), JobNameConstants.HostingAccountExpiration);
        this.m_jobs.Add(new Guid("0A701BC2-25FB-4B7B-BEC7-D5FE4046CBA6"), JobNameConstants.HostedBuildResultLogging);
        this.m_jobs.Add(new Guid("17FCCB5A-00F1-4C23-ADD0-B8CC7F310D85"), JobNameConstants.AccountPreCreation);
        this.m_jobs.Add(new Guid("8FC76967-DA5D-4D53-937B-E3EFC6AF0FCB"), JobNameConstants.JobHistoryCleanup);
        this.m_jobs.Add(new Guid("47EC1DEB-483A-441F-8F27-2E739FB69A2F"), JobNameConstants.RuntimeTelemetryDataCollector);
        this.m_jobs.Add(new Guid("63A78C70-8FE0-4743-BA2D-A00CF8C20FDF"), JobNameConstants.SecurityIdentityCleanup);
        this.m_jobs.Add(new Guid("AD9C60AE-5BF0-4AC0-BE56-F2988088859A"), JobNameConstants.TeamoundationServerActivityLoggingAdministration);
        this.m_jobs.Add(new Guid("3C601170-8080-4A7D-B8E7-48E991F80C4A"), JobNameConstants.TeamFoundationServerApplicationTierMaintanence);
        this.m_jobs.Add(new Guid("81A90003-CA23-4E55-9320-F956F62A2477"), JobNameConstants.TeamFoundationServerDataMaintanence);
        this.m_jobs.Add(new Guid("AF87E4CD-D421-4A24-BF16-D3A68371703E"), JobNameConstants.TeamFoundationServerDatabaseOptimization);
        this.m_jobs.Add(new Guid("C03C29A7-00AE-4E02-94BC-18ACA80E2EA2"), JobNameConstants.TeamFoundationServerImageCleanup);
        this.m_jobs.Add(new Guid("68D12C31-4894-49C3-8E12-4D3E954C98E7"), JobNameConstants.TeamFoundationServerOnDemandIdentitySynchronization);
        this.m_jobs.Add(new Guid("544DD581-F72A-45A9-8DE0-8CD3A5F29DFE"), JobNameConstants.TeamFoundationServerPeriodicIdentitySynchronization);
        this.m_jobs.Add(new Guid("B1516502-4633-432B-BDB3-74C802C5F2B7"), JobNameConstants.TeamFoundationServerSendMailJob);
        this.m_jobs.Add(new Guid("C372F50A-C250-4FE8-A312-297AC41D63DC"), JobNameConstants.BuildInformationCleanup);
        this.m_jobs.Add(new Guid("9ADE9F0F-93E2-4C12-952D-B9FA525D1B50"), JobNameConstants.CleanupDiscussionDatabase);
        this.m_jobs.Add(new Guid("6E4FF854-4669-47BB-BF5E-3AB54BDF188B"), JobNameConstants.CleanupServicingTables);
        this.m_jobs.Add(new Guid("D4AD074E-592B-4B59-9EAE-2E4DBB388DC0"), JobNameConstants.CleanupTestManagementDatabase);
        this.m_jobs.Add(new Guid("DE78FC95-CDB0-4AA6-B623-C26F642C0520"), JobNameConstants.CleanupTestImpactDatabase);
        this.m_jobs.Add(new Guid("35E5AC87-0ABC-47B8-AD51-AEA0F88CAD91"), JobNameConstants.SoftDeleteLegacyTestImpactDatabase);
        this.m_jobs.Add(new Guid("B3E84DBD-A92A-470C-A847-1787C00BF2E3"), JobNameConstants.CleanupLegacyTestImpactDatabase);
        this.m_jobs.Add(new Guid("4C5CCBF7-68F4-4D8B-AEFF-26C91EFB1E19"), JobNameConstants.CleanupDistributedTestRunsJob);
        this.m_jobs.Add(new Guid("B41EC799-1F3F-4A86-9C9D-3148BAFF89E4"), JobNameConstants.LabManagerOperationCleanup);
        this.m_jobs.Add(new Guid("CFD89967-91DC-448E-BF3A-A8793FFA846B"), JobNameConstants.LabManagerOperationRecovery);
        this.m_jobs.Add(new Guid("077785F6-AB53-46C3-9519-0BC8CDFD8AF0"), JobNameConstants.MessageQueueCleanup);
        this.m_jobs.Add(new Guid("18BAD1CF-7C6A-42EE-AA5E-5565FC8D3271"), JobNameConstants.RepopulateDynamicSuites);
        this.m_jobs.Add(new Guid("1A832573-6FA1-47F3-8E97-69801A02C1CA"), JobNameConstants.SynchronizeTestCases);
        this.m_jobs.Add(new Guid("747665D7-CEDD-4AF8-85A7-693EF5760024"), JobNameConstants.TeamFoundationServerCoverageAnalysis);
        this.m_jobs.Add(new Guid("1BC83577-2640-44A1-ADAF-57DD8CD74912"), JobNameConstants.VersionControlAdministration);
        this.m_jobs.Add(new Guid("3B74723B-5E93-4BC2-BD52-E39734F0E677"), JobNameConstants.VersionControlCodeChurn);
        this.m_jobs.Add(new Guid("E4749644-FA65-40A4-8450-1BA0A2AE6B87"), JobNameConstants.VersionControlDeltaProcessing);
        this.m_jobs.Add(new Guid("6215A651-285A-41B4-A43C-7B6257E8D591"), JobNameConstants.VersionControlStatisticsUpdate);
        this.m_jobs.Add(new Guid("7A9CF26E-8164-4F16-BB14-16359588FD97"), JobNameConstants.WorkItemTrackingAdministration);
        this.m_jobs.Add(new Guid("ACB90164-9E69-45AA-9C38-27BCBB514281"), JobNameConstants.WorkItemTrackingIdentityProcessing);
        this.m_jobs.Add(new Guid("69AD5827-6346-4B08-B29D-2EE8BE361F85"), JobNameConstants.WorkItemTrackingIntegrationSynchronization);
        this.m_jobs.Add(new Guid("1E288A09-8D53-4A56-9D1E-3B3547B3F25F"), JobNameConstants.WorkItemTrackingReferencedIdentitiesUpdate);
        this.m_jobs.Add(new Guid("79233EA9-5A82-4765-8815-88843CB7A993"), JobNameConstants.WorkItemTrackingRemoveunusedconstants);
        this.m_jobs.Add(new Guid("5F38F579-60C6-46A7-A599-8B8DB7D0565D"), JobNameConstants.WorkItemTrackingRemoveOrphanAttachments);
        this.m_jobs.Add(new Guid("B19DDD28-9A95-42E2-9697-966FD822F1CD"), JobNameConstants.TeamFoundationServerCleanupJob);
        this.m_jobs.Add(new Guid("A4804DCF-4BB6-4109-B61C-E59C2E8A9FF7"), JobNameConstants.TeamFoundationServerEventProcessing);
        this.m_jobs.Add(new Guid("7A3E559E-8EB7-4E90-A4F7-B7A2515D52B9"), JobNameConstants.TeamFoundationServerFrameworkFileServiceCleanup);
        this.m_jobs.Add(new Guid("780538E3-1338-46A9-B23A-D360D7A88533"), JobNameConstants.TeamFoundationServerUserActivityJob);
        this.m_jobs.Add(new Guid("7F90698C-6AEC-4F4B-882A-E836CAC18457"), JobNameConstants.ValidateJobAgentPluginsJob);
      }
    }

    public string GetJobName(IVssRequestContext requestContext, Guid id) => this.GetJobNameInternal(requestContext, id);

    internal string GetJobNameInternal(IVssRequestContext requestContext, Guid id)
    {
      lock (this.m_jobs)
      {
        if (this.m_jobs.ContainsKey(id))
          return this.m_jobs[id];
        if (!this.ShouldLookupJobName(requestContext))
        {
          this.m_jobs.Add(id, id.ToString());
        }
        else
        {
          TeamFoundationJobService service = requestContext.GetService<TeamFoundationJobService>();
          TeamFoundationJobDefinition jobDef = service.QueryJobDefinition(requestContext, id) ?? this.GetCollectionJobDefinition(requestContext, service, id);
          if (jobDef == null)
            this.m_jobs.Add(id, id.ToString());
          else if (!string.IsNullOrEmpty(jobDef.Name) && !this.IsServicingJob(jobDef))
            this.m_jobs.Add(id, jobDef.Name);
          else if (this.IsServicingJob(jobDef))
            this.m_jobs.Add(id, this.GetServicingJobName(jobDef));
          else
            this.m_jobs.Add(id, jobDef.ExtensionName);
        }
      }
      return this.m_jobs[id];
    }

    private TeamFoundationJobDefinition GetCollectionJobDefinition(
      IVssRequestContext requestContext,
      TeamFoundationJobService jobService,
      Guid jobId)
    {
      List<TeamFoundationJobHistoryEntry> foundationJobHistoryEntryList = jobService.QueryJobHistory(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        jobId
      });
      jobService.QueryJobDefinition(requestContext, jobId);
      Guid jobSource;
      if (foundationJobHistoryEntryList == null || foundationJobHistoryEntryList.Count == 0)
      {
        TeamFoundationJobReportingService service = requestContext.GetService<TeamFoundationJobReportingService>();
        List<TeamFoundationJobReportingHistory> reportingHistoryList = service.QueryHistory(requestContext, 1, new Guid?(jobId), new int?());
        if (reportingHistoryList == null || reportingHistoryList.Count == 0)
        {
          reportingHistoryList = service.QueryHistory(requestContext, 1, new Guid?(jobId), new int?(0));
          if (reportingHistoryList == null || reportingHistoryList.Count == 0)
            return (TeamFoundationJobDefinition) null;
        }
        jobSource = reportingHistoryList[0].JobSource;
      }
      else
        jobSource = foundationJobHistoryEntryList[0].JobSource;
      try
      {
        using (IVssRequestContext vssRequestContext = requestContext.GetService<TeamFoundationHostManagementService>().BeginRequest(requestContext, jobSource, RequestContextType.SystemContext, true, false))
          return vssRequestContext.GetService<TeamFoundationJobService>().QueryJobDefinition(vssRequestContext, jobId);
      }
      catch (HostShutdownException ex)
      {
        return (TeamFoundationJobDefinition) null;
      }
    }

    private bool ShouldLookupJobName(IVssRequestContext requestContext) => (requestContext.ServiceHost.HostType & TeamFoundationHostType.ProjectCollection) == TeamFoundationHostType.ProjectCollection && this.m_alwaysReadCollectionJobNames || (requestContext.ServiceHost.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment || (requestContext.ServiceHost.HostType & TeamFoundationHostType.Application) == TeamFoundationHostType.Application;

    private string GetServicingJobName(TeamFoundationJobDefinition jobDef)
    {
      if (jobDef.Data == null)
        return jobDef.ExtensionName;
      try
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        using (StringReader input = new StringReader(jobDef.JobDataToString()))
        {
          using (XmlReader reader = XmlReader.Create((TextReader) input, settings))
            xmlDocument.Load(reader);
        }
        return string.Format("ServicingJobData:{0}", (object) xmlDocument.SelectSingleNode("ServicingJobData").Attributes["class"].Value);
      }
      catch (Exception ex)
      {
        return jobDef.ExtensionName;
      }
    }

    private bool IsServicingJob(TeamFoundationJobDefinition jobDef) => jobDef != null && !string.IsNullOrEmpty(jobDef.Name) && string.Equals(jobDef.ExtensionName, "Microsoft.TeamFoundation.JobService.Extensions.Core.ServicingJobExtension", StringComparison.InvariantCultureIgnoreCase);
  }
}
