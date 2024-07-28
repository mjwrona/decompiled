// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PipelineDeltaCoverageProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class PipelineDeltaCoverageProvider : IPipelineDeltaCoverageProvider
  {
    public async Task<PipelineInstanceReference> ComputeAndUploadCoverageChanges(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      IList<CoverageSummary> currentBuildCoverageSummaryList,
      CoverageScope coverageScope)
    {
      PipelineInstanceReference deltaPipelineInstanceReference = (PipelineInstanceReference) null;
      List<FileCoverageChange> fileCoverageChangeList = new List<FileCoverageChange>();
      int summaryBatchSize = new CoverageConfiguration().GetPipelineCoverageChangeSummaryBatchSize(requestContext.RequestContext);
      int newFilesCount = 0;
      int deletedFilesCount = 0;
      int coverageChangedFilesCount = 0;
      int coverageUnchangedFilesCount = 0;
      int index1 = 0;
      int previousBuildTotalEntries = 0;
      string continuationToken = (string) null;
      IPipelineCoverageService pipelineCoverageService = requestContext.RequestContext.GetService<IPipelineCoverageService>();
      Dictionary<string, object> ciData = new Dictionary<string, object>();
      CommonHelper.PopulateCiDataWithPipelineContext(pipelineContext, ciData);
      ciData.Add("CurrentBuildTotalEntries", (object) currentBuildCoverageSummaryList.Count<CoverageSummary>());
      try
      {
        BuildHttpClient client = requestContext.RequestContext.GetClient<BuildHttpClient>();
        List<int> intList = new List<int>()
        {
          Convert.ToInt32((object) pipelineContext.PipelineId)
        };
        Guid projectId = pipelineContext.ProjectId;
        List<int> definitions = intList;
        BuildResult? nullable = new BuildResult?(BuildResult.Succeeded);
        string branchName1 = pipelineContext.BranchName;
        DateTime? minTime = new DateTime?();
        DateTime? maxTime = new DateTime?();
        BuildReason? reasonFilter = new BuildReason?();
        BuildStatus? statusFilter = new BuildStatus?();
        BuildResult? resultFilter = nullable;
        int? top = new int?();
        int? maxBuildsPerDefinition = new int?();
        QueryDeletedOption? deletedFilter = new QueryDeletedOption?();
        BuildQueryOrder? queryOrder = new BuildQueryOrder?();
        string branchName2 = branchName1;
        CancellationToken cancellationToken = new CancellationToken();
        List<Microsoft.TeamFoundation.Build.WebApi.Build> result = client.GetBuildsAsync(projectId, (IEnumerable<int>) definitions, (IEnumerable<int>) null, (string) null, minTime, maxTime, (string) null, reasonFilter, statusFilter, resultFilter, (IEnumerable<string>) null, (IEnumerable<string>) null, top, (string) null, maxBuildsPerDefinition, deletedFilter, queryOrder, branchName2, (IEnumerable<int>) null, (string) null, (string) null, (object) null, cancellationToken).Result;
        Microsoft.TeamFoundation.Build.WebApi.Build build = result != null ? result.Where<Microsoft.TeamFoundation.Build.WebApi.Build>((Func<Microsoft.TeamFoundation.Build.WebApi.Build, bool>) (x => x.Id < pipelineContext.Id)).FirstOrDefault<Microsoft.TeamFoundation.Build.WebApi.Build>() : (Microsoft.TeamFoundation.Build.WebApi.Build) null;
        deltaPipelineInstanceReference = new PipelineInstanceReference()
        {
          Id = build == null ? 0 : build.Id,
          Url = build?.Url
        };
        ciData.Add("PreviousPipelineInstance", (object) deltaPipelineInstanceReference.Id);
        IList<CoverageSummary> source = (IList<CoverageSummary>) new List<CoverageSummary>();
        if (build != null)
          source = pipelineCoverageService.GetCoverageSummaryList(requestContext, pipelineContext.ProjectId, build.Id, coverageScope, ref continuationToken);
        previousBuildTotalEntries = source.Count<CoverageSummary>();
        using (new SimpleTimer(requestContext.RequestContext, "PipelineDeltaCoverageProvider: ComputeAndUploadCoverageChanges", ciData))
        {
          int index2 = 0;
          while (index2 < currentBuildCoverageSummaryList.Count<CoverageSummary>())
          {
            CoverageSummary buildCoverageSummary = currentBuildCoverageSummaryList[index2];
            CoverageSummary previousBuildCoverageSummary;
            int num1;
            if (index1 < source.Count<CoverageSummary>())
            {
              previousBuildCoverageSummary = source[index1];
              num1 = string.Compare(previousBuildCoverageSummary.Path, buildCoverageSummary.Path, true);
            }
            else
            {
              previousBuildCoverageSummary = (CoverageSummary) null;
              num1 = 1;
            }
            if (num1 == 0)
            {
              double coverageChanged = this.GetCoverageChanged(previousBuildCoverageSummary, buildCoverageSummary);
              if (coverageChanged != 0.0)
              {
                FileCoverageChange fileCoverageChange = new FileCoverageChange()
                {
                  Change = coverageChanged,
                  IsNewFile = false,
                  Summary = buildCoverageSummary
                };
                fileCoverageChangeList.Add(fileCoverageChange);
                ++coverageChangedFilesCount;
              }
              else
                ++coverageUnchangedFilesCount;
              ++index1;
              ++index2;
            }
            else if (num1 > 0)
            {
              int num2 = buildCoverageSummary.Covered + buildCoverageSummary.PartiallyCovered + buildCoverageSummary.NotCovered;
              FileCoverageChange fileCoverageChange = new FileCoverageChange()
              {
                Change = num2 == 0 ? 100.0 : 100.0 * (double) buildCoverageSummary.Covered / (double) num2,
                IsNewFile = true,
                Summary = buildCoverageSummary
              };
              fileCoverageChangeList.Add(fileCoverageChange);
              ++index2;
              ++newFilesCount;
            }
            else if (num1 < 0)
            {
              ++index1;
              ++deletedFilesCount;
            }
            if (index1 != 0 && index1 == source.Count<CoverageSummary>() && !string.IsNullOrEmpty(continuationToken))
            {
              source = pipelineCoverageService.GetCoverageSummaryList(requestContext, pipelineContext.ProjectId, build.Id, coverageScope, ref continuationToken);
              previousBuildTotalEntries += source.Count<CoverageSummary>();
              index1 = 0;
            }
          }
          if (fileCoverageChangeList.Count<FileCoverageChange>() > 0)
          {
            IComparer<FileCoverageChange> comparer = (IComparer<FileCoverageChange>) new CoverageChangesSummaryComparer();
            fileCoverageChangeList.Sort(comparer);
            foreach (List<FileCoverageChange> split in PipelineDeltaCoverageProvider.SplitList<FileCoverageChange>(fileCoverageChangeList, summaryBatchSize))
              await this.UploadFileCoverageChangeSummary(requestContext, pipelineCoverageService, pipelineContext, deltaPipelineInstanceReference, coverageScope, split);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Logger.Error(1015903, string.Format("PipelineDeltaCoverageProvider: Error computing delta coverage: {0}", (object) ex));
        ciData.Add("Exception", (object) ex.Message);
        throw;
      }
      finally
      {
        ciData.Add("PreviousBuildTotalEntries", (object) previousBuildTotalEntries);
        ciData.Add("CoverageChangedFilesCount", (object) coverageChangedFilesCount);
        ciData.Add("CoverageUnchangedFilesCount", (object) coverageUnchangedFilesCount);
        ciData.Add("NewFilesCount", (object) newFilesCount);
        ciData.Add("DeletedFilesCount", (object) deletedFilesCount);
        TelemetryLogger.Instance.PublishData(requestContext.RequestContext, nameof (PipelineDeltaCoverageProvider), new CustomerIntelligenceData((IDictionary<string, object>) ciData));
      }
      PipelineInstanceReference uploadCoverageChanges = deltaPipelineInstanceReference;
      deltaPipelineInstanceReference = (PipelineInstanceReference) null;
      pipelineCoverageService = (IPipelineCoverageService) null;
      ciData = (Dictionary<string, object>) null;
      return uploadCoverageChanges;
    }

    private static IEnumerable<List<T>> SplitList<T>(List<T> list, int size)
    {
      for (int i = 0; i < list.Count; i += size)
        yield return list.GetRange(i, Math.Min(size, list.Count - i));
    }

    private double GetCoverageChanged(
      CoverageSummary previousBuildCoverageSummary,
      CoverageSummary currentBuildCoverageSummary)
    {
      int num1 = previousBuildCoverageSummary.Covered + previousBuildCoverageSummary.PartiallyCovered + previousBuildCoverageSummary.NotCovered;
      int num2 = currentBuildCoverageSummary.Covered + currentBuildCoverageSummary.PartiallyCovered + currentBuildCoverageSummary.NotCovered;
      if (num1 == 0 && num2 != 0)
        return Math.Round((double) currentBuildCoverageSummary.Covered / (double) num2 * 100.0, 2);
      if (num1 != 0 && num2 == 0)
        return 100.0;
      if (num1 == 0 && num2 == 0)
        return 0.0;
      double num3 = Math.Round((double) previousBuildCoverageSummary.Covered / (double) num1 * 100.0, 2);
      double num4 = Math.Round((double) currentBuildCoverageSummary.Covered / (double) num2 * 100.0, 2);
      if (num4 != 0.0)
        return Math.Round(num4 - num3, 2);
      return num3 <= 0.0 ? num3 : -num3;
    }

    private async Task UploadFileCoverageChangeSummary(
      TestManagementRequestContext requestContext,
      IPipelineCoverageService pipelineCoverageService,
      PipelineContext pipelineContext,
      PipelineInstanceReference pipelineInstanceReference,
      CoverageScope coverageScope,
      List<FileCoverageChange> fileCoverageChanges)
    {
      string fileName = DateTime.UtcNow.ToString("yyyyMMddHHmmssFFFFFF");
      await pipelineCoverageService.UploadFileCoverageChangeSummary(requestContext, pipelineContext.ProjectId, pipelineContext.Id, new CoverageChangeSummary()
      {
        DeltaPipelineRun = pipelineInstanceReference,
        Scope = coverageScope.Name,
        FileCoverageChanges = fileCoverageChanges
      }, coverageScope, fileName);
    }
  }
}
