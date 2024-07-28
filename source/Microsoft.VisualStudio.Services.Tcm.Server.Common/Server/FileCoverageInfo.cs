// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.FileCoverageInfo
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DataContract]
  public class FileCoverageInfo
  {
    public FileCoverageInfo()
    {
      this.FilePath = string.Empty;
      this.LineCoverageStatus = new Dictionary<uint, CoverageStatus>();
    }

    [DataMember]
    public string FilePath { get; set; }

    [DataMember]
    public Dictionary<uint, CoverageStatus> LineCoverageStatus { get; set; }

    public static FileCoverageInfo MergeFileCoverage(
      TestManagementRequestContext requestContext,
      FileCoverageInfo fileCoverageInfo1,
      FileCoverageInfo fileCoverageInfo2)
    {
      if (!string.Equals(fileCoverageInfo1.FilePath, fileCoverageInfo2.FilePath, StringComparison.OrdinalIgnoreCase))
      {
        requestContext.Logger.Error(1015785, "Different file paths for a changed file, file1: " + fileCoverageInfo1.FilePath + ", file2:" + fileCoverageInfo2.FilePath);
        return (FileCoverageInfo) null;
      }
      Dictionary<uint, CoverageStatus> lineCoverageStatus1 = fileCoverageInfo1.LineCoverageStatus;
      Dictionary<uint, CoverageStatus> lineCoverageStatus2 = fileCoverageInfo2.LineCoverageStatus;
      IEnumerable<uint> uints = lineCoverageStatus1.Keys.Union<uint>((IEnumerable<uint>) lineCoverageStatus2.Keys);
      FileCoverageInfo fileCoverageInfo = new FileCoverageInfo()
      {
        FilePath = fileCoverageInfo1.FilePath,
        LineCoverageStatus = new Dictionary<uint, CoverageStatus>()
      };
      foreach (uint key in uints)
      {
        CoverageStatus coverageStatus1 = lineCoverageStatus1.TryGetValue(key, out coverageStatus1) ? coverageStatus1 : CoverageStatus.NotCovered;
        CoverageStatus coverageStatus2 = lineCoverageStatus2.TryGetValue(key, out coverageStatus2) ? coverageStatus2 : CoverageStatus.NotCovered;
        if (coverageStatus1 == CoverageStatus.Covered || coverageStatus2 == CoverageStatus.Covered)
          fileCoverageInfo.LineCoverageStatus.Add(key, CoverageStatus.Covered);
        else if (coverageStatus1 == CoverageStatus.PartiallyCovered || coverageStatus2 == CoverageStatus.PartiallyCovered)
          fileCoverageInfo.LineCoverageStatus.Add(key, CoverageStatus.PartiallyCovered);
        else
          fileCoverageInfo.LineCoverageStatus.Add(key, CoverageStatus.NotCovered);
      }
      return fileCoverageInfo;
    }
  }
}
