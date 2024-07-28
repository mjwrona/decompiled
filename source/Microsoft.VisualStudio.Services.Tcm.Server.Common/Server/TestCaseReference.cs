// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseReference
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestCaseReference
  {
    public int TestCaseReferenceId { get; set; }

    public int TestCaseId { get; set; }

    public string AutomatedTestName { get; set; }

    public string AutomatedTestStorage { get; set; }

    public int PlanId { get; set; }

    public int SuiteId { get; set; }

    public int PointId { get; set; }
  }
}
