// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultInsights
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class ResultInsights
  {
    internal int TestRunId { get; set; }

    internal int TestRunContextId { get; set; }

    internal DateTime TestRunCompletedDate { get; set; }

    internal int PrevRunContextId { get; set; }

    internal int PrevBuildRefId { get; set; }

    internal int PrevReleaseRefId { get; set; }

    internal int NewFailures { get; set; }

    internal int ExistingFailures { get; set; }

    internal int FixedTests { get; set; }

    internal string NewFailedResults { get; set; }

    internal string ExistingFailedResults { get; set; }

    internal string FixedTestResults { get; set; }
  }
}
