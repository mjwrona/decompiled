// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.FileContentsController
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "fileContents")]
  [ClientGroupByResource("sourceProviders")]
  public class FileContentsController : BuildApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), null, "text/plain")]
    public HttpResponseMessage GetFileContents(
      string providerName,
      [ClientQueryParameter] Guid? serviceEndpointId = null,
      [ClientQueryParameter] string repository = null,
      [ClientQueryParameter] string commitOrBranch = null,
      [ClientQueryParameter] string path = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(providerName, nameof (providerName));
      string fileContent = this.GetSourceProvider(providerName).GetFileContent(this.TfsRequestContext, this.ProjectId, serviceEndpointId, repository, commitOrBranch, path);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StringContent(fileContent, Encoding.UTF8, "text/plain");
      return response;
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<FileNotFoundException>(HttpStatusCode.NotFound);
    }

    private IBuildSourceProvider GetSourceProvider(string repositoryType) => this.TfsRequestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(this.TfsRequestContext, repositoryType);
  }
}
