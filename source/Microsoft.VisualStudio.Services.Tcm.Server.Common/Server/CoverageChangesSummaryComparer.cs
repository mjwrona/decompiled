// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageChangesSummaryComparer
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageChangesSummaryComparer : IComparer<FileCoverageChange>
  {
    public int Compare(
      FileCoverageChange fileCoverageChange1,
      FileCoverageChange fileCoverageChange2)
    {
      if (fileCoverageChange1 == null && fileCoverageChange2 == null)
        return 0;
      if (fileCoverageChange1 == null)
        return 1;
      if (fileCoverageChange2 == null)
        return -1;
      if (fileCoverageChange1.Change != fileCoverageChange2.Change)
        return fileCoverageChange1.Change >= fileCoverageChange2.Change ? 1 : -1;
      int num1 = string.Compare(fileCoverageChange1.Summary.Path, fileCoverageChange2.Summary.Path);
      if (num1 != 0)
        return num1;
      int num2 = fileCoverageChange1.Summary.Covered + fileCoverageChange1.Summary.PartiallyCovered + fileCoverageChange1.Summary.NotCovered;
      int num3 = fileCoverageChange2.Summary.Covered + fileCoverageChange2.Summary.PartiallyCovered + fileCoverageChange2.Summary.NotCovered;
      if (num2 == num3)
        return 0;
      return num2 >= num3 ? -1 : 1;
    }
  }
}
