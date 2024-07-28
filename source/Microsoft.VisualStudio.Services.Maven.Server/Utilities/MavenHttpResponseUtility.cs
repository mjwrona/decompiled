// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.MavenHttpResponseUtility
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.Extensions;
using Microsoft.VisualStudio.Services.WebApi;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public static class MavenHttpResponseUtility
  {
    public static HttpResponseMessage CreateResponseMessage(
      string fileName,
      Stream content,
      ISecuredObject securedObject)
    {
      return MavenHttpResponseUtility.CreateResponseMessage(fileName, (HttpContent) new VssServerStreamContent(content, (object) securedObject));
    }

    private static HttpResponseMessage CreateResponseMessage(string fileName, HttpContent content)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fileName, nameof (fileName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(fileName, nameof (fileName));
      ArgumentUtility.CheckForNull<HttpContent>(content, nameof (content));
      content.Headers.ContentType = new MediaTypeHeaderValue(MavenPackageExtensions.GetMimeType(fileName));
      content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = fileName
      };
      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = content
      };
    }
  }
}
