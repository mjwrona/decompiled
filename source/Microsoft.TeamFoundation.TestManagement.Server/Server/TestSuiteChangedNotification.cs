// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestSuiteChangedNotification
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestSuiteChangedNotification
  {
    internal TestSuiteChangedNotification(int suiteId, int planId, string projectName)
    {
      this.SuiteId = suiteId;
      this.PlanId = planId;
      this.ProjectName = projectName;
    }

    public int SuiteId { get; private set; }

    public int PlanId { get; private set; }

    public string ProjectName { get; private set; }
  }
}
