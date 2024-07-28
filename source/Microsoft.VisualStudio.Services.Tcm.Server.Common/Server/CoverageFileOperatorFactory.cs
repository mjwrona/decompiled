// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageFileOperatorFactory
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageFileOperatorFactory
  {
    public ICoverageFileOperator GetCoverageFileOperator(
      TestManagementRequestContext tcmRequestContext,
      string coverageTool)
    {
      if (string.IsNullOrWhiteSpace(coverageTool))
      {
        tcmRequestContext.Logger.Error(1015668, "GetCoverageFileOperator: coverageTool is null or empty");
        throw new ArgumentNullException(nameof (coverageTool));
      }
      if (this.CoverageFileOperator != null)
        return this.CoverageFileOperator;
      if (string.Equals(new VstestCoverageFileOperator().CoverageTool, coverageTool, StringComparison.OrdinalIgnoreCase))
        return (ICoverageFileOperator) new VstestCoverageFileOperator();
      if (string.Equals(new NativeCoverageFileOperator().CoverageTool, coverageTool, StringComparison.OrdinalIgnoreCase))
        return (ICoverageFileOperator) new NativeCoverageFileOperator();
      return string.Equals(new VstestDotCoverageFileOperator().CoverageTool, coverageTool, StringComparison.OrdinalIgnoreCase) ? (ICoverageFileOperator) new VstestDotCoverageFileOperator() : (ICoverageFileOperator) null;
    }

    public ICoverageFileOperator CoverageFileOperator { get; set; }
  }
}
