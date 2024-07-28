// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultFileContainerProvider
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class DefaultFileContainerProvider : IFileContainerNameProvider
  {
    public bool IsToolSupported(string coverageTool) => string.Equals(coverageTool, new VstestCoverageFileOperator().CoverageTool, StringComparison.OrdinalIgnoreCase) || string.Equals(coverageTool, new NativeCoverageFileOperator().CoverageTool, StringComparison.OrdinalIgnoreCase);

    public string GetFileContainerName(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext)
    {
      return "ModuleCoverage";
    }

    public string GetContainerFilePath(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      string mergedFilePath,
      string platform,
      string flavor)
    {
      string withoutExtension = Path.GetFileNameWithoutExtension(mergedFilePath);
      string extension = Path.GetExtension(mergedFilePath);
      return withoutExtension + "_" + platform + flavor + extension;
    }
  }
}
