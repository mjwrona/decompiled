// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Model.RestResponse.DashboardGroupResponseExtension
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Dashboards.Model.RestResponse
{
  public static class DashboardGroupResponseExtension
  {
    public static void AddLinks(
      this DashboardGroup response,
      IEnumerable<DashboardGroupEntryResponse> entryResponses)
    {
      response.Links = new ReferenceLinks();
      response.Links.AddLink("self", response.Url, (ISecuredObject) response);
      foreach (DashboardGroupEntryResponse entryResponse in entryResponses)
        response.Links.AddLink("dashboard", entryResponse.Url, (ISecuredObject) response);
    }
  }
}
