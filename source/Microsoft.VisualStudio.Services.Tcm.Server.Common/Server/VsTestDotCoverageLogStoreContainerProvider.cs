// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.VsTestDotCoverageLogStoreContainerProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class VsTestDotCoverageLogStoreContainerProvider : ILogStoreContainerProvider
  {
    public string GetLogStoreContainerFilePath(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      string mergedFilePath,
      string platform,
      string flavor)
    {
      BuildConfiguration buildConfig = new BuildConfiguration()
      {
        BuildPlatform = platform,
        BuildFlavor = flavor,
        BuildUri = pipelineContext.Uri,
        BuildId = pipelineContext.Id,
        TeamProjectName = tcmRequestContext.ProjectServiceHelper.GetProjectName(pipelineContext.ProjectId)
      }.QueryWithPlatformAndFlavor(tcmRequestContext.RequestContext, pipelineContext.ProjectId, pipelineContext.Id, platform, flavor);
      buildConfig.BuildId = pipelineContext.Id;
      return "BuildCoverage\\" + CoverageFileNameUtility.GetCoverageFileName(pipelineContext.Number, buildConfig);
    }

    public string GetLogStoreContainerName(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext)
    {
      return "VstestDotCoverage";
    }
  }
}
