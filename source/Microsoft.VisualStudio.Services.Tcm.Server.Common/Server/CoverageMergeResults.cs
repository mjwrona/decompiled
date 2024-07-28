// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageMergeResults
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageMergeResults
  {
    public string MergedCoverageFile { get; set; }

    public List<FileCoverageInfo> FileCoverageList { get; set; } = new List<FileCoverageInfo>();

    public string JobSpecificTempFolderPath { get; }

    public string ModuleName => this.CoverageTool == new NativeCoverageFileOperator().CoverageTool ? Path.Combine(this.JobSpecificTempFolderPath, "DummyCjsonFileName") : this.MergedCoverageFile;

    public string CoverageTool { get; }

    public CoverageMergeResults(string CoverageTool, string JobSpecificTempFolderPath)
    {
      this.CoverageTool = CoverageTool;
      this.JobSpecificTempFolderPath = JobSpecificTempFolderPath;
    }
  }
}
