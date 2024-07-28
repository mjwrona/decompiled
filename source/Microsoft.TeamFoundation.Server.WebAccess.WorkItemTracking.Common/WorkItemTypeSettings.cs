// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeSettings
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [DataContract]
  public class WorkItemTypeSettings : BaseWorkItemFormLayoutUserSettingsSecuredObject
  {
    public WorkItemTypeSettings()
    {
      this.CollapsedGroups = new SecuredCollapsedGroups(StringComparer.OrdinalIgnoreCase);
      this.MobileCollapsedGroups = new SecuredCollapsedGroups(StringComparer.OrdinalIgnoreCase);
    }

    [DataMember(Name = "refName")]
    public string RefName { get; set; }

    [DataMember(Name = "id")]
    public int WorkItemTypeId { get; set; }

    [DataMember(Name = "collapsedGroups")]
    public SecuredCollapsedGroups CollapsedGroups { get; set; }

    [DataMember(Name = "mobileCollapsedGroups")]
    public SecuredCollapsedGroups MobileCollapsedGroups { get; set; }

    public void SecureMembers(string token, int requiredPermission)
    {
      this.SetTokenAndPermission(token, requiredPermission);
      this.CollapsedGroups.SetTokenAndPermission(token, requiredPermission);
      this.MobileCollapsedGroups.SetTokenAndPermission(token, requiredPermission);
    }
  }
}
