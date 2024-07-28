// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsODataPathHandler
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Routing;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class AnalyticsODataPathHandler : DefaultODataPathHandler
  {
    public override Microsoft.AspNet.OData.Routing.ODataPath Parse(
      string serviceRoot,
      string odataPath,
      IServiceProvider requestContainer)
    {
      try
      {
        return base.Parse(serviceRoot, odataPath, requestContainer);
      }
      catch (ODataException ex)
      {
        ODataException odataException = ex;
        HttpRequestMessage httpRequestMessage = HttpContext.Current.GetHttpRequestMessage();
        HttpStatusCode statusCode = HttpStatusCode.BadRequest;
        IVssRequestContext vssRequestContext = HttpContext.Current.GetVssRequestContext();
        if (odataException is ODataUnrecognizedPathException unrecognizedPathException && vssRequestContext != null)
        {
          odataException = (ODataException) unrecognizedPathException;
          statusCode = HttpStatusCode.NotFound;
        }
        WrappedException wrappedException = new WrappedException((Exception) odataException, httpRequestMessage.ShouldIncludeErrorDetail(), httpRequestMessage.GetApiVersion());
        throw new HttpResponseException(httpRequestMessage.CreateResponse<WrappedException>(statusCode, wrappedException));
      }
    }
  }
}
