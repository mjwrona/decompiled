// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.TracePermissionModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.AdminEngagement.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class TracePermissionModel
  {
    private IdentityViewModelBase m_identityViewModel;

    public TracePermissionModel(TeamFoundationIdentity identity)
    {
      if (identity.IsContainer)
        this.m_identityViewModel = (IdentityViewModelBase) new GroupIdentityViewModel(identity);
      else
        this.m_identityViewModel = (IdentityViewModelBase) new UserIdentityViewModel(identity);
    }

    public ActionDefinition ActionDefinition { get; set; }

    public Dictionary<TeamFoundationIdentity, PermissionValue> AffectingGroups { get; set; }

    public InheritanceType InheritanceType { get; set; }

    public string PermissionName => this.ActionDefinition != null ? this.ActionDefinition.DisplayName : string.Empty;

    public PermissionValue PermissionValue { get; set; }

    public string Title { get; internal set; }

    public string TitlePrefix { get; internal set; }

    public string UserDisplayName => this.m_identityViewModel.DisplayName;

    public string UserFriendlyDisplayName => this.m_identityViewModel.FriendlyDisplayName;

    public string UserPermission => PermissionModel.GetPermissionDisplayString(this.PermissionValue);

    public string TokenDisplayName { get; set; }

    public string Error { get; set; }
  }
}
