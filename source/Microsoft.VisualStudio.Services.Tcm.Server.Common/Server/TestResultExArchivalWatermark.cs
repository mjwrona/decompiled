// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultExArchivalWatermark
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestResultExArchivalWatermark
  {
    public TestResultExArchivalWatermark()
    {
    }

    [JsonConstructor]
    public TestResultExArchivalWatermark(
      DateTime testRunUpdatedDate,
      int testRunId,
      int testResultId)
    {
      this.TestRunUpdatedDate = testRunUpdatedDate;
      this.TestRunId = testRunId;
      this.TestResultId = testResultId;
    }

    public TestResultExArchivalWatermark(TestResultExArchivalRecord record)
    {
      this.TestRunUpdatedDate = record.TestRunUpdatedDate;
      this.TestRunId = record.TestRunId;
      this.TestResultId = record.TestResultId;
    }

    public DateTime TestRunUpdatedDate { get; internal set; }

    public int TestRunId { get; internal set; }

    public int TestResultId { get; internal set; }
  }
}
