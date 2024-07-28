// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Model.RestResponse.WidgetTypesResponseExtension
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Dashboards.Model.RestResponse
{
  public static class WidgetTypesResponseExtension
  {
    public static void AddLinks(this WidgetTypesResponse response, string marketplaceUrl)
    {
      response.Links = new ReferenceLinks();
      response.Links.AddLink("web", marketplaceUrl, response as ISecuredObject);
    }
  }
}
