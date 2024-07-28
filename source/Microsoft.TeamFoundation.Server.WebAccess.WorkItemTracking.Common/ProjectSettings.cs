// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class ProjectSettings : BaseWorkItemFormLayoutUserSettingsSecuredObject
  {
    public ProjectSettings() => this.WorkItemTypeSettings = (IList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeSettings>) new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeSettings>();

    [DataMember(Name = "projectId")]
    public Guid ProjectId { get; set; }

    [DataMember(Name = "workItemTypeSettings")]
    public IList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeSettings> WorkItemTypeSettings { get; set; }

    public void SecureMembers(string token, int requiredPermission)
    {
      this.SetTokenAndPermission(token, requiredPermission);
      foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeSettings workItemTypeSetting in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeSettings>) this.WorkItemTypeSettings)
        workItemTypeSetting.SecureMembers(token, requiredPermission);
    }
  }
}
