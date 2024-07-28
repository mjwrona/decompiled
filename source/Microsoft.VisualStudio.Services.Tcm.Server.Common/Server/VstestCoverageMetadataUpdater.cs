// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.VstestCoverageMetadataUpdater
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.Analysis;
using Microsoft.CodeCoverage.Analysis.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class VstestCoverageMetadataUpdater : ICoverageMetadata
  {
    public void UpdateModuleCoverage(
      TestManagementRequestContext requestContext,
      PipelineContext pipelineContext,
      string buildPlatform,
      string buildFlavor,
      string mergedCovFilePath,
      string fileUrl)
    {
      Dictionary<string, object> data = new Dictionary<string, object>();
      data.Add("BuildId", (object) pipelineContext.Id);
      data.Add("ProjectId", (object) pipelineContext.ProjectId);
      data.Add("BuildUri", (object) pipelineContext.Uri);
      try
      {
        MSCodeCoverageTransformer coverageTransformer = new MSCodeCoverageTransformer();
        BuildConfiguration build = new BuildConfiguration()
        {
          BuildPlatform = buildPlatform,
          BuildFlavor = buildFlavor,
          BuildUri = pipelineContext.Uri,
          BuildId = pipelineContext.Id,
          TeamProjectName = requestContext.ProjectServiceHelper.GetProjectName(pipelineContext.ProjectId)
        }.QueryWithPlatformAndFlavor(requestContext.RequestContext, pipelineContext.ProjectId, pipelineContext.Id, buildPlatform, buildFlavor);
        build.BuildId = pipelineContext.Id;
        using (CoverageInfo fromFile = CoverageInfo.CreateFromFile(mergedCovFilePath))
        {
          BuildCoverage newCoverage = new BuildCoverage();
          newCoverage.Configuration = build;
          data.Add("MergedFileModuleCount", (object) (fromFile == null ? new int?(-1) : fromFile.Modules?.Count));
          if (fromFile != null)
          {
            requestContext.Logger.Info(1015401, "CoverageAnalyzer: Coverage changes detected.  Merging with existing build-wide coverage data.");
            BuildCoverage oldCoverage = coverageTransformer.QueryBuildCoverage(requestContext, build);
            if (!coverageTransformer.MergeExisting(requestContext, (Coverage) oldCoverage, fromFile, (Coverage) newCoverage, this.AutoIncrementModuleId, this.MergeWithOldData))
            {
              string format = string.Format("CoverageAnalyzer: Error during build-wide coverage merge. Build Config Id {0}, Build URI {1}, Module Count {2}", (object) build.BuildConfigurationId, (object) build.BuildUri, (object) fromFile.Modules.Count);
              requestContext.Logger.Error(1015402, format);
              data.Add("ErrorMessage", (object) format);
            }
          }
          else
          {
            string format = string.Format("CoverageAnalyzer: RunCoverageInfo is null. Build Config Id {0}, Build URI {1}, Build Id {2}", (object) build.BuildConfigurationId, (object) build.BuildUri, (object) build.BuildId);
            requestContext.Logger.Error(1015410, format);
            data.Add("ErrorMessage", (object) format);
          }
          if (!string.IsNullOrEmpty(fileUrl) && newCoverage.Modules != null && newCoverage.Modules.Count > 0)
            newCoverage.Modules[0].CoverageFileUrl = fileUrl;
          else
            data.Add("WarningMessage", (object) "Coverage file was empty");
          newCoverage.Update(requestContext, 1, pipelineContext.ProjectId);
        }
      }
      catch (ImageNotFoundException ex)
      {
        data.Add("Failure", (object) string.Format("Read of merged file failed with {0}. Skipping updating module coverage", (object) ex));
        requestContext.Logger.Error(1015416, string.Format("Read of merged file failed with {0}. Skipping updating module coverage", (object) ex));
      }
      catch (CoverageAnalysisException ex)
      {
        data.Add("Failure", (object) string.Format("Read of merged file failed with {0}. Skipping updating module coverage", (object) ex));
        requestContext.Logger.Error(1015417, string.Format("Read of merged file failed with {0}. Skipping updating module coverage", (object) ex));
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) data);
        TelemetryLogger.Instance.PublishData(requestContext.RequestContext, nameof (VstestCoverageMetadataUpdater), cid);
      }
    }

    public virtual bool AutoIncrementModuleId => false;

    public virtual bool MergeWithOldData => false;
  }
}
