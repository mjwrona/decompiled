// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.WebAPI.CodeLensResourceProvider
// Assembly: Microsoft.TeamFoundation.CodeSense.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FD810605-AAA9-4CE8-B2C6-6B28A5D994C5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.dll

using Microsoft.TeamFoundation.CodeSense.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http;

namespace Microsoft.TeamFoundation.CodeSense.Server.WebAPI
{
  public sealed class CodeLensResourceProvider : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<HttpRouteCollection>(routes, nameof (routes));
      if (requestContext != null && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        areas.RegisterArea("codelens", "DB4B1D4B-13B4-4CEB-8F84-1001B5500EBC");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, CodeLensWebApiConstants.FilesLocationId, "codelens", "filedetails", "{area}/{resource}", VssRestApiVersion.v1_0, maxResourceVersion: 3, defaults: (object) new
      {
      }, routeName: "CodeLensFileDetails");
      routes.MapResourceRoute(TeamFoundationHostType.ProjectCollection, CodeLensWebApiConstants.FileSummariesLocationId, "codelens", "filesummaries", "{area}/{resource}", VssRestApiVersion.v1_0, maxResourceVersion: 3, defaults: (object) new
      {
      }, routeName: "CodeLensFileSummaries");
    }
  }
}
