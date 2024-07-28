// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.CollectionJumpPointModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class CollectionJumpPointModel : JumpPointModelBase, IComparable<CollectionJumpPointModel>
  {
    private IVssServiceHost m_organizationServiceHost;

    public CollectionJumpPointModel(
      TfsWebContext tfsWebContext,
      NavigationContextModel navigationContext,
      TfsServiceHostDescriptor collection)
      : base(tfsWebContext, navigationContext)
    {
      this.m_organizationServiceHost = tfsWebContext.TfsRequestContext.To(TeamFoundationHostType.Application).ServiceHost;
      this.CollectionServiceHost = collection;
      this.Name = this.CollectionServiceHost.Name;
      this.NavigationContextLevel = NavigationContextLevels.Collection;
      this.HasMore = this.IsOnline = collection.Status == TeamFoundationServiceHostStatus.Started;
      this.RouteValues = new RouteValueDictionary();
      this.RouteValues["serviceHost"] = (object) this.CollectionServiceHost;
      this.RouteValues["project"] = (object) string.Empty;
      this.RouteValues["team"] = (object) string.Empty;
      this.Projects = new List<ProjectJumpPointModel>();
    }

    public override RouteValueDictionary BrowseRouteValues
    {
      get
      {
        RouteValueDictionary browseRouteValues = base.BrowseRouteValues;
        browseRouteValues["serviceHost"] = (object) this.m_organizationServiceHost;
        browseRouteValues["collectionId"] = (object) this.CollectionServiceHost.Id;
        return browseRouteValues;
      }
    }

    public bool IsOnline { get; private set; }

    public List<ProjectJumpPointModel> Projects { get; private set; }

    public bool HasMore { get; set; }

    public Guid Id => this.CollectionServiceHost.Id;

    public TfsServiceHostDescriptor CollectionServiceHost { get; private set; }

    public override string Path => this.Name;

    public override JsObject ToJson()
    {
      JsObject json = base.ToJson();
      json["projects"] = (object) this.Projects.Select<ProjectJumpPointModel, JsObject>((Func<ProjectJumpPointModel, JsObject>) (x => x.ToJson()));
      json["hasMore"] = (object) this.HasMore;
      json["collectionId"] = (object) this.Id;
      return json;
    }

    public int CompareTo(CollectionJumpPointModel other) => this.Name.CompareTo(other.Name);
  }
}
