// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ProjectJumpPointModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ProjectJumpPointModel : JumpPointModelBase, IComparable<ProjectJumpPointModel>
  {
    public ProjectJumpPointModel(
      TfsWebContext tfsWebContext,
      NavigationContextModel navigationContext,
      CatalogNode project,
      CollectionJumpPointModel collection)
      : base(tfsWebContext, navigationContext)
    {
      this.Initialize(new TeamProjectModel(tfsWebContext.TfsRequestContext, project), collection);
    }

    public ProjectJumpPointModel(
      TfsWebContext tfsWebContext,
      NavigationContextModel navigationContext,
      ProjectInfo project,
      CollectionJumpPointModel collection)
      : base(tfsWebContext, navigationContext)
    {
      this.Initialize(new TeamProjectModel(tfsWebContext.TfsRequestContext, project), collection);
    }

    private void Initialize(TeamProjectModel projectModel, CollectionJumpPointModel collection)
    {
      this.Name = projectModel.DisplayName;
      this.Uri = projectModel.Uri;
      this.NavigationContextLevel = NavigationContextLevels.Project;
      this.RouteValues = new RouteValueDictionary();
      this.RouteValues["serviceHost"] = (object) collection.CollectionServiceHost;
      this.RouteValues["project"] = (object) projectModel.DisplayName;
      this.RouteValues["team"] = (object) string.Empty;
      this.Parent = collection;
      this.Teams = new List<TeamJumpPointModel>();
    }

    public CollectionJumpPointModel Parent { get; private set; }

    public List<TeamJumpPointModel> Teams { get; private set; }

    public bool TeamsPopulated { get; set; }

    public string Uri { get; private set; }

    public string DefaultTeamGuid { get; set; }

    public override string Path => this.Parent.Path + "/" + this.Name;

    public override JsObject ToJson()
    {
      JsObject json = base.ToJson();
      json["defaultTeamId"] = (object) this.DefaultTeamGuid;
      json["teams"] = (object) this.Teams.Select<TeamJumpPointModel, JsObject>((Func<TeamJumpPointModel, JsObject>) (x => x.ToJson()));
      json["teamsPopulated"] = (object) this.TeamsPopulated;
      json["projectUri"] = (object) this.Uri;
      json["collectionName"] = (object) this.Parent.Name;
      json["collectionHostVDir"] = (object) this.Parent.CollectionServiceHost.RelativeVirtualDirectory;
      json["collectionId"] = (object) this.Parent.CollectionServiceHost.Id;
      return json;
    }

    public int CompareTo(ProjectJumpPointModel other)
    {
      int num = this.Parent.CompareTo(other.Parent);
      if (num == 0)
        num = this.Name.CompareTo(other.Name);
      return num;
    }
  }
}
