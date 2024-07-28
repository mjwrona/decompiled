// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.PipelineDirectorySummaryProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class PipelineDirectorySummaryProvider : IPipelineDirectorySummaryProvider
  {
    public SortedDictionary<string, DirectoryCoverageSummary> ComputeDirectoryCoverageSummary(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      List<CoverageSummary> coverageSummaryList,
      CoverageScope coverageScope)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      CommonHelper.PopulateCiDataWithPipelineContext(pipelineContext, dictionary);
      using (new SimpleTimer(requestContext.RequestContext, nameof (ComputeDirectoryCoverageSummary), dictionary))
      {
        SortedDictionary<string, DirectoryCoverageSummary> directoryCoverageSummary = new SortedDictionary<string, DirectoryCoverageSummary>();
        try
        {
          directoryCoverageSummary = this.ComputeDirectorySummaryForChildFiles(requestContext, coverageSummaryList, coverageScope.Name);
          directoryCoverageSummary = this.ComputeDirectorySummaryForChildDirectories(requestContext, directoryCoverageSummary, coverageScope.Name);
          directoryCoverageSummary[string.Empty].Summary.Path = "RootKeyF1FA1469BD94";
          directoryCoverageSummary.Add("RootKeyF1FA1469BD94", directoryCoverageSummary[string.Empty]);
          directoryCoverageSummary.Remove(string.Empty);
          return directoryCoverageSummary;
        }
        catch (Exception ex)
        {
          requestContext.Logger.Error(1015906, string.Format("PipelineDirectorySummaryProvider: Error computing directory summary: {0}", (object) ex));
          throw;
        }
        finally
        {
          dictionary.Add("TotalDirectories", (object) directoryCoverageSummary.Count<KeyValuePair<string, DirectoryCoverageSummary>>());
          dictionary.Add("TotalFiles", (object) coverageSummaryList.Count<CoverageSummary>());
          TelemetryLogger.Instance.PublishData(requestContext.RequestContext, "PipelineDirectorySummaryProvider ", new CustomerIntelligenceData((IDictionary<string, object>) dictionary));
        }
      }
    }

    private SortedDictionary<string, DirectoryCoverageSummary> ComputeDirectorySummaryForChildFiles(
      TestManagementRequestContext requestContext,
      List<CoverageSummary> coverageSummaryList,
      string scope)
    {
      using (new SimpleTimer(requestContext.RequestContext, nameof (ComputeDirectorySummaryForChildFiles)))
      {
        SortedDictionary<string, DirectoryCoverageSummary> summaryForChildFiles = new SortedDictionary<string, DirectoryCoverageSummary>();
        string empty = string.Empty;
        foreach (CoverageSummary coverageSummary in coverageSummaryList)
        {
          coverageSummary.Path = coverageSummary.Path.Replace('/', '\\');
          string directoryName = Path.GetDirectoryName(coverageSummary.Path);
          if (!summaryForChildFiles.ContainsKey(directoryName))
          {
            DirectoryCoverageSummary directoryCoverageSummary = new DirectoryCoverageSummary()
            {
              Scope = scope,
              Summary = new CoverageSummary()
              {
                Path = directoryName,
                Covered = coverageSummary.Covered,
                PartiallyCovered = coverageSummary.PartiallyCovered,
                NotCovered = coverageSummary.NotCovered,
                IsDirectory = true
              },
              Children = new List<CoverageSummary>()
              {
                coverageSummary
              }
            };
            summaryForChildFiles.Add(directoryName, directoryCoverageSummary);
          }
          else
          {
            summaryForChildFiles[directoryName].Summary.Covered += coverageSummary.Covered;
            summaryForChildFiles[directoryName].Summary.PartiallyCovered += coverageSummary.PartiallyCovered;
            summaryForChildFiles[directoryName].Summary.NotCovered += coverageSummary.NotCovered;
            summaryForChildFiles[directoryName].Children.Add(coverageSummary);
          }
        }
        return summaryForChildFiles;
      }
    }

    private SortedDictionary<string, DirectoryCoverageSummary> ComputeDirectorySummaryForChildDirectories(
      TestManagementRequestContext requestContext,
      SortedDictionary<string, DirectoryCoverageSummary> directoriesCoverageSummary,
      string scope)
    {
      using (new SimpleTimer(requestContext.RequestContext, nameof (ComputeDirectorySummaryForChildDirectories)))
      {
        DirectoryTree directoryTree = new DirectoryTree(directoriesCoverageSummary);
        directoryTree.ComputeSummaryForImmediateDirectories(directoryTree.Root, string.Empty, directoriesCoverageSummary, scope);
        return directoriesCoverageSummary;
      }
    }
  }
}
