// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TeamIdentityViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types.Team;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TeamIdentityViewModel : GroupIdentityViewModel
  {
    public TeamIdentityViewModel(TeamFoundationIdentity identity)
      : base(identity)
    {
    }

    public List<IdentityViewModelBase> Administrators { get; private set; }

    public override string IdentityType => "team";

    public void PopulateTeamAdmins(TfsWebContext webContext)
    {
      IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity> teamAdmins = webContext.TfsRequestContext.GetService<ITeamService>().GetTeamAdmins(webContext.TfsRequestContext, IdentityUtil.Convert(this.Identity));
      this.Administrators = new List<IdentityViewModelBase>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) teamAdmins)
      {
        if (identity != null)
          this.Administrators.Add(IdentityImageUtility.GetIdentityViewModel(IdentityUtil.Convert(identity), false));
      }
      this.Administrators.Sort();
    }

    public override JsObject ToJson()
    {
      JsObject json = base.ToJson();
      json["IsTeam"] = (object) true;
      return json;
    }
  }
}
