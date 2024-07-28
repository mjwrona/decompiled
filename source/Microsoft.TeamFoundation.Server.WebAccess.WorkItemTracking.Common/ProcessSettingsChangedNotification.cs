// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessSettingsChangedNotification
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class ProcessSettingsChangedNotification
  {
    public string ProjectUri { get; private set; }

    public bool ValidationSkipped { get; private set; }

    public bool IsFeatureEnablement { get; private set; }

    public bool IsNewTeamProject { get; private set; }

    public ProcessSettingsChangedNotification(
      string projectUri,
      bool isNew,
      bool validationSkipped,
      bool isFeatureEnablement)
    {
      this.ProjectUri = projectUri;
      this.ValidationSkipped = validationSkipped;
      this.IsFeatureEnablement = isFeatureEnablement;
      this.IsNewTeamProject = isNew;
    }
  }
}
