// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultDetailsColumns
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class ResultDetailsColumns
  {
    public string AutomatedTestStorage { get; set; }

    public int Priority { get; set; }

    public int TestRunId { get; set; }

    public int TestResultId { get; set; }

    public int TestCaseRefId { get; set; }

    public int TestPointId { get; set; }

    public int WorkItemId { get; set; }

    public string Owner { get; set; }

    public string Outcome { get; set; }

    public long DurationInMs { get; set; }
  }
}
