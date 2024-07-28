// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageFileUtilityHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.CodeCoverage.Analysis;
using Microsoft.CodeCoverage.IO;
using Microsoft.CodeCoverage.IO.Coverage;
using Microsoft.CodeCoverage.IO.Coverage.Report;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageFileUtilityHelper
  {
    private AnalysisCoverageFileUtility utility;

    public CoverageFileUtilityHelper() => this.utility = new AnalysisCoverageFileUtility((ICoverageFileConfiguration) new CoverageFileConfiguration(true, false, false, false, true, true, Environment.ProcessorCount < 1 ? 2 : Environment.ProcessorCount, true));

    public async Task<CoverageReport[]> ConvertToCoverageReport(List<string> fileLocationList)
    {
      CoverageReport[] coverageReports = new CoverageReport[fileLocationList.Count];
      int counter = 0;
      foreach (string fileLocation in fileLocationList)
      {
        coverageReports[counter] = await ((CoverageFileUtilityV2) this.utility).ReadCoverageFileAsync(fileLocation, CancellationToken.None);
        ++counter;
      }
      CoverageReport[] coverageReport = coverageReports;
      coverageReports = (CoverageReport[]) null;
      return coverageReport;
    }

    public SourceReport ConvertToSourceReport(CoverageReport[] coverageReports) => ((CoverageFileUtilityV2) this.utility).ToSourceReport(coverageReports);

    public SourceReport ConvertToSourceReport(FileData[] filedata) => ((CoverageFileUtilityV2) this.utility).ToSourceReport(filedata);

    public List<FileData> ExtractFileDataFromCoverageReport(CoverageReport[] coverageReports)
    {
      List<FileData> fromCoverageReport = new List<FileData>();
      foreach (CoverageReport coverageReport in coverageReports)
      {
        HashSet<string> stringSet = new HashSet<string>(((CoverageFileUtilityV2) this.utility).GetUniqueSourceFiles(coverageReport));
        fromCoverageReport.AddRange((IEnumerable<FileData>) ((CoverageFileUtilityV2) this.utility).GetFileCoverage(coverageReport.Modules, stringSet));
      }
      return fromCoverageReport;
    }
  }
}
