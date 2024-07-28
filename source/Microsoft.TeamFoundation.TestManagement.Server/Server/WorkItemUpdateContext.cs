// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WorkItemUpdateContext
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class WorkItemUpdateContext
  {
    public TfsTestManagementRequestContext Context { get; set; }

    public string TeamProjectName { get; set; }

    public GuidAndString ProjectGuidAndString { get; set; }

    public bool BypassRulesValidation { get; set; }

    public bool IsUpgradeContext { get; set; }

    public bool isSuiteRenameScenario { get; set; }

    public bool SuppressNotifications { get; set; }

    public void CopyProperties(WorkItemUpdateContext sourceData)
    {
      this.Context = sourceData.Context;
      this.TeamProjectName = sourceData.TeamProjectName;
      this.ProjectGuidAndString = sourceData.ProjectGuidAndString;
      this.BypassRulesValidation = sourceData.BypassRulesValidation;
      this.IsUpgradeContext = sourceData.IsUpgradeContext;
      this.SuppressNotifications = sourceData.SuppressNotifications;
    }

    internal static WorkItemUpdateContext CreateWorkItemUpdateContext(
      TestManagementRequestContext context,
      string teamProjectName,
      GuidAndString projectId,
      bool bypassRulesValidation,
      bool isUpgradeContext = false,
      bool suppressNotifications = false,
      bool isSuiteRenameScenario = false)
    {
      TfsTestManagementRequestContext managementRequestContext = new TfsTestManagementRequestContext(context.RequestContext);
      return new WorkItemUpdateContext()
      {
        Context = managementRequestContext,
        TeamProjectName = teamProjectName,
        ProjectGuidAndString = projectId,
        BypassRulesValidation = bypassRulesValidation,
        IsUpgradeContext = isUpgradeContext,
        SuppressNotifications = suppressNotifications,
        isSuiteRenameScenario = isSuiteRenameScenario
      };
    }
  }
}
