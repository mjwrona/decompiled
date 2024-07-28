// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.PathContentsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Build2.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "pathContents")]
  [ClientGroupByResource("sourceProviders")]
  public class PathContentsController : BuildApiController
  {
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Build.WebApi.SourceRepositoryItem> GetPathContents(
      string providerName,
      [ClientQueryParameter] Guid? serviceEndpointId = null,
      [ClientQueryParameter] string repository = null,
      [ClientQueryParameter] string commitOrBranch = null,
      [ClientQueryParameter] string path = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(providerName, nameof (providerName));
      return this.GetSourceProvider(providerName).GetPathContents(this.TfsRequestContext, this.ProjectId, serviceEndpointId, repository, commitOrBranch, path).Select<Microsoft.TeamFoundation.Build2.Server.SourceRepositoryItem, Microsoft.TeamFoundation.Build.WebApi.SourceRepositoryItem>((Func<Microsoft.TeamFoundation.Build2.Server.SourceRepositoryItem, Microsoft.TeamFoundation.Build.WebApi.SourceRepositoryItem>) (p => p.ToWebApiSourceRepositoryItem()));
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<DirectoryNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ExternalSourceProviderException>(HttpStatusCode.BadRequest);
    }

    private IBuildSourceProvider GetSourceProvider(string repositoryType) => this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, repositoryType);
  }
}
