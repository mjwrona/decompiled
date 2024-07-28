// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.LayoutUserSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class LayoutUserSettings : BaseWorkItemFormLayoutUserSettingsSecuredObject
  {
    public LayoutUserSettings()
    {
      this.ProcessSettings = (IList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessSettings>) new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessSettings>();
      this.ProjectSettings = (IList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectSettings>) new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectSettings>();
      this.IsWitDialogFullScreen = false;
    }

    [DataMember(Name = "processSettings")]
    public IList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessSettings> ProcessSettings { get; set; }

    [DataMember(Name = "projectSettings")]
    public IList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectSettings> ProjectSettings { get; set; }

    [DataMember(Name = "isWitDialogFullScreen")]
    public bool IsWitDialogFullScreen { get; set; }

    public void SecureMembers(string token, int requiredPermission)
    {
      this.SetTokenAndPermission(token, requiredPermission);
      foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessSettings processSetting in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProcessSettings>) this.ProcessSettings)
        processSetting.SecureMembers(token, requiredPermission);
      foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectSettings projectSetting in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectSettings>) this.ProjectSettings)
        projectSetting.SecureMembers(token, requiredPermission);
    }
  }
}
