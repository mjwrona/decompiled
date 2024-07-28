// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TeamJumpPointModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TeamJumpPointModel : JumpPointModelBase, IComparable<TeamJumpPointModel>
  {
    private TeamIdentityViewModel m_team;

    public TeamJumpPointModel(
      TfsWebContext tfsWebContext,
      NavigationContextModel navigationContext,
      TeamIdentityViewModel team,
      ProjectJumpPointModel project)
      : base(tfsWebContext, navigationContext)
    {
      this.m_team = team;
      this.Name = this.m_team.FriendlyDisplayName;
      this.NavigationContextLevel = NavigationContextLevels.Team;
      this.RouteValues = new RouteValueDictionary((IDictionary<string, object>) project.RouteValues);
      this.RouteValues[nameof (team)] = (object) team.FriendlyDisplayName;
      this.Parent = project;
    }

    public ProjectJumpPointModel Parent { get; private set; }

    public override string Path => this.Parent.Path + "/" + this.Name;

    public override JsObject ToJson()
    {
      JsObject json = base.ToJson();
      json["tfid"] = (object) this.m_team.TeamFoundationId;
      json["team"] = (object) this.m_team.ToJson();
      json["projectName"] = (object) this.Parent.Name;
      json["collectionName"] = (object) this.Parent.Parent.Name;
      return json;
    }

    public int CompareTo(TeamJumpPointModel other)
    {
      int num = this.Parent.CompareTo(other.Parent);
      if (num == 0)
        num = this.Name.CompareTo(other.Name);
      return num;
    }
  }
}
