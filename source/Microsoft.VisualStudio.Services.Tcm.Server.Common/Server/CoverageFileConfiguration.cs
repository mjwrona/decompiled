// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageFileConfiguration
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.Core;
using Microsoft.CodeCoverage.IO.Coverage;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageFileConfiguration : ICoverageFileConfiguration
  {
    public CoverageFileConfiguration()
    {
    }

    public CoverageFileConfiguration(
      bool ReadModules,
      bool ReadSkippedModules,
      bool ReadSkippedFunctions,
      bool ReadSnapshotsData,
      bool GenerateCoverageBufferFiles,
      bool FixCoverageBuffersMismatch,
      int MaxDegreeOfParallelism,
      bool SkipInvalidData)
    {
      this.ReadModules = ReadModules;
      this.ReadSkippedModules = ReadSkippedModules;
      this.ReadSkippedFunctions = ReadSkippedFunctions;
      this.ReadSnapshotsData = ReadSnapshotsData;
      this.GenerateCoverageBufferFiles = GenerateCoverageBufferFiles;
      this.FixCoverageBuffersMismatch = FixCoverageBuffersMismatch;
      this.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
      this.SkipInvalidData = SkipInvalidData;
    }

    public bool ReadModules { get; set; }

    public bool ReadSkippedModules { get; set; }

    public bool ReadSkippedFunctions { get; set; }

    public bool ReadSnapshotsData { get; set; }

    public bool GenerateCoverageBufferFiles { get; set; }

    public bool FixCoverageBuffersMismatch { get; set; }

    public int MaxDegreeOfParallelism { get; set; } = 10;

    public bool SkipInvalidData { get; set; }

    public CoverageMergeOperation MergeOperation => throw new NotImplementedException();
  }
}
