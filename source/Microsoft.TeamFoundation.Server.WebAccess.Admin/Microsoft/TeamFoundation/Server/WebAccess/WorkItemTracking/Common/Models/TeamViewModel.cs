// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.TeamViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.Core;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  public class TeamViewModel
  {
    public TeamIdentityViewModel Identity;
    public ITeamSettings Settings;
    public ProjectProcessConfiguration ProcessSettings;

    public TeamViewModel(TeamFoundationIdentity teamIdentity)
    {
      this.Identity = IdentityImageUtility.GetIdentityViewModel<TeamIdentityViewModel>(teamIdentity);
      this.Settings = (ITeamSettings) null;
      this.ProcessSettings = (ProjectProcessConfiguration) null;
    }

    public TeamViewModel(
      TeamFoundationIdentity teamIdentity,
      ITeamSettings settings,
      ProjectProcessConfiguration processSettings)
    {
      this.Identity = IdentityImageUtility.GetIdentityViewModel<TeamIdentityViewModel>(teamIdentity);
      this.Settings = settings;
      this.ProcessSettings = processSettings;
    }
  }
}
