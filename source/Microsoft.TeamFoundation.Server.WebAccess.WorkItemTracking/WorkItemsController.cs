// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.WorkItemsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 74AD14A4-225D-46D2-B154-945941A2D167
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking
{
  [SupportedRouteArea(NavigationContextLevels.Collection | NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [RegisterHubMruPage(true)]
  [OutputCache(CacheProfile = "NoCache")]
  public class WorkItemsController : WorkItemsAreaController
  {
  }
}
