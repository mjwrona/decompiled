// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestRunComparer2
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestRunComparer2 : IComparer<RunSummaryByOutcomeInPipeline>
  {
    public int Compare(RunSummaryByOutcomeInPipeline x, RunSummaryByOutcomeInPipeline y)
    {
      if (x != null && y != null)
      {
        int num = y.RunCompletedDate.CompareTo(x.RunCompletedDate);
        if (num == 0)
          num = y.TestRunId.CompareTo(x.TestRunId);
        return num;
      }
      if (x == null && y == null)
        return 0;
      return x == null && y != null ? -1 : 1;
    }
  }
}
