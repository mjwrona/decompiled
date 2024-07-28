// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CvsFileDownloadRestApiResourceProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class CvsFileDownloadRestApiResourceProvider : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      routes.MapResourceRoute(TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection, CvsFileDownloadResourceIds.LocationId, "CvsFileDownload", "CvsFileDownload", "public/{resource}", VssRestApiVersion.v5_0);
    }
  }
}
