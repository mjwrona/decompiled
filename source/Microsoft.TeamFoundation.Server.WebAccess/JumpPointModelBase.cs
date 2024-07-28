// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.JumpPointModelBase
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Server.WebAccess.Html;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public abstract class JumpPointModelBase
  {
    private TfsWebContext m_tfsWebContext;
    private NavigationContextModel m_navigationContext;
    protected const char PathSeparator = '/';

    protected JumpPointModelBase(
      TfsWebContext requestContext,
      NavigationContextModel navigationContext)
    {
      this.m_tfsWebContext = requestContext;
      this.m_navigationContext = navigationContext;
    }

    public string ActionUrl => NavigationExtensions.GetTargetUrl(this.m_tfsWebContext, this.m_navigationContext.Area, this.m_navigationContext.Controller, this.m_navigationContext.Action, this.m_navigationContext.Parameters, this.m_navigationContext.Level, this.NavigationContextLevel, this.RouteValues, false);

    public string BrowseUrl
    {
      get
      {
        RouteValueDictionary routeValues = new RouteValueDictionary((IDictionary<string, object>) this.BrowseRouteValues)
        {
          {
            "routeArea",
            (object) "Api"
          }
        };
        return UrlHelperExtensions.RouteUrl(this.m_tfsWebContext.Url, PlatformRouteHelpers.GetControllerActionRouteName(routeValues, "Api"), "browse", "browse", routeValues);
      }
    }

    public NavigationContextLevels NavigationContextLevel { get; protected set; }

    public string Name { get; protected set; }

    public string Description { get; protected set; }

    public RouteValueDictionary RouteValues { get; protected set; }

    public virtual RouteValueDictionary BrowseRouteValues => this.RouteValues;

    public abstract string Path { get; }

    public virtual JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("url", (object) this.ActionUrl);
      json.Add("browseUrl", (object) this.BrowseUrl);
      json.Add("name", (object) this.Name);
      json.Add("description", (object) this.Description);
      json.Add("path", (object) this.Path);
      return json;
    }
  }
}
