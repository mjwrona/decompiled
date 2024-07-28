// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenContentResponseMessageGenerator
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.Extensions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public class MavenContentResponseMessageGenerator : 
    IAsyncHandler<MavenPackageFileResponse, HttpResponseMessage>,
    IHaveInputType<MavenPackageFileResponse>,
    IHaveOutputType<HttpResponseMessage>
  {
    public Task<HttpResponseMessage> Handle(MavenPackageFileResponse response)
    {
      string fileName = response.FileName;
      ArgumentUtility.CheckStringForNullOrEmpty(fileName, "fileName");
      if (response.Uri == (Uri) null)
      {
        ISecuredObject securedObjectReadOnly = FeedSecuredObjectFactory.CreateSecuredObjectReadOnly(response.FileRequest.Feed);
        VssServerStreamContent serverStreamContent = new VssServerStreamContent(response.Content, (object) securedObjectReadOnly);
        serverStreamContent.Headers.ContentType = new MediaTypeHeaderValue(MavenPackageExtensions.GetMimeType(fileName));
        serverStreamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
          FileName = fileName
        };
        return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = (HttpContent) serverStreamContent
        });
      }
      return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.SeeOther)
      {
        Headers = {
          Location = response.Uri
        }
      });
    }
  }
}
