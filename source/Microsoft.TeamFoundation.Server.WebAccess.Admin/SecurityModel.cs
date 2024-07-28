// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.SecurityModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class SecurityModel
  {
    public bool CanManageIdentities { get; internal set; }

    public bool CanManagePermissions { get; internal set; }

    public bool CanTokenInheritPermissions { get; internal set; }

    public string Header { get; internal set; }

    public bool InheritPermissions { get; internal set; }

    public bool ControlManagesFocus { get; internal set; }

    public Guid PermissionSetId { get; internal set; }

    public string ViewTitle { get; internal set; }

    public string Title { get; internal set; }

    public string TitlePrefix { get; internal set; }

    public string Token { get; internal set; }

    public string CustomDoNotHavePermissionsText { get; set; }

    public bool ShowAllGroupsIfCollection { get; set; }

    public bool HideExplicitClearButton { get; set; }

    public bool HideToolbar { get; set; }

    public bool AllowAADSearchInHosted { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("permissionSetId", (object) this.PermissionSetId);
      json.Add("token", (object) this.Token);
      json.Add("tokenDisplayName", (object) this.Title);
      json.Add("canManageGroups", (object) this.CanManageIdentities);
      json.Add("canManagePermissions", (object) this.CanManagePermissions);
      json.Add("inheritPermissions", (object) this.InheritPermissions);
      json.Add("controlManagesFocus", (object) this.ControlManagesFocus);
      json.Add("canTokenInheritPermissions", (object) this.CanTokenInheritPermissions);
      json.Add("customDoNotHavePermissionsText", (object) this.CustomDoNotHavePermissionsText);
      json.Add("showAllGroupsIfCollection", (object) this.ShowAllGroupsIfCollection);
      json.Add("hideExplicitClearButton", (object) this.HideExplicitClearButton);
      json.Add("hideToolbar", (object) this.HideToolbar);
      json.Add("allowAADSearchInHosted", (object) this.AllowAADSearchInHosted);
      return json;
    }
  }
}
