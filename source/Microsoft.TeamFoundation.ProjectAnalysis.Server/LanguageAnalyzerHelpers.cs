// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.LanguageAnalyzerHelpers
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.ProjectAnalysis.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  public static class LanguageAnalyzerHelpers
  {
    public static Dictionary<RepositoryType, string> AnalyzerJobName = new Dictionary<RepositoryType, string>()
    {
      {
        RepositoryType.TfsGit,
        nameof (TfsGitLanguageMetadataAnalyzerJob)
      },
      {
        RepositoryType.Tfvc,
        nameof (TfvcLanguageMetadataAnalyzerJob)
      }
    };
    private const string TfsGitLanguageMetadataAnalyzerJob = "TfsGitLanguageMetadataAnalyzerJob";
    private const string TfvcLanguageMetadataAnalyzerJob = "TfvcLanguageMetadataAnalyzerJob";
    private const string c_layer = "LanguageAnalyzerHelpers";

    public static void QueueLanguageMetadataAnalyzerJob(
      this IVssRequestContext requestContext,
      IRepositoryDescriptor repositoryDescriptor,
      string submittedFrom,
      int? lastFileCount = null,
      DateTime? lastUpdatedTime = null)
    {
      string metadataAnalyzerJobName = LanguageAnalyzerHelpers.GetLanguageMetadataAnalyzerJobName(repositoryDescriptor.Type);
      Guid metadataAnalyzerJobId = LanguageAnalyzerHelpers.GetLanguageMetadataAnalyzerJobId(repositoryDescriptor.Id, metadataAnalyzerJobName);
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition foundationJobDefinition = service.QueryJobDefinition(requestContext, metadataAnalyzerJobId);
      if (foundationJobDefinition == null || LanguageAnalyzerHelpers.IsJobDefinitionOutOfDate(requestContext, foundationJobDefinition.Data))
      {
        foundationJobDefinition = new TeamFoundationJobDefinition(metadataAnalyzerJobId, FormattableString.Invariant(FormattableStringFactory.Create("{0}, repo= {1}", (object) metadataAnalyzerJobName, (object) repositoryDescriptor.Id)), "Microsoft.TeamFoundation.ProjectAnalysis.Server.Extensions." + metadataAnalyzerJobName, LanguageMetadataAnalyzerJobData.ToXml(repositoryDescriptor.ProjectId, repositoryDescriptor.Id), TeamFoundationJobEnabledState.Enabled, false, JobPriorityClass.Normal);
        service.UpdateJobDefinitions(requestContext, (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
      }
      service.QueueJobsNow(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobDefinition.ToJobReference()
      });
      requestContext.GetService<CustomerIntelligenceService>().PublishJobQueued(requestContext, metadataAnalyzerJobId, submittedFrom, repositoryDescriptor.Type, foundationJobDefinition.Data.OuterXml, lastFileCount.HasValue, lastFileCount, lastUpdatedTime);
    }

    private static bool IsJobDefinitionOutOfDate(IVssRequestContext requestContext, XmlNode xmlData)
    {
      if (xmlData == null)
        return true;
      LanguageMetadataAnalyzerJobData data;
      try
      {
        data = LanguageMetadataAnalyzerJobData.FromXml(xmlData);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15281507, "ProjectAnalysisService", nameof (LanguageAnalyzerHelpers), ex);
        data = (LanguageMetadataAnalyzerJobData) null;
      }
      if (data != null)
        return !data.IsJobDataUpToDate();
      return true;
    }

    private static Guid GetLanguageMetadataAnalyzerJobId(Guid repositoryId, string jobName)
    {
      byte[] bytes = new UTF8Encoding(false, true).GetBytes(repositoryId.ToString("N") + ":" + jobName);
      byte[] hash = SHA1.Create().ComputeHash(bytes);
      Array.Resize<byte>(ref hash, 16);
      return new Guid(hash);
    }

    private static string GetLanguageMetadataAnalyzerJobName(RepositoryType repositoryType) => LanguageAnalyzerHelpers.AnalyzerJobName[repositoryType];

    public static void PublishHistoricalRecordIfNeeded(
      this CustomerIntelligenceService ciService,
      IVssRequestContext requestContext,
      LanguageMetadataRecord record)
    {
      int repositoryThreshold = requestContext.GetService<IVssRegistryService>().GetSmallRepositoryThreshold(requestContext);
      if (record.FileCount <= repositoryThreshold)
        return;
      ciService.PublishHistoricalRecord(requestContext, record);
    }

    public static int GetSmallRepositoryThreshold(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetValue<int>(requestContext, (RegistryQuery) ProjectAnalysisConstants.Settings.SmallRepoKey, true, 30000);
    }

    public static int GetMediumRepositoryThreshold(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetValue<int>(requestContext, (RegistryQuery) ProjectAnalysisConstants.Settings.MidSizedRepoKey, true, 150000);
    }

    public static int GetMediumUpdateFrequency(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetValue<int>(requestContext, (RegistryQuery) ProjectAnalysisConstants.Settings.MidSizedRepoAnalyseFreqKey, true, 1440);
    }

    public static int GetLargeUpdateFrequency(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext)
    {
      return registryService.GetValue<int>(requestContext, (RegistryQuery) ProjectAnalysisConstants.Settings.LargeSizedRepoAnalyseFreqKey, true, 4320);
    }

    public static bool TryGetTfvcProject(
      this IVssRequestContext requestContext,
      Guid projectId,
      out ProjectInfo project)
    {
      project = (ProjectInfo) null;
      try
      {
        IProjectService service = requestContext.GetService<IProjectService>();
        if (string.Equals(service.GetProjectProperties(requestContext, projectId, "System.SourceControlTfvcEnabled").FirstOrDefault<ProjectProperty>()?.Value?.ToString(), bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
          project = service.GetProject(requestContext, projectId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15281510, "ProjectAnalysisService", nameof (LanguageAnalyzerHelpers), ex);
        project = (ProjectInfo) null;
      }
      return project != null;
    }

    public static DateTime GetNextUpdateTimeForTfsGitRepository(
      this IVssRequestContext requestContext,
      DateTime lastUpdatedTime,
      int fileCount)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int repositoryThreshold1 = service.GetSmallRepositoryThreshold(requestContext);
      DateTime tfsGitRepository;
      if (fileCount < repositoryThreshold1)
      {
        tfsGitRepository = DateTime.MinValue;
      }
      else
      {
        int repositoryThreshold2 = service.GetMediumRepositoryThreshold(requestContext);
        int num = fileCount >= repositoryThreshold2 ? service.GetLargeUpdateFrequency(requestContext) : service.GetMediumUpdateFrequency(requestContext);
        tfsGitRepository = lastUpdatedTime.AddMinutes((double) num);
      }
      return tfsGitRepository;
    }
  }
}
