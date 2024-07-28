// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LineCoverageResult
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LineCoverageResult
  {
    public bool NoExecutableChanges;
    public bool CoverageDataNotFound;
    public uint TotalLines;
    public uint CoveredLines;
    public uint NotCoveredLines;
    public uint PartiallyCoveredLines;
    public bool TreatPartiallyCoveredLinesAsCovered;
    public double? Coverage;
    public uint TotalBlocks;

    public LineCoverageResult()
    {
      this.TotalLines = 0U;
      this.CoveredLines = 0U;
      this.NotCoveredLines = 0U;
      this.PartiallyCoveredLines = 0U;
      this.TreatPartiallyCoveredLinesAsCovered = false;
      this.Coverage = new double?();
      this.CoverageDataNotFound = false;
      this.NoExecutableChanges = false;
      this.TotalBlocks = 0U;
    }
  }
}
